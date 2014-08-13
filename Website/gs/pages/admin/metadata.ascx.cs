using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages.Admin
{
	/// <summary>
	/// A page-like user control for administering media object metadata settings.
	/// </summary>
	public partial class metadata : Pages.AdminPage
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

			RegisterJavascript();

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

		/// <summary>
		/// Handles the OnValidateControl event of the wwDataBinder control.
		/// </summary>
		/// <param name="item">The wwDataBindingItem item.</param>
		/// <returns>Returns <c>true</c> if the item is valid; otherwise returns <c>false</c>.</returns>
		protected bool wwDataBinder_ValidateControl(GalleryServerPro.WebControls.wwDataBindingItem item)
		{
			// Validate various settings to make sure they don't conflict with each other.
			return true;
		}

		/// <summary>
		/// Handles the OnBeforeUnBindControl event of the wwDataBinder control.
		/// </summary>
		/// <param name="item">The wwDataBindingItem item.</param>
		protected bool wwDataBinder_BeforeUnbindControl(WebControls.wwDataBindingItem item)
		{
			// Disabled HTML items are not posted during a postback, so we don't have accurate information about their states. 
			// Look for the checkboxes that cause other controls to be disabled, and assign the value of the disabled control to the
			// desired setting.
			if (!this.chkExtractMetadata.Checked)
			{
				// When the metadata feature is unchecked, the enhanced metadata checkbox must be false as well.
				if (item.ControlId == this.chkExtractMetadataUsingWpf.ID)
				{
					this.chkExtractMetadataUsingWpf.Checked = false;
				}
			}
			return true;
		}

		#endregion

		#region Private Methods

		private void ConfigureControlsEveryTime()
		{
			this.PageTitle = Resources.GalleryServerPro.Admin_Media_Objects_Metadata_Page_Header;
			lblGalleryDescription.Text = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_Gallery_Description_Label, Utils.GetCurrentPageUrl(), Utils.HtmlEncode(Factory.LoadGallery(GalleryId).Description));
		}

		private void RegisterJavascript()
		{
			// Add link to file to support SlickGrid.
			var url = Utils.IsDebugEnabled ? Utils.GetUrl("/script/debug/grid.js") : Utils.GetUrl("/script/debug/grid.js");
			var link = String.Format("<script type='text/javascript' src='{0}'></script>", url);
			this.Page.Header.Controls.Add(new LiteralControl(link));
		}

		private void ConfigureControlsFirstTime()
		{
			AdminPageTitle = Resources.GalleryServerPro.Admin_Media_Objects_Metadata_Page_Header;

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

			BindMetadata();

			this.wwDataBinder.DataBind();
		}

		private void BindMetadata()
		{
			hdnMetadataDefinitions.Value = GallerySettingsUpdateable.MetadataDisplaySettings.ToJson();
		}

		private void SaveSettings()
		{
			string meta = hdnMetadataDefinitions.Value;

			MetadataDefinition[] metas = meta.FromJson<MetadataDefinition[]>();

			ValidateBeforeSave(metas);

			GallerySettingsUpdateable.MetadataDisplaySettings.Clear();
			foreach (var metaDef in metas)
			{
				GallerySettingsUpdateable.MetadataDisplaySettings.Add(metaDef);
			}

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

			BindMetadata();

			ClientMessage = new ClientMessageOptions
			{
				Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
				Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
				Style = MessageStyle.Success
			};
		}

		private void ValidateBeforeSave(IEnumerable<MetadataDefinition> metas)
		{
			foreach (var metaDef in metas)
			{
				if (metaDef.MetadataItem == MetadataItemName.Rating && metaDef.IsVisibleForAlbum)
				{
					wwDataBinder.AddBindingError("Album ratings are not supported. Uncheck the album option for the Rating item.", hdnMetadataDefinitions);
				}
			}
		}

		#endregion
	}
}