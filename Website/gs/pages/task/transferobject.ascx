<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="transferobject.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Task.transferobject" %>
<%@ Register Src="../../controls/albumtreeview.ascx" TagName="albumtreeview" TagPrefix="uc1" %>
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

<div class="gsp_content" runat="server">
	<asp:PlaceHolder ID="phTaskHeader" runat="server" />
  <p class="<%= CheckCheckAllCtrCssClass %>">
    <a class="chkCheckUncheckAll" href='#' data-ischecked="false">
      <asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Site_ToggleCheckAll_Lbl %>" />
    </a>
  </p>
	<asp:Repeater ID="rptr" runat="server">
		<HeaderTemplate>
			<ul class="gsp_floatcontainer">
		</HeaderTemplate>
		<ItemTemplate>
			<li class="<%# GetThumbnailCssClass(Container.DataItem.GetType()) %>">
				<%# GetThumbnailHtml((IGalleryObject) Container.DataItem) %>
				<p class="gsp_caption">
					<asp:CheckBox ID="chkbx" runat="server" Text='<%# GetTitle(Eval("Title").ToString()) %>' CssClass="gsp_go_t" Width="<%# GetTitleWidth((IGalleryObject) Container.DataItem) %>" />
					<input id="Hidden1" runat="server" type="hidden" value='<%# GetId((IGalleryObject) Container.DataItem) %>' />
				</p>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
		</FooterTemplate>
	</asp:Repeater>
	<asp:PlaceHolder ID="phCustomValidatorContainer" runat="server" Visible="false" />
	<uc1:albumtreeview ID="tvUC" runat="server" Visible="false" RequireAlbumSelection="true" />
	<asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>