<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="rotateimage.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Task.rotateimage" %>
<%@ Import Namespace="GalleryServerPro.Business.Interfaces" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<div runat="server">
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
</div>
<div class="gsp_content">
  <asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <asp:Repeater ID="rptr" runat="server">
    <ItemTemplate>
      <div data-id="<%# Eval("ID") %>" class="thmbRotate" style="float: none;">
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
                <img src="<%# GetMediaObjectUrl((IGalleryObject) Container.DataItem) %>" style="<%# GetWidthAndHeightStyle((IGalleryObject) Container.DataItem) %>" alt="<%# RemoveHtmlTags(Eval("Title").ToString()) %>" title="<%# RemoveHtmlTags(Eval("Title").ToString()) %>" class="gsp_rotate" />
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
  </asp:Repeater>
  <asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>
