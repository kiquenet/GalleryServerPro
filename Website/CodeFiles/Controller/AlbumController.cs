using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Business.NullObjects;
using GalleryServerPro.Events.CustomExceptions;
using System.Globalization;
using System.Linq;
using GalleryServerPro.Web.Entity;

namespace GalleryServerPro.Web.Controller
{
	/// <summary>
	/// Contains functionality for interacting with albums. Typically web pages directly call the appropriate business layer objects,
	/// but when a task involves multiple steps or the functionality does not exist in the business layer, the methods here are
	/// used.
	/// </summary>
	public static class AlbumController
	{
		#region Public Static Methods

		/// <summary>
		/// Generate a read-only, inflated <see cref="IAlbum" /> instance with optionally inflated child media objects. Metadata 
		/// for media objects are automatically loaded. The album's <see cref="IAlbum.ThumbnailMediaObjectId" /> property is set 
		/// to its value from the data store, but the <see cref="IGalleryObject.Thumbnail" /> property is only inflated when 
		/// accessed. Guaranteed to not return null.
		/// </summary>
		/// <param name="albumId">The <see cref="IGalleryObject.Id">ID</see> that uniquely identifies the album to retrieve.</param>
		/// <param name="inflateChildMediaObjects">When true, the child media objects of the album are added and inflated.
		/// Child albums are added but not inflated. When false, they are not added or inflated.</param>
		/// <returns>Returns an inflated album instance with all properties set to the values from the data store.</returns>
		/// <exception cref="InvalidAlbumException">Thrown when an album with the specified <paramref name = "albumId" /> 
		/// is not found in the data store.</exception>
		public static IAlbum LoadAlbumInstance(int albumId, bool inflateChildMediaObjects)
		{
			return LoadAlbumInstance(albumId, inflateChildMediaObjects, false, true);
		}

		/// <summary>
		/// Generate an inflated <see cref="IAlbum" /> instance with optionally inflated child media objects. Metadata 
		/// for media objects are automatically loaded. Use the <paramref name="isWritable" /> parameter to specify a writeable, 
		/// thread-safe instance that can be modified and persisted to the data store. The 
		/// album's <see cref="IAlbum.ThumbnailMediaObjectId" /> property is set to its value from the data store, but the 
		/// <see cref="IGalleryObject.Thumbnail" /> property is only inflated when accessed. Guaranteed to not return null.
		/// </summary>
		/// <param name="albumId">The <see cref="IGalleryObject.Id">ID</see> that uniquely identifies the album to retrieve.</param>
		/// <param name="inflateChildMediaObjects">When true, the child media objects of the album are added and inflated.
		/// Child albums are added but not inflated. When false, they are not added or inflated.</param>
		/// <param name="isWritable">When set to <c>true</c> then return a unique instance that is not shared across threads.</param>
		/// <returns>Returns an inflated album instance with all properties set to the values from the data store.</returns>
		/// <exception cref="InvalidAlbumException">Thrown when an album with the specified <paramref name = "albumId" /> 
		/// is not found in the data store.</exception>
		public static IAlbum LoadAlbumInstance(int albumId, bool inflateChildMediaObjects, bool isWritable)
		{
			return LoadAlbumInstance(albumId, inflateChildMediaObjects, isWritable, true);
		}

		/// <summary>
		/// Generate an inflated <see cref="IAlbum" /> instance with optionally inflated child media objects, and optionally specifying
		/// whether to suppress the loading of media object metadata. Use the <paramref name="isWritable" />
		/// parameter to specify a writeable, thread-safe instance that can be modified and persisted to the data store. The 
		/// album's <see cref="IAlbum.ThumbnailMediaObjectId" /> property is set to its value from the data store, but the 
		/// <see cref="IGalleryObject.Thumbnail" /> property is only inflated when accessed. Guaranteed to not return null.
		/// </summary>
		/// <param name="albumId">The <see cref="IGalleryObject.Id">ID</see> that uniquely identifies the album to retrieve.</param>
		/// <param name="inflateChildMediaObjects">When true, the child media objects of the album are added and inflated.
		/// Child albums are added but not inflated. When false, they are not added or inflated.</param>
		/// <param name="isWritable">When set to <c>true</c> then return a unique instance that is not shared across threads.</param>
		/// <param name="allowMetadataLoading">If set to <c>false</c>, the metadata for media objects are not loaded.</param>
		/// <returns>Returns an inflated album instance with all properties set to the values from the data store.</returns>
		/// <exception cref="InvalidAlbumException">Thrown when an album with the specified <paramref name = "albumId" /> 
		/// is not found in the data store.</exception>
		public static IAlbum LoadAlbumInstance(int albumId, bool inflateChildMediaObjects, bool isWritable, bool allowMetadataLoading)
		{
			IAlbum album = Factory.LoadAlbumInstance(albumId, inflateChildMediaObjects, isWritable, allowMetadataLoading);

			ValidateAlbumOwner(album);

			return album;
		}

		/// <summary>
		/// Creates an album, assigns the user name as the owner, saves it, and returns the newly created album.
		/// A profile entry is created containing the album ID. Returns null if the ID specified in the gallery settings
		/// for the parent album does not represent an existing album. That is, returns null if <see cref="IGallerySettings.UserAlbumParentAlbumId" />
		/// does not match an existing album.
		/// </summary>
		/// <param name="userName">The user name representing the user who is the owner of the album.</param>
		/// <param name="galleryId">The gallery ID for the gallery in which the album is to be created.</param>
		/// <returns>
		/// Returns the newly created user album. It has already been persisted to the database.
		/// Returns null if the ID specified in the gallery settings for the parent album does not represent an existing album.
		/// That is, returns null if <see cref="IGallerySettings.UserAlbumParentAlbumId" />
		/// does not match an existing album.
		/// </returns>
		public static IAlbum CreateUserAlbum(string userName, int galleryId)
		{
			IGallerySettings gallerySetting = Factory.LoadGallerySetting(galleryId);

			string albumNameTemplate = gallerySetting.UserAlbumNameTemplate;

			IAlbum parentAlbum;
			try
			{
				parentAlbum = AlbumController.LoadAlbumInstance(gallerySetting.UserAlbumParentAlbumId, false);
			}
			catch (InvalidAlbumException ex)
			{
				// The parent album does not exist. Record the error and return null.
				string galleryDescription = Utils.HtmlEncode(Factory.LoadGallery(gallerySetting.GalleryId).Description);
				string msg = String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Error_User_Album_Parent_Invalid_Ex_Msg, galleryDescription, gallerySetting.UserAlbumParentAlbumId);
				AppEventController.LogError(new WebException(msg, ex), galleryId);
				return null;
			}

