using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;
using GalleryServerPro.WebControls;

namespace GalleryServerPro.Web.Pages.Admin
{
  /// <summary>
  /// A page-like user control for administering user settings.
  /// </summary>
  public partial class usersettings : Pages.AdminPage
  {
    private List<String> _defaultRolesForNewUsers;
    private List<String> _usersToNotifyForNewAccounts;

    #region Properties

    /// <summary>
    /// Gets a list of roles to assign when a user registers a new account. The value is a collection that is parsed from the 
    /// comma-delimited string stored in the DefaultRolesForSelfRegisteredUser configuration setting. During postbacks the 
    /// value is retrieved from the hidden form field that is maintained by the jQuery MultiSelect plugin.
    /// DotNetNuke only: The names include the _{GalleryId} suffix.
    /// </summary>
    /// <value>The default roles for self registered users.</value>
    private List<String> DefaultRolesForSelfRegisteredUserCollection
    {
      get
      {
        if (_defaultRolesForNewUsers == null)
        {
          _defaultRolesForNewUsers = new List<string>();

          var roleNames = IsPostBack ? GetDefaultRolesForSelfRegisteredUsers() : GallerySettingsUpdateable.DefaultRolesForSelfRegisteredUser;

          foreach (var roleName in roleNames)
          {
            _defaultRolesForNewUsers.Add(roleName.Trim());
          }
        }

        return _defaultRolesForNewUsers;
      }
    }

    /// <summary>
    /// Gets or sets a comma-delimited list of valid HTML tags. The text property of the
    /// txtAllowedHtmlTags TextBox binds to this property rather than <see cref="IGallerySettings.AllowedHtmlTags" />
    /// because it needs a string and the other property is a string array. This property essentially acts as a type 
    /// converter to the "real" property in <see cref="IGallerySettings" />.
    /// </summary>
    protected string AllowedHtmlTags
    {
      get
      {
        return String.Join(", ", GallerySettingsUpdateable.AllowedHtmlTags);
      }
      set
      {
        string[] allowedTags = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        // Trim any leading and trailing spaces
        for (int i = 0; i < allowedTags.Length; i++)
        {
          allowedTags[i] = allowedTags[i].Trim();
        }

        GallerySettingsUpdateable.AllowedHtmlTags = allowedTags;
      }
    }

    /// <summary>
    /// Gets or sets a comma-delimited list of valid HTML attributes. The text property of the
    /// txtAllowedHtmlAttributes TextBox binds to this property rather than <see cref="IGallerySettings.AllowedHtmlAttributes" />
    /// because it needs a string and the other property is a string array. This property essentially acts as a type 
    /// converter to the "real" property in <see cref="IGallerySettings" />.
    /// </summary>
    protected string AllowedHtmlAttributes
    {
      get
      {
        return String.Join(", ", GallerySettingsUpdateable.AllowedHtmlAttributes);
      }
      set
      {
        string[] allowedAtts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        // Trim any leading and trailing spaces
        for (int i = 0; i < allowedAtts.Length; i++)
        {
          allowedAtts[i] = allowedAtts[i].Trim();
        }


        GallerySettingsUpdateable.AllowedHtmlAttributes = allowedAtts;
      }
    }

    /// <summary>
    /// Gets the list user names notify when an account is created. The value is a collection that is 
    /// parsed from the comma-delimited string stored in the UsersToNotifyWhenAccountIsCreated configuration setting.
    /// During postbacks the value is retrieved from the combobox.
    /// </summary>
    /// <value>The list of user names of accounts to notify when an account is created.</value>
    private List<String> UsersToNotifyForNewAccounts
    {
      get
      {
        if (this._usersToNotifyForNewAccounts == null)
        {
          this._usersToNotifyForNewAccounts = new List<string>();

          if (IsPostBack)
          {
            foreach (string userName in GetUsersToNotifyWhenAccountIsCreated())
            {
              this._usersToNotifyForNewAccounts.Add(userName.Trim());
            }
          }
          else
          {
            foreach (var user in GallerySettingsUpdateable.UsersToNotifyWhenAccountIsCreated)
            {
              this._usersToNotifyForNewAccounts.Add(user.UserName);
            }
          }
        }

        return this._usersToNotifyForNewAccounts;
      }
    }

