<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="metadata.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Admin.metadata" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<%@ Register Assembly="GalleryServerPro.Web" Namespace="GalleryServerPro.WebControls" TagPrefix="tis" %>
<div class="gsp_indentedContent">
  <p class="gsp_a_ap_to">
    <asp:Label ID="lbl1" runat="server" CssClass="gsp_bold" Text="<%$ Resources:GalleryServerPro, Admin_Settings_Apply_To_Label %>"
      EnableViewState="false" />&nbsp;<asp:Label ID="lblGalleryDescription" runat="server" EnableViewState="false" />
  </p>
  <asp:PlaceHolder ID="phAdminHeader" runat="server" />
  <div class="gsp_addleftpadding5" runat="server">
    <div class="gsp_single_tab">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
          <asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Metadata_Options_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
        <p class="gsp_addtopmargin5">
          <asp:CheckBox ID="chkExtractMetadata" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Metadata_ExtractMetadata_Label %>" />
        </p>
        <p class="gsp_addleftmargin5">
          <asp:CheckBox ID="chkExtractMetadataUsingWpf" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Metadata_ExtractMetadataUsingWpf_Label %>" />
        </p>
        <p>
          <asp:Label ID="lblDateTimeFormatString" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Metadata_DateTimeFormatString_Label %>" />
          <asp:TextBox ID="txtDateTimeFormatString" runat="server" CssClass="gsp_textbox" />
        </p>
      </div>
    </div>

    <div class="gsp_single_tab" style="max-width: 100%;">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
          <asp:Label ID="lblMetadata" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Metadata_Display_Settings_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
        <asp:HiddenField ID="hdnMetadataDefinitions" runat="server" ClientIDMode="Static" />
        <div id="<%= cid %>_mdOptions" class="mdOptions"></div>
      </div>
    </div>
  </div>
  <tis:wwDataBinder ID="wwDataBinder" runat="server" OnValidateControl="wwDataBinder_ValidateControl"
    OnBeforeUnbindControl="wwDataBinder_BeforeUnbindControl">
    <DataBindingItems>
      <tis:wwDataBindingItem ID="wbi1" runat="server" BindingSource="GallerySettingsUpdateable"
        BindingSourceMember="ExtractMetadata" ControlId="chkExtractMetadata" BindingProperty="Checked"
        UserFieldName="<%$ Resources:GalleryServerPro, Admin_Metadata_ExtractMetadata_Label %>" />
      <tis:wwDataBindingItem ID="wbi2" runat="server" BindingSource="GallerySettingsUpdateable"
        BindingSourceMember="ExtractMetadataUsingWpf" ControlId="chkExtractMetadataUsingWpf"
        BindingProperty="Checked" UserFieldName="<%$ Resources:GalleryServerPro, Admin_Metadata_ExtractMetadataUsingWpf_Label %>" />
      <tis:wwDataBindingItem ID="wbi3" runat="server" BindingSource="GallerySettingsUpdateable"
        BindingSourceMember="MetadataDateTimeFormatString" ControlId="txtDateTimeFormatString"
        UserFieldName="<%$ Resources:GalleryServerPro, Admin_Metadata_DateTimeFormatString_Label %>" />
    </DataBindingItems>
  </tis:wwDataBinder>
  <asp:PlaceHolder ID="phAdminFooter" runat="server" />