			IAlbum album = Factory.CreateEmptyAlbumInstance(parentAlbum.GalleryId);

			album.Title = albumNameTemplate.Replace("{UserName}", userName);
			album.Caption = gallerySetting.UserAlbumSummaryTemplate;
			album.OwnerUserName = userName;
			//newAlbum.ThumbnailMediaObjectId = 0; // not needed
			album.Parent = parentAlbum;
			album.IsPrivate = parentAlbum.IsPrivate;
			GalleryObjectController.SaveGalleryObject(album, userName);

			SaveAlbumIdToProfile(album.Id, userName, album.GalleryId);

			HelperFunctions.PurgeCache();

			return album;
		}

		/// <summary>
		/// Get a reference to the highest level album in the specified <paramref name="galleryId" /> the current user has permission 
		/// to add albums to. Returns null if no album meets this criteria.
		/// </summary>
		/// <param name="galleryId">The ID of the gallery.</param>
		/// <returns>Returns a reference to the highest level album the user has permission to add albums to.</returns>
		public static IAlbum GetHighestLevelAlbumWithCreatePermission(int galleryId)
		{
			// Step 1: Loop through the roles and compile a list of album IDs where the role has create album permission.
			IGallery gallery = Factory.LoadGallery(galleryId);
			List<int> rootAlbumIdsWithCreatePermission = new List<int>();

			foreach (IGalleryServerRole role in RoleController.GetGalleryServerRolesForUser())
			{
				if (role.Galleries.Contains(gallery))
				{
					if (role.AllowAddChildAlbum)
					{
						foreach (int albumId in role.RootAlbumIds)
						{
							if (!rootAlbumIdsWithCreatePermission.Contains(albumId))
								rootAlbumIdsWithCreatePermission.Add(albumId);
						}
					}
				}
			}

			// Step 2: Loop through our list of album IDs. If any album belongs to another gallery, remove it. If any album has an ancestor 
			// that is also in the list, then remove it. We only want a list of top level albums.
			List<int> albumIdsToRemove = new List<int>();
			foreach (int albumIdWithCreatePermission in rootAlbumIdsWithCreatePermission)
			{
				IGalleryObject album = AlbumController.LoadAlbumInstance(albumIdWithCreatePermission, false);

				if (album.GalleryId != galleryId)
				{
					// Album belongs to another gallery. Mark it for deletion.
					albumIdsToRemove.Add(albumIdWithCreatePermission);
				}
				else
				{
					while (true)
					{
						album = album.Parent as IAlbum;
						if (album == null)
							break;

						if (rootAlbumIdsWithCreatePermission.Contains(album.Id))
						{
							// Album has an ancestor that is also in the list. Mark it for deletion.
							albumIdsToRemove.Add(albumIdWithCreatePermission);
							break;
						}
					}
				}
			}

			foreach (int albumId in albumIdsToRemove)
			{
				rootAlbumIdsWithCreatePermission.Remove(albumId);
			}

			// Step 3: Starting with the root album, start iterating through the child albums. When we get to
			// one in our list, we can conclude that is the highest level album for which the user has create album permission.
			return FindFirstMatchingAlbumRecursive(Factory.LoadRootAlbumInstance(galleryId), rootAlbumIdsWithCreatePermission);
		}

		/// <summary>
		/// Get a reference to the highest level album in the specified <paramref name="galleryId" /> the current user has permission to 
		/// add albums and/or media objects to. Returns null if no album meets this criteria.
		/// </summary>
		/// <param name="verifyAddAlbumPermissionExists">Specifies whether the current user must have permission to add child albums
		/// to the album.</param>
		/// <param name="verifyAddMediaObjectPermissionExists">Specifies whether the current user must have permission to add media objects
		/// to the album.</param>
		/// <param name="galleryId">The ID of the gallery.</param>
		/// <returns>
		/// Returns a reference to the highest level album the user has permission to add albums and/or media objects to.
		/// </returns>
		public static IAlbum GetHighestLevelAlbumWithAddPermission(bool verifyAddAlbumPermissionExists, bool verifyAddMediaObjectPermissionExists, int galleryId)
		{
			// Step 1: Loop through the roles and compile a list of album IDs where the role has the required permission.
			// If the verifyAddAlbumPermissionExists parameter is true, then the user must have permission to add child albums.
			// If the verifyAddMediaObjectPermissionExists parameter is true, then the user must have permission to add media objects.
			// If either parameter is false, then the absense of that permission does not disqualify an album.
			IGallery gallery = Factory.LoadGallery(galleryId);

			List<int> rootAlbumIdsWithPermission = new List<int>();
			foreach (IGalleryServerRole role in RoleController.GetGalleryServerRolesForUser())
			{
				if (role.Galleries.Contains(gallery))
				{
					bool albumPermGranted = (verifyAddAlbumPermissionExists ? role.AllowAddChildAlbum : true);
					bool mediaObjectPermGranted = (verifyAddMediaObjectPermissionExists ? role.AllowAddMediaObject : true);

					if (albumPermGranted && mediaObjectPermGranted)
					{
						// This role satisfies the requirements, so add each album to the list.
						foreach (int albumId in role.RootAlbumIds)
						{
							if (!rootAlbumIdsWithPermission.Contains(albumId))
								rootAlbumIdsWithPermission.Add(albumId);
						}
					}
				}
			}

			// Step 2: Loop through our list of album IDs. If any album belongs to another gallery, remove it. If any album has an ancestor 
			// that is also in the list, then remove it. We only want a list of top level albums.
			List<int> albumIdsToRemove = new List<int>();
			foreach (int albumIdWithPermission in rootAlbumIdsWithPermission)
			{
				IGalleryObject album = AlbumController.LoadAlbumInstance(albumIdWithPermission, false);

				if (album.GalleryId != galleryId)
				{
					// Album belongs to another gallery. Mark it for deletion.
					albumIdsToRemove.Add(albumIdWithPermission);
				}
				else
				{
					while (true)
					{
						album = album.Parent as IAlbum;
						if (album == null)
							break;

						if (rootAlbumIdsWithPermission.Contains(album.Id))
						{
							// Album has an ancestor that is also in the list. Mark it for deletion.
							albumIdsToRemove.Add(albumIdWithPermission);
							break;
						}
					}
				}
			}

			foreach (int albumId in albumIdsToRemove)
			{
				rootAlbumIdsWithPermission.Remove(albumId);
			}

			// Step 3: Starting with the root album, start iterating through the child albums. When we get to
			// one in our list, we can conclude that is the highest level album for which the user has create album permission.
			return FindFirstMatchingAlbumRecursive(Factory.LoadRootAlbumInstance(galleryId), rootAlbumIdsWithPermission);
		}

