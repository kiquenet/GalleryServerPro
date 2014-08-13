using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Metadata;

namespace GalleryServerPro.Data
{
	/// <summary>
	/// Contains functionality for seeding the gallery database.
	/// </summary>
	public static class SeedController
	{
		#region Fields

		private const string DefaultTmplName = "Default";

		#endregion

		#region Properties

		/// <summary>
		/// Gets the default HTML template for the header UI template. The replacement token {PayPalCartWidget} must be replaced with the HTML
		/// for the PayPal 'view cart' widget or an empty string if not required.
		/// </summary>
		private static string HeaderHtmlTmpl
		{
			get
			{
				return @"<nav class='gsp_usernav'>
{{if Settings.ShowSearch}}
 <div class='gsp_searchlink gsp_useroption'>
	<img class='gsp_search_trigger' src='{{:App.SkinPath}}/images/search-l.png' alt='{{:Resource.HdrSearchButtonTt}}' title='{{:Resource.HdrSearchButtonTt}}' />
 </div>
{{/if}}

{{if User.UserAlbumId > 0}}
 <div class='gsp_homealbumlink gsp_useroption'>
	<a href='{{:App.CurrentPageUrl}}?aid={{:User.UserAlbumId}}' title='{{:Resource.HdrUserAlbumLinkTt}}'>
	 <img alt='{{:Resource.HdrUserAlbumLinkTt}}' src='{{:App.SkinPath}}/images/home-l.png' title='{{:Resource.HdrUserAlbumLinkTt}}'></a></div>
{{/if}}

{PayPalCartWidget}

{{if Settings.ShowLogin}}
 {{if User.IsAuthenticated}}
	 <div class='gsp_logoffLinkCtr gsp_useroption'>
		<a class='gsp_logoffLink' href='javascript:void(0);' title='{{:Resource.HdrLogoutTt}}'>
		 <img alt='{{:Resource.HdrLogoutTt}}' src='{{:App.SkinPath}}/images/logoff-l.png' title='{{:Resource.HdrLogoutTt}}'></a></div>
	 <div class='loggedonview gsp_useroption'>
	 {{if Settings.AllowManageOwnAccount}}
		<a id='{{:Settings.ClientId}}_userwelcome' href='{{:App.CurrentPageUrl}}?g=myaccount&aid={{:Album.Id}}' class='gsp_welcome' title='{{:Resource.HdrMyAccountTt}}'>{{:User.UserName}}</a>
	 {{else}}
		<span id='{{:Settings.ClientId}}_userwelcome' class='gsp_welcome'>{{:User.UserName}}</span>
	 {{/if}}
	 </div>
 {{else}}
	{{if Settings.EnableSelfRegistration}}
	 <div class='gsp_createaccount gsp_useroption'>
		<a href='{{:App.CurrentPageUrl}}?g=createaccount' title='{{:Resource.HdrCreateAccountLinkText}}'>{{:Resource.HdrCreateAccountLinkText}}</a></div>
	{{/if}}
	 <div class='gsp_login gsp_useroption'>
		<a href='javascript:void(0);' class='gsp_login_trigger gsp_addrightmargin3'>{{:Resource.HdrLoginLinkText}}</a></div>
 {{/if}}
{{/if}}
</nav>
{{if Settings.Title}}
 <p class='gsp_bannertext'>
 {{if Settings.TitleUrl}}<a title='{{:Settings.TitleUrlTt}}' href='{{:Settings.TitleUrl}}'>{{:Settings.Title}}</a>{{else}}{{:Settings.Title}}{{/if}}</p>
{{/if}}";
			}
		}

		/// <summary>
		/// Gets the default JavaScript template for the header UI template. The replacement token {PayPalCartJs} must be replaced with the JavaScript
		/// for the PayPal 'view cart' widget or an empty string if not required.
		/// </summary>
		private static string HeaderJsTmpl
		{
			get
			{
				return @"// Call the gspHeader plug-in, which adds the HTML to the page and then configures it
$('#{{:Settings.HeaderClientId}}').gspHeader('{{:Settings.HeaderTmplName}}', window.{{:Settings.ClientId}}.gspData);
{PayPalCartJs}";
			}
		}

		/// <summary>
		/// Gets the default HTML template for the media object UI template. The replacement token {FacebookCommentWidget} must be replaced with the HTML
		/// for the Facebook Comment widget or an empty string if not required.
		/// </summary>
		private static string MediaObjectHtmlTmpl
		{
			get
			{
				return @"<div class='gsp_mvMediaView'>
 <div class='gsp_mvMediaHeader'>
	<div class='gsp_mvMediaHeaderRow'>
	{{if Settings.ShowMediaObjectNavigation}}
	 <div class='gsp_mvMediaHeaderCell gsp_mvPrevCell'>
		<a href='{{: ~prevUrl() }}'><img src='{{:App.SkinPath}}/images/arrow-left-l.png' class='gsp_mvPrevBtn' alt='{{:Resource.MoPrev}}' title='{{:Resource.MoPrev}}' /></a>
	 </div>
	{{/if}}
	 <div class='gsp_mvMediaHeaderCell gsp_mvToolbarCell'>
		<span class='gsp_mvToolbar ui-widget-header ui-corner-all'>
		<button class='gsp_mvTbEmbed'>{{:Resource.MoTbEmbed}}</button>
		{{if Settings.SlideShowType == 'Inline'}}
		<input type='checkbox' id='{{:Settings.ClientId}}_slideshow' class='gsp_mvTbSlideshow' /><label for='{{:Settings.ClientId}}_slideshow' class='gsp_mvTbSlideshowLbl'>{{:Resource.MoTbSsStart}}</label>
		{{else}}
		<button class='gsp_mvTbSlideshow gsp_mvTbSlideshowLbl'>{{:Resource.MoTbSsStart}}</button>
		{{/if}}
		<button class='gsp_mvTbMove'>{{:Resource.MoTbMove}}</button>
		<button class='gsp_mvTbCopy'>{{:Resource.MoTbCopy}}</button>
		<button class='gsp_mvTbRotate'>{{:Resource.MoTbRotate}}</button>
		<button class='gsp_mvTbDelete'>{{:Resource.MoTbDelete}}</button>
		</span>
	 </div>
	{{if Settings.ShowMediaObjectIndexPosition}}
	 <div class='gsp_mvMediaHeaderCell gsp_mvPosition'>
		(<span class='gsp_mvPositionIdx'>{{:MediaItem.Index}}</span> {{:Resource.MoPosSptr}} <span class='gsp_mvPosAlbumCount'>{{:Album.NumMediaItems}}</span>)
	 </div>
	{{/if}}
	{{if Settings.ShowMediaObjectNavigation}}
	 <div class='gsp_mvMediaHeaderCell gsp_mvNextCell'>
		<a href='{{: ~nextUrl() }}'><img src='{{:App.SkinPath}}/images/arrow-right-l.png' class='gsp_mvNextBtn' alt='{{:Resource.MoNext}}' title='{{:Resource.MoNext}}' /></a>
	 </div>
	{{/if}}
 </div>
</div>

<div class='gsp_moContainer'>
{{:MediaItem.Views[MediaItem.ViewIndex].HtmlOutput}}</div>
{{if Settings.ShowMediaObjectTitle}}
<div class='gsp_mediaObjectTitle'>{{:MediaItem.Title}}</div>
{{/if}}
</div>

{FacebookCommentWidget}

<div class='gsp_mo_share_dlg gsp_dlg'>
 <p class='gsp_mo_share_dlg_t'>{{:Resource.MoShare}}</p>
{{if Settings.AllowDownload || Settings.AllowZipDownload}}
 <p class='gsp_mo_share_dlg_s'>{{:Resource.MoShareDwnld}}</p>
{{/if}}
 <p class='gsp_addleftmargin5'>
{{if Settings.AllowDownload}}
 <select class='gsp_mo_share_dlg_ipt gsp_mo_share_dlg_ipt_select'>
	<option value='1'>{{:Resource.MoShareSlctThmb}}</option>
	<option value='2' selected='selected'>{{:Resource.MoShareSlctOpt}}</option>
	{{if Album.Permissions.ViewOriginalMediaObject}}
	<option value='3'>{{:Resource.MoShareSlctOrg}}</option>
	{{/if}}
 </select><a href='{{: ~getMediaUrl(MediaItem.Id, true) }}' class='gsp_mo_share_dwnld'>{{:Resource.MoShareDwnldFile}}</a>
{{/if}}
{{if Settings.AllowZipDownload}}
 <a href='{{: ~getDownloadUrl(Album.Id) }}' class='gsp_mo_share_dwnld_zip' title='{{:Resource.MoShareDwnldZipTt}}'>{{:Resource.MoShareDwnldZip}}</a>
{{/if}}
 </p>
 <p class='gsp_mo_share_dlg_s'>{{:Resource.MoShareThisPage}}</p>
 <p class='gsp_addleftmargin5'><input type='text' class='gsp_mo_share_dlg_ipt gsp_mo_share_dlg_ipt_url' value='{{: ~getMediaUrl(MediaItem.Id, true) }}' /></p>
 <p class='gsp_mo_share_dlg_s'>{{:Resource.MoShareHtml}}</p>
 <p class='gsp_addleftmargin5'><textarea class='gsp_mo_share_dlg_ipt gsp_mo_share_dlg_ipt_embed'>{{: ~getEmbedCode() }}</textarea></p>
</div>";
			}
		}

		/// <summary>
		/// Gets the default JavaScript template for the media object UI template. The replacement token {FacebookJs} must be replaced with the JavaScript
		/// required to interact with the Facebook API or an empty string if not required.
		/// </summary>
		private static string MediaObjectJsTmpl
		{
			get
			{
				return @"// Call the gspMedia plug-in, which adds the HTML to the page and then configures it
$('#{{:Settings.MediaClientId}}').gspMedia('{{:Settings.MediaTmplName}}', window.{{:Settings.ClientId}}.gspData);
{FacebookJs}";
			}
		}

		/// <summary>
		/// Gets the default HTML template for the left pane UI template. The replacement tokens {TagTrees} and {TagClouds} 
		/// must be replaced with the HTML for the tag trees and tag clouds or an empty string if not required.
		/// </summary>
		private static string LeftPaneHtmlTmpl
		{
			get
			{
				return @"<div id='{{:Settings.ClientId}}_lptv' class='gsp_lpalbumtree'></div>
{{if App.LatestUrl}}<p class='gsp_lplatest'><a href='{{:App.LatestUrl}}' class='jstree-anchor'><i class='jstree-icon'></i>{{:Resource.LpRecent}}</a></p>{{/if}}
{{if App.TopRatedUrl}}<p class='gsp_lptoprated'><a href='{{:App.TopRatedUrl}}' class='jstree-anchor'><i class='jstree-icon'></i>{{:Resource.LpTopRated}}</a></p>{{/if}}
{TagTrees}
{TagClouds}";
			}
		}

		/// <summary>
		/// Gets the default JavaScript template for the left pane UI template. The replacement tokens {TagTrees} and {TagClouds} 
		/// must be replaced with the JavaScript for the tag trees and tag clouds or an empty string if not required.
		/// </summary>
		private static string LeftPaneJsTmpl
		{
			get
			{
				return @"// Render the left pane if it exists
var $lp = $('#{{:Settings.LeftPaneClientId}}');

if ($lp.length > 0) {
 $lp.html( $.render [ '{{:Settings.LeftPaneTmplName}}' ]( window.{{:Settings.ClientId}}.gspData ));

 var options = {
  albumIdsToSelect: [{{:Album.Id}}],
  navigateUrl: '{{:App.CurrentPageUrl}}'
 };

 // Call the gspTreeView plug-in, which adds an album treeview
 $('#{{:Settings.ClientId}}_lptv').gspTreeView(window.{{:Settings.ClientId}}.gspAlbumTreeData, options);
}
{TagTrees}
{TagClouds}";
			}
		}

