<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="mediaobjecttypes.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Admin.mediaobjecttypes" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<%@ Register Assembly="GalleryServerPro.Web" Namespace="GalleryServerPro.WebControls" TagPrefix="tis" %>

<div class="gsp_indentedContent">
	<p class="gsp_a_ap_to">
		<asp:Label ID="lbl1" runat="server" CssClass="gsp_bold" Text="<%$ Resources:GalleryServerPro, Admin_Settings_Apply_To_Label %>"
			EnableViewState="false" />&nbsp;<asp:Label ID="lblGalleryDescription" runat="server" EnableViewState="false" />
	</p>
	<asp:PlaceHolder ID="phAdminHeader" runat="server" />
	<div class="gsp_addleftpadding5">
		<div id="gsp_mimeTypesCtr">
		</div>
		<p class="gsp_addtopmargin10">
			<asp:CheckBox ID="chkAllowAll" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_MimeTypes_AllowAll_Label %>" />
		</p>
	</div>
	<asp:HiddenField ID="hdnMimeTypes" runat="server" ClientIDMode="Static" />

	<tis:wwDataBinder ID="wwDataBinder" runat="server">
		<DataBindingItems>
			<tis:wwDataBindingItem ID="wbi1" runat="server" BindingSource="GallerySettingsUpdateable" BindingSourceMember="AllowUnspecifiedMimeTypes"
				ControlId="chkAllowAll" BindingProperty="Checked" />
		</DataBindingItems>
	</tis:wwDataBinder>
	<asp:PlaceHolder ID="phAdminFooter" runat="server" />
</div>
<script id='tmplMediaTypes' type='text/x-jsrender'>
	<table class="gsp_mimeTypesTable">
		<thead>
			<tr>
				<th class="gsp_mt_col1"><input type="checkbox" class="gsp_mt_col1_hdr_chk" /></th>
				<th class="gsp_mt_col2">File Extension</th>
				<th class="gsp_mt_col3">MIME Type</th>
			</tr>
		</thead>
		<tbody>
			{{for MimeTypes}}
			<tr>
				<td class="gsp_mt_col1"><input type="checkbox" {{if Enabled}}checked{{/if}} data-mt="{{:Extension}}" class="gsp_mt_col1_chk" /></td>
				<td class="gsp_mt_col2">{{:Extension}}</td>
				<td class="gsp_mt_col3">{{:FullType}}</td>
			</tr>
			{{/for}}
		</tbody>
	</table>
</script>
<asp:PlaceHolder runat="server">
	<script>
		(function ($) {
			$(document).ready(function() {
			  bindMimeTypes();
			  configTooltips();
			});

			var bindMimeTypes = function () {
				var mimeTypes = { MimeTypes: $.parseJSON($("#hdnMimeTypes").val()) };

				var tmplData = $("#tmplMediaTypes").render(mimeTypes); // Generate the HTML from the template
			  $("#gsp_mimeTypesCtr").html(tmplData) // Add the HTML to the page
			    .find('.gsp_mt_col1_chk').click(function () {
			      // Update the matching MIME type, then stringify to hidden field
			      var ext = $(this).data('mt');
			      var mimeType = $.grep(mimeTypes.MimeTypes, function (mt) { return mt.Extension === ext; })[0];
			      mimeType.Enabled = $(this).prop('checked');
			      $("#hdnMimeTypes").val(JSON.stringify(mimeTypes.MimeTypes));
			    }).end().find('.gsp_mt_col1_hdr_chk').click(function () {
			      // Update all checkboxes to match, then stringify to hidden field
			      var isChecked = $(this).prop('checked');
			      $(this).closest('table.gsp_mimeTypesTable').find('.gsp_mt_col1_chk').prop('checked', isChecked);

			      $.each(mimeTypes.MimeTypes, function (idx, item) {
			        item.Enabled = isChecked;
			      });
			      $("#hdnMimeTypes").val(JSON.stringify(mimeTypes.MimeTypes));
			    });
			};
		  
		  var configTooltips = function() {
		    $('.gsp_admin_h2_txt', '#<%= cid %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Admin_MimeTypes_Overview_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Admin_MimeTypes_Overview_Bdy.JsEncode() %>'
		    });

		    $('#<%= chkAllowAll.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Cfg_allowUnspecifiedMimeTypes_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Cfg_allowUnspecifiedMimeTypes_Bdy.JsEncode() %>'
		    });
		  };
		})(jQuery);
	</script>
</asp:PlaceHolder>