    /// <summary>
    /// Gets or sets the name of the album containing the user albums. This property is assigned during 
    /// <see cref="ConfigureUserAlbumParentComboBox" />, so be sure that function runs before
    /// accessing this value.
    /// </summary>
    /// <value>The name of the album containing the user albums.</value>
    protected string UserAlbumTitle
    {
      get;
      private set;
    }

    protected string ShowAlbumOwnerRolesLabel
    {
      get
      {
        return Resources.GalleryServerPro.Admin_User_Settings_Show_Album_Owner_Roles_Lbl;
      }
    }

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

    /// <summary>
    /// Handles the OnValidateControl event of the wwDataBinder control.
    /// </summary>
    /// <param name="item">The wwDataBindingItem item.</param>
    /// <returns>Returns <c>true</c> if the item is valid; otherwise returns <c>false</c>.</returns>
    protected bool wwDataBinder_ValidateControl(WebControls.wwDataBindingItem item)
    {
      if (!ValidateUserCanEnableSelfRegistration(item))
        return false;

      if (!ValidateUserAlbums(item))
        return false;

      return true;
    }

    /// <summary>
    /// Handles the OnBeforeUnBindControl event of the wwDataBinder control.
    /// </summary>
    /// <param name="item">The wwDataBindingItem item.</param>
    protected bool wwDataBinder_BeforeUnbindControl(WebControls.wwDataBindingItem item)
    {
      if (!BeforeUnbind_ProcessEnableSelfRegistrationControls(item))
        return false;

      if (!BeforeUnbind_ProcessEnableUserAlbumsControls(item))
        return false;

      if (!BeforeUnbind_ProcessUserAccountControls(item))
        return false;

      return true;
    }

    /// <summary>
    /// Handles the Click event of the btnEnableUserAlbums control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnEnableUserAlbums_Click(object sender, EventArgs e)
    {
      TurnOnUserAlbumsForAllUsers();

      ClientMessage = new ClientMessageOptions
      {
        Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
        Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
        Style = MessageStyle.Success
      };
    }

