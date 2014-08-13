using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Business.NullObjects;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Entity;

namespace GalleryServerPro.Web.Controller
{
	/// <summary>
	/// Contains functionality for interacting with gallery objects (that is, media objects and albums). Typically web pages 
	/// directly call the appropriate business layer objects, but when a task involves multiple steps or the functionality 
	/// does not exist in the business layer, the methods here are used.
	/// </summary>
	public static class GalleryObjectController
	{
		#region Public Static Methods

		/// <summary>
		/// Persist the gallery object to the data store. This method updates the audit fields before saving. The currently logged
		/// on user is recorded as responsible for the changes. All gallery objects should be
		/// saved through this method rather than directly invoking the gallery object's Save method, unless you want to 
		/// manually update the audit fields yourself.
		/// </summary>
		/// <param name="galleryObject">The gallery object to persist to the data store.</param>
		/// <remarks>When no user name is available through <see cref="Utils.UserName" />, the string &lt;unknown&gt; is
		/// substituted. Since GSP requires users to be logged on to edit objects, there will typically always be a user name 
		/// available. However, in some cases one won't be available, such as when an error occurs during self registration and
		/// the exception handling code needs to delete the just-created user album.</remarks>
		public static void SaveGalleryObject(IGalleryObject galleryObject)
		{
			string userName = (String.IsNullOrEmpty(Utils.UserName) ? Resources.GalleryServerPro.Site_Missing_Data_Text : Utils.UserName);
			SaveGalleryObject(galleryObject, userName);
		}

		/// <summary>
		/// Persist the gallery object to the data store. This method updates the audit fields before saving. All gallery objects should be
		/// saved through this method rather than directly invoking the gallery object's Save method, unless you want to
		/// manually update the audit fields yourself.
		/// </summary>
		/// <param name="galleryObject">The gallery object to persist to the data store.</param>
		/// <param name="userName">The user name to be associated with the modifications. This name is stored in the internal
		/// audit fields associated with this gallery object.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="galleryObject" /> is null.</exception>
		public static void SaveGalleryObject(IGalleryObject galleryObject, string userName)
		{
			if (galleryObject == null)
				throw new ArgumentNullException("galleryObject");

			DateTime currentTimestamp = DateTime.Now;

			if (galleryObject.IsNew)
			{
				galleryObject.CreatedByUserName = userName;
				galleryObject.DateAdded = currentTimestamp;
			}

			if (galleryObject.HasChanges)
			{
				galleryObject.LastModifiedByUserName = userName;
				galleryObject.DateLastModified = currentTimestamp;
			}

			// Verify that any role needed for album ownership exists and is properly configured.
			RoleController.ValidateRoleExistsForAlbumOwner(galleryObject as IAlbum);

			// Persist to data store.
			galleryObject.Save();
		}

		/// <summary>
		/// Move the specified object to the specified destination album. This method moves the physical files associated with this
		/// object to the destination album's physical directory. The object's Save() method is invoked to persist the changes to the
		/// data store. When moving albums, all the album's children, grandchildren, etc are also moved. 
		/// The audit fields are automatically updated before saving.
		/// </summary>
		/// <param name="galleryObjectToMove">The gallery object to move.</param>
		/// <param name="destinationAlbum">The album to which the current object should be moved.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="galleryObjectToMove" /> is null.</exception>
		public static void MoveGalleryObject(IGalleryObject galleryObjectToMove, IAlbum destinationAlbum)
		{
			if (galleryObjectToMove == null)
				throw new ArgumentNullException("galleryObjectToMove");

			string currentUser = Utils.UserName;
			DateTime currentTimestamp = DateTime.Now;

			galleryObjectToMove.LastModifiedByUserName = currentUser;
			galleryObjectToMove.DateLastModified = currentTimestamp;

			galleryObjectToMove.MoveTo(destinationAlbum);
		}

		/// <summary>
		/// Copy the specified object and place it in the specified destination album. This method creates a completely separate copy
		/// of the original, including copying the physical files associated with this object. The copy is persisted to the data
		/// store and then returned to the caller. When copying albums, all the album's children, grandchildren, etc are also copied.
		/// The audit fields of the copied objects are automatically updated before saving.
		/// </summary>
		/// <param name="galleryObjectToCopy">The gallery object to copy.</param>
		/// <param name="destinationAlbum">The album to which the current object should be copied.</param>
		/// <returns>
		/// Returns a new gallery object that is an exact copy of the original, except that it resides in the specified
		/// destination album, and of course has a new ID. Child objects are recursively copied.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="galleryObjectToCopy" /> is null.</exception>
		public static IGalleryObject CopyGalleryObject(IGalleryObject galleryObjectToCopy, IAlbum destinationAlbum)
		{
			if (galleryObjectToCopy == null)
				throw new ArgumentNullException("galleryObjectToCopy");

			string currentUser = Utils.UserName;

			return galleryObjectToCopy.CopyTo(destinationAlbum, currentUser);
		}

		/// <summary>
		/// Adds a media file to an album. Prior to calling this method, the file should exist in the 
		/// temporary upload directory (<see cref="GlobalConstants.TempUploadDirectory" />) in the 
		/// App_Data directory with the name <see cref="AddMediaObjectSettings.FileNameOnServer" />. The
		/// file is copied to the destination album and given the name of 
		/// <see cref="AddMediaObjectSettings.FileName" /> (instead of whatever name it currently has, which 
		/// may contain a GUID).
		/// </summary>
		/// <param name="settings">The settings that contain data and configuration options for the media file.</param>
		/// <exception cref="Events.CustomExceptions.GallerySecurityException">Thrown when user is not authorized to add a media object to the album.</exception>
		public static List<ActionResult> AddMediaObject(AddMediaObjectSettings settings)
		{
			List<ActionResult> results = CreateMediaObjectFromFile(settings);

			HelperFunctions.PurgeCache();

			return results;
		}

