using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using System.IO;

namespace GalleryServerPro.Web.Pages.Admin
{
	/// <summary>
	/// A page-like user control for viewing and editing the cascading style sheets used by the gallery.
	/// </summary>
	public partial class css : Pages.AdminPage
	{
		#region Properties

		/// <summary>
		/// Gets the full path to the CSS file.
		/// </summary>
		/// <value>The CSS file path.</value>
		protected string CssFileUrl
		{
			get
			{
				return Utils.GetSkinnedUrl("/styles/gallery.css");
			}
		}

		/// <summary>
		/// Gets the full path to the CSS file.
		/// </summary>
		/// <value>The CSS file path.</value>
		protected string CssFilePath
		{
			get
			{
				return HttpContext.Current.Server.MapPath(CssFileUrl);
			}
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
			if (!UserCanAdministerSite && UserCanAdministerGallery)
			{
				Utils.Redirect(PageId.admin_gallerysettings, "aid={0}", this.GetAlbumId());
			}

			this.CheckUserSecurity(SecurityActions.AdministerSite | SecurityActions.AdministerGallery);

			ConfigureControlsEveryTime();

			if (!IsPostBack)
			{
				ConfigureControlsFirstTime();
			}
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.AdminHeaderPlaceHolder = phAdminHeader;
			this.AdminFooterPlaceHolder = phAdminFooter;
		}

		/// <summary>
		/// Determines whether the event for the server control is passed up the page's UI server control hierarchy.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="args">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		/// <returns>
		/// true if the event has been cancelled; otherwise, false. The default is false.
		/// </returns>
		protected override bool OnBubbleEvent(object source, EventArgs args)
		{
			//An event from the control has bubbled up.  If it's the Ok button, then run the
			//code to save the data to the database; otherwise ignore.
			Button btn = source as Button;
			if ((btn != null) && (((btn.ID == "btnOkTop") || (btn.ID == "btnOkBottom"))))
			{
				SaveSettings();
			}

			return true;
		}

		#endregion

		#region Private Methods

		private void ConfigureControlsEveryTime()
		{
			this.PageTitle = Resources.GalleryServerPro.Admin_Site_Settings_Css_Page_Header;
		}

		private void ConfigureControlsFirstTime()
		{
			AdminPageTitle = Resources.GalleryServerPro.Admin_Site_Settings_Css_Page_Header;

			if (AppSetting.Instance.License.IsInReducedFunctionalityMode)
			{
				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Admin_Site_Settings_ProductKey_NotEntered_Label,
					Message = Resources.GalleryServerPro.Admin_Need_Product_Key_Msg2,
					Style = MessageStyle.Info
				};

				OkButtonBottom.Enabled = false;
				OkButtonTop.Enabled = false;
			}

			txtCss.Text = File.ReadAllText(CssFilePath);
		}

		private void SaveSettings()
		{
			File.WriteAllText(CssFilePath, txtCss.Text, System.Text.Encoding.UTF8);

			ClientMessage = new ClientMessageOptions
			{
				Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
				Message = String.Concat("<p>", Resources.GalleryServerPro.Admin_Save_Success_Text, "</p><p>You may need to force a browser refresh to load the CSS changes.</p>"),
				Style = MessageStyle.Success
			};
		}

		#endregion
	}
}