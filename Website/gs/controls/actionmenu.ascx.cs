using System;
using System.Globalization;

namespace GalleryServerPro.Web.Controls
{
	/// <summary>
	/// A user control that renders the Actions menu that appears when a logged-on user has permission to carry out at least one action.
	/// </summary>
	public partial class actionmenu : GalleryUserControl
	{
		#region Private Fields

		int _albumId = int.MinValue;
		int _mediaObjectId = int.MinValue;

		#endregion

		#region Protected Methods

		protected string GetMenuHtml()
		{
			const string uiStateDisabled = " ui-state-disabled";

			return String.Format(CultureInfo.InvariantCulture, @"
<ul id='{0}_mnu' class='gsp_a_m'>
	<li class='gsp_a_m_root'><a href='#'>{1}</a>
		<ul>
			<li class='gsp_a_m_c_a{2}'><a href='{18}' title='{33}'><span>{49}</span></a></li>
			<li></li>
			<li class='gsp_a_m_a_o{3}'><a href='{19}' title='{34}'><span>{50}</span></a></li>
			<li class='gsp_a_m_m_o{4}'><a href='{20}' title='{35}'><span>{51}</span></a></li>
			<li class='gsp_a_m_c_o{5}'><a href='{21}' title='{36}'><span>{52}</span></a></li>
			<li class='gsp_a_m_m_a{6}'><a href='{22}' title='{37}'><span>{53}</span></a></li>
			<li class='gsp_a_m_c_alb{7}'><a href='{23}' title='{38}'><span>{54}</span></a></li>
			<li></li>
			<li class='gsp_a_m_dl_o{8}'><a href='{24}' title='{39}'><span>{55}</span></a></li>
			<li></li>
			<li class='gsp_a_m_e_c{9}'><a href='{25}' title='{40}'><span>{56}</span></a></li>
			<li class='gsp_a_m_a_t{10}'><a href='{26}' title='{41}'><span>{57}</span></a></li>
			<li class='gsp_a_m_r_i{11}'><a href='{27}' title='{42}'><span>{58}</span></a></li>
			<li></li>
			<li class='gsp_a_m_d_o{12}'><a href='{28}' title='{43}'><span>{59}</span></a></li>
			<li class='gsp_a_m_d_o_f{13}'><a href='{29}' title='{44}'><span>{60}</span></a></li>
			<li class='gsp_a_m_d_a{14}'><a href='{30}' title='{45}'><span>{61}</span></a></li>
			<li></li>
			<li class='gsp_a_m_s{15}'><a href='{31}' title='{46}'><span>{62}</span></a></li>
			<li></li>
			<li class='gsp_a_m_s_a{16}'><a href='{32}' title='{47}'><span>{63}</span></a></li>
			<li></li>
			<li id='{0}_mnu_logoff' class='gsp_a_m_l_o{17}'><a href='#' title='{48}'><span>{64}</span></a></li>
		</ul>
	</li>
</ul>
",
	cid, // 0
	Resources.GalleryServerPro.UC_ActionMenu_Root_Text, // 1
	CreateAlbumEnabled ? String.Empty : uiStateDisabled, // 2
	AddObjectsEnabled ? String.Empty : uiStateDisabled, // 3
	MoveObjectsEnabled ? String.Empty : uiStateDisabled, // 4
	CopyObjectsEnabled ? String.Empty : uiStateDisabled, // 5
	MoveAlbumEnabled ? String.Empty : uiStateDisabled, // 6
	CopyAlbumEnabled ? String.Empty : uiStateDisabled, // 7
	DownloadObjectsEnabled ? String.Empty : uiStateDisabled, // 8
	EditCaptionsEnabled ? String.Empty : uiStateDisabled, // 9
	AssignThumbnailEnabled ? String.Empty : uiStateDisabled, // 10
	RotateImagesEnabled ? String.Empty : uiStateDisabled, // 11
	DeleteObjectsEnabled ? String.Empty : uiStateDisabled, // 12
	DeleteOriginalFilesEnabled ? String.Empty : uiStateDisabled, // 13
	DeleteAlbumEnabled ? String.Empty : uiStateDisabled, // 14
	SynchronizeEnabled ? String.Empty : uiStateDisabled, // 15
	SiteAdminEnabled ? String.Empty : uiStateDisabled, // 16
	LogOffEnabled ? String.Empty : uiStateDisabled, // 17
	Utils.GetUrl(PageId.task_createalbum, "aid={0}", AlbumId), // 18
	Utils.GetUrl(PageId.task_addobjects, "aid={0}", AlbumId), // 19
	Utils.GetUrl(PageId.task_transferobject, "aid={0}&tt=move&skipstep1=false", AlbumId), // 20
	Utils.GetUrl(PageId.task_transferobject, "aid={0}&tt=copy&skipstep1=false", AlbumId), // 21
	Utils.GetUrl(PageId.task_transferobject, "aid={0}&tt=move&skipstep1=true", AlbumId), // 22
	Utils.GetUrl(PageId.task_transferobject, "aid={0}&tt=copy&skipstep1=true", AlbumId), // 23
	GetDownloadObjectsUrl(), // 24
	Utils.GetUrl(PageId.task_editcaptions, "aid={0}", AlbumId), // 25
	Utils.GetUrl(PageId.task_assignthumbnail, "aid={0}", AlbumId), // 26
	Utils.GetUrl(PageId.task_rotateimages, "aid={0}", AlbumId), // 27
	Utils.GetUrl(PageId.task_deleteobjects, "aid={0}", AlbumId), // 28
	Utils.GetUrl(PageId.task_deletehires, "aid={0}", AlbumId), // 29
	Utils.GetUrl(PageId.task_deletealbum, "aid={0}", AlbumId), // 30
	Utils.GetUrl(PageId.task_synchronize, "aid={0}", AlbumId), // 31
	(MediaObjectId > int.MinValue ? Utils.GetUrl(PageId.admin_sitesettings, "moid={0}", MediaObjectId) : Utils.GetUrl(PageId.admin_sitesettings, "aid={0}", AlbumId)), // 32
	CreateAlbumTooltip,         // 33
	AddObjectsTooltip,          // 34
	MoveObjectsTooltip,         // 35
	CopyObjectsTooltip,         // 36
	MoveAlbumTooltip,           // 37
	CopyAlbumTooltip,           // 38
	DownloadObjectsTooltip,     // 39
	EditCaptionsTooltip,        // 40
	AssignThumbnailTooltip,     // 41
	RotateImagesTooltip,        // 42
	DeleteObjectsTooltip,       // 43
	DeleteOriginalFilesTooltip, // 44
	DeleteAlbumTooltip,         // 45
	SynchronizeTooltip,         // 46
	SiteAdminTooltip,           // 47
	LogOffTooltip,              // 48
	Resources.GalleryServerPro.UC_ActionMenu_Create_Album_Text,     // 49
	Resources.GalleryServerPro.UC_ActionMenu_Add_Objects_Text,      // 50
	Resources.GalleryServerPro.UC_ActionMenu_Transfer_Objects_Text, // 51
	Resources.GalleryServerPro.UC_ActionMenu_Copy_Objects_Text,     // 52
	Resources.GalleryServerPro.UC_ActionMenu_Move_Album_Text,       // 53
	Resources.GalleryServerPro.UC_ActionMenu_Copy_Album_Text,       // 54
	Resources.GalleryServerPro.UC_ActionMenu_Download_Objects_Text, // 55
	Resources.GalleryServerPro.UC_ActionMenu_Edit_Captions_Text,    // 56
	Resources.GalleryServerPro.UC_ActionMenu_Assign_Thumbnail_Text, // 57
	Resources.GalleryServerPro.UC_ActionMenu_Rotate_Text,           // 58
	Resources.GalleryServerPro.UC_ActionMenu_Delete_Objects_Text,   // 59
	Resources.GalleryServerPro.UC_ActionMenu_Delete_HiRes_Text,     // 60
	Resources.GalleryServerPro.UC_ActionMenu_Delete_Album_Text,     // 61
	Resources.GalleryServerPro.UC_ActionMenu_Synchronize_Text,      // 62
	Resources.GalleryServerPro.UC_ActionMenu_Admin_Console_Text,    // 63
	Resources.GalleryServerPro.UC_ActionMenu_Logout_Text            // 64
																																		 
);

		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#endregion

		#region Public Properties

		private bool MediaObjectPathIsWriteable
		{
			get { return !GalleryPage.GallerySettings.MediaObjectPathIsReadOnly; }
		}

		/// <summary>
		/// Gets a string similar to "(Disabled because you do not have permission for this action)".
		/// </summary>
		/// <value>A string.</value>
		private static string DisabledDueToInsufficientPermissionText
		{
			get
			{
				return Resources.GalleryServerPro.UC_ActionMenu_Disabled_Insufficient_Permission_Tooltip;
			}
		}

		private bool CreateAlbumEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanAddAlbumToAtLeastOneAlbum;
			}
		}