    /// <summary>
    /// Handles the Click event of the btnDisableUserAlbums control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void btnDisableUserAlbums_Click(object sender, EventArgs e)
    {
      TurnOffUserAlbumsForAllUsers();

      ClientMessage = new ClientMessageOptions
      {
        Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
        Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
        Style = MessageStyle.Success
      };
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Gets an URL to the specified album.
    /// </summary>
    /// <param name="albumId">The album ID.</param>
    /// <returns>Returns an URL to the specified album.</returns>
    protected static string GetAlbumUrl(int albumId)
    {
      return Utils.AddQueryStringParameter(Utils.GetCurrentPageUrl(), String.Concat("aid=", albumId));
    }

    #endregion

    #region Private Methods

    private void TurnOnUserAlbumsForAllUsers()
    {
      UpdateUserAlbumProfileSetting(true);
    }

    private void TurnOffUserAlbumsForAllUsers()
    {
      UpdateUserAlbumProfileSetting(false);
    }

    private void UpdateUserAlbumProfileSetting(bool enableUserAlbum)
    {
      HelperFunctions.BeginTransaction();

      try
      {
        foreach (IUserAccount user in UserController.GetAllUsers())
        {
          IUserProfile profile = ProfileController.GetProfile(user.UserName);

          profile.GetGalleryProfile(GalleryId).EnableUserAlbum = enableUserAlbum;

          ProfileController.SaveProfile(profile);
        }
        HelperFunctions.CommitTransaction();
        HelperFunctions.PurgeCache();
      }
      catch
      {
        HelperFunctions.RollbackTransaction();
        throw;
      }
    }

    private bool BeforeUnbind_ProcessUserAccountControls(wwDataBindingItem item)
    {
      // When allow HTML is unchecked, several child items are disabled via javascript. Disabled HTML items are not
      // posted during a postback, so we don't have accurate information about their states. For these controls don't save
      // anything by returning false. Furthermore, to prevent these child controls from incorrectly reverting to an
      // empty or unchecked state in the UI, assign their properties to their config setting. 

      // Step 1: Handle the "allow HTML" checkbox
      if (!chkAllowHtml.Checked)
      {
        if (item.ControlId == txtAllowedHtmlTags.ID)
        {
          txtAllowedHtmlTags.Text = AllowedHtmlTags;
          return false;
        }

        if (item.ControlId == txtAllowedHtmlAttributes.ID)
        {
          txtAllowedHtmlAttributes.Text = AllowedHtmlAttributes;
          return false;
        }
      }
      else
      {
        // User may have hit Return while editing one of the textboxes. Remove any return characters to be safe.
        if (item.ControlId == txtAllowedHtmlTags.ID)
        {
          txtAllowedHtmlTags.Text = txtAllowedHtmlTags.Text.Replace("\r\n", String.Empty);
        }

        if (item.ControlId == txtAllowedHtmlAttributes.ID)
        {
          txtAllowedHtmlAttributes.Text = txtAllowedHtmlAttributes.Text.Replace("\r\n", String.Empty);
        }
      }

      // Step 2: Handle the "allow user account management" checkbox
      if (!this.chkAllowManageAccount.Checked)
      {
        if (item.ControlId == this.chkAllowDeleteOwnAccount.ID)
        {
          this.chkAllowDeleteOwnAccount.Checked = GallerySettingsUpdateable.AllowDeleteOwnAccount;
          return false;
        }
      }

      return true;
    }

    private bool BeforeUnbind_ProcessEnableSelfRegistrationControls(wwDataBindingItem item)
    {
      if (!this.chkEnableSelfRegistration.Checked)
      {
        // When self registration is unchecked, several child items are disabled via javascript. Disabled HTML items are not
        // posted during a postback, so we don't have accurate information about their states. For these controls don't save
        // anything by returning false. Furthermore, to prevent these child controls from incorrectly reverting to an
        // empty or unchecked state in the UI, assign their properties to their config setting. 
        if (item.ControlId == this.chkRequireEmailValidation.ID)
        {
          this.chkRequireEmailValidation.Checked = GallerySettings.RequireEmailValidationForSelfRegisteredUser;
          return false;
        }

        if (item.ControlId == this.chkRequireAdminApproval.ID)
        {
          this.chkRequireAdminApproval.Checked = GallerySettings.RequireApprovalForSelfRegisteredUser;
          return false;
        }

        if (item.ControlId == this.chkUseEmailForAccountName.ID)
        {
          this.chkUseEmailForAccountName.Checked = GallerySettings.UseEmailForAccountName;
          return false;
        }
      }

      return true;
    }

    private bool BeforeUnbind_ProcessEnableUserAlbumsControls(wwDataBindingItem item)
    {
      if (!this.chkEnableUserAlbums.Checked)
      {
        // When user albums is unchecked, several child items are disabled via javascript. Disabled HTML items are not
        // posted during a postback, so we don't have accurate information about their states. For these controls don't save
        // anything by returning false. Furthermore, to prevent these child controls from incorrectly reverting to an
        // empty or unchecked state in the UI, assign their properties to their config setting. 
        if (item.ControlId == this.chkRedirectAfterLogin.ID)
        {
          this.chkRedirectAfterLogin.Checked = GallerySettings.RedirectToUserAlbumAfterLogin;
          return false;
        }

        if (item.ControlId == this.txtAlbumNameTemplate.ID)
        {
          this.txtAlbumNameTemplate.Text = GallerySettings.UserAlbumNameTemplate;
          return false;
        }

        if (item.ControlId == this.txtAlbumSummaryTemplate.ID)
        {
          this.txtAlbumSummaryTemplate.Text = GallerySettings.UserAlbumSummaryTemplate;
          return false;
        }
      }

      return true;
    }

    private bool ValidateUserCanEnableSelfRegistration(wwDataBindingItem item)
    {
      if ((item.ControlInstance == this.chkEnableSelfRegistration))
      {
        if (!UserCanEditUsersAndRoles)
        {
          item.BindingErrorMessage = Resources.GalleryServerPro.Admin_User_Settings_Cannot_Enable_Self_Registration_Msg;
          return false;
        }
      }

      return true;
    }

    private bool ValidateUserAlbums(wwDataBindingItem item)
    {
      if ((item.ControlInstance == this.chkEnableUserAlbums) && (chkEnableUserAlbums.Checked))
      {
        // User albums are selected. Make sure this isn't a read-only gallery.
        if (GallerySettings.MediaObjectPathIsReadOnly)
        {
          item.BindingErrorMessage = Resources.GalleryServerPro.Admin_User_Settings_Cannot_Enable_User_Albums_In_Read_Only_Gallery;
          return false;
        }

        // Make sure an album has been chosen to serve as the container for the user albums.
        if (tvUC.SelectedAlbum == null)
        {
          item.BindingErrorMessage = Resources.GalleryServerPro.Admin_User_Settings_Invalid_UserAlbumParent_Msg;
          return false;
        }
      }

      return true;
    }

    private void ConfigureControlsEveryTime()
    {
      this.PageTitle = Resources.GalleryServerPro.Admin_User_Settings_Page_Header;
      lblGalleryDescription.Text = String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_Gallery_Description_Label, Utils.GetCurrentPageUrl(), Utils.HtmlEncode(Factory.LoadGallery(GalleryId).Description));

      ConfigureSelfRegistrationSection();

      ConfigureComboBoxes();
    }

