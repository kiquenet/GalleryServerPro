using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages.Task
{
	/// <summary>
	/// A page-like user control that handles the Add objects task.
	/// </summary>
	public partial class addobjects : Pages.TaskPage
	{
		#region Properties

		protected string AddObjectsInstruction
		{
			get { return Resources.GalleryServerPro.Task_Add_Objects_Local_Media_Tab_Dtl; }
		}

		protected string AddObjectsUploadingText
		{
			get { return Resources.GalleryServerPro.Task_Add_Objects_Uploading_Text; }
		}

		protected string AddFilesTabCssClass
		{
			get
			{
				return (GallerySettings.AllowAddLocalContent ? String.Empty : "gsp_invisible");
			}
		}

		protected string AddHtmlTabCssClass
		{
			get
			{
				return (GallerySettings.AllowAddExternalContent ? String.Empty : "gsp_invisible");
			}
		}

		protected string AddObjectsCssClass
		{
			get
			{
				var cssClass = "gsp_addpadding2 gsp_addObjTabContainer gsp_tabContainer";

				if (!GallerySettings.AllowAddLocalContent && !GallerySettings.AllowAddExternalContent)
				{
					cssClass += " gsp_invisible";
				}

				return cssClass;
			}
		}

		/// <summary>
		/// Gets the name of the cookie that stores the index of the currently selected tab.
		/// </summary>
		/// <value>A string.</value>
		protected string SelectedTabCookieName { get { return String.Concat(cid, "_aobj_cookie"); } }

		#endregion

		#region Protected Functions

		/// <summary>
		/// Gets an string representing a javascript array containing the list of allowed file extensions. Returns an empty array
		/// when there aren't any restrictions.
		/// </summary>
		/// <returns>System.String.</returns>
		protected string GetFileFilters()
		{
			if (GallerySettings.AllowUnspecifiedMimeTypes)
			{
				return "[]";
			}

			var extensions = String.Join(",", Factory.LoadMimeTypes(GalleryId).Where(mt => mt.AllowAddToGallery).Select(mt => mt.Extension).Concat(new[] { ".zip" }));
			return String.Format(CultureInfo.InvariantCulture, "[{{ title: 'Supported files', extensions: '{0}' }}]", extensions.Replace(".", ""));
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.TaskHeaderPlaceHolder = phTaskHeader;
			this.TaskFooterPlaceHolder = phTaskFooter;
		}
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (GallerySettings.MediaObjectPathIsReadOnly)
				RedirectToAlbumViewPage("msg={0}", ((int)MessageType.CannotEditGalleryIsReadOnly).ToString(CultureInfo.InvariantCulture));

		  if (GetAlbum().IsVirtualAlbum)
        RedirectToAlbumViewPage("msg={0}", ((int)MessageType.AlbumDoesNotExist).ToString(CultureInfo.InvariantCulture));

			this.CheckUserSecurity(SecurityActions.AddMediaObject);

			if (!IsPostBack)
			{
				ConfigureControlsFirstTime();
			}

			ConfigureControlsEveryPageLoad();
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
				if (AddExternalHtmlContent())
					RedirectToAlbumViewPage();
			}

			return true;
		}


		/// <summary>
		/// Handles the Click event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnCancel_Click(object sender, EventArgs e)
		{
			this.RedirectToPreviousPage();
		}

		#endregion

		#region Private Methods

		private void ConfigureControlsFirstTime()
		{
			//this.OkButtonIsVisible = false; // Instead, we'll use our own buttons inside the tab control.
			//this.CancelButtonIsVisible = false;
			this.OkButtonText = Resources.GalleryServerPro.Task_Add_Objects_OK_Btn_Text;
			this.TaskHeaderText = Resources.GalleryServerPro.Task_Add_Objects_Header_Text;
			this.TaskBodyText = Resources.GalleryServerPro.Task_Add_Objects_Body_Text;

			this.PageTitle = Resources.GalleryServerPro.Task_Add_Objects_Page_Title;

			if (!HelperFunctions.IsFileAuthorizedForAddingToGallery("dummy.zip", GalleryId))
			{
				chkDoNotExtractZipFile.Enabled = false;
				chkDoNotExtractZipFile.CssClass = "gsp_disabledtext";
			}

			if (GallerySettings.DiscardOriginalImageDuringImport)
			{
				chkDiscardOriginal.Checked = true;
				chkDiscardOriginal.Enabled = false;
				chkDiscardOriginal.CssClass = "gsp_disabledtext";
			}

			HttpContext.Current.Session.Remove(GlobalConstants.SkippedFilesDuringUploadSessionKey);
		}

		private void ConfigureControlsEveryPageLoad()
		{
			RegisterJavascriptFiles();

			VerifyUploadSettingsAreEnabled();
		}

		private void RegisterJavascriptFiles()
		{
			HtmlHead head = this.Page.Header;
			if (head == null)
				throw new WebException(Resources.GalleryServerPro.Error_Head_Tag_Missing_Server_Attribute_Ex_Msg);

			string script = String.Format(CultureInfo.InvariantCulture, @"
	<script src='{0}'></script>
	<script src='{1}'></script>
",
 Utils.GetUrl("/script/plupload/plupload.full.min.js"),
 Utils.GetUrl("/script/plupload/jquery.ui.plupload.min.js")
 );

			head.Controls.Add(new LiteralControl(script));
		}

		private bool AddExternalHtmlContent()
		{
			string externalHtmlSource = txtExternalHtmlSource.Text.Trim();

			if (!this.ValidateExternalHtmlSource(externalHtmlSource))
				return false;

			MimeTypeCategory mimeTypeCategory = MimeTypeCategory.Other;
			string mimeTypeCategoryString = ddlMediaTypes.SelectedValue;
			try
			{
				mimeTypeCategory = (MimeTypeCategory)Enum.Parse(typeof(MimeTypeCategory), mimeTypeCategoryString, true);
			}
			catch { } // Suppress any parse errors so that category remains the default value 'Other'.

			string title = txtTitle.Text.Trim();
			if (String.IsNullOrEmpty(title))
			{
				// If user didn't enter a title, use the media category (e.g. Video, Audio, Image, Other).
				title = mimeTypeCategory.ToString();
			}

			IGalleryObject mediaObject = Factory.CreateExternalMediaObjectInstance(externalHtmlSource, mimeTypeCategory, this.GetAlbum());
			mediaObject.Title = Utils.CleanHtmlTags(title, GalleryId);
			GalleryObjectController.SaveGalleryObject(mediaObject);
			HelperFunctions.PurgeCache();

			return true;
		}

		private bool ValidateExternalHtmlSource(string externalHtmlSource)
		{
			if (Utils.IsCurrentUserGalleryAdministrator(GalleryId))
				return true; // Allow admins to enter any HTML they want

			IHtmlValidator htmlValidator = Factory.GetHtmlValidator(externalHtmlSource, GalleryId);
			htmlValidator.Validate();
			if (!htmlValidator.IsValid)
			{
				string invalidHtmlTags = String.Join(", ", htmlValidator.InvalidHtmlTags.ToArray());
				string invalidHtmlAttributes = String.Join(", ", htmlValidator.InvalidHtmlAttributes.ToArray());
				string javascriptDetected = (htmlValidator.InvalidJavascriptDetected ? Resources.GalleryServerPro.Task_Add_Objects_External_Tab_Javascript_Detected_Yes : Resources.GalleryServerPro.Task_Add_Objects_External_Tab_Javascript_Detected_No);

				if (String.IsNullOrEmpty(invalidHtmlTags))
					invalidHtmlTags = Resources.GalleryServerPro.Task_Add_Objects_External_Tab_No_Invalid_Html;

				if (String.IsNullOrEmpty(invalidHtmlAttributes))
					invalidHtmlAttributes = Resources.GalleryServerPro.Task_Add_Objects_External_Tab_No_Invalid_Html;

				ClientMessage = new ClientMessageOptions
				{
					Title = "Invalid text",
					Message = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Task_Add_Objects_External_Tab_Invalid_Html_Msg, invalidHtmlTags, invalidHtmlAttributes, javascriptDetected),
					Style = MessageStyle.Warning
				};

				return false;
			}
			return true;
		}

		private void VerifyUploadSettingsAreEnabled()
		{
			if (!GallerySettings.AllowAddLocalContent && !GallerySettings.AllowAddExternalContent)
			{
				// Both settings are disabled, which means no objects can be added. This is probably a mis-configuration,
				// so give a friendly message to help point the administrator in the right direction for changing it.
				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Task_No_Objects_Selected_Hdr,
					Message = Resources.GalleryServerPro.Task_Add_Objects_All_Adding_Types_Disabled_Msg,
					Style = MessageStyle.Info,
					AutoCloseDelay = 0
				};

				this.OkButtonTop.Visible = false;
				this.OkButtonBottom.Visible = false;
			}
		}

		#endregion
	}
}