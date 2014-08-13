using System;
using System.Globalization;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;

namespace GalleryServerPro.Web.Pages.Admin
{
	/// <summary>
	/// A page-like user control for administering image settings.
	/// </summary>
	public partial class images : Pages.AdminPage
	{
		#region Protected Events

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
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.CheckUserSecurity(SecurityActions.AdministerSite | SecurityActions.AdministerGallery);

			ConfigureControlsEveryTime();

			if (!IsPostBack)
			{
				ConfigureControlsFirstTime();
			}
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
			this.PageTitle = Resources.GalleryServerPro.Admin_Images_General_Page_Header;
			lblGalleryDescription.Text = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_Gallery_Description_Label, Utils.GetCurrentPageUrl(), Utils.HtmlEncode(Factory.LoadGallery(GalleryId).Description));
		}

		private void ConfigureControlsFirstTime()
		{
			AdminPageTitle = Resources.GalleryServerPro.Admin_Images_General_Page_Header;

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

			this.wwDataBinder.DataBind();

			ddlWmkTextLocation.DataSource = Enum.GetValues(typeof(System.Drawing.ContentAlignment));
			ddlWmkTextLocation.DataBind();

			ddlWmkImageLocation.DataSource = Enum.GetValues(typeof(System.Drawing.ContentAlignment));
			ddlWmkImageLocation.DataBind();
		}

		private void SaveSettings()
		{
			this.wwDataBinder.Unbind(this);

			if (wwDataBinder.BindingErrors.Count > 0)
			{
				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Validation_Summary_Text,
					Message = wwDataBinder.BindingErrors.ToString(),
					Style = MessageStyle.Error
				};

				return;
			}

			GallerySettingsUpdateable.Save();

			HelperFunctions.PurgeCache();

			Factory.ClearWatermarkCache();

			ClientMessage = new ClientMessageOptions
			{
				Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
				Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
				Style = MessageStyle.Success
			};
		}

		#endregion
	}
}