    private void ConfigureControlsFirstTime()
    {
      AdminPageTitle = Resources.GalleryServerPro.Admin_User_Settings_Page_Header;

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

      hdnUserRoles.Value = DefaultRolesForSelfRegisteredUserCollection.ToJson();
      hdnUsersToNotify.Value = UsersToNotifyForNewAccounts.ToJson();

      ConfigureOrphanUserAlbums();

      ConfigureUserAlbumParentComboBox();
    }

    private void ConfigureSelfRegistrationSection()
    {
      // If the current user can't edit users/roles, disable the checkbox that allows self registration to be enabled,
      // since we can't allow anything that creates a user.
      chkEnableSelfRegistration.Enabled = UserCanEditUsersAndRoles;
    }

    private void ConfigureOrphanUserAlbums()
    {
      // Check for user albums in the user album container that do not belong to a user. If we find some, display a message and give the admin the 
      // opportunity to delete them. Orphaned user albums might occur if an administrator has deleted a user outside of GSP.

      if (!GallerySettingsUpdateable.EnableUserAlbum)
      {
        pnlOrphanUserAlbums.Visible = false;
        return;
      }

      List<IAlbum> orphanUserAlbums = GetOrphanUserAlbums();

      if (orphanUserAlbums.Count > 0)
      {
        string userAlbumParentTitle = Utils.RemoveHtmlTags(AlbumController.LoadAlbumInstance(GallerySettingsUpdateable.UserAlbumParentAlbumId, false).Title);

        if (orphanUserAlbums.Count > 1)
          lblOrphanUserAlbumsMsg.Text = String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Admin_User_Settings_Orphan_User_Albums_Many_Lbl, orphanUserAlbums.Count, userAlbumParentTitle);
        else
          lblOrphanUserAlbumsMsg.Text = String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Admin_User_Settings_Orphan_User_Albums_One_Lbl, orphanUserAlbums.Count, userAlbumParentTitle);

        rptrOrphanUserAlbums.DataSource = orphanUserAlbums;
        rptrOrphanUserAlbums.DataBind();

