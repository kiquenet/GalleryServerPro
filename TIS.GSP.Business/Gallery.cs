using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Business.Properties;
using GalleryServerPro.Data;
using GalleryServerPro.Events.CustomExceptions;

namespace GalleryServerPro.Business
{
	/// <summary>
	/// Represents a gallery within Gallery Server Pro.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Gallery ID = {_id}")]
	public class Gallery : IGallery, IComparable
	{
		#region Private Fields

		private int _id;
		private int _rootAlbumId = int.MinValue;
		private string _description;
		private DateTime _creationDate;
		private Dictionary<int, List<int>> _albums;

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a gallery is first created, just after it is persisted to the data store.
		/// </summary>
		public static event EventHandler<GalleryCreatedEventArgs> GalleryCreated;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the unique identifier for this gallery.
		/// </summary>
		/// <value>The unique identifier for this gallery.</value>
		public int GalleryId
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this object is new and has not yet been persisted to the data store.
		/// </summary>
		/// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
		public bool IsNew
		{
			get
			{
				return (_id == int.MinValue);
			}
		}

		/// <summary>
		/// Gets or sets the description for this gallery.
		/// </summary>
		/// <value>The description for this gallery.</value>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets the date this gallery was created.
		/// </summary>
		/// <value>The date this gallery was created.</value>
		public DateTime CreationDate
		{
			get { return _creationDate; }
			set { _creationDate = value; }
		}

		/// <summary>
		/// Gets or sets the ID of the root album of this gallery.
		/// </summary>
		/// <value>The ID of the root album of this gallery</value>
		public int RootAlbumId
		{
			get
			{
				if (_rootAlbumId == int.MinValue)
				{
					// The root album is the item in the Albums dictionary with the most number of child albums.
					int maxCount = int.MinValue;
					foreach (KeyValuePair<int, List<int>> kvp in Albums)
					{
						if (kvp.Value.Count > maxCount)
						{
							maxCount = kvp.Value.Count;
							_rootAlbumId = kvp.Key;
						}
					}
				}
				return _rootAlbumId;
			}
			set { _rootAlbumId = value; }
		}

