using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages
{
	/// <summary>
	/// The base class that is used for administration pages.
	/// </summary>
	public abstract class AdminPage : Pages.GalleryPage
	{
		#region Private Fields

		private PlaceHolder _phAdminHeader;
		private Controls.Admin.adminheader _adminHeader;
		private PlaceHolder _phAdminFooter;
		private Controls.Admin.adminfooter _adminFooter;
		private Controls.Admin.adminmenu _adminMenu;
		private IGallerySettings _gallerySettings;
		private IGalleryControlSettings _galleryControlSettings;
		private List<String> _usersWithAdminPermission;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminPage"/> class.
		/// </summary>
		protected AdminPage()
		{
			//this.Init += AdminPage_Init;
			this.Load += AdminPage_Load;
			this.BeforeHeaderControlsAdded += AdminPage_BeforeHeaderControlsAdded;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a writeable instance of gallery settings.
		/// </summary>
		/// <value>A writeable instance of gallery settings.</value>
		public IGallerySettings GallerySettingsUpdateable
		{
			get
			{
				if (_gallerySettings == null)
				{
					_gallerySettings = Factory.LoadGallerySetting(GalleryId, true);
				}

				return _gallerySettings;
			}
		}

		/// <summary>
		/// Gets a writeable instance of gallery settings.
		/// </summary>
		/// <value>A writeable instance of gallery settings.</value>
		public IGalleryControlSettings GalleryControlSettingsUpdateable
		{
			get
			{
				if (_galleryControlSettings == null)
				{
					_galleryControlSettings = Factory.LoadGalleryControlSetting(this.GalleryControl.ControlId, true);
				}

				return _galleryControlSettings;
			}
		}

		/// <summary>
		/// Gets or sets the location for the <see cref="GalleryServerPro.Web.Controls.Admin.adminheader"/> user control. Classes that inherit 
		/// <see cref="Pages.AdminPage"/> should set this property to the <see cref="PlaceHolder"/> that is to contain
		/// the admin header control. If this property is not assigned by the inheriting class, the admin header control
		/// is not added to the page output.
		/// </summary>
		/// <value>The <see cref="GalleryServerPro.Web.Controls.Admin.adminheader"/> user control.</value>
		public PlaceHolder AdminHeaderPlaceHolder
		{
			get
			{
				return this._phAdminHeader;
			}
			set
			{
				this._phAdminHeader = value;
			}
		}

		/// <summary>
		/// Gets the admin header user control that is rendered near the top of the administration pages. This control contains the 
		/// page title and the top Save/Cancel buttons. (The bottom Save/Cancel buttons are in the <see cref="GalleryServerPro.Web.Controls.Admin.adminfooter"/> user control.
		/// </summary>
		/// <value>The admin header user control that is rendered near the top of the administration pages.</value>
		public Controls.Admin.adminheader AdminHeader
		{
			get
			{
				return this._adminHeader;
			}
		}

		/// <summary>
		/// Gets or sets the location for the <see cref="GalleryServerPro.Web.Controls.Admin.adminfooter"/> user control. Classes that inherit 
		/// <see cref="Pages.AdminPage"/> should set this property to the <see cref="PlaceHolder"/> that is to contain
		/// the admin footer control. If this property is not assigned by the inheriting class, the admin footer control
		/// is not added to the page output.
		/// </summary>
		/// <value>The <see cref="GalleryServerPro.Web.Controls.Admin.adminfooter"/> user control.</value>
		public PlaceHolder AdminFooterPlaceHolder
		{
			get
			{
				return this._phAdminFooter;
			}
			set
			{
				this._phAdminFooter = value;
			}
		}

		/// <summary>
		/// Gets the admin footer user control that is rendered near the bottom of the administration pages. This control contains the 
		/// bottom Save/Cancel buttons. (The top Save/Cancel buttons are in the <see cref="GalleryServerPro.Web.Controls.Admin.adminheader"/> user control.
		/// </summary>
		/// <value>The admin footer user control that is rendered near the bottom of the administration pages.</value>
		public Controls.Admin.adminfooter AdminFooter
		{
			get
			{
				return this._adminFooter;
			}
		}

		/// <summary>
		/// Gets / sets the page title text (e.g. Site Settings - General).
		/// </summary>
		public string AdminPageTitle
		{
			get
			{
				return this._adminHeader.AdminPageTitle;
			}
			set
			{
				this._adminHeader.AdminPageTitle = value;
			}
		}

		/// <summary>
		/// Gets / sets the text that appears on the top and bottom Ok buttons on the page. This is rendered as the value
		/// attribute of the input HTML tag.
		/// </summary>
		public string OkButtonText
		{
			get
			{
				return this.AdminHeader.OkButtonText;
			}
			set
			{
				this.AdminHeader.OkButtonText = value;
				this.AdminFooter.OkButtonText = value;
			}
		}

		/// <summary>
		/// Gets / sets the ToolTip for the top and bottom Ok buttons on the page. The ToolTip is rendered as 
		/// the title attribute of the input HTML tag.
		/// </summary>
		public string OkButtonToolTip
		{
			get
			{
				return this.AdminHeader.OkButtonToolTip;
			}
			set
			{
				this.AdminHeader.OkButtonToolTip = value;
				this.AdminFooter.OkButtonToolTip = value;
			}
		}

		/// <summary>
		/// Gets / sets the visibility of the top and bottom Ok buttons on the page. When true, the buttons
		/// are visible. When false, they are not visible (not rendered in the page output.)
		/// </summary>
		public bool OkButtonIsVisible
		{
			get
			{
				return this.AdminHeader.OkButtonIsVisible;
			}
			set
			{
				this.AdminHeader.OkButtonIsVisible = value;
				this.AdminFooter.OkButtonIsVisible = value;
			}
		}

		/// <summary>
		/// Gets a reference to the top button that initiates the completion of the task.
		/// </summary>
		public Button OkButtonTop
		{
			get
			{
				return this.AdminHeader.OkButtonTop;
			}
		}

		/// <summary>
		/// Gets a reference to the bottom button that initiates the completion of the task.
		/// </summary>
		public Button OkButtonBottom
		{
			get
			{
				return this.AdminFooter.OkButtonBottom;
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="GalleryServerPro.Web.Controls.Admin.adminmenu"/> control on the page.
		/// </summary>
		/// <value>The <see cref="GalleryServerPro.Web.Controls.Admin.adminmenu"/> control on the page.</value>
		public Controls.Admin.adminmenu AdminMenu
		{
			get
			{
				return this._adminMenu;
			}
		}

		/// <summary>
		/// Gets the list of site administrators and gallery administrators for the current gallery. That is, it 
		/// returns the user names of accounts belonging to roles with AllowAdministerSite or AllowAdministerGallery permission.
		/// </summary>
		/// <value>The list of site and gallery administrators.</value>
		public List<String> UsersWithAdminPermission
		{
			get
			{
				if (this._usersWithAdminPermission == null)
				{
					this._usersWithAdminPermission = new List<string>();

					foreach (var role in RoleController.GetGalleryServerRoles())
					{
						if (role.AllowAdministerSite || role.AllowAdministerGallery && role.Galleries.Any(g => g.GalleryId == GalleryId))
						{
							foreach (var userName in RoleController.GetUsersInRole(role.RoleName).Where(userName => !this._usersWithAdminPermission.Contains(userName)))
							{
								this._usersWithAdminPermission.Add(userName);
							}
						}
					}
				}

				return this._usersWithAdminPermission;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the logged on user can add, edit, or delete users and roles. Returns true when the user is a site administrator
		/// and - for gallery admins - when the application setting <see cref="IAppSetting.AllowGalleryAdminToManageUsersAndRoles" /> is true.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the logged on user can add, edit, or delete users and roles; otherwise, <c>false</c>.
		/// </value>
		public bool UserCanEditUsersAndRoles
		{
			get
			{
				return UserCanAdministerSite || (!UserCanAdministerSite && UserCanAdministerGallery && AppSetting.Instance.AllowGalleryAdminToManageUsersAndRoles);
			}
		}

		#endregion

		#region Event Handlers

		void AdminPage_Load(object sender, EventArgs e)
		{
			ValidateRequest();

			AddUserControls();

			ConfigureControls();

			AddFooter();

			AddMaintenanceServiceCallIfNeeded();
		}

		protected void AdminPage_BeforeHeaderControlsAdded(object sender, EventArgs e)
		{
			// Add the admin menu to the page. Note that if you use any index other than 0 in the AddAt method, the viewstate
			// is not preserved across postbacks. This is the reason why the <see cref="BeforeHeaderControlsAdded"/> event was created in 
			// <see cref="GalleryPage"/> and handled here. We need to add the admin menu *before* <see cref="GalleryPage"/> adds the album breadcrumb
			// menu and the gallery header.
			Controls.Admin.adminmenu adminMenu = (Controls.Admin.adminmenu)LoadControl(Utils.GetUrl("/controls/admin/adminmenu.ascx"));
			this._adminMenu = adminMenu;
			this.Controls.AddAt(0, adminMenu);
			//this.Controls.AddAt(Controls.IndexOf(AlbumMenu) + 1, adminMenu); // Do not use: viewstate is not preserved
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates script that executes the <paramref name="scriptToRun"/> after the DOM is fully loaded. This is needed
		/// because simply passing the script to ScriptManager.RegisterStartupScript may cause it to run before the DOM is 
		/// fully initialized. This method should only be called once for the page because it hard-codes a javascript function
		/// named adminPageLoad.
		/// </summary>
		/// <param name="scriptToRun">The script to run.</param>
		/// <returns>Returns a string that can be passed to the ScriptManager.RegisterStartupScript method. Does not include
		/// the script tags.</returns>
		protected static string GetPageLoadScript(string scriptToRun)
		{
			return String.Format(CultureInfo.InvariantCulture,
@"
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(adminPageLoad);

function adminPageLoad(sender, args)
{{
	{0}
}}
",
				scriptToRun);
		}

		#endregion

		#region Protected Methods

		///// <summary>
		///// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		///// </summary>
		///// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		//protected override void Render(HtmlTextWriter writer)
		//{
		//	// Write out the HTML for this control.
		//	base.Render(writer);

		//	//AddMaintenanceServiceCallIfNeeded();
		//}

		#endregion

		#region Private Methods

		private void AddUserControls()
		{
			Controls.Admin.adminheader adminHeader = (Controls.Admin.adminheader)LoadControl(Utils.GetUrl("/controls/admin/adminheader.ascx"));
			this._adminHeader = adminHeader;
			if (this.AdminHeaderPlaceHolder != null)
				this.AdminHeaderPlaceHolder.Controls.Add(adminHeader);

			Controls.Admin.adminfooter adminFooter = (Controls.Admin.adminfooter)LoadControl(Utils.GetUrl("/controls/admin/adminfooter.ascx"));
			this._adminFooter = adminFooter;
			if (this.AdminFooterPlaceHolder != null)
				this.AdminFooterPlaceHolder.Controls.Add(adminFooter);
		}

		private void ConfigureControls()
		{
			if ((this.AdminHeaderPlaceHolder != null) && (this.AdminHeader != null) && (this.AdminHeader.OkButtonTop != null))
				this.Page.Form.DefaultButton = this.AdminHeader.OkButtonTop.UniqueID;
		}

		/// <summary>
		/// Verify that the inferred gallery ID is the same as the one specified for this page.
		/// If not, remove the aid parameter and reload page. This helps prevent the situation
		/// where a page is assigned to a particular gallery but the album ID specified in the
		/// URL belongs to another gallery, causing the page to render with settings for the 
		/// other gallery, potentially confusing the admin.
		/// </summary>
		private void ValidateRequest()
		{
			// If the inferred gallery ID is different than the one specified for this page,
			// remove the aid parameter and reload page.
			if (GalleryId != GalleryControl.GalleryId)
			{
				string url = Utils.RemoveQueryStringParameter(Request.Url.PathAndQuery, "aid");

				Utils.Redirect(url);
			}

			// If the album ID in the query string is set to int.MinValue, remove it and reload the page.
			// I don't think there are valid reasons for ever having the aid parameter set to this value,
			// so someday we might want to change the logic so this value never becomes part of the query
			// string in the first place.
			if (Utils.IsQueryStringParameterPresent("aid") && Utils.GetQueryStringParameterInt32("aid") == int.MinValue)
			{
				string url = Utils.RemoveQueryStringParameter(Request.Url.PathAndQuery, "aid");

				Utils.Redirect(url);
			}
		}

		/// <summary>
		/// If needed, start the maintenance routine.
		/// </summary>
		/// <remarks>The background thread cannot access HttpContext.Current, so this method will probably fail under DotNetNuke.
		/// To fix that, figure out what DNN needs (portal ID?), and pass it in as a parameter.
		/// so that approach was replaced with this one.</remarks>
		private void AddMaintenanceServiceCallIfNeeded()
		{
			if (AppSetting.Instance.MaintenanceStatus == MaintenanceStatus.NotStarted && !IsPostBack)
			{
				Utils.PerformMaintenance();
				//				const string script = @"
				//$(function() {
				//	Gsp.Gallery.PerformMaintenance(function() {}, function() {}); // Swallow error on client
				//});";

				//				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "galleryPageStartupScript", script, true);
			}
		}

		private void AddFooter()
		{
			// Add the GSP version info to the end
			if (this.AdminFooterPlaceHolder != null)
			{
				var license = AppSetting.Instance.License.LicenseType;
				if (license == LicenseLevel.NotSet || license == LicenseLevel.Gpl)
					AdminFooterPlaceHolder.Controls.Add(GspLogo);
				
				AdminFooterPlaceHolder.Controls.Add(new LiteralControl(string.Format("<p class='gsp_textcenter'>Gallery Server Pro {0}</p>", Utils.GetGalleryServerVersion())));
			}
		}

		#endregion
	}
}