		/// <summary>
		/// Gets the default HTML template for the right pane UI template. The replacement tokens {PayPalAddToCartWidget} and {FacebookLikeWidget} 
		/// must be replaced with the HTML for the PayPal 'add to cart' widget and the Facebook Like widget or an empty string if not required.
		/// </summary>
		private static string RightPaneHtmlTmpl
		{
			get
			{
				return @"{PayPalAddToCartWidget}{FacebookLikeWidget}<table class='gsp_meta'>
{{if Album.VirtualType != 1 && MediaItem != null}}
 <tr class='gsp_m1Row'><td colspan='2'>{{:Resource.AbmPfx}} <a href='{{: ~getAlbumUrl(MediaItem.AlbumId) }}'>{{:MediaItem.AlbumTitle}}</a></td></tr>
{{/if}}
{{for ActiveMetaItems}}
{{if MTypeId == 113|| MTypeId == 29}}
 <tr class='gsp_m1Row gsp_mRowHdr'><td colspan='2' class='gsp_k'>{{:Desc}}</td></tr>
 <tr class='gsp_m1Row gsp_mRowDtl' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td colspan='2' class='gsp_v'>{{:Value}}</td></tr>
{{else MTypeId == 114 || MTypeId == 41}}
 <tr class='gsp_m1Row gsp_mRowHdr'><td colspan='2' class='gsp_k'>{{:Desc}}</td></tr>
 <tr class='gsp_m1Row gsp_mRowDtl' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td colspan='2' class='gsp_v gsp_mCaption'>{{:Value}}</td></tr>
{{else MTypeId == 42 || MTypeId == 22}}
 <tr class='gsp_m1Row gsp_mRowHdr'><td colspan='2' class='gsp_k'>{{:Desc}}</td></tr>
 <tr class='gsp_m1Row gsp_mRowDtl' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td colspan='2' class='gsp_v {{if MTypeId == 22}}gsp_mtag{{else}}gsp_mpeople{{/if}}'>{{:Value}}</td></tr>
{{else MTypeId == 112}}
 <tr class='gsp_m1Row gsp_mRowHdr'><td colspan='2' class='gsp_k'>{{:Desc}}</td></tr>
 <tr class='gsp_m1Row gsp_mRowDtl' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td colspan='2' class='gsp_v gsp_mCaption'>{{>Value}}</td></tr>
{{else MTypeId == 26}}
 <tr class='gsp_m2Row' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td class='gsp_k'>{{:Desc}}:</td><td class='gsp_v gsp_mrating'><div class='gsp_rating' data-rateit-value='{{:Value}}'></div></td></tr>
{{else}}
 <tr class='gsp_m2Row' data-id='{{:Id}}' data-iseditable='{{:IsEditable}}'><td class='gsp_k'>{{:Desc}}:</td><td class='gsp_v'>{{:Value}}</td></tr>
{{/if}}
{{/for}}
</table>";
			}
		}

		/// <summary>
		/// Gets the default JavaScript template for the right pane UI template. The replacement tokens {PayPalAddToCartJs} and {FacebookJs} 
		/// must be replaced with the JavaScript for the PayPal 'add to cart' widget and the Facebook API or an empty string if not required.
		/// </summary>
		private static string RightPaneJsTmpl
		{
			get
			{
				return @"var options = {
 tmplName : '{{:Settings.RightPaneTmplName}}'
};

$('#{{:Settings.RightPaneClientId}}').gspMeta({{:Settings.ClientId}}.gspData, options);
{PayPalAddToCartJs}{FacebookJs}";
			}
		}

