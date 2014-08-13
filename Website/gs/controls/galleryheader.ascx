<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="galleryheader.ascx.cs"
  Inherits="GalleryServerPro.Web.Controls.galleryheader" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<header id='<%= GalleryPage.HeaderClientId %>' class='gsp_header'></header>
<asp:LoginView ID="lv" runat="server" EnableViewState="false">
  <AnonymousTemplate>
    <div id="<%= cid %>_loginDlg" class="gsp_dlg gsp_login_dlg">
      <img src="<%= Utils.SkinPath %>/images/user-l.png" alt="" style="float: left;" class="gsp_login_icon" />
      <asp:Login ID="Login1" runat="server" CssClass="gsp_login" RememberMeSet="false"
        DisplayRememberMe="True" Width="100%" Orientation="Vertical" LabelStyle-CssClass="gsp_login_label"
        TextBoxStyle-CssClass="gsp_login_textbox" TextLayout="TextOnLeft" TitleText=""
        LoginButtonType="Button" LoginButtonStyle-CssClass="gsp_login_button" EnableViewState="false"
        OnLoginError="Login1_LoginError" OnLoggedIn="Login1_LoggedIn" FailureText="<%$ Resources:GalleryServerPro, Login_Failure_Text %>"
        LoginButtonText="<%$ Resources:GalleryServerPro, Login_Button_Text %>" PasswordLabelText="<%$ Resources:GalleryServerPro, Login_Password_Label_Text %>"
        PasswordRequiredErrorMessage="" RememberMeText="<%$ Resources:GalleryServerPro, Login_Remember_Me_Text %>"
        UserNameLabelText="<%$ Resources:GalleryServerPro, Login_UserName_Label_Text %>"
        UserNameRequiredErrorMessage="" CheckBoxStyle-CssClass="gsp_rememberme" HyperLinkStyle-CssClass="gsp_login_hyperlinks"
        PasswordRecoveryText="<%$ Resources:GalleryServerPro, Login_Password_Recovery_Text %>">
      </asp:Login>
    </div>
  </AnonymousTemplate>
</asp:LoginView>

<div id="<%= cid %>_searchDlg" class="gsp_search_dlg gsp_dlg">
  <img src="<%= Utils.SkinPath %>/images/search-l.png" class="gsp_searchicon" alt="" />
  <p class="gsp_search_title">
    <asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Search_Label %>" />
  </p>
  <p class="gsp_searchbox_container">
    <input type="text" class="gsp_searchbox" placeholder="<asp:Literal ID="l2" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Instructions %>" />" /><span class="gsp_search_msg">*</span>
  </p>
  <p class="gsp_search_type_container">
    <input type="radio" id="<%= cid %>_sTypeTitle" name="<%= cid %>_searchType" value="title" checked/><label for="<%= cid %>_sTypeTitle"><asp:Literal ID="l5" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Type_Title %>" /></label>
    <input type="radio" id="<%= cid %>_sTypeTags" name="<%= cid %>_searchType" value="tag" /><label for="<%= cid %>_sTypeTags"><asp:Literal ID="l6" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Type_Tags %>" /></label>
    <input type="radio" id="<%= cid %>_sTypePeople" name="<%= cid %>_searchType" value="people"/><label for="<%= cid %>_sTypePeople"><asp:Literal ID="l7" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Type_People %>" /></label>
    <input type="radio" id="<%= cid %>_sTypeAll" name="<%= cid %>_searchType" value="search"/><label for="<%= cid %>_sTypeAll"><asp:Literal ID="l8" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Type_Everything %>" /></label>
  </p>
  <p class="gsp_search_cmd_container">
    <button class="gsp_searchbutton" title='<asp:Literal ID="l3" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Button_Tooltip %>" />'>
      <asp:Literal ID="l4" runat="server" Text="<%$ Resources:GalleryServerPro, Search_Button_Text %>" />
    </button>
  </p>
</div>
