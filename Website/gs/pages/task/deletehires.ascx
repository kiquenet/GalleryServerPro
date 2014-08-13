<%@ Import Namespace="GalleryServerPro.Business" %>
<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="deletehires.ascx.cs"
	Inherits="GalleryServerPro.Web.Pages.Task.deletehires" %>
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
	<div class="gsp_addleftpadding1">
		<p class="gsp_textcol gsp_msgwarning" style="border-bottom: #369 1px solid;">
			<asp:Literal ID="litId" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_HiRes_Warning %>" /></p>
		<p class="gsp_h3" runat="server">
			<% =GetPotentialSavings() %>
		</p>
		<p class="gsp_textcol">
			<asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_HiRes_Potential_Savings_Hdr %>" />
		</p>
	</div>
	<asp:PlaceHolder ID="phMsg" runat="server" />
  <p class="gsp_addleftmargin2">
    <a class="chkCheckUncheckAll" href='#' data-ischecked="false">
      <asp:Literal ID="l2" runat="server" Text="<%$ Resources:GalleryServerPro, Site_ToggleCheckAll_Lbl %>" />
    </a>
  </p>
	<asp:Repeater ID="rptr" runat="server">
		<HeaderTemplate>
			<ul class="gsp_floatcontainer">
		</HeaderTemplate>
		<ItemTemplate>
			<li class="<%# GetThumbnailCssClass(Container.DataItem.GetType()) %>">
				<%# GetThumbnailHtml((IGalleryObject) Container.DataItem) %>
				<p class="gsp_go_t" style="width:<%# GetTitleWidth((IGalleryObject) Container.DataItem) %>;">
					<%# GetGalleryObjectText(Eval("Title").ToString(), Container.DataItem.GetType()) %></p>
				<p id="p2" runat="server" class="gsp_msgwarning" visible='<%# ShouldShowNoOriginalFileMsg((IGalleryObject) Container.DataItem) %>'>
					<asp:Literal ID="l3" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_Original_Not_Available_Text%>" /></p>
				<p id="p3" runat="server" class="gsp_em" visible='<%# ShouldShowCheckbox((IGalleryObject) Container.DataItem) %>'>
					<asp:CheckBox ID="chkbx" runat="server" Text='<%# GetSavings((IGalleryObject)Container.DataItem) %>' />
					<asp:HiddenField ID="hdn" runat="server" Value="<%# GetId((IGalleryObject) Container.DataItem) %>" />
				</p>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
		</FooterTemplate>
	</asp:Repeater>
	<asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>