</div>
<asp:PlaceHolder runat="server">
  <script>
    (function ($) {
      var grid;
			
      $(document).ready(function () {
        bindData();
        bindEventHandlers();
        configUi();
        configTooltips();
      });
      
      var bindEventHandlers = function () {
        $("#<%= chkExtractMetadata.ClientID %>").click(updateUi);
						
			  $('form:first').submit(function(e) {
			    // Serialize the metadata definitions and store in hidden field,
			    // where it will be accessed by server code after form submission.
			    var metaDefData = grid.getData();

			    // Update sequence to reflect current ordering of array (which may have been reordered by user)
			    var seq = 0;
			    $.each(metaDefData, function(n, v) { v.Sequence = seq++; });
					
			    var serializedStr = JSON.stringify(metaDefData);
			    $("#hdnMetadataDefinitions").val(serializedStr);
					
			    return true;
			  });
				
			  $(".gsp_m_rb_btn", <%= cid %>.p()).click(function (e) {
				  // User clicked the 'Rebuild' link. Send ajax request.
				  var self = this;
				  $(this).addClass('gsp_wait'); // Show wait animated gif
			    
				  var metaNameId = $(this).data("id");
				  var galleryId = window.<%= cid %>.gspData.Settings.GalleryId;
					var urlParms = "metaNameId=" + metaNameId + "&galleryId=" + galleryId;
					
					$.ajax(({
					  type: "POST",
					  url: window.Gsp.AppRoot + '/api/meta/rebuildmetaitem/?' + urlParms,
					  success: function (x, y, z) {
					    $(self).removeClass('gsp_wait');
					    var msg = "The rebuilding of metadata item '" + metaNameId + "' has been started on a background thread. The event log will contain additional details of its progress.";
					    $.gspShowMsg("Rebuilding initiated", msg, {autoCloseDelay: 10000});
					  },
					  error: function (response) {
					    $.gspShowMsg("Ajax Error", response.responseText, {msgType:'error'});
					  }
					}));
				});
			};

		  var bindData = function() {
		    $.extend(true, window, {
		      "Slick": {
		        "Formatters": {
		          "MetadataSync": function (row, cell, value, columnDef, dataContext) {
		            return "<a href='javascript:void(0)' data-id='" + value + "' class='gsp_m_rb_btn' title='Click to rebuild this meta item for all objects in the gallery'>Rebuild</a>";
		          }
		        }
		      }
		    });
				
		    var configureRowDragDrop = function () {
		      // The code in this function was copied from the example at
		      // http://mleibman.github.com/SlickGrid/examples/example9-row-reordering.html
		      grid.setSelectionModel(new Slick.RowSelectionModel());

		      var moveRowsPlugin = new Slick.RowMoveManager();

		      moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, thedata) {
		        for (var i = 0; i < thedata.rows.length; i++) {
		          // no point in moving before or after itself
		          if (thedata.rows[i] == thedata.insertBefore || thedata.rows[i] == thedata.insertBefore - 1) {
		            e.stopPropagation();
		            return false;
		          }
		        }
		        return true;
		      });

		      moveRowsPlugin.onMoveRows.subscribe(function (e, args) {
		        var extractedRows = [], left, right;
		        var rows = args.rows;
		        var insertBefore = args.insertBefore;
		        left = data.slice(0, insertBefore);
		        right = data.slice(insertBefore, data.length);

		        rows.sort(function (a, b) { return a - b; });

		        for (var i = 0; i < rows.length; i++) {
		          extractedRows.push(data[rows[i]]);
		        }

		        rows.reverse();

		        for (var i = 0; i < rows.length; i++) {
		          var row = rows[i];
		          if (row < insertBefore) {
		            left.splice(row, 1);
		          } else {
		            right.splice(row - insertBefore, 1);
		          }
		        }

		        data = left.concat(extractedRows.concat(right));

		        var selectedRows = [];
		        for (var i = 0; i < rows.length; i++)
		          selectedRows.push(left.length + i);

		        grid.resetActiveCell();
		        grid.setData(data);
		        grid.setSelectedRows(selectedRows);
		        grid.render();
		      });

		      grid.registerPlugin(moveRowsPlugin);
		    };
					
		    var columns = [
					{ id: "#", name: "&nbsp;", width: 40, behavior: "selectAndMove", resizable: false,cssClass: "cell-reorder dnd", headerCssClass:"md_col1" },
					{ id: "MetadataItem", name: "ID", field: "MetadataItem", cssClass:"md_ctr", headerCssClass:"md_ctr", toolTip:"The unique ID. This value can be used in templates to find specific items." },
					{ id: "Name", name: "Name", field: "Name", width: 200, toolTip:"Unique name of the item. These can be used in the default value template." },
					{ id: "DisplayName", name: "Display name", field: "DisplayName", width: 200, toolTip:"Text to display in UI", editor: Slick.Editors.Text },
					{ id: "IsVisibleForAlbum", name: "Album", field: "IsVisibleForAlbum", width: 100, cssClass: "md_bool md_ctr", headerCssClass: "md_ctr", toolTip: "Indicates whether to display the item for albums", formatter: Slick.Formatters.Checkmark, editor: Slick.Editors.Checkbox },
					{ id: "IsVisibleForGalleryObject", name: "Media object", field: "IsVisibleForGalleryObject", width: 110, cssClass: "md_bool md_ctr", headerCssClass: "md_ctr", toolTip: "Indicates whether to display the item for media objects", formatter: Slick.Formatters.Checkmark, editor: Slick.Editors.Checkbox },
					{ id: "DefaultValue", name: "Default value", field: "DefaultValue", width: 200, toolTip:"The template to use when adding a media object", editor: Slick.Editors.Text },
					{ id: "IsEditable", name: "Editable", field: "IsEditable", width: 100, cssClass: "md_bool md_ctr", headerCssClass: "md_ctr md_isEditableHdr", toolTip: "Indicates whether the item can be edited by the user", formatter: Slick.Formatters.Checkmark, editor: Slick.Editors.Checkbox },
					{ id: "Sync", name: "&nbsp;", field: "MetadataItem", cssClass: "md_ctr", headerCssClass: "md_ctr", formatter: Slick.Formatters.MetadataSync }
		    ];

		    var options = {
		      editable: true,
		      autoEdit: true,
		      enableRowReordering: true,
		      enableColumnReorder: false,
		      enableAddRow: false,
		      enableCellNavigation: true,
		      asyncEditorLoading: false,
		      autoHeight: true,
		      autoWidth:true
		    };

		    var data = $.parseJSON($("#hdnMetadataDefinitions").val(), true);

		    grid = new Slick.Grid("#<%= cid %>_mdOptions", data, columns, options);

				configureRowDragDrop();
			};
			
      var configUi = function() {
		    var extractMetadataSelected = $('#<%= chkExtractMetadata.ClientID %>').prop('checked');
		    
		    $('#<%= chkExtractMetadataUsingWpf.ClientID %>').prop('disabled', !extractMetadataSelected);
      };

		  var updateUi = function() {
		    var extractMetadataSelected = $('#<%= chkExtractMetadata.ClientID %>').prop('checked');
		    
		    $('#<%= chkExtractMetadataUsingWpf.ClientID %>').prop('checked', extractMetadataSelected).prop('disabled', !extractMetadataSelected);
			};
			
		  var configTooltips = function() {
		    $('#<%= chkExtractMetadata.ClientID %>').gspTooltip({
			    title: '<%= Resources.GalleryServerPro.Cfg_extractMetadata_Hdr.JsEncode() %>',
				  content: '<%= Resources.GalleryServerPro.Cfg_extractMetadata_Bdy.JsEncode() %>'
				});

			  $('#<%= chkExtractMetadataUsingWpf.ClientID %>').gspTooltip({
			    title: '<%= Resources.GalleryServerPro.Cfg_extractMetadataUsingWpf_Hdr.JsEncode() %>',
				  content: '<%= Resources.GalleryServerPro.Cfg_extractMetadataUsingWpf_Bdy.JsEncode() %>'
				});

			  $('#<%= lblDateTimeFormatString.ClientID %>').gspTooltip({
			    title: '<%= Resources.GalleryServerPro.Cfg_metadataDateTimeFormatString_Hdr.JsEncode() %>',
				  content: '<%= Resources.GalleryServerPro.Cfg_metadataDateTimeFormatString_Bdy.JsEncode() %>'
				});

			  $('#<%= lblMetadata.ClientID %>').gspTooltip({
			    title: '<%= Resources.GalleryServerPro.Cfg_metadataDisplaySettings_Hdr.JsEncode() %>',
				  content: '<%= Resources.GalleryServerPro.Cfg_metadataDisplaySettings_Bdy.JsEncode() %>'
				});
			};
		})(jQuery);
  </script>
</asp:PlaceHolder>
