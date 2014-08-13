<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="css.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Admin.css" %>
<%@ Import Namespace="GalleryServerPro.Web" %>

<div class="gsp_indentedContent" runat="server">
  <p class="gsp_a_ap_to">
    <asp:Label ID="lbl1" runat="server" CssClass="gsp_bold" Text="<%$ Resources:GalleryServerPro, Admin_Settings_Apply_To_Label %>"
      EnableViewState="false" />&nbsp;<asp:Literal runat="server" Text="<%$ Resources:GalleryServerPro, Admin_All_Galleries_Label %>" />
  </p>
  <asp:PlaceHolder ID="phAdminHeader" runat="server" />
  <div class="gsp_addleftpadding5">
    <div class="gsp_addtoppadding5">
      <p>
        Path to CSS file: <a href="<%= CssFileUrl %>" class="gsp_a_css_u_l"><%= CssFilePath %></a>
      </p>
      <p>
        <asp:TextBox ID="txtCss" runat="server" CssClass="gsp_a_css_txt" TextMode="MultiLine" Columns="20" Rows="20"></asp:TextBox>
      </p>
    </div>
    <asp:PlaceHolder ID="phAdminFooter" runat="server" />
  </div>
</div>
<asp:PlaceHolder runat="server">
  <script>
    (function ($) {
      $(document).ready(function () {
        configTooltips();
      });

      var configTooltips = function () {
        $('.gsp_admin_h2_txt', '#<%= cid %>').gspTooltip({
          title: '<%= Resources.GalleryServerPro.Admin_Css_Overview_Hdr.JsEncode() %>',
          content: '<%= Resources.GalleryServerPro.Admin_Css_Overview_Bdy.JsEncode() %>'
        });
      };
    })(jQuery);
  </script>
</asp:PlaceHolder>
