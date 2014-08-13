<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="deleteobjects.ascx.cs"
  Inherits="GalleryServerPro.Web.Pages.Task.deleteobjects" %>
<%@ Import Namespace="GalleryServerPro.Business.Interfaces" %>
<div runat="server">
  <script>
    (function ($) {
      jQuery(document).ready(function () {
        $("#<%= cid %> .thmb").equalSize(); // Make all thumbnail tags the same width & height
        $('#<%= cid %> .gsp_go_t').css('width', '').css('display', ''); // Remove the width that was initially set, allowing title to take the full width of thumbnail

        $("#<%= cid %> .chkCheckUncheckAll").on('click', function () {
          var checkAll = !$(this).data("isChecked"); // true when we want to check all; otherwise false
          
          $(this).data("isChecked", checkAll);
          
          $("#<%= cid %> .thmb input[type=checkbox]").prop("checked", checkAll);

          return false;
        });
      });
    })(jQuery);
  </script>
</div>
<div class="gsp_content">
  <asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <p class="gsp_textcol gsp_msgwarning gsp_addleftpadding1">
    <asp:Literal ID="litId" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_Objects_Warning %>" />
  </p>
  <p>
    <asp:CheckBox ID="chkDeleteDbRecordsOnly" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_Objects_DeleteDbRecordsOnly_Lbl %>" />
  </p>
  <p class="gsp_addleftmargin2">
    <a class="chkCheckUncheckAll" href='#' data-ischecked="false">
      <asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Site_ToggleCheckAll_Lbl %>" />
    </a>
  </p>
  <asp:Repeater ID="rptr" runat="server">
    <HeaderTemplate>
      <ul id="<%= cid %>_gsp_thmbcontainer" class="gsp_floatcontainer">
    </HeaderTemplate>
    <ItemTemplate>
      <li class="<%# GetThumbnailCssClass((IGalleryObject) Container.DataItem) %>">
        <%# GetThumbnailHtml((IGalleryObject) Container.DataItem) %>
        <p id="p" runat="server">
          <asp:CheckBox ID="chk" runat="server" Text='<%# GetGalleryObjectText(Eval("Title").ToString(), Container.DataItem.GetType()) %>'
            Enabled="<%# DoesUserHavePermissionToDeleteGalleryObject((IGalleryObject) Container.DataItem) %>" CssClass="gsp_go_t" Width="<%# GetTitleWidth((IGalleryObject) Container.DataItem) %>" />
        </p>
        <asp:HiddenField ID="hdn" runat="server" Value="<%# GetId((IGalleryObject) Container.DataItem) %>" />
      </li>
    </ItemTemplate>
    <FooterTemplate>
      </ul>
    </FooterTemplate>
  </asp:Repeater>
  <asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>
