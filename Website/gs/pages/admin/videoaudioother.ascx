<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="videoaudioother.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Admin.videoaudioother" %>
<%@ Import Namespace="GalleryServerPro.Web" %>
<%@ Register Assembly="GalleryServerPro.Web" Namespace="GalleryServerPro.WebControls" TagPrefix="tis" %>
<div class="gsp_indentedContent">
	<p class="gsp_a_ap_to">
		<asp:Label ID="lbl1" runat="server" CssClass="gsp_bold" Text="<%$ Resources:GalleryServerPro, Admin_Settings_Apply_To_Label %>"
			EnableViewState="false" />&nbsp;<asp:Label ID="lblGalleryDescription" runat="server" EnableViewState="false" />
	</p>
	<asp:PlaceHolder ID="phAdminHeader" runat="server" />
	<div class="gsp_addleftpadding5">
    <div class="gsp_single_tab">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
			    <asp:Literal ID="l1" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_General_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
			  <p>
				  <asp:CheckBox ID="chkAutoStart" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_AutoStart_Label %>" />
			  </p>
			  <p>
				  <asp:Literal ID="l3" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_VideoThumbnailPosition_Part1_Label %>" />&nbsp;
				  <asp:TextBox ID="txtVideoThumbnailPosition" runat="server" CssClass="gsp_textcenter gsp_textbox_narrow" />&nbsp;<asp:RangeValidator
					  ID="rvVideoThumbnailPosition" runat="server" Display="Dynamic" ControlToValidate="txtVideoThumbnailPosition"
					  Type="Integer" MinimumValue="0" MaximumValue="86400" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_Validation_VideoThumbnailPosition_Text %>" />
				  &nbsp;<asp:Label ID="lblVideoThumbnailPositionPart2" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_VideoThumbnailPosition_Part2_Label %>" />
			  </p>
			  <p class="gsp_bold gsp_addtopmargin5">
				  <asp:Label ID="lblMediaCtrDim" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_Media_Container_Dimensions_Label %>" />
			  </p>
			  <p class="gsp_addleftpadding5">
				  <asp:Literal ID="l5" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_MediaContainer_Video_Lbl %>" />
				  <asp:TextBox ID="txtVideoPlayerWidth" runat="server" CssClass="gsp_textcenter gsp_textbox_narrow" />&nbsp;<asp:RangeValidator
					  ID="rvVideoPlayerWidth" runat="server" Display="Dynamic" ControlToValidate="txtVideoPlayerWidth"
					  Type="Integer" MinimumValue="0" MaximumValue="10000" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_10000_Text %>" />
				  x&nbsp;
				  <asp:TextBox ID="txtVideoPlayerHeight" runat="server" CssClass="gsp_textcenter gsp_textbox_narrow" />&nbsp;<asp:RangeValidator
					  ID="rvVideoPlayerHeight" runat="server" Display="Dynamic" ControlToValidate="txtVideoPlayerHeight"
					  Type="Integer" MinimumValue="0" MaximumValue="10000" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_10000_Text %>" />
				  <asp:Literal ID="l6" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_Pixels_Lbl %>" />
			  </p>
			  <p class="gsp_addleftpadding5">
				  <asp:Literal ID="l7" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_MediaContainer_Audio_Lbl %>" />
				  <asp:TextBox ID="txtAudioPlayerWidth" runat="server" CssClass="gsp_textcenter gsp_textbox_narrow" />&nbsp;<asp:RangeValidator
					  ID="rvAudioPlayerWidth" runat="server" Display="Dynamic" ControlToValidate="txtAudioPlayerWidth"
					  Type="Integer" MinimumValue="0" MaximumValue="10000" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_10000_Text %>" />
				  x&nbsp;
				  <asp:TextBox ID="txtAudioPlayerHeight" runat="server" CssClass="gsp_textcenter gsp_textbox_narrow" />&nbsp;<asp:RangeValidator
					  ID="rvAudioPlayerHeight" runat="server" Display="Dynamic" ControlToValidate="txtAudioPlayerHeight"
					  Type="Integer" MinimumValue="0" MaximumValue="10000" Text="<%$ Resources:GalleryServerPro, Validation_Int_0_To_10000_Text %>" />
				  <asp:Literal ID="l9" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_Pixels_Lbl %>" />
			  </p>
      </div>
    </div>

    <div class="gsp_single_tab" style="max-width:100%;">
      <div class="gsp_single_tab_hdr">
        <span class="gsp_single_tab_hdr_txt">
			    <asp:Label ID="lblEncoderSettingsHdr" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderSettings_Hdr %>" />
        </span>
      </div>
      <div class="gsp_single_tab_bdy gsp_dropshadow3">
		    <p class="gsp_addleftmargin4">
			    <asp:Literal ID="l19" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderSettings_Status_Hdr %>" />&nbsp;<asp:Label
				    ID="lblEncoderStatus" runat="server" />
		    </p>
		    <p class="gsp_addleftmargin4">
			    <asp:Label ID="lblEncoderTimeout" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_EncoderTimeout_Lbl %>" />
			    <asp:TextBox ID="txtMediaEncoderTimeoutMs" runat="server" />
		    </p>
		    <p class="gsp_addleftmargin4 gsp_va_tip">
			    <asp:Literal ID="l20" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderSettings_Tip %>" />
		    </p>
		    <ul id="gsp_encoderSettingsCtr">
		    </ul>
		    <p class="gsp_addleftmargin4">
			    <a href="#" id="gsp_addEncoderSetting">
				    <asp:Literal ID="l4" runat="server" Text="<%$ Resources: GalleryServerPro, Admin_VidAudOther_AddEncoder_CmdText %>" />
			    </a>
		    </p>
		    <p runat="server" class="gsp_va_eq_hdr">
			    <asp:Literal ID="l8" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_Hdr %>" />
			    <span class="gsp_va_q_pl"><a id="gsp_removeCompletedEncodings" href="#"><asp:Literal ID="l18b" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_RemoveComplete_Text %>" /></a>&nbsp;
						<a id="gsp_removeAllEncodings" href="#"><asp:Literal ID="l18a" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_RemoveAll_Text %>" /></a>
			    </span>&nbsp;<img src="<%=Utils.GetSkinnedUrl("/images/wait-squares.gif")%>"class="gsp_spinner" alt="" />
		    </p>
		    <p class="gsp_va_q_empty gsp_invisible">
			    <asp:Literal ID="l10" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_Empty_Msg %>" />
		    </p>
		    <table class="gsp_va_q_Ctr">
			    <thead>
				    <tr>
					    <th></th>
					    <th>
						    <asp:Literal ID="l11" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_MediaObject_Hdr %>" />
					    </th>
					    <th>
						    <asp:Literal ID="l12" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_Status_Hdr %>" />
					    </th>
					    <th>
						    <asp:Literal ID="l13" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_DateAdded_Hdr %>" />
					    </th>
					    <th>
						    <asp:Literal ID="l14" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_EncodingStart_Hdr %>" />
					    </th>
					    <th>
						    <asp:Literal ID="l15" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_EncodingComplete_Hdr %>" />
					    </th>
					    <th>
						    <asp:Literal ID="l16" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_Duration_Hdr %>" />
					    </th>
					    <th class="gsp_va_q_sd">
						    <asp:Literal ID="l17" runat="server" Text="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderQueue_Result_Hdr %>" />
					    </th>
				    </tr>
			    </thead>
			    <tbody id="gsp_tblQueueBody">
			    </tbody>
		    </table>
      </div>
    </div>
	</div>
	<asp:HiddenField ID="hdnEncoderSettings" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="hdnSourceAvailableFileExtensions" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="hdnDestinationAvailableFileExtensions" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="hdnQueueItems" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="hdnDateNowOnServer" runat="server" ClientIDMode="Static" />
	<tis:wwDataBinder ID="wwDataBinder" runat="server">
		<DataBindingItems>
			<tis:wwDataBindingItem ID="wbi1" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="AutoStartMediaObject" ControlId="chkAutoStart" BindingProperty="Checked"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_AutoStart_Label %>" />
			<tis:wwDataBindingItem ID="wbi2" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="DefaultVideoPlayerWidth" ControlId="txtVideoPlayerWidth" UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_MediaContainer_Video_Lbl %>" />
			<tis:wwDataBindingItem ID="wbi3" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="DefaultVideoPlayerHeight" ControlId="txtVideoPlayerHeight"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_MediaContainer_Video_Lbl %>" />
			<tis:wwDataBindingItem ID="wbi4" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="VideoThumbnailPosition" ControlId="txtVideoThumbnailPosition"
				UserFieldName="Video Thumbnail Position" />
			<tis:wwDataBindingItem ID="wbi5" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="MediaEncoderTimeoutMs" ControlId="txtMediaEncoderTimeoutMs"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_EncoderTimeout_Lbl %>" />
			<tis:wwDataBindingItem ID="wbi6" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="DefaultAudioPlayerWidth" ControlId="txtAudioPlayerWidth" UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_MediaContainer_Audio_Lbl %>" />
			<tis:wwDataBindingItem ID="wbi7" runat="server" BindingSource="GallerySettingsUpdateable"
				BindingSourceMember="DefaultAudioPlayerHeight" ControlId="txtAudioPlayerHeight"
				UserFieldName="<%$ Resources:GalleryServerPro, Admin_VidAudOther_MediaContainer_Audio_Lbl %>" />
		</DataBindingItems>
	</tis:wwDataBinder>
	<asp:PlaceHolder ID="phAdminFooter" runat="server" />