        pnlOrphanUserAlbums.Visible = true;
      }
      else
      {
        pnlOrphanUserAlbums.Visible = false;
      }
    }

    private List<IAlbum> GetOrphanUserAlbums()
    {
      // Get a list of all the albums in the user album container that do not belong to a user.
      List<int> userAlbumIds = new List<int>();
      List<IAlbum> orphanUserAlbums = new List<IAlbum>();

      // Step 1: Get list of user album ID's.
      foreach (UserAccount user in UserController.GetAllUsers())
      {
        int userAlbumId = UserController.GetUserAlbumId(user.UserName, GalleryId);
        if (userAlbumId > int.MinValue)
        {
          userAlbumIds.Add(userAlbumId);
        }
      }

      // Step 2: Loop through each album in the user album container and see if the album is in our list of user album IDs. If not, add
      // to our list of orpan user albums.
      int albumId = GallerySettingsUpdateable.UserAlbumParentAlbumId;
      IAlbum userAlbumParent = null;

      if (albumId > 0)
      {
        try
        {
          userAlbumParent = AlbumController.LoadAlbumInstance(albumId, false);

          foreach (IAlbum album in userAlbumParent.GetChildGalleryObjects(GalleryObjectType.Album))
          {
            if (!userAlbumIds.Contains(album.Id))
            {
              orphanUserAlbums.Add(album);
            }
          }
        }
        catch (InvalidAlbumException) { }
      }

      return orphanUserAlbums;
    }

    private void ConfigureComboBoxes()
    {
      ConfigureUsersToNotifyComboBox();

      BindDefaultRolesComboBox();
    }

    private void ConfigureUsersToNotifyComboBox()
    {
      // Add the users to the list, pre-selecting any that are specified in the setting
      var userListItems = new List<ListItem>();

      foreach (var userName in UsersWithAdminPermission)
      {
        userListItems.Add(new ListItem(userName, userName));

        if (this.UsersToNotifyForNewAccounts.Contains(userName))
        {
          userListItems[userListItems.Count - 1].Selected = true;
        }
      }

      lbUsersToNotify.Items.Clear();
      lbUsersToNotify.Items.AddRange(userListItems.ToArray());
    }

    private void BindDefaultRolesComboBox()
    {
      // Add the roles to the list, pre-selecting any that are specified in the config file
      var roleListItems = new List<ListItem>();
      foreach (var role in GetRolesCurrentUserCanView())
      {
        roleListItems.Add(new ListItem(RoleController.ParseRoleNameFromGspRoleName(role.RoleName), role.RoleName)); // Don't need to HTML encode

        if (this.DefaultRolesForSelfRegisteredUserCollection.Contains(role.RoleName))
        {
          roleListItems[roleListItems.Count - 1].Selected = true;
        }

        if (RoleController.IsRoleAnAlbumOwnerRole(role.RoleName) || RoleController.IsRoleAnAlbumOwnerTemplateRole(role.RoleName))
        {
          roleListItems[roleListItems.Count - 1].Attributes["class"] = "gsp_j_albumownerrole";
        }

      }
      lbUserRoles.Items.Clear();
      lbUserRoles.Items.AddRange(roleListItems.ToArray());
    }

    private void ConfigureUserAlbumParentComboBox()
    {
      // Configure the album treeview ComboBox.
      this.tvUC.RequiredSecurityPermissions = SecurityActions.AdministerSite | SecurityActions.AdministerGallery;

      int albumId = GallerySettingsUpdateable.UserAlbumParentAlbumId;
      if (albumId > 0)
      {
        try
        {
          IAlbum albumToSelect = AlbumController.LoadAlbumInstance(albumId, false);
          UserAlbumTitle = albumToSelect.Title;
          tvUC.SelectedAlbumIds.Clear();
          tvUC.SelectedAlbumIds.Add(albumToSelect.Id);
        }
        catch (InvalidAlbumException)
        {
          UserAlbumTitle = Resources.GalleryServerPro.Admin_User_Settings_User_Album_Parent_Is_Invalid_Text;
        }
      }
      else
      {
        UserAlbumTitle = Resources.GalleryServerPro.Admin_User_Settings_User_Album_Parent_Not_Assigned_Text;
      }
    }

    private void SaveSettings()
    {
      // Step 1: Update config manually with those items that are not managed via the wwDataBinder
      UnbindUserAlbumId();

      UnbindDefaultRoles();

      UnbindUsersToNotify();

      // Step 2: Save
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
      ConfigureOrphanUserAlbums();

      ClientMessage = new ClientMessageOptions
                        {
                          Title = Resources.GalleryServerPro.Admin_Save_Success_Hdr,
                          Message = Resources.GalleryServerPro.Admin_Save_Success_Text,
                          Style = MessageStyle.Success
                        };
    }

    private void UnbindUserAlbumId()
    {
      if (tvUC.SelectedAlbum != null)
      {
        GallerySettingsUpdateable.UserAlbumParentAlbumId = tvUC.SelectedAlbum.Id;
      }
    }

    /// <summary>
    /// Gets the list of roles to assign to self-registered users, validate it, and assign it to the writeable version of
    /// <see cref="IGallerySettings.DefaultRolesForSelfRegisteredUser" />, where later it will be persisted to the data store.
    /// The values are retrieved from a hidden field that is maintained by the jQuery MultiSelect widget.
    /// </summary>
    private void UnbindDefaultRoles()
    {
      var roleNames = GetDefaultRolesForSelfRegisteredUsers();

      if (!ValidateRoles(roleNames))
        return;

      GallerySettingsUpdateable.DefaultRolesForSelfRegisteredUser = roleNames;
    }

    /// <summary>
    /// Gets the list user names notify when an account is created, validate it, and assign it to the writeable version of
    /// <see cref="IGallerySettings.UsersToNotifyWhenAccountIsCreated" />, where later it will be persisted to the data store.
    /// The values are retrieved from a hidden field that is maintained by the jQuery MultiSelect widget.
    /// </summary>
    private void UnbindUsersToNotify()
    {
      GallerySettingsUpdateable.UsersToNotifyWhenAccountIsCreated.Clear();

      foreach (var userName in GetUsersToNotifyWhenAccountIsCreated())
      {
        if (!UsersWithAdminPermission.Contains(userName.Trim()))
          continue;

        var user = UserController.GetUser(userName.Trim(), false);

        if (user != null)
        {
          if (!HelperFunctions.IsValidEmail(user.Email))
          {
            wwDataBinder.AddBindingError(String.Format(CultureInfo.InvariantCulture, Resources.GalleryServerPro.Admin_General_Invalid_User_Email_Msg, userName.Trim()), lbUsersToNotify);
            return;
          }

          GallerySettingsUpdateable.UsersToNotifyWhenAccountIsCreated.Add(user);
        }
      }
    }

    private bool ValidateRoles(string[] roleNames)
    {
      string errorMsg;
      if (!VerifyUserHasPermissionToAddUserToDefaultRolesForSelfRegisteredUser(roleNames, out errorMsg))
      {
        wwDataBinder.AddBindingError(errorMsg, lblUserRoles);
        return false;
      }

      foreach (var roleName in roleNames)
      {
        if (!RoleController.RoleExists(Utils.HtmlDecode(roleName.Trim())))
        {
          wwDataBinder.AddBindingError(String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Admin_User_Settings_Invalid_Role_Name_Msg, roleName), lblUserRoles);
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Verifies the logged on user has permission to add users to the roles defined in <paramref name="roleNames" />.
    /// </summary>
    /// <param name="roleNames">The role names.</param>
    /// <param name="errorMessage">The error message. Populated only when validation fails (return value is <c>false</c>).</param>
    /// <returns>Returns true if logged on user has permission to add users to the roles defined in <paramref name="roleNames" />;
    /// otherwise false.</returns>
    private bool VerifyUserHasPermissionToAddUserToDefaultRolesForSelfRegisteredUser(string[] roleNames, out string errorMessage)
    {
      IUserAccount sampleNewUser = new UserAccount(Guid.NewGuid().ToString());
      try
      {
        UserController.ValidateLoggedOnUserHasPermissionToSaveUser(sampleNewUser, roleNames);
      }
      catch (GallerySecurityException ex)
      {
        errorMessage = ex.Message;
        return false;
      }

      errorMessage = null;
      return true;
    }

    /// <summary>
    /// Gets the list of roles self registered users are to be assigned to. This is created from the hidden form field 
    /// that is maintained by the jQuery MultiSelect plugin.
    /// </summary>
    /// <returns>An array of strings.</returns>
    private string[] GetDefaultRolesForSelfRegisteredUsers()
    {
      if (String.IsNullOrWhiteSpace(hdnUserRoles.Value))
        return new string[] { };

      try
      {
        return hdnUserRoles.Value.FromJson<string[]>() ?? new string[] { };
      }
      catch (InvalidCastException ex)
      {
        AppEventController.LogError(ex, GalleryId);
        return new string[] { };
      }
    }

    /// <summary>
    /// Gets the list of user names notify when an account is created. This is created from the hidden form field 
    /// that is maintained by the jQuery MultiSelect plugin.
    /// </summary>
    /// <returns>An array of strings.</returns>
    private IEnumerable<string> GetUsersToNotifyWhenAccountIsCreated()
    {
      if (String.IsNullOrWhiteSpace(hdnUsersToNotify.Value))
        return new string[] { };

      try
      {
        return hdnUsersToNotify.Value.FromJson<string[]>() ?? new string[] { };
      }
      catch (InvalidCastException ex)
      {
        AppEventController.LogError(ex, GalleryId);
        return new string[] { };
      }
    }

    #endregion
  }
}