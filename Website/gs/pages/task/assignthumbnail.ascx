<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="assignthumbnail.ascx.cs"
  Inherits="GalleryServerPro.Web.Pages.Task.assignthumbnail" %>
<%@ Import Namespace="GalleryServerPro.Business.Interfaces" %>
<div runat="server">
  <script>
    (function ($) {

      $(document).ready(function () {
        $("#<%= cid %> .thmb").equalSize(); // Make all thumbnail tags the same width & height
        $('#<%= cid %> .gsp_go_t').css('width', '').css('display', ''); // Remove the width that was initially set, allowing title to take the full width of thumbnail

        // Clear all radio buttons when one is clicked and re-select the one that was clicked. This is a workaround
        // for a bug (see KB 316495 at microsoft.com).
        $(".<%= cid %>_caption input[type=radio]").click(function () {
          $(".<%= cid %>_caption input[type=radio]").prop("checked", false);
          $(this).prop("checked", true);
        });
      });

    })(jQuery);

  </script>
</div>
<div class="gsp_content">
  <asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <asp:Repeater ID="rptr" runat="server">
    <HeaderTemplate>
      <ul class="gsp_floatcontainer">
    </HeaderTemplate>
    <ItemTemplate>
      <li class="thmb">
        <%# GetThumbnailHtml((IGalleryObject) Container.DataItem) %>
        <p class="<%# cid %>_caption">
          <asp:RadioButton ID="rb" runat="server" GroupName="thmb" Text='<%# GetTitle(Eval("Title").ToString()) %>'
            Checked='<%# IsAlbumThumbnail(Convert.ToInt32(Eval("ID"))) %>' CssClass="gsp_go_t" Width="<%# GetTitleWidth((IGalleryObject) Container.DataItem) %>" />
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