		/// <summary>
		/// Gets the meta items for the specified album <paramref name="id" />.
		/// </summary>
		/// <param name="id">The album ID.</param>
		/// <returns></returns>
		/// <exception cref="GalleryServerPro.Events.CustomExceptions.GallerySecurityException">Thrown when the 
		/// user does not have view permission to the specified album.</exception>
		public static Entity.MetaItem[] GetMetaItemsForAlbum(int id)
		{
			IAlbum album = Factory.LoadAlbumInstance(id, false);
			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);

			return GalleryObjectController.ToMetaItems(album.MetadataItems.GetVisibleItems(), album);
		}

		/// <summary>
		/// Converts the <paramref name="albums" /> to an enumerable collection of 
		/// <see cref="Entity.Album" /> instances. Guaranteed to not return null.
		/// </summary>
		/// <param name="albums">The albums.</param>
		/// <returns>An enumerable collection of <see cref="Entity.Album" /> instances.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static Entity.Album[] ToAlbumEntities(IList<IGalleryObject> albums, Entity.GalleryDataLoadOptions options)
		{
			if (albums == null)
				throw new ArgumentNullException("albums");

			var albumEntities = new List<Entity.Album>(albums.Count);

			foreach (IGalleryObject album in albums)
			{
				albumEntities.Add(ToAlbumEntity((IAlbum)album, options));
			}

			return albumEntities.ToArray();
		}

		/// <summary>
		/// Gets a data entity containing information about the current album. The instance can be JSON-parsed and sent to the
		/// browser. Returns null if the requested album does not exist or the user does not have permission to view it.
		/// </summary>
		/// <param name="album">The album.</param>
		/// <param name="options">Specifies options for configuring the return data. To use default
		/// settings, specify an empty instance with properties left at default values.</param>
		/// <returns>
		/// Returns <see cref="Entity.Album" /> object containing information about the current album.
		/// </returns>
		/// <overloads>
		/// Converts the <paramref name="album" /> to an instance of <see cref="Entity.Album" />.
		///   </overloads>
		public static Entity.Album ToAlbumEntity(IAlbum album, Entity.GalleryDataLoadOptions options)
		{
			try
			{
				return ToAlbumEntity(album, GetPermissionsEntity(album), options);
			}
			catch (InvalidAlbumException) { return null; }
			catch (GallerySecurityException) { return null; }
		}

		/// <summary>
		/// Gets a data entity containing album information for the specified <paramref name="album" />. Returns an object with empty
		/// properties if the user does not have permission to view the specified album. The instance can be JSON-parsed and sent to the
		/// browser.
		/// </summary>
		/// <param name="album">The album to convert to an instance of <see cref="GalleryServerPro.Web.Entity.Album" />.</param>
		/// <param name="perms">The permissions the current user has for the album.</param>
		/// <param name="options">Specifies options for configuring the return data. To use default
		/// settings, specify an empty instance with properties left at default values.</param>
		/// <returns>
		/// Returns an <see cref="GalleryServerPro.Web.Entity.Album" /> object containing information about the requested album.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="album" /> is null.</exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static Entity.Album ToAlbumEntity(IAlbum album, Entity.Permissions perms, Entity.GalleryDataLoadOptions options)
		{
			if (album == null)
				throw new ArgumentNullException("album");

			var albumEntity = new Entity.Album();

			albumEntity.Id = album.Id;
			albumEntity.GalleryId = album.GalleryId;
			albumEntity.Title = album.Title;
			albumEntity.Caption = album.Caption;
			albumEntity.Owner = (perms.AdministerGallery ? album.OwnerUserName : null);
			albumEntity.InheritedOwners = (perms.AdministerGallery ? String.Join(", ", album.InheritedOwners) : null);
			albumEntity.DateStart = album.DateStart;
			albumEntity.DateEnd = album.DateEnd;
			albumEntity.IsPrivate = album.IsPrivate;
			albumEntity.VirtualType = (int)album.VirtualAlbumType;
			albumEntity.RssUrl = GetRssUrl(album);
			albumEntity.Permissions = perms;
			albumEntity.MetaItems = GalleryObjectController.ToMetaItems(album.MetadataItems.GetVisibleItems(), album);
			albumEntity.NumAlbums = album.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).Count;

			// Optionally load gallery items
			if (options.LoadGalleryItems)
			{
				var albumSortDef = ProfileController.GetProfile().AlbumProfiles.Find(album.Id);

				IList<IGalleryObject> items;
				if (albumSortDef != null)
				{
					items = album
						.GetChildGalleryObjects(options.Filter, !Utils.IsAuthenticated)
						.ToSortedList(albumSortDef.SortByMetaName, albumSortDef.SortAscending, album.GalleryId);

					albumEntity.SortById = (int)albumSortDef.SortByMetaName;
					albumEntity.SortUp = albumSortDef.SortAscending;
				}
				else
				{
					if (album.IsVirtualAlbum)
					{
						items = album.GetChildGalleryObjects(options.Filter, !Utils.IsAuthenticated).ToSortedList(album.SortByMetaName, album.SortAscending, album.GalleryId);
					}
					else
					{
						// Real (non-virtual) albums are already sorted on their Seq property, so return items based on that.
						items = album.GetChildGalleryObjects(options.Filter, !Utils.IsAuthenticated).ToSortedList();
					}

					albumEntity.SortById = (int)album.SortByMetaName;
					albumEntity.SortUp = album.SortAscending;
				}

				if (options.NumGalleryItemsToRetrieve > 0)
					items = items.Skip(options.NumGalleryItemsToSkip).Take(options.NumGalleryItemsToRetrieve).ToList();

				albumEntity.GalleryItems = GalleryObjectController.ToGalleryItems(items);
				albumEntity.NumGalleryItems = albumEntity.GalleryItems.Length;
			}
			else
			{
				albumEntity.NumGalleryItems = album.GetChildGalleryObjects(options.Filter, !Utils.IsAuthenticated).Count;
			}

			// Optionally load media items
			if (options.LoadMediaItems)
			{
				IList<IGalleryObject> items;

				if (album.IsVirtualAlbum)
				{
					items = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList(album.SortByMetaName, album.SortAscending, album.GalleryId);
				}
				else
				{
					// Real (non-virtual) albums are already sorted on their Seq property, so return items based on that.
					items = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList();
				}

				//IList<IGalleryObject> items = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).ToSortedList();
				albumEntity.NumMediaItems = items.Count;
				albumEntity.MediaItems = GalleryObjectController.ToMediaItems(items);
			}
			else
			{
				albumEntity.NumMediaItems = album.GetChildGalleryObjects(GalleryObjectType.MediaObject, !Utils.IsAuthenticated).Count;
			}

			return albumEntity;
		}

		/// <summary>
		/// Gets a data entity containing permission information for the specified <paramref name="album" />.
		/// The instance can be JSON-parsed and sent to the browser. The permissions take into account whether the media files
		/// are configured as read only (<see cref="IGallerySettings.MediaObjectPathIsReadOnly" />).
		/// </summary>
		/// <returns>
		/// Returns <see cref="Entity.Permissions"/> object containing permission information.
		/// </returns>
		private static Entity.Permissions GetPermissionsEntity(IAlbum album)
		{
			int albumId = album.Id;
			int galleryId = album.GalleryId;
			bool isPrivate = album.IsPrivate;
			bool isVirtual = album.IsVirtualAlbum;
			var rootAlbum = Factory.LoadRootAlbumInstance(album.GalleryId);
			IGalleryServerRoleCollection roles = RoleController.GetGalleryServerRolesForUser();
			var isAdmin = Utils.IsUserAuthorized(SecurityActions.AdministerSite, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, isVirtual);
			var isGalleryAdmin = isAdmin || Utils.IsUserAuthorized(SecurityActions.AdministerGallery, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, isVirtual);
			var isGalleryWriteable = !Factory.LoadGallerySetting(galleryId).MediaObjectPathIsReadOnly;

			var perms = new Entity.Permissions();

			perms.AdministerGallery = isGalleryAdmin;
			perms.AdministerSite = isAdmin;

			if (album.IsVirtualAlbum)
			{
				// When we have a virtual album we use the permissions assigned to the root album. 
				perms.ViewAlbumOrMediaObject = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.ViewAlbumOrMediaObject, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum);
				perms.ViewOriginalMediaObject = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.ViewOriginalMediaObject, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum);
				perms.AddChildAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.AddChildAlbum, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.AddMediaObject = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.AddMediaObject, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.EditAlbum = false;
				perms.EditMediaObject = (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.EditMediaObject, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.DeleteAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteAlbum, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.DeleteChildAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteChildAlbum, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.DeleteMediaObject = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteMediaObject, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum));
				perms.Synchronize = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.Synchronize, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum);
				perms.HideWatermark = Utils.IsUserAuthorized(SecurityActions.HideWatermark, roles, rootAlbum.Id, galleryId, rootAlbum.IsPrivate, rootAlbum.IsVirtualAlbum);
			}
			else
			{
				perms.ViewAlbumOrMediaObject = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.ViewAlbumOrMediaObject, roles, albumId, galleryId, isPrivate, isVirtual);
				perms.ViewOriginalMediaObject = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.ViewOriginalMediaObject, roles, albumId, galleryId, isPrivate, isVirtual);
				perms.AddChildAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.AddChildAlbum, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.AddMediaObject = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.AddMediaObject, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.EditAlbum = (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.EditAlbum, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.EditMediaObject = (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.EditMediaObject, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.DeleteAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteAlbum, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.DeleteChildAlbum = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteChildAlbum, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.DeleteMediaObject = isGalleryWriteable && (isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.DeleteMediaObject, roles, albumId, galleryId, isPrivate, isVirtual));
				perms.Synchronize = isGalleryAdmin || Utils.IsUserAuthorized(SecurityActions.Synchronize, roles, albumId, galleryId, isPrivate, isVirtual);
				perms.HideWatermark = Utils.IsUserAuthorized(SecurityActions.HideWatermark, roles, albumId, galleryId, isPrivate, isVirtual);
			}

			return perms;
		}

		/// <summary>
		/// Update the album with the specified properties in the albumEntity parameter. Only the following properties are
		/// persisted: <see cref="Entity.Album.DateStart" />, <see cref="Entity.Album.DateEnd" />, <see cref="Entity.Album.SortById" />,
		/// <see cref="Entity.Album.SortUp" />, <see cref="Entity.Album.IsPrivate" />, <see cref="Entity.Album.Owner" />
		/// </summary>
		/// <param name="album">An <see cref="Entity.Album" /> instance containing data to be persisted to the data store.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="album" /> is null.</exception>
		/// <exception cref="GalleryServerPro.Events.CustomExceptions.GallerySecurityException">Thrown when the 
		/// user does not have edit permission to the specified album.</exception>
		public static void UpdateAlbumInfo(Entity.Album album)
		{
			if (album == null)
				throw new ArgumentNullException("album");

			if (album.Owner == Resources.GalleryServerPro.UC_Album_Header_Edit_Album_No_Owner_Text)
			{
				album.Owner = String.Empty;
			}

			var alb = AlbumController.LoadAlbumInstance(album.Id, false, true);

			// Update remaining properties if user has edit album permission.
			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.EditAlbum, RoleController.GetGalleryServerRolesForUser(), alb.Id, alb.GalleryId, Utils.IsAuthenticated, alb.IsPrivate, alb.IsVirtualAlbum);

			// OBSOLETE: As of 3.0, the title is updated through the metadata controller.
			//if (alb.Title != album.Title)
			//{
			//	IGallerySettings gallerySetting = Factory.LoadGallerySetting(alb.GalleryId);

			//	alb.Title = Utils.CleanHtmlTags(album.Title, alb.GalleryId);
			//	if ((!alb.IsRootAlbum) && (gallerySetting.SynchAlbumTitleAndDirectoryName))
			//	{
			//		// Root albums do not have a directory name that reflects the album's title, so only update this property for non-root albums.
			//		alb.DirectoryName = HelperFunctions.ValidateDirectoryName(alb.Parent.FullPhysicalPath, alb.Title, gallerySetting.DefaultAlbumDirectoryNameLength);
			//	}
			//}

			//alb.Summary = Utils.CleanHtmlTags(album.Summary, alb.GalleryId);

			alb.DateStart = album.DateStart.Date;
			alb.DateEnd = album.DateEnd.Date;
			alb.SortByMetaName = (MetadataItemName)album.SortById;
			alb.SortAscending = album.SortUp;

			if (album.IsPrivate != alb.IsPrivate)
			{
				if (!album.IsPrivate && alb.Parent.IsPrivate)
				{
					throw new NotSupportedException("Cannot make album public: It is invalid to make an album public when it's parent album is private.");
				}
				alb.IsPrivate = album.IsPrivate;

				var userName = Utils.UserName;
				Task.Factory.StartNew(() => SynchIsPrivatePropertyOnChildGalleryObjects(alb, userName));
			}

			// If the owner has changed, update it, but only if the user is administrator.
			if (album.Owner != alb.OwnerUserName)
			{
				if (Utils.IsUserAuthorized(SecurityActions.AdministerSite | SecurityActions.AdministerGallery, RoleController.GetGalleryServerRolesForUser(), alb.Id, alb.GalleryId, alb.IsPrivate, alb.IsVirtualAlbum))
				{
					if (!String.IsNullOrEmpty(alb.OwnerUserName))
					{
						// Another user was previously assigned as owner. Delete role since this person will no longer be the owner.
						RoleController.DeleteGalleryServerProRole(alb.OwnerRoleName);
					}

					if (UserController.GetUsersCurrentUserCanView(alb.GalleryId).Contains(album.Owner) || String.IsNullOrEmpty(album.Owner))
					{
						// GalleryObjectController.SaveGalleryObject will make sure there is a role created for this user.
						alb.OwnerUserName = album.Owner ?? String.Empty;
					}
				}
			}

			GalleryObjectController.SaveGalleryObject(alb);
			HelperFunctions.PurgeCache();
		}

		/// <overloads>
		/// Permanently delete this album from the data store and optionally the hard drive.
		/// </overloads>
		/// <summary>
		/// Permanently delete this album from the data store and optionally the hard drive. Validation is performed prior to deletion to ensure
		/// current user has delete permission and the album can be safely deleted. The validation is contained in the method 
		/// <see cref="ValidateBeforeAlbumDelete"/> and may be invoked separately if desired.
		/// </summary>
		/// <param name="albumId">The ID of the album to delete.</param>
		/// <exception cref="CannotDeleteAlbumException">Thrown when the album does not meet the requirements for safe deletion.
		/// This includes detecting when the media objects path is read only and when the album is or contains the user album
		/// parent album and user albums are enabled.</exception>
		/// <exception cref="InvalidAlbumException">Thrown when <paramref name="albumId" /> does not represent an existing album.</exception>
		/// <exception cref="GallerySecurityException">Thrown when the current user does not have permission to delete the album.</exception>
		public static void DeleteAlbum(int albumId)
		{
			DeleteAlbum(AlbumController.LoadAlbumInstance(albumId, false));
		}

		/// <summary>
		/// Permanently delete this album from the data store and optionally the hard drive. Validation is performed prior to deletion to ensure
		/// current user has delete permission and the album can be safely deleted. The validation is contained in the method 
		/// <see cref="ValidateBeforeAlbumDelete"/> and may be invoked separately if desired.
		/// </summary>
		/// <param name="album">The album to delete. If null, the function returns without taking any action.</param>
		/// <param name="deleteFromFileSystem">if set to <c>true</c> the files and directories associated with the album
		/// are deleted from the hard disk. Set this to <c>false</c> to delete only the database records.</param>
		/// <exception cref="CannotDeleteAlbumException">Thrown when the album does not meet the requirements for safe deletion.
		/// This includes detecting when the media objects path is read only and when the album is or contains the user album
		/// parent album and user albums are enabled.</exception>
		/// <exception cref="GallerySecurityException">Thrown when the current user does not have permission to delete the album.</exception>
		public static void DeleteAlbum(IAlbum album, bool deleteFromFileSystem = true)
		{
			if (album == null)
				return;

			ValidateBeforeAlbumDelete(album);

			OnBeforeAlbumDelete(album);

			if (deleteFromFileSystem)
			{
				album.Delete();
			}
			else
			{
				album.DeleteFromGallery();
			}

			HelperFunctions.PurgeCache();
		}

		/// <summary>
		/// Verifies that the album meets the prerequisites to be safely deleted but does not actually delete the album. Throws a
		/// <see cref="CannotDeleteAlbumException" /> when deleting it would violate a business rule. Throws a
		/// <see cref="GallerySecurityException" /> when the current user does not have permission to delete the album.
		/// </summary>
		/// <param name="albumToDelete">The album to delete.</param>
		/// <remarks>This function is automatically called when using the <see cref="DeleteAlbum(IAlbum, bool)"/> method, so it is not necessary to 
		/// invoke when using that method. Typically you will call this method when there are several items to delete and you want to 
		/// check all of them before deleting any of them, such as we have on the Delete Objects page.</remarks>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="albumToDelete" /> is null.</exception>
		/// <exception cref="CannotDeleteAlbumException">Thrown when the album does not meet the 
		/// requirements for safe deletion.</exception>
		/// <exception cref="GallerySecurityException">Thrown when the current user does not have permission to delete the album.</exception>
		public static void ValidateBeforeAlbumDelete(IAlbum albumToDelete)
		{
			if (albumToDelete == null)
				throw new ArgumentNullException("albumToDelete");

			var userAlbum = UserController.GetUserAlbum(Utils.UserName, albumToDelete.GalleryId);
			var curUserDeletingOwnUserAlbum = (userAlbum != null && userAlbum.Id == albumToDelete.Id);
			// Skip security check when user is deleting their own user album. Normally this won't happen (the menu action for deleting will be 
			// disabled), but it will happen when they delete their user album or their account on the account page, and this is one situation 
			// where it is OK for them to delete their album.
			if (!curUserDeletingOwnUserAlbum)
			{
				SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.DeleteAlbum, RoleController.GetGalleryServerRolesForUser(), albumToDelete.Id, albumToDelete.GalleryId, Utils.IsAuthenticated, albumToDelete.IsPrivate, albumToDelete.IsVirtualAlbum);
			}

			if (Factory.LoadGallerySetting(albumToDelete.GalleryId).MediaObjectPathIsReadOnly)
			{
				throw new CannotDeleteAlbumException(Resources.GalleryServerPro.Task_Delete_Album_Cannot_Delete_MediaPathIsReadOnly);
			}

			var validator = new AlbumDeleteValidator(albumToDelete);

			validator.Validate();

			if (!validator.CanBeDeleted)
			{
				switch (validator.ValidationFailureReason)
				{
					case GalleryObjectDeleteValidationFailureReason.AlbumSpecifiedAsUserAlbumContainer:
					case GalleryObjectDeleteValidationFailureReason.AlbumContainsUserAlbumContainer:
						{
							string albumTitle = String.Concat("'", albumToDelete.Title, "' (ID# ", albumToDelete.Id, ")");
							string msg = String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Task_Delete_Album_Cannot_Delete_Contains_User_Album_Parent_Ex_Msg, albumTitle);

							throw new CannotDeleteAlbumException(msg);
						}
					case GalleryObjectDeleteValidationFailureReason.AlbumSpecifiedAsDefaultGalleryObject:
					case GalleryObjectDeleteValidationFailureReason.AlbumContainsDefaultGalleryObjectAlbum:
					case GalleryObjectDeleteValidationFailureReason.AlbumContainsDefaultGalleryObjectMediaObject:
						{
							string albumTitle = String.Concat("'", albumToDelete.Title, "' (ID# ", albumToDelete.Id, ")");
							string msg = String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Task_Delete_Album_Cannot_Delete_Contains_Default_Gallery_Object_Ex_Msg, albumTitle);

							throw new CannotDeleteAlbumException(msg);
						}
					default:
						throw new InvalidEnumArgumentException(String.Format(CultureInfo.CurrentCulture, "The function ValidateBeforeAlbumDelete is not designed to handle the enumeration value {0}. The function must be updated.", validator.ValidationFailureReason));
				}
			}
		}

		/// <summary>
		/// Gets the URL to the specified <paramref name="album" />.
		/// </summary>
		/// <param name="album">The album.</param>
		/// <returns>System.String.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown when the function encounters a virtual album
		/// type it was not designed to handle.</exception>
		public static string GetUrl(IAlbum album)
		{
			var appPath = Utils.GetCurrentPageUrl();

			switch (album.VirtualAlbumType)
			{
				case VirtualAlbumType.NotSpecified:
				case VirtualAlbumType.NotVirtual:
					return Utils.GetUrl(PageId.album, "aid={0}", album.Id);
				case VirtualAlbumType.Root:
					return appPath;
				case VirtualAlbumType.TitleOrCaption:
					return Utils.GetUrl(PageId.album, "title={0}", Utils.UrlEncode(Utils.GetQueryStringParameterString("title")));
				case VirtualAlbumType.Tag:
					return Utils.GetUrl(PageId.album, "tag={0}", Utils.UrlEncode(Utils.GetQueryStringParameterString("tag")));
				case VirtualAlbumType.People:
					return Utils.GetUrl(PageId.album, "people={0}", Utils.UrlEncode(Utils.GetQueryStringParameterString("people")));
				case VirtualAlbumType.Search:
					return Utils.GetUrl(PageId.album, "search={0}", Utils.UrlEncode(Utils.GetQueryStringParameterString("search")));
				case VirtualAlbumType.MostRecentlyAdded:
					return Utils.GetUrl(PageId.album, "latest={0}", Utils.GetQueryStringParameterInt32("latest"));
				case VirtualAlbumType.Rated:
					return Utils.GetUrl(PageId.album, "rating={0}&top={1}", Utils.GetQueryStringParameterString("rating"), Utils.GetQueryStringParameterInt32("top"));
				default:
					throw new InvalidOperationException(String.Format("The method AlbumController.GetUrl() encountered a VirtualAlbumType ({0}) it was not designed to handle. The developer must update this method.", album.VirtualAlbumType));
			}
		}

		/// <summary>
		/// Gets the RSS URL for the specified <paramref name="album" />. Returns null if the user is not 
		/// running the Enterprise version or no applicable RSS URL exists for the album. For example, 
		/// virtual root albums that are used for restricted users will return null.
		/// </summary>
		/// <param name="album">The album.</param>
		/// <returns>System.String.</returns>
		public static string GetRssUrl(IAlbum album)
		{
			if (AppSetting.Instance.License.LicenseType != LicenseLevel.Enterprise)
			{
				return null;
			}

			switch (album.VirtualAlbumType)
			{
				case VirtualAlbumType.NotVirtual:
					return String.Concat(Utils.AppRoot, "/api/feed/album?id=", album.Id);
				case VirtualAlbumType.TitleOrCaption:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/title?q={1}&galleryid={2}", Utils.AppRoot, Utils.GetQueryStringParameterString("title"), album.GalleryId);
				case VirtualAlbumType.Search:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/search?q={1}&galleryid={2}", Utils.AppRoot, Utils.GetQueryStringParameterString("search"), album.GalleryId);
				case VirtualAlbumType.Tag:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/tag?q={1}&galleryid={2}", Utils.AppRoot, Utils.GetQueryStringParameterString("tag"), album.GalleryId);
				case VirtualAlbumType.People:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/people?q={1}&galleryid={2}", Utils.AppRoot, Utils.GetQueryStringParameterString("people"), album.GalleryId);
				case VirtualAlbumType.MostRecentlyAdded:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/latest?galleryid={1}", Utils.AppRoot, album.GalleryId);
				case VirtualAlbumType.Rated:
					return String.Format(CultureInfo.InvariantCulture, "{0}/api/feed/rating?rating={1}&top={2}&galleryid={3}", Utils.AppRoot, Utils.GetQueryStringParameterString("rating"), Utils.GetQueryStringParameterInt32("top"), album.GalleryId);
				default:
					return null;
			}
		}

		/// <summary>
		/// Sorts the <paramref name="galleryItems" /> in the order in which they are passed.
		/// This method is used when a user is manually sorting an album and has dragged an item to a new position.
		/// </summary>
		/// <param name="galleryItems">The gallery objects to sort. Their position in the array indicates the desired
		/// sequence. Only <see cref="Entity.GalleryItem.Id" /> and <see cref="Entity.GalleryItem.ItemType" /> need be
		/// populated.</param>
		/// <param name="userName">Name of the logged on user.</param>
		public static void Sort(GalleryItem[] galleryItems, string userName)
		{
			if (galleryItems == null || galleryItems.Length == 0)
			{
				return;
			}

			try
			{
				// To improve performance, grab a writable collection of all the items in the album containing the first item.
				// At the time this function was written, the galleryItems parameter will only include items in a single album,
				// so this step allows us to get all the items in a single step. For robustness and to support all possible usage,
				// the code in the iteration manually loads a writable instance if it's not in the collection.
				var galleryObjects = GetWritableSiblingGalleryObjects(galleryItems[0]);

				var seq = 1;
				foreach (var galleryItem in galleryItems)
				{
					// Loop through each item and update its Sequence property to match the order in which it was passed.
					var item = galleryItem;

					var galleryObject = galleryObjects.FirstOrDefault(go => go.Id == item.Id && go.GalleryObjectType == (GalleryObjectType)item.ItemType);

					if (galleryObject == null)
					{
						// Not found, so load it manually. This is not expected to ever occur when manually sorting an album, but we 
						// include it for robustness.
						if ((GalleryObjectType)galleryItem.ItemType == GalleryObjectType.Album)
						{
							galleryObject = Factory.LoadAlbumInstance(galleryItem.Id, false, true);
						}
						else
						{
							galleryObject = Factory.LoadMediaObjectInstance(galleryItem.Id, true);
						}
					}

					galleryObject.Sequence = seq;
					GalleryObjectController.SaveGalleryObject(galleryObject, userName);
					seq++;
				}

				HelperFunctions.PurgeCache();
			}
			catch (Exception ex)
			{
				AppEventController.LogError(ex);

				throw;
			}
		}

		/// <summary>
		/// Re-sort the items in the album according to the criteria and store this updated sequence in the
		/// database. Callers must have <see cref="SecurityActions.EditAlbum" /> permission.
		/// </summary>
		/// <param name="albumId">The album ID.</param>
		/// <param name="sortByMetaNameId">The name of the metadata item to sort on.</param>
		/// <param name="sortAscending">If set to <c>true</c> sort in ascending order.</param>
		/// <exception cref="System.Web.Http.HttpResponseException"></exception>
		public static void Sort(int albumId, int sortByMetaNameId, bool sortAscending)
		{
			var album = LoadAlbumInstance(albumId, true, true);

			SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.EditAlbum, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);

			var oldSortByMetaName = album.SortByMetaName;
			var oldSortAscending = album.SortAscending;

			album.SortByMetaName = (MetadataItemName)sortByMetaNameId;
			album.SortAscending = sortAscending;

			ReverseCustomSortIfNeeded(album, oldSortByMetaName, oldSortAscending);

			album.Sort(true, Utils.UserName);

			HelperFunctions.PurgeCache();
		}

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Performs any necessary actions that must occur before an album is deleted. Specifically, it deletes the owner role 
		/// if one exists for the album, but only when this album is the only one assigned to the role. It also clears out  
		/// <see cref="IGallerySettings.UserAlbumParentAlbumId" /> if the album's ID matches it. This function recursively calls
		/// itself to make sure all child albums are processed.
		/// </summary>
		/// <param name="album">The album to be deleted, or one of its child albums.</param>
		private static void OnBeforeAlbumDelete(IAlbum album)
		{
			// If there is an owner role associated with this album, and the role is not assigned to any other albums, delete it.
			if (!String.IsNullOrEmpty(album.OwnerRoleName))
			{
				IGalleryServerRole role = RoleController.GetGalleryServerRoles().GetRole(album.OwnerRoleName);

				if ((role != null) && (role.AllAlbumIds.Count == 1) && role.AllAlbumIds.Contains(album.Id))
				{
					RoleController.DeleteGalleryServerProRole(role.RoleName);
				}
			}

			// If the album is specified as the user album container, clear out the setting. The ValidateBeforeAlbumDelete()
			// function will throw an exception if user albums are enabled, so this should only happen when user albums
			// are disabled, so it is safe to clear it out.
			int userAlbumParentAlbumId = Factory.LoadGallerySetting(album.GalleryId).UserAlbumParentAlbumId;
			if (album.Id == userAlbumParentAlbumId)
			{
				IGallerySettings gallerySettingsWriteable = Factory.LoadGallerySetting(album.GalleryId, true);
				gallerySettingsWriteable.UserAlbumParentAlbumId = 0;
				gallerySettingsWriteable.Save();
			}

			// Recursively validate child albums.
			foreach (IGalleryObject childAlbum in album.GetChildGalleryObjects(GalleryObjectType.Album))
			{
				OnBeforeAlbumDelete((IAlbum)childAlbum);
			}
		}

		/// <summary>
		/// Finds the first album within the heirarchy of the specified <paramref name="album"/> whose ID is in 
		/// <paramref name="albumIds"/>. Acts recursively in an across-first, then-down search pattern, resulting 
		/// in the highest level matching album to be returned. Returns null if there are no matching albums.
		/// </summary>
		/// <param name="album">The album to be searched to see if it, or any of its children, matches one of the IDs
		/// in <paramref name="albumIds"/>.</param>
		/// <param name="albumIds">Contains the IDs of the albums to search for.</param>
		/// <returns>Returns the first album within the heirarchy of the specified <paramref name="album"/> whose ID is in 
		/// <paramref name="albumIds"/>.</returns>
		private static IAlbum FindFirstMatchingAlbumRecursive(IAlbum album, ICollection<int> albumIds)
		{
			// Is the current album in the list?
			if (albumIds.Contains(album.Id))
				return album;

			// Nope, so look at the child albums of this album.
			IAlbum albumToSelect = null;
			var childAlbums = album.GetChildGalleryObjects(GalleryObjectType.Album).ToSortedList();

			foreach (IGalleryObject childAlbum in childAlbums)
			{
				if (albumIds.Contains(childAlbum.Id))
				{
					albumToSelect = (IAlbum)childAlbum;
					break;
				}
			}

			// Not the child albums either, so iterate through the children of the child albums. Act recursively.
			if (albumToSelect == null)
			{
				foreach (IGalleryObject childAlbum in childAlbums)
				{
					albumToSelect = FindFirstMatchingAlbumRecursive((IAlbum)childAlbum, albumIds);

					if (albumToSelect != null)
						break;
				}
			}

			return albumToSelect; // Returns null if no matching album is found
		}

		private static void SaveAlbumIdToProfile(int albumId, string userName, int galleryId)
		{
			IUserProfile profile = ProfileController.GetProfile(userName);

			IUserGalleryProfile pg = profile.GetGalleryProfile(galleryId);
			pg.UserAlbumId = albumId;

			ProfileController.SaveProfile(profile);
		}

		/// <summary>
		/// Set the IsPrivate property of all child albums and media objects of the specified album to have the same value
		/// as the specified album. This can be a long running operation and should be scheduled on a background thread.
		/// This function, and its decendents, have no dependence on the HTTP Context.
		/// </summary>
		/// <param name="album">The album whose child objects are to be updated to have the same IsPrivate value.</param>
		/// <param name="userName">Name of the current user.</param>
		private static void SynchIsPrivatePropertyOnChildGalleryObjects(IAlbum album, string userName)
		{
			try
			{
				SynchIsPrivatePropertyOnChildGalleryObjectsRecursive(album, userName);
			}
			catch (Exception ex)
			{
				AppEventController.LogError(ex);
			}

			HelperFunctions.PurgeCache();
		}

		/// <summary>
		/// Set the IsPrivate property of all child albums and media objects of the specified album to have the same value
		/// as the specified album.
		/// </summary>
		/// <param name="album">The album whose child objects are to be updated to have the same IsPrivate value.</param>
		/// <param name="userName">Name of the current user.</param>
		private static void SynchIsPrivatePropertyOnChildGalleryObjectsRecursive(IAlbum album, string userName)
		{
			album.Inflate(true);
			foreach (IAlbum childAlbum in album.GetChildGalleryObjects(GalleryObjectType.Album))
			{
				childAlbum.Inflate(true); // The above Inflate() does not inflate child albums, so we need to explicitly inflate it.
				childAlbum.IsPrivate = album.IsPrivate;
				GalleryObjectController.SaveGalleryObject(childAlbum, userName);
				SynchIsPrivatePropertyOnChildGalleryObjectsRecursive(childAlbum, userName);
			}

			foreach (IGalleryObject childGalleryObject in album.GetChildGalleryObjects(GalleryObjectType.MediaObject))
			{
				childGalleryObject.IsPrivate = album.IsPrivate;
				GalleryObjectController.SaveGalleryObject(childGalleryObject, userName);
			}
		}

		/// <summary>
		/// Inspects the specified <paramref name="album" /> to see if the <see cref="IAlbum.OwnerUserName" /> is an existing user.
		/// If not, the property is cleared out (which also clears out the <see cref="IAlbum.OwnerRoleName" /> property).
		/// </summary>
		/// <param name="album">The album to inspect.</param>
		private static void ValidateAlbumOwner(IAlbum album)
		{
			if ((!String.IsNullOrEmpty(album.OwnerUserName)) && (!UserController.GetAllUsers().Contains(album.OwnerUserName)))
			{
				if (RoleController.GetUsersInRole(album.OwnerRoleName).Length == 0)
				{
					RoleController.DeleteGalleryServerProRole(album.OwnerRoleName);
				}

				if (album.IsWritable)
				{
					album.OwnerUserName = String.Empty; // This will also clear out the OwnerRoleName property.

					GalleryObjectController.SaveGalleryObject(album);
				}
				else
				{
					// Load a writeable version and update the database, then do the same update to our in-memory instance.
					IAlbum albumWritable = Factory.LoadAlbumInstance(album.Id, false, true);

					albumWritable.OwnerUserName = String.Empty; // This will also clear out the OwnerRoleName property.

					GalleryObjectController.SaveGalleryObject(albumWritable);

					// Update our local in-memory object to match the one we just saved.
					album.OwnerUserName = String.Empty;
					album.OwnerRoleName = String.Empty;
				}

				// Remove this item from cache so that we don't have any old copies floating around.
				var albumCache = (System.Collections.Concurrent.ConcurrentDictionary<int, IAlbum>)HelperFunctions.GetCache(CacheItem.Albums);

				if (albumCache != null)
				{
					IAlbum value;
					albumCache.TryRemove(album.Id, out value);
				}
			}
		}

		/// <summary>
		/// Gets a writable collection of the gallery objects in the album containing <paramref name="galleryItem" />, including 
		/// <paramref name="galleryItem" />. If <paramref name="galleryItem" /> does not represent a valid object, an empty 
		/// collection is returned. Guaranteed to not return null.
		/// </summary>
		/// <param name="galleryItem">A gallery item. The object must have the <see cref="GalleryItem.Id" /> and 
		/// <see cref="GalleryItem.ItemType" /> properties specified; the others are optional.</param>
		/// <returns>An instance of <see cref="IGalleryObjectCollection" />.</returns>
		private static IGalleryObjectCollection GetWritableSiblingGalleryObjects(GalleryItem galleryItem)
		{
			IGalleryObject parentAlbum;

			try
			{
				int parentAlbumId;
				if ((GalleryObjectType)galleryItem.ItemType == GalleryObjectType.Album)
				{
					parentAlbumId = LoadAlbumInstance(galleryItem.Id, false).Parent.Id;
				}
				else
				{
					parentAlbumId = Factory.LoadMediaObjectInstance(galleryItem.Id).Parent.Id;
				}

				parentAlbum = LoadAlbumInstance(parentAlbumId, true, true);
			}
			catch (InvalidAlbumException)
			{
				parentAlbum = new NullGalleryObject();
			}
			catch (InvalidMediaObjectException)
			{
				parentAlbum = new NullGalleryObject();
			}

			return parentAlbum.GetChildGalleryObjects();
		}

		/// <summary>
		/// Reverse the gallery objects in the <paramref name="album" /> if they are custom sorted and the user
		/// clicked the reverse sort button (i.e. changed the <paramref name="previousSortAscending" /> value).
		/// This can't be handled by the normal sort routine because we aren't actually sorting on any particular
		/// metadata value.
		/// </summary>
		/// <param name="album">The album whose items are to be sorted.</param>
		/// <param name="previousSortByMetaName">The name of the metadata property the album was previously sorted on.</param>
		/// <param name="previousSortAscending">Indicates whether the album was previously sorted in ascending order.</param>
		private static void ReverseCustomSortIfNeeded(IAlbum album, MetadataItemName previousSortByMetaName, bool previousSortAscending)
		{
			var albumIsCustomSortedAndUserIsChangingSortDirection = ((album.SortByMetaName == MetadataItemName.NotSpecified)
				&& (album.SortByMetaName == previousSortByMetaName)
				&& (album.SortAscending != previousSortAscending));

			if (albumIsCustomSortedAndUserIsChangingSortDirection)
			{
				// Album is being manually sorted and user clicked the reverse button.
				var seq = 1;
				foreach (var galleryObject in album.GetChildGalleryObjects().ToSortedList().Reverse())
				{
					galleryObject.Sequence = seq;
					seq++;
				}
			}
		}

		#endregion
	}
}
