<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="rotateimages.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Task.rotateimages" %>
<%@ Import Namespace="GalleryServerPro.Business.Interfaces" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<asp:PlaceHolder runat="server">
  <script>
    (function ($) {

      jQuery(document).ready(function () {
        $("#<%= cid %> .thmbRotate").equalSize(); // Make all thumbnail tags the same width & height

        configureEventHandlers();
      });

      var configureEventHandlers = function () {
        // When side of image is clicked, save choice in hidden field and update background image of old and new side markers
        $('a', $("#<%= cid %> .thmbRotate")).click(function () {
          // Assign selected side to hidden input
          var $parentThmb = $(this).closest('.thmbRotate');
          var $hdnInput = $parentThmb.find('input.hdnSelectedSide');
          var oldSide = $hdnInput.val();
          var newSide = $(this).data('side');

          $hdnInput.val(newSide);

          // Reset previously selected side to default image
          $parentThmb.find('a[data-side=' + oldSide + ']').css('background', 'transparent url(<%= Utils.SkinPath %>/images/rotate/' + oldSide + '.gif) no-repeat 0 0');

          // Mark newly selected side
          $(this).css('background', '#ccc url(<%= Utils.SkinPath %>/images/rotate/' + newSide + '1.gif) no-repeat 0 0');
        });
      };

    })(jQuery);

  </script>
</asp:PlaceHolder>
<div class="gsp_content">
  <asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <asp:Repeater ID="rptr" runat="server">
    <HeaderTemplate>
      <div class="gsp_floatcontainer">
    </HeaderTemplate>
    <ItemTemplate>
      <div data-id="<%# Eval("ID") %>" class="thmbRotate">
        <table>
          <tr>
            <td colspan="3">
              <a class="gsp_hor" href="#" data-side='top' style="background: #ccc url(<%= Utils.SkinPath %>/images/rotate/top1.gif) no-repeat 0 0" title="<%= Rotate0Text %>"></a>
            </td>
          </tr>
          <tr>
            <td>
              <a class="gsp_vert" href="#" data-side='left' style="background-image: url(<%= Utils.SkinPath %>/images/rotate/left.gif)" title="<%= Rotate90Text %>"></a>
            </td>
            <td>
              <div>
                <img src="<%# GetThumbnailUrl((IGalleryObject) Container.DataItem) %>" alt="<%# RemoveHtmlTags(Eval("Title").ToString()) %>" title="<%# RemoveHtmlTags(Eval("Title").ToString()) %>" class="gsp_rotate"
                  style="width: <%# ((IGalleryObject) Container.DataItem).Thumbnail.Width %>px; height: <%# ((IGalleryObject) Container.DataItem).Thumbnail.Height %>px;" />
              </div>
            </td>
            <td>
              <a class="gsp_vert" href="#" data-side='right' style="background-image: url(<%= Utils.SkinPath %>/images/rotate/right.gif)" title="<%= Rotate270Text %>"></a>
            </td>
          </tr>
          <tr>
            <td colspan="3">
              <a class="gsp_hor" href="#" data-side='bottom' style="background-image: url(<%= Utils.SkinPath %>/images/rotate/bottom.gif)" title="<%= Rotate180Text %>"></a>
            </td>
          </tr>
        </table>
        <input id="hdnSelectedSide" class="hdnSelectedSide" runat="server" type="hidden" value="top" />
      </div>
      <input id="moid" runat="server" type="hidden" value='<%# Eval("ID") %>' />
    </ItemTemplate>
    <FooterTemplate>
      </div>
    </FooterTemplate>
  </asp:Repeater>
  <asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>
