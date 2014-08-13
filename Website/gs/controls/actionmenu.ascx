<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="actionmenu.ascx.cs" Inherits="GalleryServerPro.Web.Controls.actionmenu" %>

<script>
  (function ($) {
    $(document).ready(function() {
      $("#<%=cid %>_mnu").menubar({
        autoExpand: true
      }).show();
      
      $('#<%= cid %>_mnu_logoff').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        Gsp.DataService.logOff(function (data) { Gsp.ReloadPage(); });
      });
    });
  })(jQuery);
</script>
<%= GetMenuHtml() %>