<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="deletealbum.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Task.deletealbum" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<div class="gsp_content">
	<asp:PlaceHolder ID="phTaskHeader" runat="server" />
	<p class="gsp_textcol gsp_msgwarning">
		<asp:Literal ID="litId" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_Album_Warning %>" /></p>
	<p>
		<asp:CheckBox ID="chkDeleteDbRecordsOnly" runat="server" Text="<%$ Resources:GalleryServerPro, Task_Delete_Album_DeleteDbRecordsOnly_Lbl %>" /></p>
	<asp:PlaceHolder ID="phTaskFooter" runat="server" />
</div>

<asp:PlaceHolder runat="server">
	<script>
	  (function ($) {
	    $(document).ready(function () {
	      configTooltips();
	    });

	    var configTooltips = function () {
	      $('#<%= chkDeleteDbRecordsOnly.ClientID %>').gspTooltip({
	        title: '<%= Resources.GalleryServerPro.Task_Delete_Album_DeleteDbRecordsOnly_Hdr.JsEncode() %>',
	        content: '<%= Resources.GalleryServerPro.Task_Delete_Album_DeleteDbRecordsOnly_Bdy.JsEncode() %>'
	      });
	    };
	  })(jQuery);
	</script>
</asp:PlaceHolder>