</div>
<asp:PlaceHolder runat="server">
	<script id='tmplEncoderSetting' type='text/x-jsrender'>
<li class="gsp_va_et">
<table class="gsp_encoderSettingRowCtr">
<tr><td style="white-space:nowrap;">
	<a href="#" title='Delete' class="gsp_va_et_d_btn gsp_hoverLink"><img src='<%= Utils.GetSkinnedUrl("images/delete-red-s.png") %>' alt='Delete' /></a>
	<img src='<%= Utils.GetSkinnedUrl("images/move-updown-s.png") %>' class="gsp_hoverLink" title='<%= EsMoveTooltip %>' alt='' />
	 <%= EsConvertString %> <select name="sltSourceFileExtension">{{for SourceAvailableFileExtensions}}
		<option {{if Value === #parent.parent.data.SourceFileExtension}}selected="selected"{{/if}} value="{{:Value}}">{{:Text}}</option>
		{{/for}}</select>
	 <%= EsToString %> <select name="sltDestinationFileExtension">{{for DestinationAvailableFileExtensions}}
		<option {{if Value === #parent.parent.data.DestinationFileExtension}}selected="selected"{{/if}} value="{{:Value}}">{{:Text}}</option>
		{{/for}}</select>
	 <%= EsFFmpegArgsString %> </td>
	<td width="100%"><input type="text" name="iptArgs" value="{{>EncoderArguments}}" style="width:100%;" /></td></tr>
</table>
</li>
	</script>
	<script id='tmplQueue' type='text/x-jsrender'>
<tr class="{{: ~getRowCssClass(#data)}}">
	<td><a href="#" title='Remove from queue' class="gsp_va_qi_d_btn gsp_hoverLink" data-id="{{:MediaQueueId}}" data-status="{{:Status}}"><img src='<%= Utils.GetSkinnedUrl("images/delete-red-s.png") %>' alt='Delete' /></a></td>
	<td><a href="<%= Utils.GetUrl(PageId.mediaobject) %>&moid={{:MediaObjectId}}">{{:MediaObjectId}}</a></td>
	<td>{{:Status}}</td>
	<td>{{: ~getDateAsFormattedString(DateAdded)}}</td>
	<td>{{: ~getDateAsFormattedString(DateConversionStarted)}}</td>
	<td>{{: ~getDateAsFormattedString(DateConversionCompleted)}}</td>
	<td>{{: ~getDuration()}}</td>
	<td class="gsp_va_q_sd"><div>[<a class="gsp_va_q_sd_sa" href="#"><%= EqExpandString %></a>] {{:StatusDetail}}</div></td>
</tr>
	</script>
	<script>
	  (function($) {

	    $(document).ready(function() {
	      bindEventHandlers();
	      bindEncoderSettings();
	      bindQueue();
	      configTooltips();
	    });

	    var bindEventHandlers = function() {
	      $("#gsp_addEncoderSetting").click(onAddEncoderRow);
	      $("#gsp_removeAllEncodings").click(removeAllEncodings);
	      $("#gsp_removeCompletedEncodings").click(removeCompletedEncodings);
	      $('form:first').submit(function(e) {
	        updateHiddenField();
	        //e.preventDefault();
	        return true;
	      });
	    };

	    var bindQueue = function() {
	      var queueItems = $.parseJSON($("#hdnQueueItems").val(), true);

	      if (!queueItems || queueItems.length == 0) {
	        hideQueueTable();
	        return;
	      }
	      // Generate the HTML from the template
	      var tmplData = $("#tmplQueue").render(queueItems, {
	        getDateAsFormattedString: function(dateValue) {
	          if (dateValue != null) {
	            return Globalize.format(new Date(dateValue), "MMM d h:mm tt");
	          } else
	            return "";
	        },
	        getRowCssClass: function(data) {
	          if ((data != null) && (data.Status == "Processing"))
	            return "gsp_va_q_cr";
	          else
	            return "";
	        },
	        getDuration: function() {
	          if (this.data.DateConversionStarted == null)
	            return "";

	          var dtEnd = (this.data.DateConversionCompleted != null ? new Date(this.data.DateConversionCompleted) : new Date($.parseJSON($("#hdnDateNowOnServer").val(), true)));
	          return Globalize.format((dtEnd - new Date(this.data.DateConversionStarted)) / 60000, "n1") + " <%= EqMinutesString %>";
					}
				});

			  $("#gsp_tblQueueBody").html(tmplData); // Add the HTML to the page

			  bindDeleteBtn($("#gsp_tblQueueBody"));
			  bindQueueStatus();
			};

		  var hideQueueTable = function() {
		    $(".gsp_va_q_Ctr").hide();
		    $(".gsp_va_q_empty").show();
		  };

		  var bindDeleteBtn = function(tmplData) {
		    $(".gsp_va_qi_d_btn", tmplData).click(function(e) {
		      deleteQueueItems(jQuery.makeArray($(this).data("id")));
		      return false;
		    });
		  };

		  var removeAllEncodings = function(e) {
		    // Delete all media queue IDs
		    var ids = $('.gsp_va_qi_d_btn').map(function() {
		      return $(this).data("id");
		    }).get();

		    deleteQueueItems(ids);

		    return false;
		  };

		  var removeCompletedEncodings = function(e) {
		    // Delete all media queue IDs with a status of 'Complete' or 'Error'
		    var ids = $('.gsp_va_qi_d_btn').map(function() {
		      var status = $(this).data("status");
		      if ((status == "Complete") || (status == "Error"))
		        return $(this).data("id");
		      else
		        return null;
		    }).get();

		    deleteQueueItems(ids);

		    return false;
		  };

		  var deleteQueueItems = function(mediaQueueIds) {
		    var onQueueItemDeleted = function() {
		      $.each(mediaQueueIds, function(idx, id) {
		        $(".gsp_va_q_Ctr tr:has(a[data-id=" + id + "])").remove(); // Remove row from HTML table
		      });

		      if ($(".gsp_va_q_Ctr tbody tr").length == 0)
		        hideQueueTable();
		    };

		    if ((mediaQueueIds == null) || (mediaQueueIds.length == 0))
		      return;

		    $(".gsp_spinner").show();
		    $.ajax(({
		      type: "DELETE",
		      url: window.Gsp.AppRoot + '/api/mediaqueueitem',
		      data: JSON.stringify(mediaQueueIds),
		      contentType: "application/json; charset=utf-8",
		      success: function(response) {
		        onQueueItemDeleted();
		      },
		      error: function(response) {
		        $.gspShowMsg("Cannot Save Changes", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
		      },
		      complete: function() {
		        $(".gsp_spinner").hide();
		      }
		    }));
		  };

		  var bindEncoderSettings = function() {
		    var encoderSettings = $.parseJSON($("#hdnEncoderSettings").val());

		    // Get available file extensions and add as property on each encoder setting
		    var sourceAvailableFileExtensions = $.parseJSON($("#hdnSourceAvailableFileExtensions").val());
		    var destinationAvailableFileExtensions = $.parseJSON($("#hdnDestinationAvailableFileExtensions").val());

		    $.each(encoderSettings, function(i, item) {
		      item.SourceAvailableFileExtensions = sourceAvailableFileExtensions;
		      item.DestinationAvailableFileExtensions = destinationAvailableFileExtensions;
		    });

		    var tmplData = $("#tmplEncoderSetting").render(encoderSettings); // Generate the HTML from the template
		    $("#gsp_encoderSettingsCtr").html(tmplData) // Add the HTML to the page
					.find('.gsp_va_et_d_btn').click(function() {
					  $(this).parents("li").remove();
					  return false;
					});

		    $("#gsp_encoderSettingsCtr").sortable({
		      axis: 'y'
		    });
		  };

		  var onAddEncoderRow = function(e) {
		    updateHiddenField();

		    // Add an item to the encoder settings stored in the hidden field, then re-bind the template
		    var encoderSetting = {};
		    encoderSetting.SourceFileExtension = "";
		    encoderSetting.DestinationFileExtension = "";
		    encoderSetting.EncoderArguments = "";

		    var encoderSettings = $.parseJSON($("#hdnEncoderSettings").val());
		    encoderSettings.push(encoderSetting);
		    var serializedStr = JSON.stringify(encoderSettings);
		    $("#hdnEncoderSettings").val(serializedStr);

		    bindEncoderSettings();

		    return false;
		  };

		  var bindQueueStatus = function() {
		    // Expand/collapse the status detail when clicked
		    $('.gsp_va_q_sd_sa').click(function() {
		      var parEl = $(this).parent();
		      if (parEl.hasClass('gsp_va_q_sd_all'))
		        $(".gsp_va_q_sd_sa", parEl).text("<%= EqExpandString %>");
						else
						  $(".gsp_va_q_sd_sa", parEl).text("<%= EqCollapseString %>");

				  parEl.toggleClass('gsp_va_q_sd_all');
				  return false;
				});
			};

		  var updateHiddenField = function() {
		    // Convert data in encoder settings HTML to MediaEncoderSettings array, then serialize and store in hidden field,
		    // where it will be accessed by server code after form submission.
		    var encoderSettings = new Array();

		    $("#gsp_encoderSettingsCtr li").each(function() {
		      var sourceFileExt = $("select[name=sltSourceFileExtension]", $(this)).val();
		      var destFileExt = $("select[name=sltDestinationFileExtension]", $(this)).val();
		      var args = $("input[name=iptArgs]", $(this)).val();

		      var encoderSetting = {};
		      encoderSetting.SourceFileExtension = sourceFileExt;
		      encoderSetting.DestinationFileExtension = destFileExt;
		      encoderSetting.EncoderArguments = args;

		      encoderSettings.push(encoderSetting);
		    });

		    var serializedStr = JSON.stringify(encoderSettings);
		    $("#hdnEncoderSettings").val(serializedStr);
		  };

		  var configTooltips = function() {
		    $('#<%= chkAutoStart.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Cfg_autoStartMediaObject_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Cfg_autoStartMediaObject_Bdy.JsEncode() %>'
		    });

		    $('#<%= lblVideoThumbnailPositionPart2.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Cfg_VideoThumbnailPosition_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Cfg_VideoThumbnailPosition_Bdy.JsEncode() %>'
		    });

		    $('#<%= lblMediaCtrDim.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Cfg_MediaContainerDimensions_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Cfg_MediaContainerDimensions_Bdy.JsEncode() %>'
		    });

		    $('#<%= lblEncoderSettingsHdr.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Admin_VidAudOther_EncoderSettings_Overview_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Admin_VidAudOther_EncoderSettings_Overview_Bdy.JsEncode() %>'
		    });

		    $('#<%= lblEncoderTimeout.ClientID %>').gspTooltip({
		      title: '<%= Resources.GalleryServerPro.Admin_VidAudOther_EncoderTimeout_Hdr.JsEncode() %>',
		      content: '<%= Resources.GalleryServerPro.Admin_VidAudOther_EncoderTimeout_Bdy.JsEncode() %>'
		    });
		  };
		})(jQuery);
	</script>
</asp:PlaceHolder>
