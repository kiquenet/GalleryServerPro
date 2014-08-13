<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="createalbum.ascx.cs"
  Inherits="GalleryServerPro.Web.Pages.Task.createalbum" %>
<%@ Register Src="../../controls/albumtreeview.ascx" TagName="albumtreeview" TagPrefix="uc1" %>
<div class="gsp_content">
  <asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <p class="gsp_addleftpadding1">
    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="<%$ Resources:GalleryServerPro, Task_Create_Album_Title_Label_Text %>" />
    <asp:TextBox ID="txtTitle" runat="server" CssClass="gsp_textbox" />
    <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="txtTitle"
      Display="Dynamic" ErrorMessage="<%$ Resources:GalleryServerPro, Task_Create_Album_Missing_Title_Text %>"
      ForeColor="" CssClass="gsp_msgfailure">
    </asp:RequiredFieldValidator>
  </p>
  <p class="gsp_addtopmargin5 gsp_addleftpadding1">
    <asp:Literal ID="litInstructions" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Create_Album_Instructions %>" />
  </p>
  <div class="gsp_addleftpadding1">
    <uc1:albumtreeview ID="tvUC" runat="server" AllowMultiCheck="false" RequireAlbumSelection="true" />
  </div>
  <asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>
