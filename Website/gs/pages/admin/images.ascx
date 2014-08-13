<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="images.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Admin.images" %>
<%@ Register Assembly="GalleryServerPro.Web" Namespace="GalleryServerPro.WebControls" TagPrefix="tis" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<div class="gsp_indentedContent">
	<p class="gsp_a_ap_to">
		<asp:Label ID="lbl1" runat="server" CssClass="gsp_bold" Text="<%$ Resources:GalleryServerPro, Admin_Settings_Apply_To_Label %>"
			EnableViewState="false" />&nbsp;<asp:Label ID="lblGalleryDescription" runat="server" EnableViewState="false" /></p>
	<asp:PlaceHolder ID="phAdminHeader" runat="server" />
	<div class="gsp_addleftpadding5">
    <div class="gsp_single_tab">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
				  <asp:Label ID="lblCompressedHdr" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_Compressed_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
			  <table class="gsp_standardTable">
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblOptTriggerSize" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OptTriggerSize_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtOptTriggerSize" runat="server" />&nbsp;<asp:RangeValidator ID="rvOptTriggerSize"
							  runat="server" Display="Dynamic" ControlToValidate="txtOptTriggerSize" Type="Integer"
							  MinimumValue="0" MaximumValue="2147483647" Text="<%$ Resources:GalleryServerPro, Validation_Positive_Int_Include_0_Text %>" />
					  </td>
					  <td>
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblOptMaxLength" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OptMaxLength_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtOptMaxLength" runat="server" />&nbsp;<asp:RangeValidator ID="rvOptMaxLength"
							  runat="server" Display="Dynamic" ControlToValidate="txtOptMaxLength" Type="Integer"
							  MinimumValue="10" MaximumValue="100000" Text="<%$ Resources:GalleryServerPro, Validation_Int_10_To_100000_Text %>" />
					  </td>
					  <td>
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblOptJpegQuality" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OptJpegQuality_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtOptJpegQuality" runat="server" />&nbsp;<asp:RangeValidator ID="rvOptJpegQuality"
							  runat="server" Display="Dynamic" ControlToValidate="txtOptJpegQuality" Type="Integer"
							  MinimumValue="1" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_1_To_100_Text %>" />
					  </td>
					  <td class="gsp_fs">
						  <asp:Literal ID="l5" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OptJpegQuality_Label2 %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblOptFileNamePrefix" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OptFileNamePrefix_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtOptFileNamePrefix" runat="server" />
					  </td>
					  <td>
						  <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="txtOptFileNamePrefix"
							  Display="Static" ErrorMessage="<%$ Resources:GalleryServerPro, Site_Field_Required_Text %>"
							  ForeColor="" CssClass="gsp_msgfailure">
						  </asp:RequiredFieldValidator>
					  </td>
				  </tr>
			  </table>
      </div>
    </div>

    <div class="gsp_single_tab">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
			    <asp:Literal ID="l6" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_Original_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
			  <asp:Label ID="lblOriginalJpegQuality" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_OriginalJpegQuality_Label %>" />&nbsp;
			  <asp:TextBox ID="txtOriginalJpegQuality" runat="server" />&nbsp;<asp:RangeValidator
				  ID="rvOriginalJpegQuality" runat="server" Display="Dynamic" ControlToValidate="txtOriginalJpegQuality"
				  Type="Integer" MinimumValue="1" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_1_To_100_Text %>" />&nbsp;
			  <asp:Label ID="lblOriginalJpegQuality2" runat="server" CssClass="gsp_fs" Text="<%$ Resources:GalleryServerPro, Admin_Images_OriginalJpegQuality_Label2 %>" />
			  <p>
				  <asp:CheckBox ID="chkDiscardOriginal" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_DiscardOriginal_Label %>" />
			  </p>
      </div>
    </div>

    <div class="gsp_single_tab">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
			    <asp:Literal ID="l8" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_Watermark_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
			  <p>
				  <asp:CheckBox ID="chkApplyWatermark" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_ApplyWatermark_Label %>" />
			  </p>
			  <p class="gsp_addleftpadding6">
				  <asp:CheckBox ID="chkApplyWmkToThumb" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_ApplyWmkToThumb_Label %>" />
			  </p>
			  <p class="gsp_bold gsp_addtopmargin5">
				  <asp:Literal ID="l9" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_Watermark_Text_Hdr %>" />
			  </p>
			  <table class="gsp_addleftpadding6 gsp_standardTable">
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkText" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkText_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkText" runat="server" class="gsp_textbox" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkFontName" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontName_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkFontName" runat="server" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkFontSize" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontSize_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkFontSize" runat="server" />&nbsp;<asp:RangeValidator ID="rvWmkFontSize"
							  runat="server" Display="Dynamic" ControlToValidate="txtWmkFontSize" Type="Integer"
							  MinimumValue="0" MaximumValue="10000" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_10000_Text %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkTextWidthPct" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextWidthPct_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkTextWidthPct" runat="server" />&nbsp;<asp:RangeValidator ID="rvWmkTextWidthPct"
							  runat="server" Display="Dynamic" ControlToValidate="txtWmkTextWidthPct" Type="Integer"
							  MinimumValue="0" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_100_Text %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkFontColor" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontColor_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkFontColor" runat="server" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkTextOpacity" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextOpacity_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkTextOpacity" runat="server" />&nbsp;<asp:RangeValidator ID="rvWmkTextOpacity"
							  runat="server" Display="Dynamic" ControlToValidate="txtWmkTextOpacity" Type="Integer"
							  MinimumValue="0" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_100_Text %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkTextLocation" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextLocation_Label %>" />
					  </td>
					  <td>
						  <asp:DropDownList ID="ddlWmkTextLocation" runat="server" />
					  </td>
				  </tr>
			  </table>
			  <p class="gsp_bold gsp_addtopmargin5">
				  <asp:Literal ID="l17" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_Watermark_Image_Hdr %>" />
			  </p>
			  <table class="gsp_addleftpadding6 gsp_standardTable">
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkImagePath" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkImagePath_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkImagePath" runat="server" CssClass="gsp_textbox" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkImageWidthPct" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageWidthPct_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkImageWidthPct" runat="server" />&nbsp;<asp:RangeValidator
							  ID="rvWmkImageWidthPct" runat="server" Display="Dynamic" ControlToValidate="txtWmkImageWidthPct"
							  Type="Integer" MinimumValue="0" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_100_Text %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkImageOpacity" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageOpacity_Label %>" />
					  </td>
					  <td>
						  <asp:TextBox ID="txtWmkImageOpacity" runat="server" />&nbsp;<asp:RangeValidator ID="rvWmkImageOpacity"
							  runat="server" Display="Dynamic" ControlToValidate="txtWmkImageOpacity" Type="Integer"
							  MinimumValue="0" MaximumValue="100" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_100_Text %>" />
					  </td>
				  </tr>
				  <tr>
					  <td class="gsp_col1">
						  <asp:Label ID="lblWmkImageLocation" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageLocation_Label %>" />
					  </td>
					  <td>
						  <asp:DropDownList ID="ddlWmkImageLocation" runat="server" />
					  </td>
				  </tr>
			  </table>
      </div>
    </div>
	</div>
	<tis:wwDataBinder ID="wwDataBinder" runat="server">
		<DataBindingItems>
			<tis:wwDataBindingItem ID="WwDataBindingItem2" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="OptimizedImageTriggerSizeKB" ControlId="txtOptTriggerSize"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_OptTriggerSize_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem3" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="MaxOptimizedLength" ControlId="txtOptMaxLength" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_OptMaxLength_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem4" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="OptimizedImageJpegQuality" ControlId="txtOptJpegQuality" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_OptJpegQuality_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem4b" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="OptimizedFileNamePrefix" ControlId="txtOptFileNamePrefix"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_OptFileNamePrefix_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem5" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="OriginalImageJpegQuality" ControlId="txtOriginalJpegQuality"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_OriginalJpegQuality_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem5b" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="DiscardOriginalImageDuringImport" ControlId="chkDiscardOriginal"
				BindingProperty="Checked" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_DiscardOriginal_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem6" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="ApplyWatermark" ControlId="chkApplyWatermark" BindingProperty="Checked"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_ApplyWatermark_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem7" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="ApplyWatermarkToThumbnails" ControlId="chkApplyWmkToThumb"
				BindingProperty="Checked" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_ApplyWmkToThumb_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem8" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkText" ControlId="txtWmkText" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkText_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem9" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextFontName" ControlId="txtWmkFontName" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontName_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem10" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextFontSize" ControlId="txtWmkFontSize" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontSize_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem11" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextWidthPercent" ControlId="txtWmkTextWidthPct"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextWidthPct_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem12" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextColor" ControlId="txtWmkFontColor" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkFontColor_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem13" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextOpacityPercent" ControlId="txtWmkTextOpacity"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextOpacity_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem14" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkTextLocation" ControlId="ddlWmkTextLocation" BindingProperty="SelectedValue"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkTextLocation_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem15" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkImagePath" ControlId="txtWmkImagePath" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkImagePath_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem16" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkImageWidthPercent" ControlId="txtWmkImageWidthPct"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageWidthPct_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem17" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="WatermarkImageOpacityPercent" ControlId="txtWmkImageOpacity"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageOpacity_Label %>" />
			<tis:wwDataBindingItem ID="WwDataBindingItem18" runat="server" BindingProperty="SelectedValue"
				BindingSource="GallerySettingsUpdateable" BindingSourceMember="WatermarkImageLocation"
				ControlId="ddlWmkImageLocation" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Images_WmkImageLocation_Label %>" />
		</DataBindingItems>
	</tis:wwDataBinder>
	<asp:PlaceHolder ID="phAdminFooter" runat="server" />
