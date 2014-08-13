<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="generic.ascx.cs" Inherits="GalleryServerPro.Web.Pages.Err.generic" EnableViewState="false" %>
<style>
 html, body { margin: 0; padding: 0; background-color: #353535;font: 12px Verdana, Arial, Helvetica, sans-serif; }  
.gsp_ns { color: #fff; }
.gsp_ns a { text-decoration: underline; }
.gsp_ns a:link { color: #e6e6e6; }
.gsp_ns a:visited { color: #e6e6e6; }
.gsp_ns a:hover { color: #fff; }
.gsp_ns a:active { color: #fff; }
.gsp_ns .gsp_errHeader { height: 75px; background-color: #a7a7a7; border-bottom: 1px solid #353535;padding: 10px; }
.gsp_ns .gsp_errContent { background-color: #353535;padding: 1em; }
.gsp_ns p.gsp_h2 { color:#ff7171;margin: 0 0 1em 0; padding: 0; font-size: 1.2em; font-weight: bold;  }
.gsp_ns .gsp_event_h1 {color:#ff7171;}
.gsp_ns .gsp_event_table {border: none;}
.gsp_ns .gsp_event_col1 {background-color:#545454;white-space:nowrap;width:150px;border: none;}
.gsp_ns .gsp_event_col2 {border: none;}
</style>
<div class="gsp_ns gsp_errContainer">
	<div class="gsp_errHeader">
		<asp:Image ID="imgGspLogo" runat="server" AlternateText="Gallery Server Pro logo"/>
	</div>
	<div class="gsp_errContent">
		<p>
			<asp:HyperLink ID="hlHome" runat="server" ToolTip="<%$ Resources:GalleryServerPro, Error_Home_Page_Link_Text %>"
				Text="<%$ Resources:GalleryServerPro, Error_Home_Page_Link_Text %>" /><asp:Label ID="lblSeparator" runat="server" Text=" | " /><asp:HyperLink ID="hlSiteAdmin" runat="server" ToolTip="<%$ Resources:GalleryServerPro, UC_ActionMenu_Admin_Console_Tooltip %>"
				Text="<%$ Resources:GalleryServerPro, UC_ActionMenu_Admin_Console_Text %>" /></p>
		<p class="gsp_h2">
		<asp:Literal ID="l2" runat="server" Text="<%$ Resources:GalleryServerPro, Error_Hdr %>" /></p>
		<p>
			<asp:Literal ID="l3" runat="server" Text="<%$ Resources:GalleryServerPro, Error_Dtl1 %>" /></p>
		<p id="pErrorDtl2" runat="server">
			<asp:Literal ID="l4" runat="server" Text="<%$ Resources:GalleryServerPro, Error_Dtl2 %>" /></p>
		<asp:Literal ID="litErrorDetails" runat="server" />
	</div>
</div>
