using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages.Admin
{
	/// <summary>
	/// A page-like user control for administering settings for the gallery template functionality.
	/// </summary>
	public partial class uitemplates : Pages.AdminPage
	{
		#region Private Fields / Enums

		private enum PageMode
		{
			Unknown = 0,
			Edit,
			Insert
		}

		#endregion

		#region Properties

		private PageMode ViewMode
		{
			get
			{
				object viewStateValue = ViewState["ViewMode"];

				return (viewStateValue != null ? (PageMode)viewStateValue : PageMode.Unknown);
			}
			set
			{
				ViewState["ViewMode"] = value;
			}
		}

		private IUiTemplate CurrentUiTemplate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the name of the cookie that stores the index of the currently selected tab.
		/// </summary>
		/// <value>A string.</value>
		protected string SelectedTabCookieName { get { return String.Concat(cid, "_ui_tmpl_cookie"); } }

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.CheckUserSecurity(SecurityActions.AdministerSite | SecurityActions.AdministerGallery);

			if (!IsPostBack)
			{
				ConfigureControlsFirstTime();
			}

			ConfigureControlsEveryTime();
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

		protected void ddlGalleryItem_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindTemplateNameDropDownList();

			CurrentUiTemplate = GetSelectedJQueryTemplate();

			BindUiTemplate();
		}

		protected void ddlTemplateName_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentUiTemplate = GetSelectedJQueryTemplate();
			BindUiTemplate();
		}

		protected void lbCreate_Click(object sender, EventArgs e)
		{
			ViewMode = PageMode.Insert;

			IUiTemplate tmplCopy = CurrentUiTemplate.Copy();

			tmplCopy.Name = GenerateUniqueTemplateName();

			CurrentUiTemplate = tmplCopy;

			ddlTemplateName.Items.Add(new ListItem(tmplCopy.Name, int.MinValue.ToString()));
			ddlTemplateName.SelectedIndex = ddlTemplateName.Items.Count - 1;

			btnDelete.Enabled = false;

			BindUiTemplate();
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			CurrentUiTemplate = GetSelectedJQueryTemplate();

			string invalidReason;
			if (ValidateUiTemplateBeforeSave(out invalidReason))
			{
				UnbindJQueryTemplate();
				CurrentUiTemplate.Save();

				BindTemplateNameDropDownList();
				ddlTemplateName.SelectedIndex = ddlTemplateName.Items.IndexOf(ddlTemplateName.Items.FindByValue(CurrentUiTemplate.UiTemplateId.ToString(CultureInfo.InvariantCulture)));
				ViewMode = PageMode.Edit;

				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
					Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
					Style = MessageStyle.Success
				};
			}
			else
			{
				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Validation_Summary_Text,
					Message = invalidReason,
					Style = MessageStyle.Error
				};
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			if (ViewMode == PageMode.Insert)
				BindTemplateNameDropDownList();

			CurrentUiTemplate = GetSelectedJQueryTemplate();
			BindUiTemplate();
			ViewMode = PageMode.Edit;
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			CurrentUiTemplate = GetSelectedJQueryTemplate();

			string invalidReason;
			if (ValidateUiTemplateBeforeDelete(out invalidReason))
			{
				CurrentUiTemplate.Delete();
				BindTemplateNameDropDownList();
				CurrentUiTemplate = GetSelectedJQueryTemplate();
				BindUiTemplate();
				ViewMode = PageMode.Edit;

				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
					Message = Resources.GalleryServerPro.Admin_Templates_Deleted_Msg,
					Style = MessageStyle.Success
				};
			}
			else
			{
				ClientMessage = new ClientMessageOptions
				{
					Title = Resources.GalleryServerPro.Validation_Summary_Text,
					Message = invalidReason,
					Style = MessageStyle.Error
				};
			}
		}

		#endregion

		#region Private Methods

		private void ConfigureControlsEveryTime()
		{
			this.PageTitle = Resources.GalleryServerPro.Admin_UiTemplates_Page_Header;
			lblGalleryDescription.Text = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_Gallery_Description_Label, Utils.GetCurrentPageUrl(), Utils.HtmlEncode(Factory.LoadGallery(GalleryId).Description));

			this.tvUC.RequiredSecurityPermissions = SecurityActions.AdministerSite | SecurityActions.AdministerGallery;

			btnDelete.Enabled = true;

			CurrentUiTemplate = GetSelectedJQueryTemplate();
		}

		private void ConfigureControlsFirstTime()
		{
			ViewMode = PageMode.Edit;
			OkButtonIsVisible = false;

			CurrentUiTemplate = LoadDefaultJQueryTemplate();

			BindDropDownLists();

			BindUiTemplate();

			AdminPageTitle = Resources.GalleryServerPro.Admin_UiTemplates_Page_Header;

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

			ActivateFirstTab();
		}

		/// <summary>
		/// Update the cookie that stores the selected tab index to 0. This forces the first tab to be selected on the
		/// first page load.
		/// </summary>
		private void ActivateFirstTab()
		{
			var script = String.Format("	document.cookie = '{0}=0;';", SelectedTabCookieName);
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Concat(this.cid, "_uitmplScript"), script, true);
		}

		private void BindDropDownLists()
		{
			BindGalleryItemDropDownList();

			BindTemplateNameDropDownList();
		}

		private void BindGalleryItemDropDownList()
		{
			ddlGalleryItem.DataTextField = "Value";
			ddlGalleryItem.DataValueField = "Key";

			var kvps = Utils.GetEnumList(typeof(UiTemplateType)).AsQueryable();

			ddlGalleryItem.DataSource = kvps.Where(k => k.Key != UiTemplateType.NotSpecified.ToString());
			ddlGalleryItem.DataBind();
		}

		private void BindTemplateNameDropDownList()
		{
			var templateType = Enum<UiTemplateType>.Parse(ddlGalleryItem.SelectedValue);

			var tmplNames = (from a in GalleryController.GetUiTemplates()
											 where a.TemplateType == templateType && a.GalleryId == GalleryId
											 select new KeyValuePair<int, string>(a.UiTemplateId, a.Name));

			ddlTemplateName.DataTextField = "Value";
			ddlTemplateName.DataValueField = "Key";
			ddlTemplateName.DataSource = tmplNames;
			ddlTemplateName.DataBind();
		}

		/// <summary>
		/// Gets the template that matches the selected item in the dropdown list.
		/// </summary>
		/// <returns>An instance of <see cref="IUiTemplate" />.</returns>
		private IUiTemplate GetSelectedJQueryTemplate()
		{
			if (ddlTemplateName.SelectedValue != int.MinValue.ToString(CultureInfo.InvariantCulture))
			{
				return (from t in UiTemplates
								where t.UiTemplateId == Convert.ToInt32(ddlTemplateName.SelectedValue, CultureInfo.InvariantCulture)
								select t).FirstOrDefault();
			}
			else
			{
				return new UiTemplate
								{
									UiTemplateId = int.MinValue,
									TemplateType = Enum<UiTemplateType>.Parse(ddlGalleryItem.SelectedValue),
									GalleryId = GalleryId,
									Name = ddlTemplateName.SelectedItem.Text,
									RootAlbumIds = new IntegerCollection(),
									HtmlTemplate = String.Empty,
									ScriptTemplate = String.Empty
								};
			}
		}

		/// <summary>
		/// Gets the default UI template for the album. Guaranteed to not return null.
		/// </summary>
		/// <returns>Returns an instance of <see cref="IUiTemplate" />.</returns>
		/// <exception cref="WebException">Thrown when no UI template exists in the data store having 
		/// TemplateType='Album' and TemplateName='Default'</exception>
		private IUiTemplate LoadDefaultJQueryTemplate()
		{
			IUiTemplate tmpl = (from a in UiTemplates
													where a.TemplateType == UiTemplateType.Album && a.GalleryId == GalleryId && a.Name == "Default"
													select a).FirstOrDefault();

			if (tmpl == null)
				throw new WebException("Could not find a UI template in the data store having TemplateType='Album' and TemplateName='Default'");

			return tmpl;
		}

		/// <summary>
		/// Copies the data from <see cref="CurrentUiTemplate" /> to the relevant web form controls.
		/// </summary>
		private void BindUiTemplate()
		{
			txtTemplateName.Text = CurrentUiTemplate.Name;
			txtTemplate.Text = CurrentUiTemplate.HtmlTemplate;
			txtScript.Text = CurrentUiTemplate.ScriptTemplate;
			tvUC.SelectedAlbumIds = CurrentUiTemplate.RootAlbumIds;
		}

		/// <summary>
		/// Copies the data from the web form to the relevant properties of the <see cref="CurrentUiTemplate" /> property.
		/// </summary>
		private void UnbindJQueryTemplate()
		{
			CurrentUiTemplate.Name = txtTemplateName.Text.Trim();
			CurrentUiTemplate.HtmlTemplate = txtTemplate.Text.Trim();
			CurrentUiTemplate.ScriptTemplate = txtScript.Text.Trim();
			CurrentUiTemplate.Description = String.Empty;
			CurrentUiTemplate.RootAlbumIds = tvUC.SelectedAlbumIds;
		}

		private string GenerateUniqueTemplateName()
		{
			List<string> tmplNames = new List<string>(ddlTemplateName.Items.Count);
			foreach (ListItem item in ddlTemplateName.Items)
			{
				tmplNames.Add(item.Text);
			}

			string proposedName = String.Concat(CurrentUiTemplate.Name, " (copy)");
			int counter = 1;

			while (tmplNames.Contains(proposedName))
			{
				// There is already a template with our proposed name. We need to strip off the
				// previous suffix and try again.
				proposedName = proposedName.Remove(proposedName.LastIndexOf(" (copy"));

				// Generate the new suffix to append to the filename (e.g. "(3)")
				proposedName = String.Concat(proposedName, " (copy ", counter, ")");

				counter++;
			}

			return proposedName;
		}

		/// <summary>
		/// Verify the UI template can be saved.
		/// </summary>
		/// <param name="invalidReason">A message describing why the validation failed. Set to <see cref="String.Empty" /> when
		/// validation succeeds.</param>
		/// <returns><c>true</c> if business rules for saving are satisfied, <c>false</c> otherwise</returns>
		private bool ValidateUiTemplateBeforeSave(out string invalidReason)
		{
			// TEST 1: Cannot save changes to the default template.
			if (CurrentUiTemplate.Name.Equals("Default", StringComparison.OrdinalIgnoreCase))
			{
				var htmlHasChanged = !CurrentUiTemplate.HtmlTemplate.Equals(txtTemplate.Text, StringComparison.Ordinal);
				var scriptHasChanged = !CurrentUiTemplate.ScriptTemplate.Equals(txtScript.Text, StringComparison.Ordinal);

				if (htmlHasChanged || scriptHasChanged)
				{
					invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Modify_Default_Tmpl_Msg;
					return false;
				}
			}

			// TEST 2: Verify no other template has the same name in this category.
			var tmpl = (from t in UiTemplates
									where t.TemplateType == CurrentUiTemplate.TemplateType &&
									t.GalleryId == GalleryId &&
									t.Name == txtTemplateName.Text &&
									t.UiTemplateId != CurrentUiTemplate.UiTemplateId
									select t).FirstOrDefault();

			if (tmpl != null)
			{
				invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Save_Duplicate_Name_Msg;
				return false;
			}

			// TEST 3: Verify user isn't removing the last template from the root album.
			var rootAlbumId = Factory.LoadRootAlbumInstance(GalleryId).Id;

			var curTmplNotAssignedToRootAlbum = !tvUC.SelectedAlbumIds.Contains(rootAlbumId); // Need to use tvUC.SelectedAlbumIds instead of CurrentUiTemplate.RootAlbumIds because CurrentUiTemplate has not yet been unbound
			var noOtherTmplAssignedToRootAlbum = !UiTemplates.Any(t => t.TemplateType == CurrentUiTemplate.TemplateType && t.UiTemplateId != CurrentUiTemplate.UiTemplateId && t.RootAlbumIds.Contains(rootAlbumId));

			if (curTmplNotAssignedToRootAlbum && noOtherTmplAssignedToRootAlbum)
			{
				invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Save_No_Tmpl_Msg;
				return false;
			}

			// TEST 4: The default template cannot be renamed to something else.
			if (CurrentUiTemplate.Name.Equals("Default", StringComparison.OrdinalIgnoreCase) && !txtTemplateName.Text.Equals("Default", StringComparison.OrdinalIgnoreCase))
			{
				invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Save_No_Default_Tmpl_Msg;
				return false;
			}

			// All the tests pass, so return true.
			invalidReason = String.Empty;
			return true;
		}

		/// <summary>
		/// Verify the UI template can be deleted.
		/// </summary>
		/// <param name="invalidReason">A message describing why the validation failed. Set to <see cref="String.Empty" /> when
		/// validation succeeds.</param>
		/// <returns><c>true</c> if business rules for deleting are satisfied, <c>false</c> otherwise</returns>
		private bool ValidateUiTemplateBeforeDelete(out string invalidReason)
		{
			// TEST 1: Cannot delete the default template.
			if (CurrentUiTemplate.Name.Equals("Default", StringComparison.OrdinalIgnoreCase))
			{
				invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Modify_Default_Tmpl_Msg;
				return false;
			}

			// TEST 2: Cannot delete a template if it leaves one ore more albums without a template
			var rootAlbumId = Factory.LoadRootAlbumInstance(GalleryId).Id;
			var noOtherTmplAssignedToRootAlbum = !UiTemplates.Any(t => t.TemplateType == CurrentUiTemplate.TemplateType && t.UiTemplateId != CurrentUiTemplate.UiTemplateId && t.RootAlbumIds.Contains(rootAlbumId));

			if (noOtherTmplAssignedToRootAlbum)
			{
				invalidReason = Resources.GalleryServerPro.Admin_Templates_Cannot_Delete_No_Tmpl_Msg;
				return false;
			}

			// All the tests pass, so return true.
			invalidReason = String.Empty;
			return true;
		}

		#endregion
	}
}