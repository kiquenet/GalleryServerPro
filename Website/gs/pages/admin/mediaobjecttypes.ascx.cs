using GalleryServerPro.Business;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace GalleryServerPro.Web.Pages.Admin
{
	/// <summary>
	/// A page-like user control for administering media object MIME types.
	/// </summary>
	public partial class mediaobjecttypes : Pages.AdminPage
	{
		#region Properties

		#endregion

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

		private void ConfigureControlsFirstTime()
		{
			AssignHiddenFormFields();

			AdminPageTitle = Resources.GalleryServerPro.Admin_Media_Objects_Mime_Types_Page_Header;

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
		}

		private void AssignHiddenFormFields()
		{
			hdnMimeTypes.Value = Factory.LoadMimeTypes(GalleryId)
				.Select(mt => new Entity.MimeType()
					{
						Enabled = mt.AllowAddToGallery,
						Extension = mt.Extension,
						FullType = mt.FullType
					}).ToJson();
		}

		private void ConfigureControlsEveryTime()
		{
			this.PageTitle = Resources.GalleryServerPro.Admin_Media_Objects_Mime_Types_Page_Header;
			lblGalleryDescription.Text = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_Gallery_Description_Label, Utils.GetCurrentPageUrl(), Utils.HtmlEncode(Factory.LoadGallery(GalleryId).Description));
		}

		private void SaveSettings()
		{
			this.wwDataBinder.Unbind(this);

			bool previousAllowAllValue = GallerySettings.AllowUnspecifiedMimeTypes;
			if (GallerySettingsUpdateable.AllowUnspecifiedMimeTypes != previousAllowAllValue)
			{
				GallerySettingsUpdateable.Save();
			}

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

			var encoderSettingsStr = hdnMimeTypes.Value;

			var mimeTypes = encoderSettingsStr.FromJson<Entity.MimeType[]>();

			// Loop through each MIME type entity. For each file extension, get the matching MIME type. If the value has changed, update it.
			foreach (var mte in mimeTypes)
			{
				var mimeType = Factory.LoadMimeType(GalleryId, mte.Extension);
				if (mimeType.AllowAddToGallery != mte.Enabled)
				{
					mimeType.AllowAddToGallery = mte.Enabled;
					mimeType.Save();
				}
			}

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