		/// <summary>
		/// Gets the JavaScript required to interact with the Facebook API. Includes additional script to activate Facebook each time the
		/// next and previous functions are invoked when browsing through media objects.
		/// </summary>
		private static string FacebookJs
		{
			get
			{
				return @"
(function(d, s, id) {
	var js, fjs = d.getElementsByTagName(s)[0];
	if (d.getElementById(id)) return;
	js = d.createElement(s); js.id = id;
	js.src = '//connect.facebook.net/en_US/all.js#xfbml=1';
	fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

$('#{{:Settings.MediaClientId}}').on('next.{{:Settings.ClientId}} previous.{{:Settings.ClientId}}', function() {
 if (typeof (FB) != 'undefined') FB.XFBML.parse();
});";
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Inserts the seed data into the Gallery Server Pro tables. This will reset all data to their default values.
		/// </summary>
		/// <param name="ctx">The data context.</param>
		public static void InsertSeedData(GalleryDb ctx)
		{
			InsertAppSettings(ctx);
			InsertGalleries(ctx);
			InsertAlbums(ctx);
			InsertMetadata(ctx);
			InsertGallerySettings(ctx);
			InsertMimeTypes(ctx);
			InsertMimeTypeGalleries(ctx);
			InsertMediaTemplates(ctx);
			InsertUiTemplates(ctx);
			InsertUiTemplateAlbums(ctx);
		}

		/// <summary>
		/// Adds or updates the templates available to Enterprise license holders. This includes Facebook and PayPal templates.
		/// Any existing enterprise templates are replaced. Note that only UI templates associated with the template gallery are
		/// updated. The calling code must ensure that these templates are propagated to the remaining galleries.
		/// </summary>
		public static void InsertEnterpriseTemplates()
		{
			InsertLeftPaneEnterpriseTemplates();

			InsertFacebookTemplates();

			InsertPayPalTemplates();
		}

		#endregion

		#region Functions

		private static void InsertAppSettings(GalleryDb ctx)
		{
			var appSettings = new[]
													{
														new AppSettingDto {SettingName = "Skin", SettingValue = "dark"},
														new AppSettingDto {SettingName = "MediaObjectDownloadBufferSize", SettingValue = "32768"},
														new AppSettingDto {SettingName = "EncryptMediaObjectUrlOnClient", SettingValue = "False"},
														new AppSettingDto {SettingName = "EncryptionKey", SettingValue = "mNU-h7:5f_)3=c%@^}#U9Tn*"},
														new AppSettingDto {SettingName = "JQueryScriptPath", SettingValue = "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"},
														new AppSettingDto {SettingName = "JQueryMigrateScriptPath", SettingValue = "//code.jquery.com/jquery-migrate-1.2.1.js"},
														new AppSettingDto {SettingName = "JQueryUiScriptPath", SettingValue = "//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"},
														new AppSettingDto {SettingName = "MembershipProviderName", SettingValue = ""},
														new AppSettingDto {SettingName = "RoleProviderName", SettingValue = ""},
														new AppSettingDto {SettingName = "ProductKey", SettingValue = ""},
														new AppSettingDto {SettingName = "EnableCache", SettingValue = "True"},
														new AppSettingDto {SettingName = "AllowGalleryAdminToManageUsersAndRoles", SettingValue = "True"},
														new AppSettingDto {SettingName = "AllowGalleryAdminToViewAllUsersAndRoles", SettingValue = "True"},
														new AppSettingDto {SettingName = "MaxNumberErrorItems", SettingValue = "200"},
														new AppSettingDto {SettingName = "EmailFromName", SettingValue = "Gallery Server Pro"},
														new AppSettingDto {SettingName = "EmailFromAddress", SettingValue = "webmaster@yourisp.com"},
														new AppSettingDto {SettingName = "SmtpServer", SettingValue = ""},
														new AppSettingDto {SettingName = "SmtpServerPort", SettingValue = ""},
														new AppSettingDto {SettingName = "SendEmailUsingSsl", SettingValue = "False"},
														new AppSettingDto {SettingName = "DataSchemaVersion", SettingValue = GalleryDataSchemaVersionEnumHelper.ConvertGalleryDataSchemaVersionToString(GalleryDb.DataSchemaVersion)}
													};

			ctx.AppSettings.AddOrUpdate(a => a.SettingName, appSettings);

			SaveChanges(ctx);
		}

		private static void InsertGalleries(GalleryDb ctx)
		{
			ctx.Galleries.AddOrUpdate(a => a.IsTemplate, new GalleryDto { Description = "Template Gallery", IsTemplate = true, DateAdded = DateTime.UtcNow });

			if (!ctx.Galleries.Any(g => !g.IsTemplate))
			{
				// Need to add a non-template gallery
				ctx.Galleries.Add(new GalleryDto { Description = "My Gallery", IsTemplate = false, DateAdded = DateTime.UtcNow });
			}

			SaveChanges(ctx); // Need to save so we can get the newly assigned gallery ID.
		}

		private static void InsertAlbums(GalleryDb ctx)
		{
			// Insert the root album into the first non-template gallery.
			var galleryId = ctx.Galleries.First(g => !g.IsTemplate).GalleryId;

			if (!ctx.Albums.Any(a => a.FKAlbumParentId == null && a.FKGalleryId == galleryId))
			{
				var rootAlbum = new AlbumDto
													{
														FKGalleryId = galleryId,
														FKAlbumParentId = null,
														DirectoryName = String.Empty,
														ThumbnailMediaObjectId = 0,
														SortByMetaName = MetadataItemName.DateAdded,
														SortAscending = true,
														Seq = 0,
														DateAdded = DateTime.Now,
														CreatedBy = "System",
														LastModifiedBy = "System",
														DateLastModified = DateTime.Now,
														OwnedBy = String.Empty,
														OwnerRoleName = String.Empty,
														IsPrivate = false
													};

				ctx.Albums.Add(rootAlbum);
			}

			SaveChanges(ctx);

			// NOTE: The title & summary for this album are validated and inserted if necessary in the function InsertMetadata().

			// Add the title
			//ctx.Metadatas.AddOrUpdate(m => new { m.FKAlbumId, m.MetaName }, new MetadataDto
			//	{
			//		FKAlbumId = rootAlbum.AlbumId,
			//		MetaName = MetadataItemName.Title,
			//		Value = "All albums"
			//	});

			//// Add the caption
			//ctx.Metadatas.AddOrUpdate(m => new { m.FKAlbumId, m.MetaName }, new MetadataDto
			//	{
			//		FKAlbumId = rootAlbum.AlbumId,
			//		MetaName = MetadataItemName.Caption,
			//		Value = "Welcome to Gallery Server Pro!"
			//	});
		}

		private static void InsertGallerySettings(GalleryDb ctx)
		{
			foreach (var galleryDto in ctx.Galleries)
			{
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaObjectPath", SettingValue = @"gs\mediaobjects" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ThumbnailPath", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "OptimizedPath", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaObjectPathIsReadOnly", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ShowHeader", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "GalleryTitle", SettingValue = "Media Gallery" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "GalleryTitleUrl", SettingValue = "~/" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ShowLogin", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ShowSearch", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ShowErrorDetails", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableExceptionHandler", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultAlbumDirectoryNameLength", SettingValue = "25" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "SynchAlbumTitleAndDirectoryName", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultAlbumSortMetaName", SettingValue = "111" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultAlbumSortAscending", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailBackgroundColor", SettingValue = "#353535" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailText", SettingValue = "Empty" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailFontName", SettingValue = "Verdana" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailFontSize", SettingValue = "13" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailFontColor", SettingValue = "White" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EmptyAlbumThumbnailWidthToHeightRatio", SettingValue = "1.33" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MaxThumbnailTitleDisplayLength", SettingValue = "50" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowUserEnteredHtml", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowUserEnteredJavascript", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowedHtmlTags", SettingValue = "p,a,div,span,br,ul,ol,li,table,tr,td,th,h1,h2,h3,h4,h5,h6,strong,b,em,i,u,cite,blockquote,address,pre,hr,img,dl,dt,dd,code,tt" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowedHtmlAttributes", SettingValue = "href,class,style,id,src,title,alt,target,name" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowCopyingReadOnlyObjects", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowManageOwnAccount", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowDeleteOwnAccount", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaObjectTransitionType", SettingValue = "Fade" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaObjectTransitionDuration", SettingValue = "0.2" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "SlideshowInterval", SettingValue = "4000" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowUnspecifiedMimeTypes", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ImageTypesStandardBrowsersCanDisplay", SettingValue = ".jpg,.jpeg,.gif,.png" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ImageMagickFileTypes", SettingValue = ".pdf,.txt,.eps,.psd,.tif,.tiff" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowAnonymousRating", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ExtractMetadata", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ExtractMetadataUsingWpf", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MetadataDisplaySettings", SettingValue = "[{'MetadataItem':29,'Name':'Title','DisplayName':'TITLE','IsVisibleForAlbum':true,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{Title}','Sequence':0},{'MetadataItem':41,'Name':'Caption','DisplayName':'CAPTION','IsVisibleForAlbum':true,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{Comment}','Sequence':1},{'MetadataItem':22,'Name':'Tags','DisplayName':'TAGS','IsVisibleForAlbum':true,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{Tags}','Sequence':2},{'MetadataItem':42,'Name':'People','DisplayName':'PEOPLE','IsVisibleForAlbum':true,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{People}','Sequence':3},{'MetadataItem':112,'Name':'HtmlSource','DisplayName':'SOURCE HTML','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{HtmlSource}','Sequence':4},{'MetadataItem':34,'Name':'FileName','DisplayName':'File name','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{FileName}','Sequence':5},{'MetadataItem':35,'Name':'FileNameWithoutExtension','DisplayName':'File name','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{FileNameWithoutExtension}','Sequence':6},{'MetadataItem':111,'Name':'DateAdded','DisplayName':'Date Added','IsVisibleForAlbum':true,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{DateAdded}','Sequence':7},{'MetadataItem':8,'Name':'DatePictureTaken','DisplayName':'Date photo taken','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{DatePictureTaken}','Sequence':8},{'MetadataItem':26,'Name':'Rating','DisplayName':'Rating','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':true,'DefaultValue':'{Rating}','Sequence':9},{'MetadataItem':102,'Name':'GpsLocationWithMapLink','DisplayName':'Geotag','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'<a href=\\'http://maps.google.com/maps?q={GpsLatitude},{GpsLongitude}\\' target=\\'_blank\\' title=\\'View map\\'>{GpsLocation}</a>','Sequence':10},{'MetadataItem':106,'Name':'GpsDestLocationWithMapLink','DisplayName':'Geotag','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'<a href=\\'http://maps.google.com/maps?q={GpsLatitude},{GpsLongitude}\\' target=\\'_blank\\' title=\\'View map\\'>{GpsLocation}</a>','Sequence':11},{'MetadataItem':43,'Name':'Orientation','DisplayName':'Orientation','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Orientation}','Sequence':12},{'MetadataItem':14,'Name':'ExposureProgram','DisplayName':'Exposure program','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{ExposureProgram}','Sequence':13},{'MetadataItem':9,'Name':'Description','DisplayName':'Description','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Description}','Sequence':14},{'MetadataItem':5,'Name':'Comment','DisplayName':'Comment','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{Comment}','Sequence':15},{'MetadataItem':28,'Name':'Subject','DisplayName':'Subject','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Subject}','Sequence':16},{'MetadataItem':2,'Name':'Author','DisplayName':'Author','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Author}','Sequence':17},{'MetadataItem':4,'Name':'CameraModel','DisplayName':'Camera model','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{CameraModel}','Sequence':18},{'MetadataItem':6,'Name':'ColorRepresentation','DisplayName':'Color representation','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{ColorRepresentation}','Sequence':19},{'MetadataItem':7,'Name':'Copyright','DisplayName':'Copyright','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Copyright}','Sequence':20},{'MetadataItem':12,'Name':'EquipmentManufacturer','DisplayName':'Camera maker','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{EquipmentManufacturer}','Sequence':21},{'MetadataItem':13,'Name':'ExposureCompensation','DisplayName':'Exposure compensation','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{ExposureCompensation}','Sequence':22},{'MetadataItem':15,'Name':'ExposureTime','DisplayName':'Exposure time','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{ExposureTime}','Sequence':23},{'MetadataItem':16,'Name':'FlashMode','DisplayName':'Flash mode','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{FlashMode}','Sequence':24},{'MetadataItem':17,'Name':'FNumber','DisplayName':'F-stop','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{FNumber}','Sequence':25},{'MetadataItem':18,'Name':'FocalLength','DisplayName':'Focal length','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{FocalLength}','Sequence':26},{'MetadataItem':21,'Name':'IsoSpeed','DisplayName':'ISO speed','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IsoSpeed}','Sequence':27},{'MetadataItem':23,'Name':'LensAperture','DisplayName':'Aperture','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{LensAperture}','Sequence':28},{'MetadataItem':24,'Name':'LightSource','DisplayName':'Light source','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{LightSource}','Sequence':29},{'MetadataItem':10,'Name':'Dimensions','DisplayName':'Dimensions (pixels)','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{Dimensions}','Sequence':30},{'MetadataItem':25,'Name':'MeteringMode','DisplayName':'Metering mode','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{MeteringMode}','Sequence':31},{'MetadataItem':27,'Name':'SubjectDistance','DisplayName':'Subject distance','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{SubjectDistance}','Sequence':32},{'MetadataItem':11,'Name':'Duration','DisplayName':'Duration','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Duration}','Sequence':33},{'MetadataItem':1,'Name':'AudioFormat','DisplayName':'Audio format','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{AudioFormat}','Sequence':34},{'MetadataItem':32,'Name':'VideoFormat','DisplayName':'Video format','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{VideoFormat}','Sequence':35},{'MetadataItem':3,'Name':'BitRate','DisplayName':'Bit rate','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{BitRate}','Sequence':36},{'MetadataItem':0,'Name':'AudioBitRate','DisplayName':'AudioBitRate','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{AudioBitRate}','Sequence':37},{'MetadataItem':31,'Name':'VideoBitRate','DisplayName':'VideoBitRate','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{VideoBitRate}','Sequence':38},{'MetadataItem':20,'Name':'HorizontalResolution','DisplayName':'Horizontal resolution','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{HorizontalResolution}','Sequence':39},{'MetadataItem':30,'Name':'VerticalResolution','DisplayName':'Vertical resolution','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{VerticalResolution}','Sequence':40},{'MetadataItem':33,'Name':'Width','DisplayName':'Width','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Width}','Sequence':41},{'MetadataItem':19,'Name':'Height','DisplayName':'Height','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Height}','Sequence':42},{'MetadataItem':36,'Name':'FileSizeKb','DisplayName':'File size','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{FileSizeKb}','Sequence':43},{'MetadataItem':37,'Name':'DateFileCreated','DisplayName':'File created','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{DateFileCreated}','Sequence':44},{'MetadataItem':38,'Name':'DateFileCreatedUtc','DisplayName':'File created (UTC)','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{DateFileCreatedUtc}','Sequence':45},{'MetadataItem':39,'Name':'DateFileLastModified','DisplayName':'File last modified','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{DateFileLastModified}','Sequence':46},{'MetadataItem':40,'Name':'DateFileLastModifiedUtc','DisplayName':'File last modified (UTC)','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{DateFileLastModifiedUtc}','Sequence':47},{'MetadataItem':101,'Name':'GpsLocation','DisplayName':'GPS location','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsLocation}','Sequence':48},{'MetadataItem':103,'Name':'GpsLatitude','DisplayName':'GPS latitude','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsLatitude}','Sequence':49},{'MetadataItem':104,'Name':'GpsLongitude','DisplayName':'GPS longitude','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsLongitude}','Sequence':50},{'MetadataItem':105,'Name':'GpsDestLocation','DisplayName':'GPS dest. location','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsDestLocation}','Sequence':51},{'MetadataItem':108,'Name':'GpsDestLongitude','DisplayName':'GPS dest. longitude','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsDestLongitude}','Sequence':52},{'MetadataItem':107,'Name':'GpsDestLatitude','DisplayName':'GPS dest. latitude','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsDestLatitude}','Sequence':53},{'MetadataItem':110,'Name':'GpsVersion','DisplayName':'GPS version','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsVersion}','Sequence':54},{'MetadataItem':109,'Name':'GpsAltitude','DisplayName':'GPS altitude','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'{GpsAltitude}','Sequence':55},{'MetadataItem':113,'Name':'RatingCount','DisplayName':'Number of ratings','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'0','Sequence':56},{'MetadataItem':1012,'Name':'IptcOriginalTransmissionReference','DisplayName':'Transmission ref.','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcOriginalTransmissionReference}','Sequence':57},{'MetadataItem':1013,'Name':'IptcProvinceState','DisplayName':'Province/State','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcProvinceState}','Sequence':58},{'MetadataItem':1010,'Name':'IptcKeywords','DisplayName':'IptcKeywords','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcKeywords}','Sequence':59},{'MetadataItem':1011,'Name':'IptcObjectName','DisplayName':'Object name','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcObjectName}','Sequence':60},{'MetadataItem':1014,'Name':'IptcRecordVersion','DisplayName':'Record version','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcRecordVersion}','Sequence':61},{'MetadataItem':1017,'Name':'IptcSublocation','DisplayName':'Sub-location','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcSublocation}','Sequence':62},{'MetadataItem':1018,'Name':'IptcWriterEditor','DisplayName':'Writer/Editor','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcWriterEditor}','Sequence':63},{'MetadataItem':1015,'Name':'IptcSource','DisplayName':'Source','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcSource}','Sequence':64},{'MetadataItem':1016,'Name':'IptcSpecialInstructions','DisplayName':'Instructions','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcSpecialInstructions}','Sequence':65},{'MetadataItem':1003,'Name':'IptcCaption','DisplayName':'Caption','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcCaption}','Sequence':66},{'MetadataItem':1004,'Name':'IptcCity','DisplayName':'City','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcCity}','Sequence':67},{'MetadataItem':1001,'Name':'IptcByline','DisplayName':'By-line','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcByline}','Sequence':68},{'MetadataItem':1002,'Name':'IptcBylineTitle','DisplayName':'By-line title','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcBylineTitle}','Sequence':69},{'MetadataItem':1005,'Name':'IptcCopyrightNotice','DisplayName':'Copyright','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcCopyrightNotice}','Sequence':70},{'MetadataItem':1008,'Name':'IptcDateCreated','DisplayName':'Date created','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcDateCreated}','Sequence':71},{'MetadataItem':1009,'Name':'IptcHeadline','DisplayName':'Headline','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcHeadline}','Sequence':72},{'MetadataItem':1006,'Name':'IptcCountryPrimaryLocationName','DisplayName':'Country','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcCountryPrimaryLocationName}','Sequence':73},{'MetadataItem':1007,'Name':'IptcCredit','DisplayName':'Credit','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{IptcCredit}','Sequence':74},{'MetadataItem':2000,'Name':'Custom1','DisplayName':'Custom1','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':75},{'MetadataItem':2001,'Name':'Custom2','DisplayName':'Custom2','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':76},{'MetadataItem':2002,'Name':'Custom3','DisplayName':'Custom3','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':77},{'MetadataItem':2003,'Name':'Custom4','DisplayName':'Custom4','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':78},{'MetadataItem':2004,'Name':'Custom5','DisplayName':'Custom5','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':79},{'MetadataItem':2005,'Name':'Custom6','DisplayName':'Custom6','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':80},{'MetadataItem':2006,'Name':'Custom7','DisplayName':'Custom7','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':81},{'MetadataItem':2007,'Name':'Custom8','DisplayName':'Custom8','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':82},{'MetadataItem':2008,'Name':'Custom9','DisplayName':'Custom9','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':83},{'MetadataItem':2009,'Name':'Custom10','DisplayName':'Custom10','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':84},{'MetadataItem':2010,'Name':'Custom11','DisplayName':'Custom11','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':85},{'MetadataItem':2011,'Name':'Custom12','DisplayName':'Custom12','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':86},{'MetadataItem':2012,'Name':'Custom13','DisplayName':'Custom13','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':87},{'MetadataItem':2013,'Name':'Custom14','DisplayName':'Custom14','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':88},{'MetadataItem':2014,'Name':'Custom15','DisplayName':'Custom15','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':89},{'MetadataItem':2015,'Name':'Custom16','DisplayName':'Custom16','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':90},{'MetadataItem':2016,'Name':'Custom17','DisplayName':'Custom17','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':91},{'MetadataItem':2017,'Name':'Custom18','DisplayName':'Custom18','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':92},{'MetadataItem':2018,'Name':'Custom19','DisplayName':'Custom19','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':93},{'MetadataItem':2019,'Name':'Custom20','DisplayName':'Custom20','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':false,'IsEditable':false,'DefaultValue':'','Sequence':94}]" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MetadataDateTimeFormatString", SettingValue = "MMM dd, yyyy h:mm:ss tt" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableMediaObjectDownload", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableAnonymousOriginalMediaObjectDownload", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableGalleryObjectZipDownload", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableAlbumZipDownload", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableSlideShow", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "SlideShowType", SettingValue = "FullScreen" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MaxThumbnailLength", SettingValue = "115" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ThumbnailImageJpegQuality", SettingValue = "70" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ThumbnailFileNamePrefix", SettingValue = "zThumb_" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MaxOptimizedLength", SettingValue = "640" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "OptimizedImageJpegQuality", SettingValue = "70" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "OptimizedImageTriggerSizeKb", SettingValue = "50" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "OptimizedFileNamePrefix", SettingValue = "zOpt_" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "OriginalImageJpegQuality", SettingValue = "95" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DiscardOriginalImageDuringImport", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ApplyWatermark", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "ApplyWatermarkToThumbnails", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkText", SettingValue = "Copyright 2014, Your Company Name, All Rights Reserved" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextFontName", SettingValue = "Verdana" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextFontSize", SettingValue = "13" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextWidthPercent", SettingValue = "50" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextColor", SettingValue = "White" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextOpacityPercent", SettingValue = "35" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkTextLocation", SettingValue = "BottomCenter" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkImagePath", SettingValue = "gs/skins/dark/images/gsp-logo.png" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkImageWidthPercent", SettingValue = "85" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkImageOpacityPercent", SettingValue = "25" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "WatermarkImageLocation", SettingValue = "MiddleCenter" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "SendEmailOnError", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AutoStartMediaObject", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultVideoPlayerWidth", SettingValue = "640" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultVideoPlayerHeight", SettingValue = "480" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultAudioPlayerWidth", SettingValue = "600" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultAudioPlayerHeight", SettingValue = "60" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultGenericObjectWidth", SettingValue = "640" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultGenericObjectHeight", SettingValue = "480" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MaxUploadSize", SettingValue = "2097151" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowAddLocalContent", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowAddExternalContent", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AllowAnonymousBrowsing", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "PageSize", SettingValue = "0" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "PagerLocation", SettingValue = "TopAndBottom" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableSelfRegistration", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "RequireEmailValidationForSelfRegisteredUser", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "RequireApprovalForSelfRegisteredUser", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UseEmailForAccountName", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "DefaultRolesForSelfRegisteredUser", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UsersToNotifyWhenAccountIsCreated", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UsersToNotifyWhenErrorOccurs", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableUserAlbum", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableUserAlbumDefaultForUser", SettingValue = "True" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UserAlbumParentAlbumId", SettingValue = "0" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UserAlbumNameTemplate", SettingValue = "{UserName}'s gallery" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "UserAlbumSummaryTemplate", SettingValue = "Welcome to your personal gallery. You can easily add photos, video, and other files. When you are logged in, an Actions menu appears in the upper left to help you manage your gallery." });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "RedirectToUserAlbumAfterLogin", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "VideoThumbnailPosition", SettingValue = "3" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableAutoSync", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "AutoSyncIntervalMinutes", SettingValue = "1440" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "LastAutoSync", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "EnableRemoteSync", SettingValue = "False" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "RemoteAccessPassword", SettingValue = "" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaEncoderSettings", SettingValue = @".mp3||.mp3||~~.flv||.flv||~~.m4a||.m4a||~~*video||.mp4||-y -i ""{SourceFilePath}"" -vf ""scale=min(iw*min(640/iw\,480/ih)\,iw):min(ih*min(640/iw\,480/ih)\,ih){AutoRotateFilter}"" -vcodec libx264 -movflags +faststart -metadata:s:v:0 rotate=0 ""{DestinationFilePath}""~~*video||.flv||-i ""{SourceFilePath}"" -y ""{DestinationFilePath}""~~*audio||.m4a||-i ""{SourceFilePath}"" -y ""{DestinationFilePath}""" });
				ctx.GallerySettings.AddOrUpdate(a => new { a.FKGalleryId, a.SettingName }, new GallerySettingDto { FKGalleryId = galleryDto.GalleryId, SettingName = "MediaEncoderTimeoutMs", SettingValue = "900000" });
			}
		}

		private static void InsertMetadata(GalleryDb ctx)
		{
			// Insert default set of data for root album.
			var rootAlbumWithMissingTitle = ctx.Albums.FirstOrDefault(a => a.FKAlbumParentId == null && a.Metadata.All(md => md.MetaName != MetadataItemName.Title));

			if (rootAlbumWithMissingTitle != null)
			{
				ctx.Metadatas.Add(new MetadataDto { MetaName = MetadataItemName.Title, FKAlbumId = rootAlbumWithMissingTitle.AlbumId, Value = "ALL ALBUMS" });
			}

			var rootAlbumWithMissingCaption = ctx.Albums.FirstOrDefault(a => a.FKAlbumParentId == null && a.Metadata.All(md => md.MetaName != MetadataItemName.Caption));

			if (rootAlbumWithMissingCaption != null)
			{
				ctx.Metadatas.Add(new MetadataDto { MetaName = MetadataItemName.Caption, FKAlbumId = rootAlbumWithMissingCaption.AlbumId, Value = "Welcome to Gallery Server Pro! <span class='gsp_msgfriendly'>Start by <a href='?g=createaccount' style='color: #7ad199;'>creating an admin account</a>.</span>" });
			}

			//ctx.Metadatas.AddOrUpdate(m => new { m.MetaName, m.Value }, new MetadataDto { MetaName = MetadataItemName.AlbumTitle, FKAlbumId = rootAlbumId, Value = "Welcome to Gallery Server Pro!" });
			//ctx.Metadatas.AddOrUpdate(m => m.MetaName, new MetadataDto { MetaName = MetadataItemName.AlbumTitle, FKAlbumId = rootAlbumId, Value = "Welcome to Gallery Server Pro!" });
		}

		private static void InsertMimeTypes(GalleryDb ctx)
		{
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".3gp", MimeTypeValue = "video/mp4", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".afl", MimeTypeValue = "video/animaflex", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".aif", MimeTypeValue = "audio/aiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".aifc", MimeTypeValue = "audio/aiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".aiff", MimeTypeValue = "audio/aiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".asf", MimeTypeValue = "video/x-ms-asf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".asx", MimeTypeValue = "video/x-ms-asf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".au", MimeTypeValue = "audio/basic", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".avi", MimeTypeValue = "video/x-ms-wvx", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".avs", MimeTypeValue = "video/avs-video", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".bm", MimeTypeValue = "image/bmp", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".bmp", MimeTypeValue = "image/bmp", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".chm", MimeTypeValue = "application/vnd.ms-htmlhelp", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".css", MimeTypeValue = "text/css", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".divx", MimeTypeValue = "video/divx", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dl", MimeTypeValue = "video/dl", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".doc", MimeTypeValue = "application/msword", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".docm", MimeTypeValue = "application/vnd.ms-word.document.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".docx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.wordprocessingml.document", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dotx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.wordprocessingml.template", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dot", MimeTypeValue = "application/msword", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dotm", MimeTypeValue = "application/vnd.ms-word.template.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dtd", MimeTypeValue = "application/xml-dtd", BrowserMimeTypeValue = "text/plain" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dv", MimeTypeValue = "video/x-dv", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dwg", MimeTypeValue = "image/vnd.dwg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".dxf", MimeTypeValue = "image/vnd.dwg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".emf", MimeTypeValue = "image/x-emf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".eps", MimeTypeValue = "image/postscript", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".exe", MimeTypeValue = "application/octet-stream", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".f4v", MimeTypeValue = "video/f4v", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".fif", MimeTypeValue = "image/fif", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".fli", MimeTypeValue = "video/fli", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".flo", MimeTypeValue = "image/florian", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".flv", MimeTypeValue = "video/x-flv", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".fpx", MimeTypeValue = "image/vnd.fpx", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".funk", MimeTypeValue = "audio/make", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".g3", MimeTypeValue = "image/g3fax", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".gif", MimeTypeValue = "image/gif", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".gl", MimeTypeValue = "video/gl", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".htm", MimeTypeValue = "text/html", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".html", MimeTypeValue = "text/html", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ico", MimeTypeValue = "image/ico", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ief", MimeTypeValue = "image/ief", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".iefs", MimeTypeValue = "image/ief", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".it", MimeTypeValue = "audio/it", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jar", MimeTypeValue = "application/java-archive", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jfif", MimeTypeValue = "image/jpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jfif-tbnl", MimeTypeValue = "image/jpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jpe", MimeTypeValue = "image/jpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jpeg", MimeTypeValue = "image/jpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jpg", MimeTypeValue = "image/jpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".js", MimeTypeValue = "text/javascript", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".jut", MimeTypeValue = "image/jutvision", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".kar", MimeTypeValue = "audio/midi", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".la", MimeTypeValue = "audio/nspaudio", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".lma", MimeTypeValue = "audio/nspaudio", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m1v", MimeTypeValue = "video/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m2a", MimeTypeValue = "audio/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m2ts", MimeTypeValue = "video/MP2T", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m2v", MimeTypeValue = "video/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m4a", MimeTypeValue = "audio/m4a", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".m4v", MimeTypeValue = "video/m4v", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mcf", MimeTypeValue = "image/vasa", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mht", MimeTypeValue = "message/rfc822", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mid", MimeTypeValue = "audio/midi", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".midi", MimeTypeValue = "audio/midi", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mod", MimeTypeValue = "audio/mod", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".moov", MimeTypeValue = "video/mp4", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mov", MimeTypeValue = "video/mp4", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mp2", MimeTypeValue = "audio/mpeg", BrowserMimeTypeValue = "application/x-mplayer2" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mp3", MimeTypeValue = "audio/x-mp3", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mp4", MimeTypeValue = "video/mp4", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mpa", MimeTypeValue = "audio/mpeg", BrowserMimeTypeValue = "application/x-mplayer2" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mpe", MimeTypeValue = "video/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mpeg", MimeTypeValue = "video/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mpg", MimeTypeValue = "video/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mpga", MimeTypeValue = "audio/mpeg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".mts", MimeTypeValue = "video/MP2T", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".my", MimeTypeValue = "audio/make", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".nap", MimeTypeValue = "image/naplps", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".naplps", MimeTypeValue = "image/naplps", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".oga", MimeTypeValue = "audio/ogg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ogg", MimeTypeValue = "video/ogg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ogv", MimeTypeValue = "video/ogg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pdf", MimeTypeValue = "application/pdf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pfunk", MimeTypeValue = "audio/make", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pic", MimeTypeValue = "image/pict", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pict", MimeTypeValue = "image/pict", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".png", MimeTypeValue = "image/png", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".potm", MimeTypeValue = "application/vnd.ms-powerpoint.template.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".potx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.presentationml.template", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ppam", MimeTypeValue = "application/vnd.ms-powerpoint.addin.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pps", MimeTypeValue = "application/vnd.ms-powerpoint", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ppsm", MimeTypeValue = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ppsx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.presentationml.slideshow", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ppt", MimeTypeValue = "application/vnd.ms-powerpoint", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pptm", MimeTypeValue = "application/vnd.ms-powerpoint.presentation.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".pptx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.presentationml.presentation", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".psd", MimeTypeValue = "image/psd", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".qcp", MimeTypeValue = "audio/vnd.qcelp", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".qt", MimeTypeValue = "video/mp4", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ra", MimeTypeValue = "audio/x-pn-realaudio", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ram", MimeTypeValue = "audio/x-pn-realaudio", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".ras", MimeTypeValue = "image/cmu-raster", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rast", MimeTypeValue = "image/cmu-raster", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rf", MimeTypeValue = "image/vnd.rn-realflash", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rmi", MimeTypeValue = "audio/mid", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rp", MimeTypeValue = "image/vnd.rn-realpix", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rtf", MimeTypeValue = "application/rtf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".rv", MimeTypeValue = "video/vnd.rn-realvideo", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".sgml", MimeTypeValue = "text/sgml", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".s3m", MimeTypeValue = "audio/s3m", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".snd", MimeTypeValue = "audio/basic", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".svf", MimeTypeValue = "image/vnd.dwg", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".svg", MimeTypeValue = "image/svg+xml", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".swf", MimeTypeValue = "application/x-shockwave-flash", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".tif", MimeTypeValue = "image/tiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".tiff", MimeTypeValue = "image/tiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".tsi", MimeTypeValue = "audio/tsp-audio", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".tsp", MimeTypeValue = "audio/tsplayer", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".turbot", MimeTypeValue = "image/florian", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".txt", MimeTypeValue = "text/plain", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".vdo", MimeTypeValue = "video/vdo", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".viv", MimeTypeValue = "video/vivo", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".vivo", MimeTypeValue = "video/vivo", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".voc", MimeTypeValue = "audio/voc", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".vos", MimeTypeValue = "video/vosaic", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".vox", MimeTypeValue = "audio/voxware", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wax", MimeTypeValue = "audio/x-ms-wax", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wav", MimeTypeValue = "audio/wav", BrowserMimeTypeValue = "application/x-mplayer2" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wbmp", MimeTypeValue = "image/vnd.wap.wbmp", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".webm", MimeTypeValue = "video/webm", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wmf", MimeTypeValue = "image/wmf", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wma", MimeTypeValue = "audio/x-ms-wma", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wmv", MimeTypeValue = "video/x-ms-wmv", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".wvx", MimeTypeValue = "video/x-ms-wvx", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xbap", MimeTypeValue = "application/x-ms-xbap", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xaml", MimeTypeValue = "application/xaml+xml", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xlam", MimeTypeValue = "application/vnd.ms-excel.addin.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xls", MimeTypeValue = "application/vnd.ms-excel", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xlsb", MimeTypeValue = "application/vnd.ms-excel.sheet.binary.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xlsm", MimeTypeValue = "application/vnd.ms-excel.sheet.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xlsx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xltm", MimeTypeValue = "application/vnd.ms-excel.template.macroEnabled.12", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xltx", MimeTypeValue = "application/vnd.openxmlformats-officedocument.spreadsheetml.template", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xif", MimeTypeValue = "image/vnd.xiff", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xml", MimeTypeValue = "text/xml", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".xps", MimeTypeValue = "application/vnd.ms-xpsdocument", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".x-png", MimeTypeValue = "image/png", BrowserMimeTypeValue = "" });
			ctx.MimeTypes.AddOrUpdate(a => a.FileExtension, new MimeTypeDto { FileExtension = ".zip", MimeTypeValue = "application/octet-stream", BrowserMimeTypeValue = "" });

			ctx.SaveChanges();
		}

		private static void InsertMimeTypeGalleries(GalleryDb ctx)
		{
			var galleryId = ctx.Galleries.First(g => !g.IsTemplate).GalleryId;

			foreach (var mimeTypeDto in ctx.MimeTypes)
			{
				ctx.MimeTypeGalleries.AddOrUpdate(a => new { a.FKGalleryId, a.FKMimeTypeId }, new MimeTypeGalleryDto
																																											{
																																												FKGalleryId = galleryId,
																																												FKMimeTypeId = mimeTypeDto.MimeTypeId,
																																												IsEnabled = mimeTypeDto.FileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || mimeTypeDto.FileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
																																											});
			}
		}

		private static void InsertMediaTemplates(GalleryDb ctx)
		{
			// Define the Flash and Silverlight templates. We use these for several media types so let's just define them once here.
			const string flashHtmlTmpl = "<a href='{MediaObjectUrl}' style='display:block;width:{Width}px;height:{Height}px;margin:0 auto;' id='{UniqueId}_player'></a>";

			const string flashScriptTmpl = @"window.{UniqueId}RunFlowPlayer=function(){
 jQuery('#{UniqueId}_player').attr('href',function(){return this.href.replace(/&/g,'%26')});
 flowplayer('{UniqueId}_player',{src:'{GalleryPath}/script/flowplayer-3.2.16.swf',wmode:'opaque'},{clip:{autoPlay:{AutoStartMediaObjectText},scaling:'fit'}})
};

if (window.flowplayer)
 {UniqueId}RunFlowPlayer();
else
 jQuery.getScript('{GalleryPath}/script/flowplayer-3.2.12.min.js',{UniqueId}RunFlowPlayer);";

			const string silverlightAudioSkin = "AudioGray.xaml";
			const string silverlightVideoSkin = "Professional.xaml";
			const string silverlightHtmlTmpl = "<div id=\'{UniqueId}_mp1p\'></div>";

			const string silverlightScriptTmpl = @"var loadScripts=function(files, callback) {
	$.getScript(files.shift(), files.length ? function() { loadScripts(files, callback); } : callback);
};
		
var runSilverlight = function () {
	Sys.UI.Silverlight.Control.createObject('{UniqueId}_mp1p','<object type=\'application/x-silverlight\' id=\'{UniqueId}_mp1\' style=\'height:{Height}px;width:{Width}px;\'><param name=\'Windowless\' value=\'True\' /><a href=\'http://go2.microsoft.com/fwlink/?LinkID=114576&amp;v=1.0\'><img src=\'http://go2.microsoft.com/fwlink/?LinkID=108181\' alt=\'Get Microsoft Silverlight\' style=\'border-width:0;\' /></a></object>');
	Sys.Application.add_init(function() {
		$create(Sys.UI.Silverlight.MediaPlayer, {
			mediaSource: '{MediaObjectUrl}',
			scaleMode: 1,
			source: '{GalleryPath}/xaml/mediaplayer/{{0}}',
			autoPlay: {AutoStartMediaObjectText}
		}, null, null, document.getElementById('{UniqueId}_mp1p'));
	});
	Sys.Application.initialize();
};
		
window.Gsp.msAjaxComponentId='{UniqueId}_mp1';
if ((typeof Sys === 'undefined') || !Sys.UI.Silverlight) {
	var scripts = ['{GalleryPath}/script/MicrosoftAjax.js', '{GalleryPath}/script/SilverlightControl.js', '{GalleryPath}/script/SilverlightMedia.js'];
	loadScripts(scripts, runSilverlight);
} else {
	runSilverlight();
}";

			const string pdfScriptTmplIE = @"// IE and Safari render Adobe Reader iframes on top of jQuery UI dialogs, so add event handler to hide frame while dialog is visible
$('.gsp_mo_share_dlg').on('dialogopen', function() {
 $('#{UniqueId}_frame').css('visibility', 'hidden');
}).on('dialogclose', function() {
$('#{UniqueId}_frame').css('visibility', 'visible');
});";

			const string pdfScriptTmplSafari = @"// IE and Safari render Adobe Reader iframes on top of jQuery UI dialogs, so add event handler to hide frame while dialog is visible
// Safari requires that we clear the iframe src before we can hide it
$('.gsp_mo_share_dlg').on('dialogopen', function() {
 $('#{UniqueId}_frame').attr('src', '').css('visibility', 'hidden');
}).on('dialogclose', function() {
$('#{UniqueId}_frame').attr('src', '{MediaObjectUrl}').css('visibility', 'visible');
});";

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "image/*", BrowserId = "default", HtmlTemplate = "<img src='{MediaObjectUrl}' class='gsp_mo_img' alt='{TitleNoHtml}' title='{TitleNoHtml}' />", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/*", BrowserId = "default", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;' ><param name='autostart' value='{AutoStartMediaObjectInt}' /><param name='controller' value='true' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/*", BrowserId = "ie", HtmlTemplate = "<object classid='clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6' standby='Loading audio...' style='width:{Width}px;height:{Height}px;'><param name='url' value='{MediaObjectUrl}' /><param name='src' value='{MediaObjectUrl}' /><param name='autostart' value='{AutoStartMediaObjectText}' /><param name='showcontrols' value='true' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/ogg", BrowserId = "default", HtmlTemplate = "<audio src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>audio</code> element or the codec of this file. Try another browser or download the file.</p></audio>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/ogg", BrowserId = "ie", HtmlTemplate = "<p>Cannot play: Internet Explorer cannot play Ogg Theora files. Try another browser or download the file.</p>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/wav", BrowserId = "default", HtmlTemplate = "<audio src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>audio</code> element or the codec of this file. Try another browser or download the file.</p></audio>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/wav", BrowserId = "ie", HtmlTemplate = "<object classid='clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6' standby='Loading audio...' style='width:{Width}px;height:{Height}px;'><param name='url' value='{MediaObjectUrl}' /><param name='src' value='{MediaObjectUrl}' /><param name='autostart' value='{AutoStartMediaObjectText}' /><param name='showcontrols' value='true' /></object>", ScriptTemplate = "" });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/x-mp3", BrowserId = "default", HtmlTemplate = "<audio src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>audio</code> element or the codec of this file. Try another browser or download the file.</p></audio>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/x-mp3", BrowserId = "firefox", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightAudioSkin) });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/x-mp3", BrowserId = "ie1to8", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightAudioSkin) });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/m4a", BrowserId = "default", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightAudioSkin) });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/m4a", BrowserId = "chrome", HtmlTemplate = "<audio src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>audio</code> element or the codec of this file. Try another browser or download the file.</p></audio>", ScriptTemplate = "" });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "audio/x-ms-wma", BrowserId = "default", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightAudioSkin) });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/*", BrowserId = "default", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;' ><param name='src' value='{MediaObjectUrl}' /><param name='autostart' value='{AutoStartMediaObjectInt}' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/*", BrowserId = "ie", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;'><param name='src' value='{MediaObjectUrl}' /><param name='autostart' value='{AutoStartMediaObjectText}' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/ogg", BrowserId = "default", HtmlTemplate = "<video src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>video</code> element or the codec of this file. Try another browser or download the file.</p></video>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/ogg", BrowserId = "ie", HtmlTemplate = "<p>Cannot play: Internet Explorer cannot play Ogg Theora files. Try another browser or download the file.</p>", ScriptTemplate = "" });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/x-ms-wmv", BrowserId = "default", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightVideoSkin) });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/mp4", BrowserId = "default", HtmlTemplate = "<video src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>video</code> element or the codec of this file. Try another browser or download the file.</p></video>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/mp4", BrowserId = "ie1to8", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/mp4", BrowserId = "opera", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/m4v", BrowserId = "default", HtmlTemplate = "<video src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>video</code> element or the codec of this file. Try another browser or download the file.</p></video>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/m4v", BrowserId = "ie1to8", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/m4v", BrowserId = "opera", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/x-ms-asf", BrowserId = "default", HtmlTemplate = silverlightHtmlTmpl, ScriptTemplate = silverlightScriptTmpl.Replace("{{0}}", silverlightVideoSkin) });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/divx", BrowserId = "default", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;'><param name='src' value='{MediaObjectUrl}' /><param name='mode' value='full' /><param name='minVersion' value='1.0.0' /><param name='allowContextMenu' value='true' /><param name='autoPlay' value='{AutoStartMediaObjectText}' /><param name='loop' value='false' /><param name='bannerEnabled' value='false' /><param name='bufferingMode' value='auto' /><param name='previewMessage' value='Click to start video' /><param name='previewMessageFontSize' value='24' /><param name='movieTitle' value='{TitleNoHtml}' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/divx", BrowserId = "ie", HtmlTemplate = "<object classid='clsid:67DABFBF-D0AB-41fa-9C46-CC0F21721616' codebase='http://go.divx.com/plugin/DivXBrowserPlugin.cab' style='width:{Width}px;height:{Height}px;'><param name='src' value='{MediaObjectUrl}' /><param name='mode' value='full' /><param name='minVersion' value='1.0.0' /><param name='allowContextMenu' value='true' /><param name='autoPlay' value='{AutoStartMediaObjectText}' /><param name='loop' value='false' /><param name='bannerEnabled' value='false' /><param name='bufferingMode' value='auto' /><param name='previewMessage' value='Click to start video' /><param name='previewMessageFontSize' value='24' /><param name='movieTitle' value='{TitleNoHtml}' /></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/webm", BrowserId = "default", HtmlTemplate = "<video src='{MediaObjectUrl}' controls autobuffer preload {AutoPlay}><p>Cannot play: Your browser does not support the <code>video</code> element or the codec of this file. Try another browser or download the file.</p></video>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/x-shockwave-flash", BrowserId = "default", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;' id='flash_plugin' standby='loading movie...'><param name='movie' value='{MediaObjectUrl}' /><param name='allowScriptAccess' value='sameDomain' /><param name='quality' value='best' /><param name='wmode' value='opaque' /><param name='scale' value='default' /><param name='bgcolor' value='#FFFFFF' /><param name='salign' value='TL' /><param name='FlashVars' value='playerMode=embedded' /><p><strong>Cannot play Flash content</strong> Your browser does not have the Flash plugin or it is disabled. To view the content, install the Macromedia Flash plugin or, if it is already installed, enable it.</p></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/x-shockwave-flash", BrowserId = "ie", HtmlTemplate = "<object type='{MimeType}' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0&quot; id='flash_activex' standby='loading movie...' style='width:{Width}px;height:{Height}px;'><param name='movie' value='{MediaObjectUrl}' /><param name='quality' value='high' /><param name='wmode' value='opaque' /><param name='bgcolor' value='#FFFFFF' /><p><strong>Cannot play Flash content</strong> Your browser does not have the Flash plugin or it is disabled. To view the content, install the Macromedia Flash plugin or, if it is already installed, enable it.</p></object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/x-shockwave-flash", BrowserId = "ie5to9mac", HtmlTemplate = "<object type='{MimeType}' data='{MediaObjectUrl}' style='width:{Width}px;height:{Height}px;' id='flash_plugin' standby='loading movie...'><param name='movie' value='{MediaObjectUrl}' /><param name='allowScriptAccess' value='sameDomain' /><param name='quality' value='best' /><param name='scale' value='default' /><param name='bgcolor' value='#FFFFFF' /><param name='wmode' value='opaque' /><param name='salign' value='TL' /><param name='FlashVars' value='playerMode=embedded' /><strong>Cannot play Flash content</strong> Your browser does not have the Flash plugin or it is disabled. To view the content, install the Macromedia Flash plugin or, if it is already installed, enable it.</object>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/f4v", BrowserId = "default", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "video/x-flv", BrowserId = "default", HtmlTemplate = flashHtmlTmpl, ScriptTemplate = flashScriptTmpl });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/pdf", BrowserId = "default", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge PDF to fit browser window</a></p><iframe id='{UniqueId}_frame' src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;'></iframe>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/pdf", BrowserId = "ie", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge PDF to fit browser window</a></p><iframe id='{UniqueId}_frame' src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;'></iframe>", ScriptTemplate = pdfScriptTmplIE });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/pdf", BrowserId = "safari", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge PDF to fit browser window</a></p><iframe id='{UniqueId}_frame' src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;'></iframe>", ScriptTemplate = pdfScriptTmplSafari });

			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "text/plain", BrowserId = "default", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge file to fit browser window</a></p><iframe src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;background-color:#fff;'></iframe>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "text/html", BrowserId = "default", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge file to fit browser window</a></p><iframe src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;background-color:#fff;'></iframe>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document", BrowserId = "default", HtmlTemplate = "<p style='margin-bottom:5em;'><a href='{MediaObjectUrl}&sa=1' title='Download {TitleNoHtml}'>Download {TitleNoHtml}</a></p>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "application/msword", BrowserId = "default", HtmlTemplate = "<p style='margin-bottom:5em;'><a href='{MediaObjectUrl}&sa=1' title='Download {TitleNoHtml}'>Download {TitleNoHtml}</a></p>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "message/rfc822", BrowserId = "default", HtmlTemplate = "<p class='gsp_msgfriendly'>This browser cannot display web archive files (.mht). <a href='{MediaObjectUrl}&sa=1' title='Download {TitleNoHtml}'>Download {TitleNoHtml}</a></p>", ScriptTemplate = "" });
			ctx.MediaTemplates.AddOrUpdate(a => new { a.MimeType, a.BrowserId }, new MediaTemplateDto { MimeType = "message/rfc822", BrowserId = "ie", HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge to fit browser window</a></p><iframe src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;background-color:#fff;'></iframe>", ScriptTemplate = "" });
		}

		private static void InsertUiTemplates(GalleryDb ctx)
		{
			var galleryId = ctx.Galleries.Single(g => g.IsTemplate).GalleryId;

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.Album,
																																												Name = DefaultTmplName,
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = @"<div class='gsp_abm_sum'>
 <div class='gsp_abm_sum_col2'>
	<p class='gsp_abm_sum_col2_row1'>({{:Album.NumGalleryItems}}{{:Resource.AbmNumObjSuffix}})</p>
	<div class='gsp_abm_sum_col2_row2'>
	 {{if Settings.ShowSlideShowButton}}
	 <a class='gsp_abm_sum_ss_trigger gsp_abm_sum_btn' href='#'>
		<img src='{{:App.SkinPath}}/images/play-ss-m.png' title='{{:Resource.MoTbSsStart}}' alt=''>
	 </a>
	 {{/if}}
	 {{if Settings.ShowUrlsButton}}
	 <a class='gsp_abm_sum_sa_trigger gsp_abm_sum_btn' href='#'>
		<img src='{{:App.SkinPath}}/images/link-m.png' title='{{:Resource.AbmShareAlbum}}' alt=''>
	 </a>
	 {{/if}}
	 {{if Settings.AllowZipDownload}}
	 <a class='gsp_abm_sum_DownloadZip gsp_abm_sum_btn' href='{{: ~getDownloadUrl(Album.Id) }}'>
		<img src='{{:App.SkinPath}}/images/download-zip-m.png' title='{{:Resource.AbmDwnldZip}}' alt=''>
	 </a>
	 {{/if}}
	 <span>
		<button class='gsp_abm_sum_rs'>{{:Resource.AbmRvsSortTt}}</button>
		<button class='gsp_btn_sb'>{{:Resource.AbmSortbyTt}}</button>
	 </span>
	 <ul class='gsp_abm_sum_sbi'>
		<li class='gsp_abm_sum_sbi_hdr'>{{:Resource.AbmSortbyTt}}</li>
{{if Album.VirtualType == 1 && Album.Permissions.EditAlbum}}<li><a href='#' data-id='-2147483648'>{{:Resource.AbmSortbyCustom}}</a></li>{{/if}}
		<li><a href='#' data-id='8'>{{:Resource.AbmSortbyDatePictureTaken}}</a></li>
		<li><a href='#' data-id='111'>{{:Resource.AbmSortbyDateAdded}}</a></li>
		<li><a href='#' data-id='26'>{{:Resource.AbmSortbyRating}}</a></li>
		<li><a href='#' data-id='29'>{{:Resource.AbmSortbyTitle}}</a></li>
		<li><a href='#' data-id='34'>{{:Resource.AbmSortbyFilename}}</a></li>
	 </ul>
	</div>
 </div>
 <p class='gsp_abm_sum_col1_row1'>
{{if Album.VirtualType == 1 && Album.Permissions.EditAlbum}}
	<a class='gsp_abm_sum_pvt_trigger gsp_abm_sum_btn' href='#'>
		<img src='{{:App.SkinPath}}/images/lock-{{if Album.IsPrivate || !Settings.AllowAnonBrowsing}}active-{{/if}}s.png' title='{{if !Settings.AllowAnonBrowsing}}{{:Resource.AbmAnonDisabledTt}}{{else}}{{if Album.IsPrivate}}{{:Resource.AbmIsPvtTt}}{{else}}{{:Resource.AbmNotPvtTt}}{{/if}}{{/if}}' alt=''>
	 </a>
{{/if}}
{{if Album.VirtualType == 1 && Album.Permissions.AdministerGallery}}
	<a class='gsp_abm_sum_ownr_trigger gsp_abm_sum_btn' href='#'>
		<img src='{{:App.SkinPath}}/images/user-s.png' title='{{:Resource.AbmOwnrTt}}' alt=''>
	 </a>
{{/if}}
{{if Album.RssUrl}}
	<a class='gsp_abm_sum_btn' href='{{:Album.RssUrl}}'>
		<img src='{{:App.SkinPath}}/images/rss-s.png' title='{{:Resource.AbmRssTt}}' alt=''>
	</a>
{{/if}}
	<span class='gsp_abm_sum_col1_row1_hdr'>{{:Resource.AbmPfx}}</span>
	<span class='gsp_abm_sum_col1_row1_dtl'>{{:Album.Title}}</span>
 </p>
 <div class='gsp_abm_sum_col1_row2'>
	<span class='gsp_abm_sum_col1_row2_hdr'></span>
	<span class='gsp_abm_sum_col1_row2_dtl'>{{:Album.Caption}}</span>
 </div>
</div>

{{if Album.GalleryItems.length == 0}}
 <p class='gsp_abm_noobj'>{{:Resource.AbmNoObj}} {{if Album.VirtualType == 1 && Album.Permissions.AddMediaObject}}<a href='{{: ~getAddUrl(#data) }}'>{{:Resource.AbmAddObj}}</a>{{/if}}</p>
{{/if}}

<ul class='gsp_floatcontainer gsp_abm_thmbs'>
 {{for Album.GalleryItems}}
 <li class='thmb{{if IsAlbum}} album{{/if}}' data-id='{{:Id}}' data-it='{{:ItemType}}' style='width:{{:Views[ViewIndex].Width + 40}}px;'>
	<a class='gsp_thmbLink' href='{{: ~getGalleryItemUrl(#data, !IsAlbum) }}'>
	 <img class='gsp_thmb_img' style='width:{{:Views[ViewIndex].Width}}px;height:{{:Views[ViewIndex].Height}}px;' src='{{:Views[ViewIndex].Url}}'>
	</a>
	<p class='gsp_go_t' title='{{stripHtml:Title}}'>{{stripHtmlAndTruncate:Title}}</p>
 </li>
 {{/for}}
</ul>

<div class='gsp_abm_sum_share_dlg gsp_dlg'>
 <p class='gsp_abm_sum_share_dlg_t'>{{:Resource.AbmShareAlbum}}</p>
 <p class='gsp_abm_sum_share_dlg_s'>{{:Resource.AbmLinkToAlbum}}</p>
 <p><input type='text' class='gsp_abm_sum_share_dlg_ipt' value='{{: ~getAlbumUrl(Album.Id, true) }}' /></p>
</div>

<div class='gsp_abm_sum_ownr_dlg gsp_dlg'>
 <p class='gsp_abm_sum_ownr_dlg_t'>{{:Resource.AbmOwnr}}</p>
 <p class='gsp_abm_sum_ownr_dlg_s'>{{:Resource.AbmOwnrDtl}}</p>
{{if Album.Permissions.AdministerGallery}}
 <p class='gsp_abm_sum_ownr_dlg_o'><span>{{:Resource.AbmOwnrLbl}}</span> <input type='text' class='gsp_abm_sum_ownr_dlg_ipt' value='{{:Album.Owner}}' /></p>
{{/if}}
{{if Album.InheritedOwners}}
 <p class='gsp_abm_sum_ownr_dlg_io'>{{:Resource.AbmOwnrInhtd}} {{:Album.InheritedOwners}}</p>
{{/if}}
</div>",
																																												ScriptTemplate = @"// Call the gspThumbnails plug-in, which adds the HTML to the page and then configures it
$('#{{:Settings.ThumbnailClientId}}').gspThumbnails('{{:Settings.ThumbnailTmplName}}', window.{{:Settings.ClientId}}.gspData);"
																																											});

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.MediaObject,
																																												Name = DefaultTmplName,
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = GetMediaObjectHtmlTmpl(false),
																																												ScriptTemplate = GetMediaObjectJsTmpl(false)
																																											});

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.Header,
																																												Name = DefaultTmplName,
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = GetHeaderHtmlTmpl(false),
																																												ScriptTemplate = GetHeaderJsTmpl(false)
																																											});

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.LeftPane,
																																												Name = DefaultTmplName,
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = GetLeftPaneHtmlTmpl(false, false),
																																												ScriptTemplate = GetLeftPaneJsTmpl(false, false)
																																											});

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.RightPane,
																																												Name = DefaultTmplName,
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = GetRightPaneHtmlTmpl(false, false),
																																												ScriptTemplate = GetRightPaneJsTmpl(false, false)
																																											});

			ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
																																											{
																																												TemplateType = UiTemplateType.Album,
																																												Name = "List View",
																																												FKGalleryId = galleryId,
																																												Description = "",
																																												HtmlTemplate = @"<table>
<thead style='font-weight:bold;'><td>Title</td><td>Type</td><thead>
{{for Album.GalleryItems}}
<tr>
<td>
 <p><a title='{{:Title}}' href='{{:#parent.parent.data.App.CurrentPageUrl}}{{if IsAlbum}}?aid={{:Id}}'>{{:#parent.parent.parent.data.Resource.AbmPfx}} {{else}}?moid={{:Id}}'>{{/if}}{{:Title}}</a></p>
</td>
<td><p>{{getItemTypeDesc:ItemType}}</p></td>
</tr>
{{/for}}
</table>",
																																												ScriptTemplate = "$('#{{:Settings.ThumbnailClientId}}').html($.render [ '{{:Settings.ThumbnailTmplName}}' ](window.{{:Settings.ClientId}}.gspData));"
																																											});

			ctx.SaveChanges(); // Save so we can reference these records in InsertJQueryTemplateAlbums()
		}

		private static void InsertUiTemplateAlbums(GalleryDb ctx)
		{
			// Don't do anything. At this point the only UI templates that have been created are for the template gallery. Later, in
			// Gallery.Configure, there is validation code that makes sure both the UI templates and the template/album relationships
			// have been created for each gallery.
		}

		/// <summary>
		/// Wrapper for SaveChanges adding the Validation Messages to the generated exception
		/// </summary>
		/// <param name="context">The context.</param>
		private static void SaveChanges(DbContext context)
		{
			try
			{
				context.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				StringBuilder sb = new StringBuilder();

				foreach (var failure in ex.EntityValidationErrors)
				{
					sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

					foreach (var error in failure.ValidationErrors)
					{
						sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
						sb.AppendLine();
					}
				}

				throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); //addthe original exception as the innerException
			}
		}

		/// <summary>
		/// Adds or updates the left pane templates available to Enterprise users.
		/// </summary>
		private static void InsertLeftPaneEnterpriseTemplates()
		{
			using (var ctx = new GalleryDb())
			{
				var galleryId = ctx.Galleries.Single(g => g.IsTemplate).GalleryId;

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
					{
						TemplateType = UiTemplateType.LeftPane,
						Name = "Default with Tag and People Trees",
						FKGalleryId = galleryId,
						Description = "",
						HtmlTemplate = GetLeftPaneHtmlTmpl(true, false),
						ScriptTemplate = GetLeftPaneJsTmpl(true, false)
					});

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
					{
						TemplateType = UiTemplateType.LeftPane,
						Name = "Default with Tag and People Clouds",
						FKGalleryId = galleryId,
						Description = "",
						HtmlTemplate = GetLeftPaneHtmlTmpl(false, true),
						ScriptTemplate = GetLeftPaneJsTmpl(false, true)
					});

				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// Adds or updates the media object Facebook Comments widget and the right pane Facebook Like widget.
		/// </summary>
		private static void InsertFacebookTemplates()
		{
			using (var ctx = new GalleryDb())
			{
				var galleryId = ctx.Galleries.Single(g => g.IsTemplate).GalleryId;

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
				{
					TemplateType = UiTemplateType.MediaObject,
					Name = "Default with Facebook Comments Widget",
					FKGalleryId = galleryId,
					Description = "",
					HtmlTemplate = GetMediaObjectHtmlTmpl(true),
					ScriptTemplate = GetMediaObjectJsTmpl(true)
				});

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
				{
					TemplateType = UiTemplateType.RightPane,
					Name = "Default with Facebook Like Widget",
					FKGalleryId = galleryId,
					Description = "",
					HtmlTemplate = GetRightPaneHtmlTmpl(false, true),
					ScriptTemplate = GetRightPaneJsTmpl(false, true)
				});

				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// Adds or updates the PayPal 'view cart' and 'add to cart' widgets.
		/// </summary>
		private static void InsertPayPalTemplates()
		{
			using (var ctx = new GalleryDb())
			{
				var galleryId = ctx.Galleries.Single(g => g.IsTemplate).GalleryId;

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
				{
					TemplateType = UiTemplateType.Header,
					Name = "Default with PayPal View Cart Widget",
					FKGalleryId = galleryId,
					Description = "",
					HtmlTemplate = GetHeaderHtmlTmpl(true),
					ScriptTemplate = GetHeaderJsTmpl(true)
				});

				ctx.UiTemplates.AddOrUpdate(a => new { a.TemplateType, a.FKGalleryId, a.Name }, new UiTemplateDto
				{
					TemplateType = UiTemplateType.RightPane,
					Name = "Default with PayPal Add To Cart Widget",
					FKGalleryId = galleryId,
					Description = "",
					HtmlTemplate = GetRightPaneHtmlTmpl(true, false), 
					ScriptTemplate = GetRightPaneJsTmpl(true, false)
				});

				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// Gets the default HTML template for the header UI template, optionally including HTML to support the PayPal 'view cart' widget.
		/// </summary>
		/// <param name="includePayPalCartWidget">if set to <c>true</c> include HTML to support the PayPal 'view cart' widget.</param>
		/// <returns>System.String.</returns>
		private static string GetHeaderHtmlTmpl(bool includePayPalCartWidget)
		{
			// Note that this snippet is configured for payment to Gallery Server Pro. The end user will need to replace with
			// their own PayPal HTML snippet.
			const string payPalCart = @"<input type='hidden' name='cmd' value='_s-xclick'>
<input type='hidden' name='encrypted' value='-----BEGIN PKCS7-----MIIG1QYJKoZIhvcNAQcEoIIGxjCCBsICAQExggEwMIIBLAIBADCBlDCBjjELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAkNBMRYwFAYDVQQHEw1Nb3VudGFpbiBWaWV3MRQwEgYDVQQKEwtQYXlQYWwgSW5jLjETMBEGA1UECxQKbGl2ZV9jZXJ0czERMA8GA1UEAxQIbGl2ZV9hcGkxHDAaBgkqhkiG9w0BCQEWDXJlQHBheXBhbC5jb20CAQAwDQYJKoZIhvcNAQEBBQAEgYCAgSOJPKlhbi65uXcxqm/144dltnmM3C/x/0OElzcUpMG1Lys8kY0rudkxmi1ZdVcoBflXcZDYdrXekZ19bsyMW6aeFDed4q5U1YyHo6GQtUJm0p7j00AutbeHoUXh6uWWVYRXQe6ceH3m2hfGP45qRuI3rtnLpYnKxX/u8Ht1TzELMAkGBSsOAwIaBQAwUwYJKoZIhvcNAQcBMBQGCCqGSIb3DQMHBAjH1GlAoHdKVYAwJ8oK/d1S5ff6h2l3g0Ah9dNHb7ZlFLRzdVZ7x3z0mH8QJof86n6gzzfI3EO9ygmLoIIDhzCCA4MwggLsoAMCAQICAQAwDQYJKoZIhvcNAQEFBQAwgY4xCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJDQTEWMBQGA1UEBxMNTW91bnRhaW4gVmlldzEUMBIGA1UEChMLUGF5UGFsIEluYy4xEzARBgNVBAsUCmxpdmVfY2VydHMxETAPBgNVBAMUCGxpdmVfYXBpMRwwGgYJKoZIhvcNAQkBFg1yZUBwYXlwYWwuY29tMB4XDTA0MDIxMzEwMTMxNVoXDTM1MDIxMzEwMTMxNVowgY4xCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJDQTEWMBQGA1UEBxMNTW91bnRhaW4gVmlldzEUMBIGA1UEChMLUGF5UGFsIEluYy4xEzARBgNVBAsUCmxpdmVfY2VydHMxETAPBgNVBAMUCGxpdmVfYXBpMRwwGgYJKoZIhvcNAQkBFg1yZUBwYXlwYWwuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDBR07d/ETMS1ycjtkpkvjXZe9k+6CieLuLsPumsJ7QC1odNz3sJiCbs2wC0nLE0uLGaEtXynIgRqIddYCHx88pb5HTXv4SZeuv0Rqq4+axW9PLAAATU8w04qqjaSXgbGLP3NmohqM6bV9kZZwZLR/klDaQGo1u9uDb9lr4Yn+rBQIDAQABo4HuMIHrMB0GA1UdDgQWBBSWn3y7xm8XvVk/UtcKG+wQ1mSUazCBuwYDVR0jBIGzMIGwgBSWn3y7xm8XvVk/UtcKG+wQ1mSUa6GBlKSBkTCBjjELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAkNBMRYwFAYDVQQHEw1Nb3VudGFpbiBWaWV3MRQwEgYDVQQKEwtQYXlQYWwgSW5jLjETMBEGA1UECxQKbGl2ZV9jZXJ0czERMA8GA1UEAxQIbGl2ZV9hcGkxHDAaBgkqhkiG9w0BCQEWDXJlQHBheXBhbC5jb22CAQAwDAYDVR0TBAUwAwEB/zANBgkqhkiG9w0BAQUFAAOBgQCBXzpWmoBa5e9fo6ujionW1hUhPkOBakTr3YCDjbYfvJEiv/2P+IobhOGJr85+XHhN0v4gUkEDI8r2/rNk1m0GA8HKddvTjyGw/XqXa+LSTlDYkqI8OwR8GEYj4efEtcRpRYBxV8KxAW93YDWzFGvruKnnLbDAF6VR5w/cCMn5hzGCAZowggGWAgEBMIGUMIGOMQswCQYDVQQGEwJVUzELMAkGA1UECBMCQ0ExFjAUBgNVBAcTDU1vdW50YWluIFZpZXcxFDASBgNVBAoTC1BheVBhbCBJbmMuMRMwEQYDVQQLFApsaXZlX2NlcnRzMREwDwYDVQQDFAhsaXZlX2FwaTEcMBoGCSqGSIb3DQEJARYNcmVAcGF5cGFsLmNvbQIBADAJBgUrDgMCGgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkqhkiG9w0BCQUxDxcNMTMwMzI3MTk1MjAxWjAjBgkqhkiG9w0BCQQxFgQU1YXTC9Dqu21RZMCKhDX9ztZBwGIwDQYJKoZIhvcNAQEBBQAEgYAhY2gahJQiGyuGZrUb4KN282BuKkz6ex3ArCJvtjgADiYIC7uOnnRR6UbrW9ET83dSHqufueE1Bs9bw2Ccvb+KtBcL6WVI0Ml5F2SDM7rKCtcXk7ccclnvPfHDwqfzJWZQcy9NJYDf5jsh1/+ht1dFjgHJ+1SLDnBCCMdcZVQYAA==-----END PKCS7-----
	'>
<input id='{{:Settings.ClientId}}_viewCart' type='image' src='https://www.paypalobjects.com/en_US/i/btn/btn_viewcart_LG.gif' border='0' name='btnPayPal' alt='PayPal - The safer, easier way to pay online!' style='float:right;margin-top:5px'>
<img alt='' border='0' src='https://www.paypalobjects.com/en_US/i/scr/pixel.gif' width='1' height='1'>";

			return HeaderHtmlTmpl.Replace("{PayPalCartWidget}", (includePayPalCartWidget ? payPalCart : String.Empty));
		}

		/// <summary>
		/// Gets the default JavaScript template for the header UI template, optionally including script to support the PayPal 'view cart' widget.
		/// </summary>
		/// <param name="includePayPalViewCartWidget">if set to <c>true</c> include script to support the PayPal 'view cart' widget.</param>
		/// <returns>System.String.</returns>
		private static string GetHeaderJsTmpl(bool includePayPalViewCartWidget)
		{
			const string payPalCartJs = @"
$('#{{:Settings.ClientId}}_viewCart').click(function() {
 var f = $('form')[0];
 $('input[name=hosted_button_id]', f).remove(); // Needed to prevent conflict with add to cart widget
 f.action = 'https://www.paypal.com/cgi-bin/webscr';
 f.submit();
 return false;
});";

			return HeaderJsTmpl.Replace("{PayPalCartJs}", (includePayPalViewCartWidget ? payPalCartJs : String.Empty));
		}

		/// <summary>
		/// Gets the default HTML template for the media object UI template, optionally including HTML to support the Facebook Comment widget.
		/// </summary>
		/// <param name="includeFacebookCommentWidget">if set to <c>true</c> HTML to support the Facebook Comment widget is included.</param>
		/// <returns>System.String.</returns>
		private static string GetMediaObjectHtmlTmpl(bool includeFacebookCommentWidget)
		{
			var facebookCommentWidget = (includeFacebookCommentWidget ? "<div class='fb-comments' data-href='{{:App.HostUrl}}{{:App.CurrentPageUrl}}?moid={{:MediaItem.Id}}' data-width='470' data-num-posts='10' data-colorscheme='dark'></div>" : String.Empty);

			return MediaObjectHtmlTmpl.Replace("{FacebookCommentWidget}", facebookCommentWidget);
		}

		/// <summary>
		/// Gets the default JavaScript template for the media object UI template, optionally including script to support the Facebook API.
		/// </summary>
		/// <param name="includeFacebookJs">if set to <c>true</c> the JavaScript required to invoke the Facebook API is included.</param>
		/// <returns>System.String.</returns>
		private static string GetMediaObjectJsTmpl(bool includeFacebookJs)
		{
			return MediaObjectJsTmpl.Replace("{FacebookJs}", (includeFacebookJs ? FacebookJs : String.Empty));
		}

		/// <summary>
		/// Gets the default HTML template for the left pane UI template, optionally including HTML to support the tag trees and
		/// tag clouds.
		/// </summary>
		/// <param name="includeTagTrees">if set to <c>true</c> HTML to support the tag trees is included.</param>
		/// <param name="includeTagClouds">if set to <c>true</c> HTML to support the tag clouds is included.</param>
		/// <returns>System.String.</returns>
		private static string GetLeftPaneHtmlTmpl(bool includeTagTrees, bool includeTagClouds)
		{
			const string tagTrees = @"
<div id='{{:Settings.ClientId}}_lptagtv' class='gsp_lptagtv gsp_wait'></div>
<div id='{{:Settings.ClientId}}_lppeopletv' class='gsp_lppeopletv gsp_wait'></div>";

			const string tagClouds = @"
<p class='gsp_msgfriendly gsp_addtopmargin10 gsp_addleftmargin4'>{{:Resource.LpTags}}</p>
<div id='{{:Settings.ClientId}}_lptagcloud' class='gsp_lptagcloud gsp_wait'></div>

<p class='gsp_msgfriendly gsp_addtopmargin10 gsp_addleftmargin4'>{{:Resource.LpPeople}}</p>
<div id='{{:Settings.ClientId}}_lppeoplecloud' class='gsp_lppeoplecloud gsp_wait'></div>";

			return LeftPaneHtmlTmpl
				.Replace("{TagTrees}", (includeTagTrees ? tagTrees : String.Empty))
				.Replace("{TagClouds}", (includeTagClouds ? tagClouds : String.Empty));
		}

		/// <summary>
		/// Gets the default JavaScript template for the left pane UI template, optionally including script to support the tag trees and
		/// tag clouds.
		/// </summary>
		/// <param name="includeTagTrees">if set to <c>true</c> JavaScript to support the tag trees is included.</param>
		/// <param name="includeTagClouds">if set to <c>true</c> JavaScript to support the tag clouds is included.</param>
		/// <returns>System.String.</returns>
		private static string GetLeftPaneJsTmpl(bool includeTagTrees, bool includeTagClouds)
		{
			const string tagTrees = @"
var appUrl = window.{{:Settings.ClientId}}.gspData.App.AppUrl;
var galleryId = window.{{:Settings.ClientId}}.gspData.Album.GalleryId;

var tagTreeOptions = {
 clientId: '{{:Settings.ClientId}}',
 albumIdsToSelect : [window.Gsp.GetQSParm('tag')],
 treeDataUrl: appUrl  + '/api/meta/gettagtreeasjson?galleryId=' + galleryId + '&top=10&sortBy=count&sortAscending=false&expanded=false'
};

var peopleTreeOptions = {
 clientId: '{{:Settings.ClientId}}',
 albumIdsToSelect : [window.Gsp.GetQSParm('people')],
 treeDataUrl: appUrl + '/api/meta/getpeopletreeasjson?galleryId=' + galleryId + '&top=10&sortBy=count&sortAscending=false&expanded=false'
};

$('#{{:Settings.ClientId}}_lptagtv').gspTreeView(null, tagTreeOptions);
$('#{{:Settings.ClientId}}_lppeopletv').gspTreeView(null, peopleTreeOptions );

$('#{{:Settings.ClientId}}_lptagtv,#{{:Settings.ClientId}}_lppeopletv').on('select_node.jstree', function (e, data) { 
 data.instance.toggle_node(data.node); 
})";

			const string tagClouds = @"
var appUrl = window.{{:Settings.ClientId}}.gspData.App.AppUrl;
var galleryId = window.{{:Settings.ClientId}}.gspData.Album.GalleryId;

var tagCloudOptions = {
 clientId: '{{:Settings.ClientId}}',
 tagCloudType: 'tag',
 tagCloudUrl: appUrl  + '/api/meta/tags?q=&galleryId=' + galleryId + '&top=20&sortBy=count&sortAscending=false'
}

var peopleCloudOptions = {
 clientId: '{{:Settings.ClientId}}',
 tagCloudType: 'people',
 tagCloudUrl: appUrl  + '/api/meta/people?q=&galleryId=' + galleryId + '&top=10&sortBy=count&sortAscending=false'
}

$('#{{:Settings.ClientId}}_lptagcloud').gspTagCloud(null, tagCloudOptions);
$('#{{:Settings.ClientId}}_lppeoplecloud').gspTagCloud(null, peopleCloudOptions );";

			return LeftPaneJsTmpl
				.Replace("{TagTrees}", (includeTagTrees ? tagTrees : String.Empty))
				.Replace("{TagClouds}", (includeTagClouds ? tagClouds : String.Empty));
		}

		/// <summary>
		/// Gets the default HTML template for the right pane UI template, optionally including HTML to support the PayPal 'add to cart' and
		/// Facebook Like widgets.
		/// </summary>
		/// <param name="includePayPalAddToCartWidget">if set to <c>true</c> HTML to support the PayPal 'add to cart' widget is included.</param>
		/// <param name="includeFacebookLikeWidget">if set to <c>true</c> HTML to support the Facebook Like widget is included.</param>
		/// <returns>System.String.</returns>
		private static string GetRightPaneHtmlTmpl(bool includePayPalAddToCartWidget, bool includeFacebookLikeWidget)
		{
			const string payPalAddToCart = @"
{{if MediaItem != null}}
<input type='hidden' name='cmd' value='_s-xclick'>
<input type='hidden' name='hosted_button_id' value='JP2UFSSRLBSM8'>
<input type='hidden' name='item_name' value='Photograph - {{:MediaItem.Title}} (Item # {{:MediaItem.Id}})'>
<input id='{{:Settings.ClientId}}_addToCart' type='image' src='https://www.paypalobjects.com/en_US/i/btn/btn_cart_LG.gif' border='0' name='addToCart' alt='PayPal - The safer, easier way to pay online!' style='padding:5px;'>
<span style='display:inline-block;vertical-align:top;margin-top:10px;'>$1.00</span>
<img alt='' border='0' src='https://www.paypalobjects.com/en_US/i/scr/pixel.gif' width='1' height='1'>
{{/if}}";

			const string facebookLike = @"
{{if MediaItem != null}}
<iframe src='//www.facebook.com/plugins/like.php?href={{:App.HostUrl}}{{:App.CurrentPageUrl}}?moid={{:MediaItem.Id}}&amp;width=450&amp;colorscheme=dark&amp;height=80' scrolling='no' frameborder='0' style='border:none; overflow:hidden; width:400px; height:27px;display:block;margin:5px 0 0 5px;' allowTransparency='true'></iframe>
{{/if}}";

			return RightPaneHtmlTmpl
				.Replace("{PayPalAddToCartWidget}", (includePayPalAddToCartWidget ? payPalAddToCart : String.Empty))
				.Replace("{FacebookLikeWidget}", (includeFacebookLikeWidget ? facebookLike : String.Empty));
		}

		/// <summary>
		/// Gets the default JavaScript template for the right pane UI template, optionally including script to support the PayPal 'add to cart' and
		/// Facebook Like widgets.
		/// </summary>
		/// <param name="includePayPalAddToCartJs">if set to <c>true</c> JavaScript to support the PayPal 'add to cart' widget is included.</param>
		/// <param name="includeFacebookJs">if set to <c>true</c> JavaScript to support the Facebook API is included.</param>
		/// <returns>System.String.</returns>
		private static string GetRightPaneJsTmpl(bool includePayPalAddToCartJs, bool includeFacebookJs)
		{
			const string payPalAddToCartJs = @"
var bindAddToCartEvent = function() {
 $('#{{:Settings.ClientId}}_addToCart').click(function() {
	var f = $('form')[0];
	f.action = 'https://www.paypal.com/cgi-bin/webscr';
	f.submit();
	return false;
 });
};

$('#{{:Settings.MediaClientId}}').on('next.{{:Settings.ClientId}} previous.{{:Settings.ClientId}}', function() {
 bindAddToCartEvent();
});

bindAddToCartEvent();";

			return RightPaneJsTmpl
				.Replace("{PayPalAddToCartJs}", (includePayPalAddToCartJs ? payPalAddToCartJs : String.Empty))
				.Replace("{FacebookJs}", (includeFacebookJs ? FacebookJs : String.Empty));
		}

		#endregion
	}
}