		/// <summary>
		/// Gets or sets a dictionary containing a list of album IDs (key) and the flattened list of
		/// all child album IDs below each album.
		/// </summary>
		/// <value>An instance of Dictionary&lt;int, List&lt;int&gt;&gt;.</value>
		public Dictionary<int, List<int>> Albums
		{
			get { return _albums; }
			set { _albums = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Gallery"/> class.
		/// </summary>
		public Gallery()
		{
			this._id = int.MinValue;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a deep copy of this instance.
		/// </summary>
		/// <returns>Returns a deep copy of this instance.</returns>
		public IGallery Copy()
		{
			IGallery galleryCopy = new Gallery();

			galleryCopy.GalleryId = this.GalleryId;
			galleryCopy.Description = this.Description;
			galleryCopy.CreationDate = this.CreationDate;

			galleryCopy.Albums = new Dictionary<int, List<int>>(this.Albums.Count);
			foreach (KeyValuePair<int, List<int>> kvp in this.Albums)
			{
				galleryCopy.Albums.Add(kvp.Key, new List<int>(kvp.Value));
			}

			return galleryCopy;
		}

		/// <summary>
		/// Persist this gallery object to the data store.
		/// </summary>
		public void Save()
		{
			bool isNew = IsNew;

			using (var repo = new GalleryRepository())
			{
				if (IsNew)
				{
					var galleryDto = new GalleryDto { Description = Description, DateAdded = CreationDate };
					repo.Add(galleryDto);
					repo.Save();
					_id = galleryDto.GalleryId;
				}
				else
				{
					var galleryDto = repo.Find(GalleryId);

					if (galleryDto != null)
					{
						galleryDto.Description = Description;
						repo.Save();
					}
					else
					{
						throw new BusinessException(String.Format(CultureInfo.CurrentCulture, "Cannot save gallery: No existing gallery with Gallery ID {0} was found in the database.", GalleryId));
					}
				}
			}

			// For new galleries, configure it and then trigger the created event.
			if (isNew)
			{
				Configure();

				EventHandler<GalleryCreatedEventArgs> galleryCreated = GalleryCreated;
				if (galleryCreated != null)
				{
					galleryCreated(null, new GalleryCreatedEventArgs(GalleryId));
				}
			}

			Factory.ClearAllCaches();
		}

		/// <summary>
		/// Permanently delete the current gallery from the data store, including all related records. This action cannot
		/// be undone.
		/// </summary>
		public void Delete()
		{
			//Factory.GetDataProvider().Gallery_Delete(this);
			OnBeforeDeleteGallery();

			// Cascade delete relationships should take care of any related records not deleted in OnBeforeDeleteGallery.
			using (var repo = new GalleryRepository())
			{
				var galleryDto = repo.Find(GalleryId);
				if (galleryDto != null)
				{
					// Delete gallery. Cascade delete rules in DB will delete related records.
					repo.Delete(galleryDto);
					repo.Save();
				}
			}

			Factory.ClearAllCaches();
		}

		/// <summary>
		/// Configure the gallery by verifying that a default set of
		/// records exist in the relevant tables (Album, GallerySetting, MimeTypeGallery, Role_Album, UiTemplate,
		/// UiTemplateAlbum). No changes are made to the file system as part of this operation. This method does not overwrite 
		/// existing data, but it does insert missing data. This function can be used during application initialization to validate 
		/// the data integrity for a gallery. For example, if the user has added a record to the MIME types or template gallery 
		/// settings tables, this method will ensure that the new records are associated with this gallery.
		/// </summary>
		public void Configure()
		{
			//Factory.GetDataProvider().Gallery_Configure(this);

			// Step 1: Check for missing gallery settings, copying them from the template settings if necessary.
			ConfigureGallerySettingsTable();

			// Step 2: Create a new set of gallery MIME types (do nothing if already present).
			ConfigureMimeTypeGalleryTable();

			// Step 3: Create the root album if necessary.
			var rootAlbumDto = ConfigureAlbumTable();

			// Step 4: For each role with AllowAdministerSite permission, add a corresponding record in gs_Role_Album giving it 
			// access to the root album.
			ConfigureRoleAlbumTable(rootAlbumDto.AlbumId);

			// Step 5: Validate the UI templates.
			ConfigureUiTemplateTable();
			ConfigureUiTemplateAlbumTable(rootAlbumDto);

			// Step 6: Reset the sync table.
			ConfigureSyncTable();

			// Verify each album/media object has a title and caption? This would be a pretty big perf hit, so let's not do it.
		}

		/// <summary>
		/// Verify there are gallery settings for the current gallery that match every template gallery setting, creating any
		/// if necessary.
		/// </summary>
		private void ConfigureGallerySettingsTable()
		{
			var foundTmplGallerySettings = false;
			using (var repo = new GallerySettingRepository())
			{
				repo.Load();

				// Loop through each template gallery setting.
				foreach (var gsTmpl in repo.Where(g => g.Gallery.IsTemplate))
				{
					foundTmplGallerySettings = true;
					if (!repo.Local.Any(gs => gs.SettingName == gsTmpl.SettingName && gs.FKGalleryId == GalleryId))
					{
						// This gallery is missing an entry for a gallery setting. Create one by copying it from the template gallery.
						repo.Add(new GallerySettingDto()
											 {
												 FKGalleryId = GalleryId,
												 SettingName = gsTmpl.SettingName,
												 SettingValue = gsTmpl.SettingValue
											 });
					}
				}

				repo.Save();
			}

			if (!foundTmplGallerySettings)
			{
				// If there weren't *any* template gallery settings, insert the seed data. Generally this won't be necessary, but it
				// can help recover from certain conditions, such as when a SQL Server connection is accidentally specified without
				// the MultipleActiveResultSets keyword (or it was false). In this situation the galleries are inserted but an error 
				// prevents the remaining data from being inserted. Once the user corrects this and tries again, this code can run to
				// finish inserting the seed data.
				using (var ctx = new GalleryDb())
				{
					SeedController.InsertSeedData(ctx);
				}
			}
		}

		/// <summary>
		/// Verify there is a MIME type/gallery mapping for the current gallery for every MIME type, creating any
		/// if necessary.
		/// </summary>
		private void ConfigureMimeTypeGalleryTable()
		{
			var defaultEnabledExtensions = new List<string> { ".jpg", ".jpeg" };

			using (var repoMt = new MimeTypeRepository())
			{
				using (var repoMtg = new MimeTypeGalleryRepository())
				{
					// Get MIME types that don't have a match in the MIME Type Gallery table
					foreach (var mtDto in repoMt.Where(mt => mt.MimeTypeGalleries.All(mtg => mtg.FKGalleryId != GalleryId)))
					{
						repoMtg.Add(new MimeTypeGalleryDto()
												{
													FKGalleryId = GalleryId,
													FKMimeTypeId = mtDto.MimeTypeId,
													IsEnabled = defaultEnabledExtensions.Contains(mtDto.FileExtension)
												});
					}

					repoMtg.Save();
				}
			}
		}

		/// <summary>
		/// Verify the current gallery has a root album, creating one if necessary. The root album is returned.
		/// </summary>
		/// <returns>An instance of <see cref="AlbumDto" />.</returns>
		private AlbumDto ConfigureAlbumTable()
		{
			using (var repo = new AlbumRepository())
			{
				var rootAlbumDto = repo.Where(a => a.FKGalleryId == GalleryId && a.FKAlbumParentId == null).FirstOrDefault();

				if (rootAlbumDto == null)
				{
					rootAlbumDto = new AlbumDto
					{
						FKGalleryId = GalleryId,
						FKAlbumParentId = null,
						DirectoryName = String.Empty,
						ThumbnailMediaObjectId = 0,
						SortByMetaName = MetadataItemName.DateAdded,
						SortAscending = true,
						Seq = 0,
						DateAdded = DateTime.Now,
						CreatedBy = GlobalConstants.SystemUserName,
						LastModifiedBy = GlobalConstants.SystemUserName,
						DateLastModified = DateTime.Now,
						OwnedBy = String.Empty,
						OwnerRoleName = String.Empty,
						IsPrivate = false,
						Metadata = new Collection<MetadataDto>
						{
							new MetadataDto {MetaName = MetadataItemName.Caption, Value = Resources.Root_Album_Default_Summary},
							new MetadataDto {MetaName = MetadataItemName.Title, Value = Resources.Root_Album_Default_Title}
						}
					};

					repo.Add(rootAlbumDto);
					repo.Save();
				}

				return rootAlbumDto;
			}
		}

		/// <summary>
		/// Verify there is a site admin role/album mapping for the root album in the current gallery, creating one
		/// if necessary.
		/// </summary>
		/// <param name="albumId">The album ID of the root album in the current gallery.</param>
		private static void ConfigureRoleAlbumTable(int albumId)
		{
			using (var repoR = new RoleRepository())
			{
				using (var repoRa = new RoleAlbumRepository())
				{
					// Get admin roles that aren't assigned to the album, then assign them
					foreach (var rDto in repoR.Where(r => r.AllowAdministerSite && r.RoleAlbums.All(ra => ra.FKAlbumId != albumId)))
					{
						repoRa.Add(new RoleAlbumDto()
												{
													FKRoleName = rDto.RoleName,
													FKAlbumId = albumId
												});
					}

					repoRa.Save();
				}
			}
		}

		/// <summary>
		/// Verify there are UI templates for the current gallery that match every UI template associated with
		/// the template gallery, creating any if necessary.
		/// </summary>
		private void ConfigureUiTemplateTable()
		{
			using (var repoUiTmpl = new UiTemplateRepository())
			{
				var ctx = repoUiTmpl.Context;

				repoUiTmpl.Load();

				// Get the UI templates belonging to the template gallery. We have to do a join here because the data
				// model doesn't have a relationship. (Doing so would conflict with the relationship between
				// the UITemplateAlbum and Album tables.)
				var tmplForTmplGallery = from uiTmpl in ctx.UiTemplates join g in ctx.Galleries on uiTmpl.FKGalleryId equals g.GalleryId where g.IsTemplate select uiTmpl;

				// For each UI template, make sure one exists in the gallery
				foreach (var uiTmpl in tmplForTmplGallery)
				{
					if (!repoUiTmpl.Local.Any(ui => ui.TemplateType == uiTmpl.TemplateType && ui.FKGalleryId == GalleryId && ui.Name == uiTmpl.Name))
					{
						// We need to add a UI template
						repoUiTmpl.Add(new UiTemplateDto()
														 {
															 TemplateType = uiTmpl.TemplateType,
															 FKGalleryId = GalleryId,
															 Name = uiTmpl.Name,
															 Description = uiTmpl.Description,
															 HtmlTemplate = uiTmpl.HtmlTemplate,
															 ScriptTemplate = uiTmpl.ScriptTemplate
														 });
					}
				}

				repoUiTmpl.Save();
			}
		}

		/// <summary>
		/// Verify there is a UI template/album mapping for the root album in the current gallery, creating them
		/// if necessary.
		/// </summary>
		/// <param name="rootAlbum">The root album.</param>
		private static void ConfigureUiTemplateAlbumTable(AlbumDto rootAlbum)
		{
			using (var repoUiTmpl = new UiTemplateRepository())
			{
				using (var repoUiTmplA = new UiTemplateAlbumRepository(repoUiTmpl.Context))
				{
					// Make sure each template category has at least one template assigned to the root album.
					// We do this with a union of two queries:
					// 1. For categories where there is at least one album assignment, determine if at least one of
					//    those assignments is the root album.
					// 2. Find categories without any albums at all (this is necessary because the SelectMany() in the first
					//    query won't return any categories that don't have related records in the template/album table).
					var dtos = repoUiTmpl.Where(t => t.FKGalleryId == rootAlbum.FKGalleryId)
															 .SelectMany(t => t.TemplateAlbums, (t, tt) => new { t.TemplateType, tt.FKAlbumId })
															 .GroupBy(t => t.TemplateType)
															 .Where(t => t.All(ta => ta.FKAlbumId != rootAlbum.AlbumId))
															 .Select(t => t.Key)
															 .Union(repoUiTmpl.Where(t => t.FKGalleryId == rootAlbum.FKGalleryId).GroupBy(t => t.TemplateType).Where(t => t.All(t2 => !t2.TemplateAlbums.Any())).Select(t => t.Key))
															 ;

					foreach (var dto in dtos)
					{
						// We have a template type without a root album. Find the default template and assign that one.
						var dto1 = dto;
						repoUiTmplA.Add(new UiTemplateAlbumDto()
						{
							FKUiTemplateId = repoUiTmpl.Where(t => t.FKGalleryId == rootAlbum.FKGalleryId && t.TemplateType == dto1 && t.Name.Equals("default", StringComparison.OrdinalIgnoreCase)).First().UiTemplateId,
							FKAlbumId = rootAlbum.AlbumId
						});
					}

					repoUiTmplA.Save();
				}
			}
		}

		/// <summary>
		/// Deletes the synchronization record belonging to the current gallery. When a sync is initiated it will be created.
		/// </summary>
		private void ConfigureSyncTable()
		{
			using (var repo = new SynchronizeRepository())
			{
				var syncDto = repo.Find(GalleryId);

				if (syncDto != null)
				{
					repo.Delete(syncDto);
					repo.Save();
				}
			}
		}

		/// <summary>
		/// Called before deleting a gallery. This function deletes the albums and any related records that won't be automatically
		/// deleted by the cascade delete relationship on the gallery table.
		/// </summary>
		private void OnBeforeDeleteGallery()
		{
			DeleteRootAlbum();

			DeleteUiTemplates();
		}

		/// <summary>
		/// Deletes the root album for the current gallery and all child items, but leaves the directories and original files on disk.
		/// This function also deletes the metadata for the root album, which will leave it in an invalid state. For this reason, 
		/// call this function *only* when also deleting the gallery the album is in.
		/// </summary>
		private void DeleteRootAlbum()
		{
			// Step 1: Delete the root album contents
			var rootAlbum = Factory.LoadRootAlbumInstance(GalleryId);
			rootAlbum.DeleteFromGallery();

			// Step 2: Delete all metadata associated with the root album of this gallery
			using (var repo = new MetadataRepository())
			{
				foreach (var dto in repo.Where(m => m.FKAlbumId == rootAlbum.Id))
				{
					repo.Delete(dto);
				}
				repo.Save();
			}
		}

		/// <summary>
		/// Deletes the UI templates associated with the current gallery.
		/// </summary>
		private void DeleteUiTemplates()
		{
			using (var repo = new UiTemplateRepository())
			{
				foreach (var dto in repo.Where(m => m.FKGalleryId == GalleryId))
				{
					repo.Delete(dto);
				}
				repo.Save();
			}
		}

		#endregion

		#region IComparable Members

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="obj"/> is not the same type as this instance. </exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			else
			{
				IGallery other = obj as IGallery;
				if (other != null)
					return this.GalleryId.CompareTo(other.GalleryId);
				else
					return 1;
			}
		}

		#endregion
	}

	/// <summary>
	/// Provides data for the <see cref="Gallery.GalleryCreated" /> event.
	/// </summary>
	public class GalleryCreatedEventArgs : EventArgs
	{
		private readonly int _galleryId;

		/// <summary>
		/// Initializes a new instance of the <see cref="GalleryCreatedEventArgs"/> class.
		/// </summary>
		/// <param name="galleryId">The ID of the newly created gallery.</param>
		public GalleryCreatedEventArgs(int galleryId)
		{
			_galleryId = galleryId;
		}

		/// <summary>
		/// Gets the ID of the newly created gallery.
		/// </summary>
		/// <value>The gallery ID.</value>
		public int GalleryId
		{
			get { return _galleryId; }
		}
	}
}