		/// <summary>
		/// Gets the gallery objects in the album. Includes albums and media objects.
		/// </summary>
		/// <param name="albumId">The album ID.</param>
		/// <param name="sortByMetaName">The sort by meta name id.</param>
		/// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
		/// <returns>Returns an <see cref="IQueryable" /> instance of <see cref="Entity.GalleryItem" />.</returns>
		/// <exception cref="InvalidAlbumException">Thrown when an album with the specified
		/// <paramref name="albumId" /> is not found in the data store.</exception>
		/// <exception cref="GalleryServerPro.Events.CustomExceptions.GallerySecurityException">Thrown when the user does not have at least one of the requested permissions to the
		/// specified album.</exception>
		public static IQueryable<GalleryItem> GetGalleryItemsInAlbum(int albumId, MetadataItemName sortByMetaName, bool sortAscending)
		{
			IAlbum album = Factory.LoadAlbumInstance(albumId, true);

			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);

			IList<IGalleryObject> galleryObjects;

			if (MetadataItemNameEnumHelper.IsValidFormattedMetadataItemName(sortByMetaName))
			{
				galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated).ToSortedList(sortByMetaName, sortAscending, album.GalleryId);
			}
			else
			{
				galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated).ToSortedList();
			}

			return ToGalleryItems(galleryObjects).AsQueryable();
		}

		//public static IQueryable<GalleryItem> GetGalleryItemsHavingTags(string[] tags, string[] people, int galleryId, MetadataItemName sortByMetaName, bool sortAscending, GalleryObjectType filter)
		//{
		//	IAlbum album = GetGalleryObjectsHavingTags(tags, people, filter, galleryId);

		//	IList<IGalleryObject> galleryObjects;

		//	if (MetadataItemNameEnumHelper.IsValidFormattedMetadataItemName(sortByMetaName))
		//	{
		//		galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated).ToSortedList(sortByMetaName, sortAscending, album.GalleryId);
		//	}
		//	else
		//	{
		//		galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated).ToSortedList();
		//	}

		//	return ToGalleryItems(galleryObjects).AsQueryable();
		//}

		/// <summary>
		/// Return a virtual album containing gallery objects whose title or caption contain the specified search strings and
		/// for which the current user has authorization to view. Guaranteed to not return null. A gallery 
		/// object is considered a match when all search terms are found in the relevant fields.
		/// </summary>
		/// <param name="searchStrings">The strings to search for.</param>
		/// <param name="filter">A filter that limits the types of gallery objects that are returned.
		/// Maps to the <see cref="GalleryObjectType" /> enumeration.</param>
		/// <param name="galleryId">The ID for the gallery containing the objects to search.</param>
		/// <returns>
		/// Returns an <see cref="IAlbum" /> containing the matching items. This may include albums and media
		/// objects from different albums.
		/// </returns>
		public static IAlbum GetGalleryObjectsHavingTitleOrCaption(string[] searchStrings, GalleryObjectType filter, int galleryId)
		{
			if (searchStrings == null)
				throw new ArgumentNullException();

			var tmpAlbum = Factory.CreateEmptyAlbumInstance(galleryId);
			tmpAlbum.IsVirtualAlbum = true;
			tmpAlbum.VirtualAlbumType = VirtualAlbumType.TitleOrCaption;
			tmpAlbum.Title = String.Concat(Resources.GalleryServerPro.Site_Search_Title, String.Join(Resources.GalleryServerPro.Site_Search_Concat, searchStrings));
			tmpAlbum.Caption = String.Empty;

			var searchOptions = new GalleryObjectSearchOptions
			{
				GalleryId = galleryId,
				SearchType = GalleryObjectSearchType.SearchByTitleOrCaption,
				SearchTerms = searchStrings,
				IsUserAuthenticated = Utils.IsAuthenticated,
				Roles = RoleController.GetGalleryServerRolesForUser(),
				Filter = filter
			};

			var searcher = new GalleryObjectSearcher(searchOptions);

			foreach (var galleryObject in searcher.Find())
			{
				tmpAlbum.AddGalleryObject(galleryObject);
			}

			return tmpAlbum;
		}

		/// <summary>
		/// Return a virtual album containing gallery objects that match the specified search strings and
		/// for which the current user has authorization to view. Guaranteed to not return null. A gallery 
		/// object is considered a match when all search terms are found in the relevant fields.
		/// </summary>
		/// <param name="searchStrings">The strings to search for.</param>
		/// <param name="filter">A filter that limits the types of gallery objects that are returned.
		/// Maps to the <see cref="GalleryObjectType" /> enumeration.</param>
		/// <param name="galleryId">The ID for the gallery containing the objects to search.</param>
		/// <returns>
		/// Returns an <see cref="IAlbum" /> containing the matching items. This may include albums and media
		/// objects from different albums.
		/// </returns>
		public static IAlbum GetGalleryObjectsHavingSearchString(string[] searchStrings, GalleryObjectType filter, int galleryId)
		{
			if (searchStrings == null)
				throw new ArgumentNullException();

			var tmpAlbum = Factory.CreateEmptyAlbumInstance(galleryId);
			tmpAlbum.IsVirtualAlbum = true;
			tmpAlbum.VirtualAlbumType = VirtualAlbumType.Search;
			tmpAlbum.Title = String.Concat(Resources.GalleryServerPro.Site_Search_Title, String.Join(Resources.GalleryServerPro.Site_Search_Concat, searchStrings));
			tmpAlbum.Caption = String.Empty;

			var searchOptions = new GalleryObjectSearchOptions
			{
				GalleryId = galleryId,
				SearchType = GalleryObjectSearchType.SearchByKeyword,
				SearchTerms = searchStrings,
				IsUserAuthenticated = Utils.IsAuthenticated,
				Roles = RoleController.GetGalleryServerRolesForUser(),
				Filter = filter
			};

			var searcher = new GalleryObjectSearcher(searchOptions);

			foreach (var galleryObject in searcher.Find())
			{
				tmpAlbum.AddGalleryObject(galleryObject);
			}

			return tmpAlbum;
		}

		/// <summary>
		/// Gets a virtual album containing gallery objects that match the specified <paramref name="tags" /> or <paramref name="people" />
		/// belonging to the specified <paramref name="galleryId" />. Guaranteed to not return null. The returned album 
		/// is a virtual one (<see cref="IAlbum.IsVirtualAlbum" />=<c>true</c>) containing the collection of matching 
		/// items the current user has permission to view. Returns an empty album when no matches are found or the 
		/// query string does not contain the search terms.
		/// </summary>
		/// <param name="tags">The tags to search for. If specified, the <paramref name="people" /> parameter must be null.</param>
		/// <param name="people">The people to search for. If specified, the <paramref name="tags" /> parameter must be null.</param>
		/// <param name="filter">A filter that limits the types of gallery objects that are returned.
		/// Maps to the <see cref="GalleryObjectType" /> enumeration.</param>
		/// <param name="galleryId">The ID of the gallery. Only objects in this gallery are returned.</param>
		/// <returns>An instance of <see cref="IAlbum" />.</returns>
		/// <exception cref="System.ArgumentException">Throw when the tags and people parameters are both null or empty, or both
		/// have values.</exception>
		public static IAlbum GetGalleryObjectsHavingTags(string[] tags, string[] people, GalleryObjectType filter, int galleryId)
		{
			if (((tags == null) || (tags.Length == 0)) && ((people == null) || (people.Length == 0)))
				throw new ArgumentException("GalleryObjectController.GetGalleryObjectsHavingTags() requires the tags or people parameters to be specified, but they were both null or empty.");

			if ((tags != null) && (tags.Length > 0) && (people != null) && (people.Length > 0))
				throw new ArgumentException("GalleryObjectController.GetGalleryObjectsHavingTags() requires EITHER the tags or people parameters to be specified, but not both. Instead, they were both populated.");

			var searchType = (tags != null && tags.Length > 0 ? GalleryObjectSearchType.SearchByTag : GalleryObjectSearchType.SearchByPeople);
			var searchTags = (searchType == GalleryObjectSearchType.SearchByTag ? tags : people);

			var tmpAlbum = Factory.CreateEmptyAlbumInstance(galleryId);
			tmpAlbum.IsVirtualAlbum = true;
			tmpAlbum.VirtualAlbumType = (searchType == GalleryObjectSearchType.SearchByTag ? VirtualAlbumType.Tag : VirtualAlbumType.People);
			tmpAlbum.Title = String.Concat(Resources.GalleryServerPro.Site_Tag_Title, String.Join(Resources.GalleryServerPro.Site_Search_Concat, searchTags));
			tmpAlbum.Caption = String.Empty;

			var searcher = new GalleryObjectSearcher(new GalleryObjectSearchOptions
			{
				SearchType = searchType,
				Tags = searchTags,
				GalleryId = galleryId,
				Roles = RoleController.GetGalleryServerRolesForUser(),
				IsUserAuthenticated = Utils.IsAuthenticated,
				Filter = filter
			});

			foreach (var galleryObject in searcher.Find())
			{
				tmpAlbum.AddGalleryObject(galleryObject);
			}

			return tmpAlbum;
		}

		/// <summary>
		/// Gets the gallery objects most recently added to the gallery having <paramref name="galleryId" />.
		/// </summary>
		/// <param name="top">The maximum number of results to return. Must be greater than zero.</param>
		/// <param name="galleryId">The gallery ID.</param>
		/// <param name="filter">A filter that limits the types of gallery objects that are returned.</param>
		/// <returns>An instance of <see cref="IAlbum" />.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="top" /> is less than or equal to zero.</exception>
		public static IAlbum GetMostRecentlyAddedGalleryObjects(int top, int galleryId, GalleryObjectType filter)
		{
			if (top <= 0)
				throw new ArgumentException("The top parameter must contain a number greater than zero.", "top");

			var tmpAlbum = Factory.CreateEmptyAlbumInstance(galleryId);

			tmpAlbum.IsVirtualAlbum = true;
			tmpAlbum.VirtualAlbumType = VirtualAlbumType.MostRecentlyAdded;
			tmpAlbum.Title = Resources.GalleryServerPro.Site_Recently_Added_Title;
			tmpAlbum.Caption = String.Empty;
			tmpAlbum.SortByMetaName = MetadataItemName.DateAdded;
			tmpAlbum.SortAscending = false;

			var searcher = new GalleryObjectSearcher(new GalleryObjectSearchOptions
			{
				SearchType = GalleryObjectSearchType.MostRecentlyAdded,
				GalleryId = galleryId,
				Roles = RoleController.GetGalleryServerRolesForUser(),
				IsUserAuthenticated = Utils.IsAuthenticated,
				MaxNumberResults = top,
				Filter = filter
			});

			foreach (var galleryObject in searcher.Find())
			{
				tmpAlbum.AddGalleryObject(galleryObject);
			}

			return tmpAlbum;
		}

		/// <summary>
		/// Gets the media objects having the specified <paramref name="rating" /> and belonging to the
		/// <paramref name="galleryId" />.
		/// </summary>
		/// <param name="rating">Identifies the type of rating to retrieve. Valid values: "highest", "lowest", "none", or a number
		/// from 0 to 5 in half-step increments (eg. 0, 0.5, 1, 1.5, ... 4.5, 5).</param>
		/// <param name="top">The maximum number of results to return. Must be greater than zero.</param>
		/// <param name="galleryId">The gallery ID.</param>
		/// <param name="filter">A filter that limits the types of gallery objects that are returned.</param>
		/// <returns>An instance of <see cref="IAlbum" />.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="top" /> is less than or equal to zero.</exception>
		public static IAlbum GetRatedMediaObjects(string rating, int top, int galleryId, GalleryObjectType filter)
		{
			if (top <= 0)
				throw new ArgumentException("The top parameter must contain a number greater than zero.", "top");

			var tmpAlbum = Factory.CreateEmptyAlbumInstance(galleryId);

			tmpAlbum.IsVirtualAlbum = true;
			tmpAlbum.VirtualAlbumType = VirtualAlbumType.Rated;
			tmpAlbum.Title = GetRatedAlbumTitle(rating);
			tmpAlbum.Caption = String.Empty;

			var ratingSortTrigger = new[] {"lowest", "highest"};
			if (ratingSortTrigger.Contains(rating))
			{
				// Sort on rating field for lowest or highest. All others use the default album sort setting.
				tmpAlbum.SortByMetaName = MetadataItemName.Rating;
				tmpAlbum.SortAscending = !rating.Equals("highest", StringComparison.OrdinalIgnoreCase);
			}

			var searcher = new GalleryObjectSearcher(new GalleryObjectSearchOptions
			{
				SearchType = GalleryObjectSearchType.SearchByRating,
				SearchTerms = new [] { rating },
				GalleryId = galleryId,
				Roles = RoleController.GetGalleryServerRolesForUser(),
				IsUserAuthenticated = Utils.IsAuthenticated,
				MaxNumberResults = top,
				Filter = filter
			});

			foreach (var galleryObject in searcher.Find())
			{
				tmpAlbum.AddGalleryObject(galleryObject);
			}

			return tmpAlbum;
		}

		/// <summary>
		/// Sorts the gallery items passed to this method and return. No changes are made to the data store.
		/// When the album is virtual, the <see cref="Entity.AlbumAction.Album.GalleryItems" /> property
		/// must be populated with the items to sort. For non-virtual albums (those with a valid ID), the 
		/// gallery objects are retrieved based on the ID and then sorted. The sort preference is saved to 
		/// the current user's profile, except when the album is virtual. The method incorporates security to
		/// ensure only authorized items are returned to the user.
		/// </summary>
		/// <param name="albumAction">An instance containing the album to sort and the sort preferences.</param>
		/// <returns>IQueryable{Entity.GalleryItem}.</returns>
		/// <exception cref="GalleryServerPro.Events.CustomExceptions.GallerySecurityException">Thrown when 
		/// the user does not have view permission to the specified album.</exception>
		public static IQueryable<Entity.GalleryItem> SortGalleryItems(Entity.AlbumAction albumAction)
		{
			IAlbum album;
			if (albumAction.Album.Id > int.MinValue)
			{
				album = Factory.LoadAlbumInstance(albumAction.Album.Id, true);

				SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);

				PersistUserSortPreference(album, albumAction.SortByMetaNameId, albumAction.SortAscending);
			}
			else
			{
				album = Factory.CreateAlbumInstance(albumAction.Album.Id, albumAction.Album.GalleryId);
				album.IsVirtualAlbum = (albumAction.Album.VirtualType != (int)VirtualAlbumType.NotVirtual);
				album.VirtualAlbumType = (VirtualAlbumType)albumAction.Album.VirtualType;

				var roles = RoleController.GetGalleryServerRolesForUser();

				foreach (var galleryItem in albumAction.Album.GalleryItems)
				{
					if (galleryItem.IsAlbum)
					{
						var childAlbum = Factory.LoadAlbumInstance(galleryItem.Id, false);

						if (SecurityManager.IsUserAuthorized(SecurityActions.ViewAlbumOrMediaObject, roles, childAlbum.Id, childAlbum.GalleryId, Utils.IsAuthenticated, childAlbum.IsPrivate, childAlbum.IsVirtualAlbum))
							album.AddGalleryObject(childAlbum);
					}
					else
					{
						var mediaObject = Factory.LoadMediaObjectInstance(galleryItem.Id);

						if (SecurityManager.IsUserAuthorized(SecurityActions.ViewAlbumOrMediaObject, roles, mediaObject.Parent.Id, mediaObject.GalleryId, Utils.IsAuthenticated, mediaObject.Parent.IsPrivate, ((IAlbum)mediaObject.Parent).IsVirtualAlbum))
							album.AddGalleryObject(mediaObject);
					}
				}
			}

			var galleryObjects = album
				.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated)
				.ToSortedList(albumAction.SortByMetaNameId, albumAction.SortAscending, album.GalleryId);

			return ToGalleryItems(galleryObjects).AsQueryable();
		}

		/// <summary>
		/// Gets the media objects in the album (excludes albums).
		/// </summary>
		/// <param name="albumId">The album id.</param>
		/// <param name="sortByMetaName">The sort by meta name id.</param>
		/// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
		/// <returns>Returns an <see cref="IQueryable" /> instance of <see cref="Entity.MediaItem" />.</returns>
		/// <exception cref="InvalidAlbumException">Thrown when an album with the specified 
		/// <paramref name = "albumId" /> is not found in the data store.</exception>
		/// <exception cref="GalleryServerPro.Events.CustomExceptions.GallerySecurityException">
		/// Throw when the user does not have view permission to the specified album.</exception>
		public static IQueryable<MediaItem> GetMediaItemsInAlbum(int albumId, MetadataItemName sortByMetaName, bool sortAscending)
		{
			IAlbum album = Factory.LoadAlbumInstance(albumId, true);
			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);

			IList<IGalleryObject> galleryObjects;

			if (MetadataItemNameEnumHelper.IsValidFormattedMetadataItemName(sortByMetaName))
			{
				galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList(sortByMetaName, sortAscending, album.GalleryId);
			}
			else
			{
				galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList();
			}

			//var galleryObjects = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList();

			return ToMediaItems(galleryObjects).AsQueryable();
		}

		public static MetaItem[] GetMetaItemsForMediaObject(int id)
		{
			IGalleryObject mo = Factory.LoadMediaObjectInstance(id);
			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), mo.Parent.Id, mo.GalleryId, Utils.IsAuthenticated, mo.Parent.IsPrivate, ((IAlbum)mo.Parent).IsVirtualAlbum);

			return ToMetaItems(mo.MetadataItems.GetVisibleItems(), mo);
		}

		public static MetaItem[] ToMetaItems(IGalleryObjectMetadataItemCollection metadataItems, IGalleryObject galleryObject)
		{
			var metaItems = new MetaItem[metadataItems.Count];
			var metaDefs = Factory.LoadGallerySetting(galleryObject.GalleryId).MetadataDisplaySettings;
			var moProfiles = ProfileController.GetProfile().MediaObjectProfiles;

			for (int i = 0; i < metaItems.Length; i++)
			{
				IGalleryObjectMetadataItem md = metadataItems[i];

				metaItems[i] = new MetaItem
					{
						Id = md.MediaObjectMetadataId,
						MediaId = galleryObject.Id,
						MTypeId = (int)md.MetadataItemName,
						GTypeId = (int)galleryObject.GalleryObjectType,
						Desc = md.Description,
						Value = md.Value,
						IsEditable = metaDefs.Find(md.MetadataItemName).IsEditable
					};

				if (md.MetadataItemName == MetadataItemName.Rating)
				{
					ReplaceAvgRatingWithUserRating(metaItems[i], moProfiles);
				}
			}

			return metaItems;
		}

		/// <summary>
		/// When the current user has previously rated an item, replace the average user rating with user's
		/// own rating.
		/// </summary>
		/// <param name="metaItem">The meta item. It must be a <see cref="MetadataItemName.Rating" /> item.</param>
		/// <param name="moProfiles"></param>
		private static void ReplaceAvgRatingWithUserRating(MetaItem metaItem, IMediaObjectProfileCollection moProfiles)
		{
			var moProfile = moProfiles.Find(metaItem.MediaId);

			if (moProfile != null)
			{
				metaItem.Desc = Resources.GalleryServerPro.UC_Metadata_UserRated_Rating_Lbl;
				metaItem.Value = moProfile.Rating;
			}
		}

		//public static IQueryable<Entity.MetaItem> GetMetaItemsForMediaObject(int id)
		//{
		//	var metadataItems = new List<Entity.MetaItem>();

		//	IGalleryObject mo = Factory.LoadMediaObjectInstance(id);
		//	SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), mo.Parent.Id, mo.GalleryId, Utils.IsAuthenticated, mo.Parent.IsPrivate);

		//	foreach (IGalleryObjectMetadataItem md in mo.MetadataItems.GetVisibleItems())
		//	{
		//		metadataItems.Add(new Entity.MetaItem
		//												{
		//													Id = md.MediaObjectMetadataId,
		//													TypeId = (int)md.MetadataItemName,
		//													Desc = md.Description,
		//													Value = md.Value,
		//													IsEditable = false
		//												});
		//	}

		//	return metadataItems.AsQueryable();
		//}

		/// <summary>
		/// Converts the <paramref name="galleryObjects" /> to an enumerable collection of 
		/// <see cref="Entity.GalleryItem" /> instances. Guaranteed to not return null.
		/// </summary>
		/// <param name="galleryObjects">The gallery objects.</param>
		/// <returns>An enumerable collection of <see cref="Entity.GalleryItem" /> instances.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static GalleryItem[] ToGalleryItems(IList<IGalleryObject> galleryObjects)
		{
			if (galleryObjects == null)
				throw new ArgumentNullException("galleryObjects");

			var gEntities = new List<GalleryItem>(galleryObjects.Count);

			gEntities.AddRange(galleryObjects.Select(galleryObject => ToGalleryItem(galleryObject, MediaObjectHtmlBuilder.GetMediaObjectHtmlBuilderOptions(galleryObject))));

			return gEntities.ToArray();
		}

		/// <summary>
		/// Converts the <paramref name="mediaObjects" /> to an enumerable collection of 
		/// <see cref="Entity.MediaItem" /> instances. Guaranteed to not return null. Do not pass any 
		/// <see cref="IAlbum" /> instances to this function.
		/// </summary>
		/// <param name="mediaObjects">The media objects.</param>
		/// <returns>An enumerable collection of <see cref="Entity.MediaItem" /> instances.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static MediaItem[] ToMediaItems(IList<IGalleryObject> mediaObjects)
		{
			if (mediaObjects == null)
				throw new ArgumentNullException("mediaObjects");

			var moEntities = new List<MediaItem>(mediaObjects.Count);
			var moBuilderOptions = MediaObjectHtmlBuilder.GetMediaObjectHtmlBuilderOptions(null);

			var i = 1;
			moEntities.AddRange(mediaObjects.Select(mo => ToMediaItem(mo, i++, moBuilderOptions)));

			return moEntities.ToArray();
		}

		/// <summary>
		/// Converts the <paramref name="galleryObject" /> to an instance of <see cref="Entity.GalleryItem" />.
		/// The instance can be JSON-serialized and sent to the browser.
		/// </summary>
		/// <param name="galleryObject">The gallery object to convert to an instance of
		/// <see cref="Entity.GalleryItem" />. It may be a media object or album.</param>
		/// <param name="moBuilderOptions">A set of properties to be used to build the HTML, JavaScript or URL for the 
		/// <paramref name="galleryObject" />.</param>
		/// <returns>Returns an <see cref="Entity.GalleryItem" /> object containing information
		/// about the requested item.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="galleryObject" /> or 
		/// <paramref name="moBuilderOptions" /> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when <paramref name="moBuilderOptions" /> does
		/// has a null or empty <see cref="MediaObjectHtmlBuilderOptions.Browsers" /> property.</exception>
		public static GalleryItem ToGalleryItem(IGalleryObject galleryObject, MediaObjectHtmlBuilderOptions moBuilderOptions)
		{
			if (galleryObject == null)
				throw new ArgumentNullException("galleryObject");

			if (moBuilderOptions == null)
				throw new ArgumentNullException("moBuilderOptions");

			if (moBuilderOptions.Browsers == null || moBuilderOptions.Browsers.Length == 0)
				throw new ArgumentOutOfRangeException("moBuilderOptions.Browsers", "The Browsers array property must have at least one element.");

			moBuilderOptions.GalleryObject = galleryObject;

			var gItem = new GalleryItem
										{
											Id = galleryObject.Id,
											Title = galleryObject.Title,
											Caption = galleryObject.Caption,
											Views = GetViews(moBuilderOptions).ToArray(),
											ViewIndex = 0,
											MimeType = (int)galleryObject.MimeType.TypeCategory,
											ItemType = (int)galleryObject.GalleryObjectType
										};

			IAlbum album = galleryObject as IAlbum;
			if (album != null)
			{
				gItem.IsAlbum = true;
				//gItem.DateStart = album.DateStart;
				//gItem.DateEnd = album.DateEnd;
				gItem.NumAlbums = album.GetChildGalleryObjects(GalleryObjectType.All, !Utils.IsAuthenticated).Count;
				gItem.NumMediaItems = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).Count;
			}

			return gItem;
		}

		/// <summary>
		/// Converts the <paramref name="mediaObject"/> to an instance of <see cref="Entity.MediaItem" />.
		/// The returned object DOES have the <see cref="Entity.MediaItem.MetaItems" /> property assigned.
		/// The instance can be JSON-serialized and sent to the browser. Do not pass an 
		/// <see cref="IAlbum" /> to this function.
		/// </summary>
		/// <param name="mediaObject">The media object to convert to an instance of
		/// <see cref="Entity.MediaItem"/>.</param>
		/// <param name="indexInAlbum">The one-based index of this media object within its album. This value is assigned to 
		/// <see cref="Entity.MediaItem.Index" />.</param>
		/// <param name="moBuilderOptions">A set of properties to be used to build the HTML, JavaScript or URL for the 
		/// <paramref name="mediaObject" />.</param>
		/// <returns>Returns an <see cref="Entity.MediaItem"/> object containing information
		/// about the requested media object.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="mediaObject" /> or 
		/// <paramref name="moBuilderOptions" /> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when <paramref name="moBuilderOptions" /> does
		/// has a null or empty <see cref="MediaObjectHtmlBuilderOptions.Browsers" /> property.</exception>
		public static MediaItem ToMediaItem(IGalleryObject mediaObject, int indexInAlbum, MediaObjectHtmlBuilderOptions moBuilderOptions)
		{
			if (mediaObject == null)
				throw new ArgumentNullException("mediaObject");

			if (moBuilderOptions == null)
				throw new ArgumentNullException("moBuilderOptions");

			if (moBuilderOptions.Browsers == null || moBuilderOptions.Browsers.Length == 0)
				throw new ArgumentOutOfRangeException("moBuilderOptions.Browsers", "The Browsers array property must have at least one element.");

			moBuilderOptions.GalleryObject = mediaObject;

			var isBeingProcessed = MediaConversionQueue.Instance.IsWaitingInQueueOrProcessing(mediaObject.Id, MediaQueueItemConversionType.CreateOptimized);

			var moEntity = new MediaItem
											 {
												 Id = mediaObject.Id,
												 AlbumId = mediaObject.Parent.Id,
												 AlbumTitle = mediaObject.Parent.Title,
												 Index = indexInAlbum,
												 Title = mediaObject.Title,
												 Views = GetViews(moBuilderOptions).ToArray(),
												 HighResAvailable = isBeingProcessed || (!String.IsNullOrEmpty(mediaObject.Optimized.FileName)) && (mediaObject.Original.FileName != mediaObject.Optimized.FileName),
												 IsDownloadable = !(mediaObject is ExternalMediaObject),
												 MimeType = (int)mediaObject.MimeType.TypeCategory,
												 ItemType = (int)mediaObject.GalleryObjectType,
												 MetaItems = ToMetaItems(mediaObject.MetadataItems.GetVisibleItems(), mediaObject)
											 };

			return moEntity;
		}

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Creates the media object from the file specified in <paramref name="options" />.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <returns>List{ActionResult}.</returns>
		/// <exception cref="Events.CustomExceptions.GallerySecurityException">Thrown when user is not authorized to add a media object to the album.</exception>
		/// <remarks>This function can be invoked from a thread that does not have access to the current HTTP context (for example, when
		/// uploading ZIP files). Therefore, be sure nothing in this body (or the functions it calls) uses HttpContext.Current, or at 
		/// least check it for null first.</remarks>
		private static List<ActionResult> CreateMediaObjectFromFile(AddMediaObjectSettings options)
		{
			string sourceFilePath = Path.Combine(AppSetting.Instance.PhysicalApplicationPath, GlobalConstants.TempUploadDirectory, options.FileNameOnServer);

			try
			{
				IAlbum album = AlbumController.LoadAlbumInstance(options.AlbumId, true, true);

				if (HttpContext.Current != null)
					SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.AddMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);
				else
				{
					// We are extracting files from a zip archive (we know this because this is the only scenario that happens on a background
					// thread where HttpContext.Current is null). Tweak the security check slightly to ensure the HTTP context isn't used.
					// The changes are still secure because options.CurrentUserName is assigned in the server's API method.
					SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.AddMediaObject, RoleController.GetGalleryServerRolesForUser(options.CurrentUserName), album.Id, album.GalleryId, !String.IsNullOrWhiteSpace(options.CurrentUserName), album.IsPrivate, album.IsVirtualAlbum);
				}

				var extension = Path.GetExtension(options.FileName);
				if (extension != null && ((extension.Equals(".zip", StringComparison.OrdinalIgnoreCase)) && (options.ExtractZipFile)))
				{
					List<ActionResult> result;

					// Extract the files from the zipped file.
					using (var zip = new ZipUtility(options.CurrentUserName, RoleController.GetGalleryServerRolesForUser(options.CurrentUserName)))
					{
						using (var fs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
						{
							result = zip.ExtractZipFile(fs, album, options.DiscardOriginalFile);
						}
					}

					album.SortAsync(true, options.CurrentUserName, true);

					return result;
				}
				else
				{
					string albumPhysicalPath = album.FullPhysicalPathOnDisk;
					string filename = HelperFunctions.ValidateFileName(albumPhysicalPath, options.FileName);
					string filepath = Path.Combine(albumPhysicalPath, filename);

					MoveFile(filepath, sourceFilePath);

					ActionResult result = CreateMediaObject(filepath, album, options);

					album.Sort(true, options.CurrentUserName);

					return new List<ActionResult> { result };
				}
			}
			catch (Exception ex)
			{
				AppEventController.LogError(ex);
				return new List<ActionResult>
					       {
				       		new ActionResult
				       			{
				       				Title = options.FileName,
				       				Status = ActionResultStatus.Error.ToString(),
				       				Message = "The event log may have additional details."
				       			}
				       	};
			}
			finally
			{
				try
				{
					// If the file still exists in the temp directory, delete it. Typically this happens when we've
					// extracted the contents of a zip file (since other files will have already been moved to the dest album.)
					if (File.Exists(sourceFilePath))
					{
						File.Delete(sourceFilePath);
					}
				}
				catch (IOException) { } // Ignore an error; not a big deal if it continues to exist in the temp directory
				catch (UnauthorizedAccessException) { } // Ignore an error; not a big deal if it continues to exist in the temp directory
			}
		}

		private static void MoveFile(string filepath, string sourceFilePath)
		{
			// Move file to album. If IOException happens, wait 1 second and try again, up to 10 times.
			int counter = 0;
			const int maxTries = 10;

			while (true)
			{
				try
				{
					File.Move(sourceFilePath, filepath);
					break;
				}
				catch (IOException ex)
				{
					counter++;
					ex.Data.Add("CannotMoveFile", String.Format("This error occurred while trying to move file '{0}' to '{1}'. This error has occurred {2} times. The system will try again up to a maximum of {3} attempts.", sourceFilePath, filepath, counter, maxTries));
					AppEventController.LogError(ex);

					if (counter >= maxTries)
						throw;

					Thread.Sleep(1000);
				}
			}
		}

		private static ActionResult CreateMediaObject(string filePath, IAlbum album, AddMediaObjectSettings options)
		{
			var result = new ActionResult
										{
											Title = Path.GetFileName(filePath)
										};

			try
			{
				IGalleryObject go = Factory.CreateMediaObjectInstance(filePath, album);
				SaveGalleryObject(go, options.CurrentUserName);

				if (options.DiscardOriginalFile)
				{
					go.DeleteOriginalFile();
					SaveGalleryObject(go);
				}

				result.Status = ActionResultStatus.Success.ToString();
			}
			catch (UnsupportedMediaObjectTypeException ex)
			{
				try
				{
					File.Delete(filePath);
				}
				catch (UnauthorizedAccessException) { } // Ignore an error; the file will continue to exist in the destination album directory

				result.Status = ActionResultStatus.Error.ToString();
				result.Message = ex.Message;
			}

			return result;
		}

		/// <summary>
		/// Determines whether the <paramref name="galleryObject" /> has an optimized media object.
		/// </summary>
		/// <param name="galleryObject">The gallery object.</param>
		/// <returns>
		///   <c>true</c> if it has an optimized media object; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		private static bool HasOptimizedVersion(IGalleryObject galleryObject)
		{
			if (galleryObject == null)
				throw new ArgumentNullException("galleryObject");

			if (galleryObject.GalleryObjectType == GalleryObjectType.Album)
				return false;

			bool inQueue = MediaConversionQueue.Instance.IsWaitingInQueueOrProcessing(galleryObject.Id, MediaQueueItemConversionType.CreateOptimized);
			bool hasOptFile = !String.IsNullOrEmpty(galleryObject.Optimized.FileName);
			bool optFileDifferentThanOriginal = (galleryObject.Optimized.FileName != galleryObject.Original.FileName);

			return (inQueue || (hasOptFile && optFileDifferentThanOriginal));
		}

		/// <summary>
		/// Determines whether the <paramref name="galleryObject" /> has an original media object.
		/// Generally, all media objects do have one and all albums do not.
		/// </summary>
		/// <param name="galleryObject">The gallery object.</param>
		/// <returns>
		///   <c>true</c> if it has an original media object; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		private static bool HasOriginalVersion(IGalleryObject galleryObject)
		{
			if (galleryObject == null)
				throw new ArgumentNullException("galleryObject");

			return !(galleryObject.Original is NullDisplayObject);
		}

		/// <summary>
		/// Gets a collection of views corresponding to the gallery object and other specs in <paramref name="moBuilderOptions" />.
		/// </summary>
		/// <param name="moBuilderOptions">A set of properties to be used when building the output.</param>
		/// <returns>Returns a collection of <see cref="Entity.DisplayObject" /> instances.</returns>
		private static List<Entity.DisplayObject> GetViews(MediaObjectHtmlBuilderOptions moBuilderOptions)
		{
			var views = new List<Entity.DisplayObject>(3);

			moBuilderOptions.DisplayType = DisplayObjectType.Thumbnail;

			var moBuilder = new MediaObjectHtmlBuilder(moBuilderOptions);

			views.Add(new Entity.DisplayObject
			{
				ViewSize = (int)DisplayObjectType.Thumbnail,
				ViewType = (int)moBuilder.MimeType.TypeCategory,
				HtmlOutput = moBuilder.GenerateHtml(),
				ScriptOutput = moBuilder.GenerateScript(),
				Width = moBuilder.Width,
				Height = moBuilder.Height,
				Url = moBuilder.GetMediaObjectUrl()
			});

			if (HasOptimizedVersion(moBuilderOptions.GalleryObject))
			{
				moBuilderOptions.DisplayType = DisplayObjectType.Optimized;

				moBuilder = new MediaObjectHtmlBuilder(moBuilderOptions);

				views.Add(new Entity.DisplayObject
				{
					ViewSize = (int)DisplayObjectType.Optimized,
					ViewType = (int)moBuilder.MimeType.TypeCategory,
					HtmlOutput = moBuilder.GenerateHtml(),
					ScriptOutput = moBuilder.GenerateScript(),
					Width = moBuilder.Width,
					Height = moBuilder.Height,
					Url = moBuilder.GetMediaObjectUrl()
				});
			}

			if (HasOriginalVersion(moBuilderOptions.GalleryObject))
			{
				moBuilderOptions.DisplayType = moBuilderOptions.GalleryObject.Original.DisplayType; // May be Original or External

				moBuilder = new MediaObjectHtmlBuilder(moBuilderOptions);

				views.Add(new Entity.DisplayObject
				{
					ViewSize = (int)DisplayObjectType.Original,
					ViewType = (int)moBuilder.MimeType.TypeCategory,
					HtmlOutput = moBuilder.GenerateHtml(),
					ScriptOutput = moBuilder.GenerateScript(),
					Width = moBuilder.Width,
					Height = moBuilder.Height,
					Url = moBuilder.GetMediaObjectUrl()
				});
			}

			return views;
		}

		/// <summary>
		/// Persists the current user's sort preference for the specified <paramref name="album" />. No action is taken if the 
		/// album is virtual. Anonymous user data is stored in session only; logged on users' data are permanently stored.
		/// </summary>
		/// <param name="album">The album whose sort preference is to be preserved.</param>
		/// <param name="sortByMetaName">Name of the metadata item to sort by.</param>
		/// <param name="sortAscending">Indicates the sort direction.</param>
		private static void PersistUserSortPreference(IAlbum album, MetadataItemName sortByMetaName, bool sortAscending)
		{
			if (album.IsVirtualAlbum)
				return;

			var profile = ProfileController.GetProfile();

			var aProfile = profile.AlbumProfiles.Find(album.Id);

			if (aProfile == null)
			{
				profile.AlbumProfiles.Add(new AlbumProfile(album.Id, sortByMetaName, sortAscending));
			}
			else
			{
				aProfile.SortByMetaName = sortByMetaName;
				aProfile.SortAscending = sortAscending;
			}

			ProfileController.SaveProfile(profile);
		}

		/// <summary>
		/// Gets the title for the album that is appropriate for the specified <paramref name="rating" />.
		/// </summary>
		/// <param name="rating">The rating. Valid values include "highest", "lowest", "none", or a decimal.</param>
		/// <returns>System.String.</returns>
		private static string GetRatedAlbumTitle(string rating)
		{
			switch (rating.ToLowerInvariant())
			{
				case "highest":
					return Resources.GalleryServerPro.Site_Highest_Rated_Title; // "Highest rated items"
				case "lowest":
					return Resources.GalleryServerPro.Site_Lowest_Rated_Title; // "Lowest rated items"
				case "none":
					return Resources.GalleryServerPro.Site_None_Rated_Title; // "Items without a rating"
				default:
					return String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Site_Rated_Title, rating); // "Items with a rating of 3"
			}
		}

		#endregion
	}
}