		private string CreateAlbumTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return CreateAlbumEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Create_Album_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool AddObjectsEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanAddMediaObject;
			}
		}

		private string AddObjectsTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return AddObjectsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Add_Objects_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool MoveObjectsEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable &&
					(GalleryPage.UserCanDeleteMediaObject && GalleryPage.UserCanAddMediaObjectToAtLeastOneAlbum) ||
					(GalleryPage.UserCanDeleteChildAlbum && GalleryPage.UserCanAddAlbumToAtLeastOneAlbum);
			}
		}

		private string MoveObjectsTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return MoveObjectsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Transfer_Objects_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool CopyObjectsEnabled
		{
			get
			{
				bool userCanCopyInCurrentGallery;
				if (GalleryPage.GallerySettings.AllowCopyingReadOnlyObjects)
				{
					userCanCopyInCurrentGallery =(MediaObjectPathIsWriteable && (GalleryPage.UserCanAddAlbumToAtLeastOneAlbum || GalleryPage.UserCanAddMediaObjectToAtLeastOneAlbum));
				}
				else
				{
					userCanCopyInCurrentGallery = (MediaObjectPathIsWriteable && GalleryPage.UserCanAddMediaObject);
				}

				return (userCanCopyInCurrentGallery || GalleryPage.UserIsAdminForAtLeastOneOtherGallery);
			}
		}
		private string CopyObjectsTooltip
		{
			get
			{
				if (!CopyObjectsEnabled && !MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return CopyObjectsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Copy_Objects_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool MoveAlbumEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && (!GalleryPage.GetAlbum().IsRootAlbum && GalleryPage.UserCanDeleteCurrentAlbum && GalleryPage.UserCanAddAlbumToAtLeastOneAlbum);
			}
		}

		private string MoveAlbumTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return MoveAlbumEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Move_Album_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool CopyAlbumEnabled
		{
			get
			{
				bool userCanCopyInCurrentGallery;
				if (GalleryPage.GallerySettings.AllowCopyingReadOnlyObjects)
				{
					userCanCopyInCurrentGallery = (MediaObjectPathIsWriteable && (!GalleryPage.GetAlbum().IsRootAlbum && GalleryPage.UserCanAddAlbumToAtLeastOneAlbum));
				}
				else
				{
					userCanCopyInCurrentGallery = (MediaObjectPathIsWriteable && (!GalleryPage.GetAlbum().IsRootAlbum && GalleryPage.UserCanCreateAlbum));
				}

				return userCanCopyInCurrentGallery || GalleryPage.UserIsAdminForAtLeastOneOtherGallery;
			}
		}

		private string CopyAlbumTooltip
		{
			get
			{
				if (!CopyAlbumEnabled && !MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return CopyAlbumEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Copy_Album_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool DownloadObjectsEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.GallerySettings.EnableGalleryObjectZipDownload;
			}
		}

		private string DownloadObjectsTooltip
		{
			get
			{
				return DownloadObjectsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Download_Objects_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool EditCaptionsEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanEditMediaObject;
			}
		}

		private string EditCaptionsTooltip
		{
			get
			{
				return EditCaptionsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Edit_Captions_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool AssignThumbnailEnabled
		{
			get
			{
				return GalleryPage.UserCanEditAlbum;
			}
		}

		private string AssignThumbnailTooltip
		{
			get
			{
				return AssignThumbnailEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Assign_Thumbnail_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool RotateImagesEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanEditMediaObject;
			}
		}

		private string RotateImagesTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return RotateImagesEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Rotate_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool DeleteObjectsEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && (GalleryPage.UserCanDeleteMediaObject || GalleryPage.UserCanDeleteChildAlbum);
			}
		}

		private string DeleteObjectsTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return DeleteObjectsEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Delete_Objects_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool DeleteOriginalFilesEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanEditMediaObject;
			}
		}

		private string DeleteOriginalFilesTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return DeleteOriginalFilesEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Delete_HiRes_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool DeleteAlbumEnabled
		{
			get
			{
				return MediaObjectPathIsWriteable && GalleryPage.UserCanDeleteCurrentAlbum;
			}
		}

		private string DeleteAlbumTooltip
		{
			get
			{
				if (!MediaObjectPathIsWriteable)
				{
					return Resources.GalleryServerPro.UC_ActionMenu_Disabled_ReadOnly_Tooltip;
				}

				return DeleteAlbumEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Delete_Album_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool SynchronizeEnabled
		{
			get
			{
				return GalleryPage.UserCanSynchronize;
			}
		}

		private string SynchronizeTooltip
		{
			get
			{
				return SynchronizeEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Synchronize_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool SiteAdminEnabled
		{
			get
			{
				return GalleryPage.UserCanAdministerSite || GalleryPage.UserCanAdministerGallery;
			}
		}

		private string SiteAdminTooltip
		{
			get
			{
				return SiteAdminEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Admin_Console_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		private bool LogOffEnabled
		{
			get
			{
				return true;
			}
		}

		private string LogOffTooltip
		{
			get
			{
				return LogOffEnabled ? Resources.GalleryServerPro.UC_ActionMenu_Logout_Tooltip : DisabledDueToInsufficientPermissionText;
			}
		}

		/// <summary>
		/// Gets the album ID.
		/// </summary>
		/// <value>The album ID.</value>
		private int AlbumId
		{
			get
			{
				if (_albumId == int.MinValue)
				{
					_albumId = GalleryPage.GetAlbumId();
				}

				return _albumId;
			}
		}

		/// <summary>
		/// Gets the media object ID.
		/// </summary>
		/// <value>The media object ID.</value>
		private int MediaObjectId
		{
			get
			{
				if (_mediaObjectId == int.MinValue)
				{
					_mediaObjectId = GalleryPage.GetMediaObjectId();
				}

				return _mediaObjectId;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the URL that links to the download objects page. The URL includes any tags or people that exist
		/// in the current URL.
		/// </summary>
		/// <returns>System.String.</returns>
		private string GetDownloadObjectsUrl()
		{
			var tag = Utils.GetQueryStringParameterString("tag");
			var people = Utils.GetQueryStringParameterString("people");

			if (AlbumId > int.MinValue)
			{
				if (String.IsNullOrWhiteSpace(tag) && String.IsNullOrWhiteSpace(people))
					return Utils.GetUrl(PageId.task_downloadobjects, "aid={0}", AlbumId);

				if (!String.IsNullOrWhiteSpace(tag) && !String.IsNullOrWhiteSpace(people))
					return Utils.GetUrl(PageId.task_downloadobjects, "aid={0}&tag={1}&people={2}", AlbumId, tag, people);

				if (!String.IsNullOrWhiteSpace(tag))
					return Utils.GetUrl(PageId.task_downloadobjects, "aid={0}&tag={1}", AlbumId, tag);

				return Utils.GetUrl(PageId.task_downloadobjects, "aid={0}&people={1}", AlbumId, people);
			}

			if (!String.IsNullOrWhiteSpace(tag) && !String.IsNullOrWhiteSpace(people))
				return Utils.GetUrl(PageId.task_downloadobjects, "tag={0}&people={1}", tag, people);

			if (!String.IsNullOrWhiteSpace(tag))
				return Utils.GetUrl(PageId.task_downloadobjects, "tag={0}", tag);

			return Utils.GetUrl(PageId.task_downloadobjects, "people={0}", people);
		}

		#endregion
	}
}