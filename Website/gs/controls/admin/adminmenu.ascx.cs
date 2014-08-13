using System;
using System.Globalization;
using GalleryServerPro.Business;

namespace GalleryServerPro.Web.Controls.Admin
{
	/// <summary>
	/// The menu in the Site admin area.
	/// </summary>
	public partial class adminmenu : GalleryUserControl
	{
		#region Private Fields

		int _albumId = int.MinValue;
		int _mediaObjectId = int.MinValue;

		#endregion

		#region Properties

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

		/// <summary>
		/// Gets a value indicating whether the Manage Users menu option should be visible.
		/// </summary>
		/// <value><c>true</c> if the Manage Users menu option should be visible; otherwise, <c>false</c>.</value>
		private bool ManageUsersVisible
		{
			get
			{
				return GalleryPage.UserCanAdministerSite || (GalleryPage.UserCanAdministerGallery && AppSetting.Instance.AllowGalleryAdminToManageUsersAndRoles);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Manage Roles menu option should be visible.
		/// </summary>
		/// <value><c>true</c> if the Manage Roles menu option should be visible; otherwise, <c>false</c>.</value>
		private bool ManageRolesVisible
		{
			get
			{
				return GalleryPage.UserCanAdministerSite || (GalleryPage.UserCanAdministerGallery && AppSetting.Instance.AllowGalleryAdminToManageUsersAndRoles);
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the HTML for the admin menu. Returns an empty string if the user has insufficient permission.
		/// </summary>
		/// <returns>System.String.</returns>
		protected string GetMenuHtml()
		{
			// Proactive security: Even though the pages that use this control have their own security that make this redundant,
			// we do it anyway for extra protection. Only show menu when user is a site or gallery admin.
			if (this.GalleryPage.UserCanAdministerSite || this.GalleryPage.UserCanAdministerGallery)
				return GenerateMenuHtml();
			else
				return String.Empty;
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

		#region Private Methods

		private string GenerateMenuHtml()
		{
			return String.Format(CultureInfo.InvariantCulture, @"
<div class='gsp_adm_m_f_ctr'>
 <nav class='gsp_adm_m_ctr'>
  <ul class='gsp_adm_m'>
 	 <li class='gsp_adm_m_h1{18}'>{21}</li>
 	 <li class='gsp_adm_m_h2{18} gsp_{0}'><a href='{26}'>{44}</a></li>
 	 <li class='gsp_adm_m_h2{18} gsp_{1}'><a href='{27}'>{45}</a></li>
 	 <li class='gsp_adm_m_h2{18} gsp_{2}'><a href='{28}'>{46}</a></li>
 	 <li class='gsp_adm_m_h2{18} gsp_{3}'><a href='{29}'>{47}</a></li>
 	 <li class='gsp_adm_m_h1'>{22}</li>		 
 	 <li class='gsp_adm_m_h2 gsp_{4}'><a href='{30}'>{48}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{5}'><a href='{31}'>{49}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{6}'><a href='{32}'>{50}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{7}'><a href='{33}'>{51}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{8}'><a href='{34}'>{52}</a></li>
 	 <li class='gsp_adm_m_h1'>{23}</li>
 	 <li class='gsp_adm_m_h2 gsp_{9}'><a href='{35}'>{53}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{10}{19}'><a href='{36}'>{54}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{11}{20}'><a href='{37}'>{55}</a></li>
 	 <li class='gsp_adm_m_h1'>{24}</li>
 	 <li class='gsp_adm_m_h2 gsp_{12}'><a href='{38}'>{56}</a></li>
   <li class='gsp_adm_m_h1'>{25}</li>
	 <li class='gsp_adm_m_h2 gsp_{13}'><a href='{39}'>{57}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{14}'><a href='{40}'>{58}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{15}'><a href='{41}'>{59}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{16}'><a href='{42}'>{60}</a></li>
 	 <li class='gsp_adm_m_h2 gsp_{17}'><a href='{43}'>{61}</a></li>
  </ul>
 </nav>
</div>
",
										PageId.admin_sitesettings,
										PageId.admin_backuprestore,
										PageId.admin_mediatemplates,
										PageId.admin_css,
										PageId.admin_galleries,
										PageId.admin_gallerysettings,
										PageId.admin_gallerycontrolsettings,
										PageId.admin_uitemplates,
										PageId.admin_eventlog,
										PageId.admin_usersettings,
										PageId.admin_manageusers,
										PageId.admin_manageroles,
										PageId.admin_albums,
										PageId.admin_mediaobjects,
										PageId.admin_metadata,
										PageId.admin_mediaobjecttypes,
										PageId.admin_images,
										PageId.admin_videoaudioother,
										GalleryPage.UserCanAdministerSite ? String.Empty : " gsp_invisible",
										ManageUsersVisible ? String.Empty : " gsp_invisible",
										ManageRolesVisible ? String.Empty : " gsp_invisible",
										Resources.GalleryServerPro.Admin_Site_Settings_Hdr_Text,
										Resources.GalleryServerPro.Admin_Gallery_Settings_Hdr_Text,
										Resources.GalleryServerPro.Admin_Membership_Hdr_Text,
										Resources.GalleryServerPro.Admin_Albums_Hdr_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_Hdr_Text,
										GetAdminUrl(PageId.admin_sitesettings),
										GetAdminUrl(PageId.admin_backuprestore),
										GetAdminUrl(PageId.admin_mediatemplates),
										GetAdminUrl(PageId.admin_css),
										GetAdminUrl(PageId.admin_galleries),
										GetAdminUrl(PageId.admin_gallerysettings),
										GetAdminUrl(PageId.admin_gallerycontrolsettings),
										GetAdminUrl(PageId.admin_uitemplates),
										GetAdminUrl(PageId.admin_eventlog),
										GetAdminUrl(PageId.admin_usersettings),
										GetAdminUrl(PageId.admin_manageusers),
										GetAdminUrl(PageId.admin_manageroles),
										GetAdminUrl(PageId.admin_albums),
										GetAdminUrl(PageId.admin_mediaobjects),
										GetAdminUrl(PageId.admin_metadata),
										GetAdminUrl(PageId.admin_mediaobjecttypes),
										GetAdminUrl(PageId.admin_images),
										GetAdminUrl(PageId.admin_videoaudioother), 
										Resources.GalleryServerPro.Admin_Site_Settings_General_Link_Text,
										Resources.GalleryServerPro.Admin_Site_Settings_Backup_Restore_Link_Text,
										Resources.GalleryServerPro.Admin_Site_Settings_Media_Templates_Link_Text,
										Resources.GalleryServerPro.Admin_Site_Settings_Css_Link_Text,
										Resources.GalleryServerPro.Admin_Galleries_Link_Text,
										Resources.GalleryServerPro.Admin_Gallery_Settings_Link_Text,
										Resources.GalleryServerPro.Admin_Gallery_Control_Settings_Link_Text,
										Resources.GalleryServerPro.Admin_UiTemplate_Link_Text,
										Resources.GalleryServerPro.Admin_Site_Settings_Event_Log_Link_Text,
										Resources.GalleryServerPro.Admin_Membership_User_Settings_Link_Text,
										Resources.GalleryServerPro.Admin_Membership_Manage_Users_Link_Text,
										Resources.GalleryServerPro.Admin_Membership_Manage_Roles_Link_Text,
										Resources.GalleryServerPro.Admin_Albums_General_Link_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_General_Link_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_Metadata_Link_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_Types_Link_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_Types_Images_Text,
										Resources.GalleryServerPro.Admin_Media_Objects_Video_Audio_Other_Link_Text
										);
		}

		/// <summary>
		/// Gets the URL to the requested page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>System.String.</returns>
		private string GetAdminUrl(PageId page)
		{
			if (MediaObjectId > int.MinValue)
			{
				return Utils.GetUrl(page, "moid={0}", MediaObjectId);
			}
			else
			{
				return Utils.GetUrl(page, "aid={0}", AlbumId);
			}
		}

		#endregion
	}
}