</div>
<asp:PlaceHolder runat="server">
	<script>
		(function ($) {
			$(document).ready(function () {
			  bindEventHandlers();
			  configUi();
			  configTooltips();
			});
		  
		  var bindEventHandlers = function() {
		    $("#<%= chkApplyWatermark.ClientID %>").click(updateUi);
		  };
		  
		  var configUi = function () {
			  var applyWatermarkSelected = $('#<%= chkApplyWatermark.ClientID %>').prop('checked');

        $('#<%= chkApplyWmkToThumb.ClientID %>').prop('disabled', !applyWatermarkSelected);
      };

		  var updateUi = function () {
		    var applyWatermarkSelected = $('#<%= chkApplyWatermark.ClientID %>').prop('checked');

		    if (!applyWatermarkSelected) {
		      $('#<%= chkApplyWmkToThumb.ClientID %>').prop('checked', applyWatermarkSelected);		      
		    }
		    
		    $('#<%= chkApplyWmkToThumb.ClientID %>').prop('disabled', !applyWatermarkSelected);
		  };

			var configTooltips = function () {
				$('#<%= lblOptTriggerSize.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_optimizedImageTriggerSizeKB_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_optimizedImageTriggerSizeKB_Bdy.JsEncode() %>'
				});

				$('#<%= lblOptMaxLength.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_maxOptimizedLength_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_maxOptimizedLength_Bdy.JsEncode() %>'
				});

				$('#<%= lblOptJpegQuality.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_optimizedImageJpegQuality_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_optimizedImageJpegQuality_Bdy.JsEncode() %>'
				});

				$('#<%= lblOptFileNamePrefix.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_optimizedFileNamePrefix_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_optimizedFileNamePrefix_Bdy.JsEncode() %>'
				});

				$('#<%= lblOriginalJpegQuality.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_originalImageJpegQuality_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_originalImageJpegQuality_Bdy.JsEncode() %>'
				});

				$('#<%= chkDiscardOriginal.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_discardOriginalImageDuringImport_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_discardOriginalImageDuringImport_Bdy.JsEncode() %>'
				});

				$('#<%= chkApplyWatermark.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_applyWatermark_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_applyWatermark_Bdy.JsEncode() %>'
				});

				$('#<%= chkApplyWmkToThumb.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_applyWatermarkToThumbnails_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_applyWatermarkToThumbnails_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkText.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkText_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkText_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkFontName.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextFontName_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextFontName_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkFontSize.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextFontSize_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextFontSize_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkTextWidthPct.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextWidthPercent_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextWidthPercent_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkFontColor.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextColor_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextColor_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkTextOpacity.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextOpacityPercent_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextOpacityPercent_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkTextLocation.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkTextLocation_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkTextLocation_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkImagePath.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkImagePath_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkImagePath_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkImageWidthPct.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkImageWidthPercent_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkImageWidthPercent_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkImageOpacity.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkImageOpacityPercent_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkImageOpacityPercent_Bdy.JsEncode() %>'
				});

				$('#<%= lblWmkImageLocation.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Cfg_watermarkImageLocation_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Cfg_watermarkImageLocation_Bdy.JsEncode() %>'
				});

				$('#<%= lblCompressedHdr.ClientID %>').gspTooltip({
					title: '<%= Resources.GalleryServerPro.Admin_Images_About_Compressed_Images_Hdr.JsEncode() %>',
					content: '<%= Resources.GalleryServerPro.Admin_Images_About_Compressed_Images_Bdy.JsEncode() %>'
				});
			};
		})(jQuery);
	</script>
</asp:PlaceHolder>