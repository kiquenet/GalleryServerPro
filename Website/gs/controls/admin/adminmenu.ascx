<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="adminmenu.ascx.cs" Inherits="GalleryServerPro.Web.Controls.Admin.adminmenu" %>
<script>
  (function ($) {
    $(document).ready(function () {
      // Add class to the menu item for the current page
      $('.gsp_adm_m .gsp_<%= GalleryPage.PageId %>', $('#<%=cid %>')).addClass('gsp_adm_m_h2_slctd');

      $('.gsp_adm_m_h2 a', $('#<%=cid %>')).click(function () {
        // Activate the newly clicked menu item
        $(this).parent().parent().find('.gsp_adm_m_h2_slctd')
          .removeClass('gsp_adm_m_h2_slctd').end().end()
          .addClass('gsp_adm_m_h2_slctd');
      });
    });
  })(jQuery);
</script>
<%= GetMenuHtml() %>
