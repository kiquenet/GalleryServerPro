using GalleryServerPro.Business;
using GalleryServerPro.Business.Metadata;
using System;
using System.Globalization;
using System.Linq;

namespace GalleryServerPro.Data
{
	/// <summary>
	/// Handle the migration of data changes during updates to newer versions. 
	/// </summary>
	public static class MigrateController
	{
		#region Methods

		/// <summary>
		/// Update database values as required for the current version. Typically this is used to apply bug fixes
		/// that require updates to database settings (such as media and UI templates).
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		/// <remarks>This function detects the current app schema version as defined in the AppSetting table and applies
		/// all relevant updates to bring it up to the current version. By the time this method exits, the app schema version
		/// in the AppSetting table will match the current schema version as defined in <see cref="GalleryDb.DataSchemaVersion" />.
		/// </remarks>
		/// <exception cref="System.Exception"></exception>
		public static void ApplyDbUpdates(GalleryDb ctx)
		{
			if (!ctx.AppSettings.Any())
			{
				SeedController.InsertSeedData(ctx);
			}

			var curSchema = GetCurrentSchema(ctx);

			while (curSchema < GalleryDb.DataSchemaVersion)
			{
				var oldSchema = curSchema;

				switch (curSchema)
				{
					case GalleryDataSchemaVersion.V3_0_0: UpgradeTo301(ctx); break;
					case GalleryDataSchemaVersion.V3_0_1: UpgradeTo302(ctx); break;
					case GalleryDataSchemaVersion.V3_0_2: UpgradeTo303(ctx); break;
					case GalleryDataSchemaVersion.V3_0_3: UpgradeTo310(ctx); break;
					case GalleryDataSchemaVersion.V3_1_0: UpgradeTo320(ctx); break;
					case GalleryDataSchemaVersion.V3_2_0: UpgradeTo321(ctx); break;
				}

				curSchema = GetCurrentSchema(ctx);

				if (curSchema == oldSchema)
				{
					throw new Exception(String.Format("The migration function for schema {0} should have incremented the data schema version in the AppSetting table, but it did not.", curSchema));
				}
			}
		}

		#endregion

		#region Functions

		/// <summary>
		/// Returns the current data schema version as defined in the AppSetting table.
		/// </summary>
		/// <returns>An instance of <see cref="GalleryDataSchemaVersion" /> indicating the current data schema version
		/// as defined in the AppSetting table.</returns>
		private static GalleryDataSchemaVersion GetCurrentSchema(GalleryDb ctx)
		{
			return GalleryDataSchemaVersionEnumHelper.ConvertGalleryDataSchemaVersionToEnum(ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion").SettingValue);
		}

		/// <summary>
		/// Upgrades the 3.0.0 data to the 3.0.1 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo301(GalleryDb ctx)
		{
			// Bug 547: Change jQuery 1.10.0 to 1.10.1 (the migration code for 2.6 => 3.0 mistakenly specified 1.10.0 instead of 1.10.1)
			var appSetting = ctx.AppSettings.FirstOrDefault(a => a.SettingName == "JQueryScriptPath");

			if (appSetting != null && appSetting.SettingValue == "//ajax.googleapis.com/ajax/libs/jquery/1.10.0/jquery.min.js")
			{
				appSetting.SettingValue = "//ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js";
			}

			// Bug 570: Change "DateAdded" to "Date Added"
			foreach (var metaDef in ctx.GallerySettings.Where(gs => gs.SettingName == "MetadataDisplaySettings"))
			{
				// Serialized values are separated by apostrophes when first inserted; they are replaced by quotes by the JSON serializer when subsequently
				// saved, so we check for both.
				metaDef.SettingValue = metaDef.SettingValue.Replace(@"""MetadataItem"":111,""Name"":""DateAdded"",""DisplayName"":""DateAdded""", @"""MetadataItem"":111,""Name"":""DateAdded"",""DisplayName"":""Date Added""");
				metaDef.SettingValue = metaDef.SettingValue.Replace(@"'MetadataItem':111,'Name':'DateAdded','DisplayName':'DateAdded'", @"'MetadataItem':111,'Name':'DateAdded','DisplayName':'Date Added'");
			}

			// Bug 578: Change MP4 encoder setting
			foreach (var metaDef in ctx.GallerySettings.Where(gs => gs.SettingName == "MediaEncoderSettings"))
			{
				metaDef.SettingValue = metaDef.SettingValue.Replace(@"-s {Width}x{Height} -b:v 384k", @"-vf ""scale=min(iw*min(640/iw\,480/ih)\,iw):min(ih*min(640/iw\,480/ih)\,ih)"" -b:v 384k");
			}

			// Bug 554: (a) Fix M4V templates
			var mimeType = ctx.MimeTypes.FirstOrDefault(mt => mt.FileExtension == ".m4v" && mt.MimeTypeValue == "video/x-m4v");
			if (mimeType != null)
			{
				mimeType.MimeTypeValue = "video/m4v";
			}

			// Bug 554: (b) Delete x-m4v / safari template
			var mediaTmpl = ctx.MediaTemplates.FirstOrDefault(mt => mt.MimeType == "video/x-m4v" && mt.BrowserId == "safari");
			if (mediaTmpl != null)
			{
				ctx.MediaTemplates.Remove(mediaTmpl);
			}

			// Bug 554: (c) Delete existing m4v templates
			foreach (var tmpl in ctx.MediaTemplates.Where(mt => mt.MimeType == "video/m4v"))
			{
				ctx.MediaTemplates.Remove(tmpl);
			}

			// Bug 554: (d) Add m4v templates based on the mp4 ones
			foreach (var tmpl in ctx.MediaTemplates.Where(mt => mt.MimeType == "video/mp4"))
			{
				ctx.MediaTemplates.Add(new MediaTemplateDto()
				{
					MimeType = "video/m4v",
					BrowserId = tmpl.BrowserId,
					HtmlTemplate = tmpl.HtmlTemplate,
					ScriptTemplate = tmpl.ScriptTemplate
				});
			}

			// Bug 555: (a) Add MP3 template for IE1TO8 (copy it from Firefox, which uses Silverlight)
			var mp3MediaTmpl = ctx.MediaTemplates.FirstOrDefault(mt => mt.MimeType == "audio/x-mp3" && mt.BrowserId == "firefox");
			if (mp3MediaTmpl != null && (!ctx.MediaTemplates.Any(mt => mt.MimeType == mp3MediaTmpl.MimeType && mt.BrowserId == "ie1to8")))
			{
				ctx.MediaTemplates.Add(new MediaTemplateDto()
				{
					MimeType = mp3MediaTmpl.MimeType,
					BrowserId = "ie1to8",
					HtmlTemplate = mp3MediaTmpl.HtmlTemplate,
					ScriptTemplate = mp3MediaTmpl.ScriptTemplate
				});

			}

			// Bug 555: (b) Delete MP3 template for Safari
			mp3MediaTmpl = ctx.MediaTemplates.FirstOrDefault(mt => mt.MimeType == "audio/x-mp3" && mt.BrowserId == "safari");
			if (mp3MediaTmpl != null)
			{
				ctx.MediaTemplates.Remove(mp3MediaTmpl);
			}

			// Bug 564: (a) Change MIME type of .qt and .moov files from video/quicktime to video/mp4
			foreach (var qtMimeType in ctx.MimeTypes.Where(mt => new[] { ".qt", ".moov" }.Contains(mt.FileExtension) && mt.MimeTypeValue == "video/quicktime"))
			{
				qtMimeType.MimeTypeValue = "video/mp4";
			}

			// Bug 564: (b) Delete video/quicktime safari template
			foreach (var qtMediaTmpl in ctx.MediaTemplates.Where(mt => mt.MimeType == "video/quicktime" && mt.BrowserId == "safari"))
			{
				ctx.MediaTemplates.Remove(qtMediaTmpl);
			}

			// Bug 562: Add PDF template for Safari. It looks mostly like the IE one except we have to clear the iframe src before we can hide it.

			const string pdfScriptTmplSafari = @"// IE and Safari render Adobe Reader iframes on top of jQuery UI dialogs, so add event handler to hide frame while dialog is visible
// Safari requires that we clear the iframe src before we can hide it
$('.gsp_mo_share_dlg').on('dialogopen', function() {
 $('#{UniqueId}_frame').attr('src', '').css('visibility', 'hidden');
}).on('dialogclose', function() {
$('#{UniqueId}_frame').attr('src', '{MediaObjectUrl}').css('visibility', 'visible');
});";

			if (!ctx.MediaTemplates.Any(mt => mt.MimeType == "application/pdf" && mt.BrowserId == "safari"))
			{
				ctx.MediaTemplates.Add(new MediaTemplateDto()
				{
					MimeType = "application/pdf",
					BrowserId = "safari",
					HtmlTemplate = "<p><a href='{MediaObjectUrl}'>Enlarge PDF to fit browser window</a></p><iframe id='{UniqueId}_frame' src='{MediaObjectUrl}' frameborder='0' style='width:680px;height:600px;border:1px solid #000;'></iframe>",
					ScriptTemplate = pdfScriptTmplSafari
				});
			}

			// Bug 580: Need to use AddMediaObject permission instead of EditAlbum permission when deciding whether to render the Add link in an empty album.
			// Task 579: Change lock icon tooltip when anonymous access is disabled
			// Task 575: Update signature of jsRender helper functions getAlbumUrl, getGalleryItemUrl, getDownloadUrl, currentUrl
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.Album))
			{
				const string srcText580 = "{{if Album.Permissions.EditAlbum}}<a href='{{: ~getAddUrl(#data) }}'>{{:Resource.AbmAddObj}}</a>{{/if}}";
				const string replText580 = "{{if Album.Permissions.AddMediaObject}}<a href='{{: ~getAddUrl(#data) }}'>{{:Resource.AbmAddObj}}</a>{{/if}}";

				const string srcText579 = "<img src='{{:App.SkinPath}}/images/lock-{{if Album.IsPrivate}}active-{{/if}}s.png' title='{{if Album.IsPrivate}}{{:Resource.AbmIsPvtTt}}{{else}}{{:Resource.AbmNotPvtTt}}{{/if}}' alt=''>";
				const string replText579 = "<img src='{{:App.SkinPath}}/images/lock-{{if Album.IsPrivate || !Settings.AllowAnonBrowsing}}active-{{/if}}s.png' title='{{if !Settings.AllowAnonBrowsing}}{{:Resource.AbmAnonDisabledTt}}{{else}}{{if Album.IsPrivate}}{{:Resource.AbmIsPvtTt}}{{else}}{{:Resource.AbmNotPvtTt}}{{/if}}{{/if}}' alt=''>";

				const string srcText575a = "~getAlbumUrl(#data)";
				const string replText575a = "~getAlbumUrl(Album.Id, true)";

				const string srcText575b = "~getGalleryItemUrl(#data)";
				const string replText575b = "~getGalleryItemUrl(#data, !IsAlbum)";

				const string srcText575c = "~getDownloadUrl(#data)";
				const string replText575c = "~getDownloadUrl(Album.Id)";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText580, replText580);
				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText579, replText579);
				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText575a, replText575a);
				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText575b, replText575b);
				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText575c, replText575c);
			}

			// Task 575: Update signature of jsRender helper function getDownloadUrl
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.MediaObject))
			{
				const string srcText575c = "~getDownloadUrl()";
				const string replText575c = "~getDownloadUrl(Album.Id)";

				const string srcText575d = "~currentUrl()";
				const string replText575d = "~getMediaUrl(MediaItem.Id, true)";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText575c, replText575c);
				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText575d, replText575d);
			}

			// Update data schema version to 3.0.1
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.0.1";

			ctx.SaveChanges();
		}

		/// <summary>
		/// Upgrades the 3.0.1 data to the 3.0.2 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo302(GalleryDb ctx)
		{
			// Bug 625: Search results do not allow downloading original file
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.MediaObject))
			{
				const string srcText = "{{if Settings.AllowOriginalDownload}}";
				const string replText = "{{if Album.Permissions.ViewOriginalMediaObject}}";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);
			}

			// Update data schema version to 3.0.2
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.0.2";

			ctx.SaveChanges();
		}

		/// <summary>
		/// Upgrades the 3.0.2 data to the 3.0.3 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo303(GalleryDb ctx)
		{
			// Fix bug# 632: Error "Cannot find skin path"
			// Change skin name from "Dark" to "dark".
			var asSkin = ctx.AppSettings.First(a => a.SettingName == "Skin");
			if (asSkin.SettingValue == "Dark")
			{
				asSkin.SettingValue = "dark";
			}

			// Fix bug# 633: Upgrading from 2.6 may result in duplicate sets of tags
			// Delete any duplicate "tag" metadata rows
			// FYI: We need the ToList() to avoid this error in SQL CE: The ntext and image data types cannot be used in WHERE, HAVING, GROUP BY, ON, or IN clauses, except when these data types are used with the LIKE or IS NULL predicates.
			var dupMetaTags = ctx.Metadatas
				.Where(m => m.MetaName == MetadataItemName.Tags)
				.GroupBy(m => m.FKMediaObjectId).Where(m => m.Count() > 1)
				.ToList()
				.Select(m => m.Where(t => t.Value == String.Empty))
				.Select(m => m.FirstOrDefault());

			foreach (var dupMetaTag in dupMetaTags)
			{
				ctx.Metadatas.Remove(dupMetaTag);
			}

			// Update data schema version to 3.0.3
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.0.3";

			ctx.SaveChanges();
		}

		/// <summary>
		/// Upgrades the 3.0.3 data to the 3.1.0 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo310(GalleryDb ctx)
		{
			// Insert Orientation meta item into metadata definitions just before the ExposureProgram item.
			foreach (var metaDef in ctx.GallerySettings.Where(gs => gs.SettingName == "MetadataDisplaySettings"))
			{
				// First grab the sequence of ExposureProgram, then subtract one.
				// Matches: 'MetadataItem':14{ANY_TEXT}'Sequence':{ANY_DIGITS} => {ANY_DIGITS} is assigned to seq group name
				var match = System.Text.RegularExpressions.Regex.Match(metaDef.SettingValue, @"['""]MetadataItem['""]:14.+?['""]Sequence['""]:(?<seq>\d+)");

				var sequence = 12; // Default to 12 if we don't find one, which is correct if the user hasn't modified the original order
				if (match.Success)
				{
					sequence = Convert.ToInt32(match.Groups["seq"].Value, CultureInfo.InvariantCulture) - 1;
				}

				// Serialized values are separated by apostrophes when first inserted; they are replaced by quotes by the JSON serializer when subsequently
				// saved, so we check for both. Look for the beginning of the ExposureProgram item and insert the orientation item just before it.
				if (!metaDef.SettingValue.Contains(@"""MetadataItem"":43") && !metaDef.SettingValue.Contains(@"'MetadataItem':43"))
				{
					metaDef.SettingValue = metaDef.SettingValue.Replace(@"{""MetadataItem"":14", String.Concat(@"{""MetadataItem"":43,""Name"":""Orientation"",""DisplayName"":""Orientation"",""IsVisibleForAlbum"":false,""IsVisibleForGalleryObject"":true,""IsEditable"":false,""DefaultValue"":""{Orientation}"",""Sequence"":", sequence, @"},{""MetadataItem"":14"));
					metaDef.SettingValue = metaDef.SettingValue.Replace(@"{'MetadataItem':14", String.Concat(@"{'MetadataItem':43,'Name':'Orientation','DisplayName':'Orientation','IsVisibleForAlbum':false,'IsVisibleForGalleryObject':true,'IsEditable':false,'DefaultValue':'{Orientation}','Sequence':", sequence, @"},{'MetadataItem':14"));
				}
			}

			// Task 611 & 645: Update MP4 encoder setting to (1) perform higher quality transcoding (2) auto-rotate videos (3) remove orientation flag
			const string mp4Setting303 = @"-y -i ""{SourceFilePath}"" -vf ""scale=min(iw*min(640/iw\,480/ih)\,iw):min(ih*min(640/iw\,480/ih)\,ih)"" -b:v 384k -vcodec libx264 -flags +loop+mv4 -cmp 256 -partitions +parti4x4+parti8x8+partp4x4+partp8x8 -subq 6 -trellis 0 -refs 5 -bf 0 -coder 0 -me_range 16 -g 250 -keyint_min 25 -sc_threshold 40 -i_qfactor 0.71 -qmin 10 -qmax 51 -qdiff 4 -ac 1 -ar 16000 -r 13 -ab 32000 -movflags +faststart ""{DestinationFilePath}""";
			const string mp4Setting310 = @"-y -i ""{SourceFilePath}"" -vf ""scale=min(iw*min(640/iw\,480/ih)\,iw):min(ih*min(640/iw\,480/ih)\,ih){AutoRotateFilter}"" -vcodec libx264 -movflags +faststart -metadata:s:v:0 rotate=0 ""{DestinationFilePath}""";
			foreach (var metaDef in ctx.GallerySettings.Where(gs => gs.SettingName == "MediaEncoderSettings"))
			{
				metaDef.SettingValue = metaDef.SettingValue.Replace(mp4Setting303, mp4Setting310);
			}

			// Task 664: Change jQuery 1.10.1 to 1.10.2
			var appSetting = ctx.AppSettings.FirstOrDefault(a => a.SettingName == "JQueryScriptPath");

			if (appSetting != null && appSetting.SettingValue == "//ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js")
			{
				appSetting.SettingValue = "//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js";
			}

			// Task 649: Add .3GP file support
			if (!ctx.MimeTypes.Any(mt => mt.FileExtension == ".3gp"))
			{
				ctx.MimeTypes.Add(new MimeTypeDto
					{
						FileExtension = ".3gp",
						MimeTypeValue = "video/mp4",
						BrowserMimeTypeValue = ""
					});
			}

			// Update data schema version to 3.1.0
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.1.0";

			ctx.SaveChanges();
		}

		/// <summary>
		/// Upgrades the 3.1.0 data to the 3.2.0 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo320(GalleryDb ctx)
		{
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.Album))
			{
				// Task 669: Allow manual sorting of media objects
				// Step 1: Add the Custom option to the sort by dropdown
				var srcText = @"<li class='gsp_abm_sum_sbi_hdr'>{{:Resource.AbmSortbyTt}}</li>";

				var replText = @"<li class='gsp_abm_sum_sbi_hdr'>{{:Resource.AbmSortbyTt}}</li>
{{if Album.VirtualType == 1 && Album.Permissions.EditAlbum}}<li><a href='#' data-id='-2147483648'>{{:Resource.AbmSortbyCustom}}</a></li>{{/if}}";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);

				// Step 2: Convert the thumbnails into a list of <li> tags instead of <div>'s. Required for jQuery UI Sortable.
				srcText = @"<div class='gsp_floatcontainer'>
 {{for Album.GalleryItems}}
 <div class='thmb{{if IsAlbum}} album{{/if}}' data-id='{{:Id}}' data-it='{{:ItemType}}'>
	<a class='gsp_thmbLink' href='{{: ~getGalleryItemUrl(#data, !IsAlbum) }}'>
	 <img class='gsp_thmb_img' style='width:{{:Views[ViewIndex].Width}}px;height:{{:Views[ViewIndex].Height}}px;' src='{{:Views[ViewIndex].Url}}'>
	</a>
	<p class='gsp_go_t' style='width:{{:Views[ViewIndex].Width + 40}}px;' title='{{stripHtml:Title}}'>{{stripHtmlAndTruncate:Title}}</p>
 </div>
 {{/for}}
</div>";

				replText = @"<ul class='gsp_floatcontainer gsp_abm_thmbs'>
 {{for Album.GalleryItems}}
 <li class='thmb{{if IsAlbum}} album{{/if}}' data-id='{{:Id}}' data-it='{{:ItemType}}' style='width:{{:Views[ViewIndex].Width + 40}}px;'>
	<a class='gsp_thmbLink' href='{{: ~getGalleryItemUrl(#data, !IsAlbum) }}'>
	 <img class='gsp_thmb_img' style='width:{{:Views[ViewIndex].Width}}px;height:{{:Views[ViewIndex].Height}}px;' src='{{:Views[ViewIndex].Url}}'>
	</a>
	<p class='gsp_go_t' title='{{stripHtml:Title}}'>{{stripHtmlAndTruncate:Title}}</p>
 </li>
 {{/for}}
</ul>";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);

				// Step 3: Update the JavaScript so that left pane is rendered regardless of whether the client is on a touchscreen device
				srcText = @"var isTouch = window.Gsp.isTouchScreen();
var renderLeftPane = !isTouch  || (isTouch && ($('.gsp_tb_s_CenterPane:visible, .gsp_tb_s_RightPane:visible').length == 0));

if (renderLeftPane ) {
 $('#{{:Settings.LeftPaneClientId}}').html( $.render [ '{{:Settings.LeftPaneTmplName}}' ]( window.{{:Settings.ClientId}}.gspData ));
";

				replText = @"var $lp = $('#{{:Settings.LeftPaneClientId}}');

if ($lp.length > 0) {
 $lp.html( $.render [ '{{:Settings.LeftPaneTmplName}}' ]( window.{{:Settings.ClientId}}.gspData ));
";

				uiTmpl.ScriptTemplate = uiTmpl.ScriptTemplate.Replace(srcText, replText);

				// Bug 709: Link to add objects page should not appear in empty virtual albums
				// Add a condition to the if statement so that Add link appears only for non-virtual albums
				srcText = @"{{if Album.Permissions.AddMediaObject}}<a href='{{: ~getAddUrl(#data) }}'>{{:Resource.AbmAddObj}}</a>{{/if}}";

				replText = @"{{if Album.VirtualType == 1 && Album.Permissions.AddMediaObject}}<a href='{{: ~getAddUrl(#data) }}'>{{:Resource.AbmAddObj}}</a>{{/if}}";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);

				// Task 722: Add RSS link to album template
				srcText = @"
	<span class='gsp_abm_sum_col1_row1_hdr'>{{:Resource.AbmPfx}}</span>";

				replText = @"
{{if Album.RssUrl}}
	<a class='gsp_abm_sum_btn' href='{{:Album.RssUrl}}'>
		<img src='{{:App.SkinPath}}/images/rss-s.png' title='{{:Resource.AbmRssTt}}' alt=''>
	</a>
{{/if}}
	<span class='gsp_abm_sum_col1_row1_hdr'>{{:Resource.AbmPfx}}</span>";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);

				// Task 730: Assign owner button should not be visible for virtual albums
				srcText = @"
{{if Album.Permissions.AdministerGallery}}
	<a class='gsp_abm_sum_ownr_trigger gsp_abm_sum_btn' href='#'>";

				replText = @"
{{if Album.VirtualType == 1 && Album.Permissions.AdministerGallery}}
	<a class='gsp_abm_sum_ownr_trigger gsp_abm_sum_btn' href='#'>";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);
			}

			// Task 681: Merge My account button into username in header
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.Header))
			{
				// Step 1: Remove the HTML for the 'My account' icon
				var srcText = @"
	{{if Settings.AllowManageOwnAccount}}
	<div class='gsp_useroption'>
	 <a class='gsp_myaccountlink' href='{{:App.CurrentPageUrl}}?g=myaccount&aid={{:Album.Id}}'>
		<img class='gsp_myaccount-user-icon' title='{{:Resource.HdrMyAccountTt}}' alt='{{:Resource.HdrMyAccountTt}}' src='{{:App.SkinPath}}/images/user-l.png'>
	 </a></div>
	 {{/if}}";

				var replText = "";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);

				// Step 2: Replace the username with a hyperlinked username that takes user to 'My account' page,
				// except when AllowManageOwnAccount=false, in which case just display the username like before.
				srcText = @"	<span id='{{:Settings.ClientId}}_userwelcome' class='gsp_welcome'>{{:User.UserName}}</span></div>";

				replText = @"{{if Settings.AllowManageOwnAccount}}
		<a id='{{:Settings.ClientId}}_userwelcome' href='{{:App.CurrentPageUrl}}?g=myaccount&aid={{:Album.Id}}' class='gsp_welcome' title='{{:Resource.HdrMyAccountTt}}'>{{:User.UserName}}</a>
	 {{else}}
		<span id='{{:Settings.ClientId}}_userwelcome' class='gsp_welcome'>{{:User.UserName}}</span>
	 {{/if}}
	 </div>";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);
			}

			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.LeftPane))
			{
				// Task 675: Add RECENTLY ADDED and TOP RATED links to left pane
				const string srcText = @"<div id='{{:Settings.ClientId}}_lptv'></div>";

				const string replText = @"<div id='{{:Settings.ClientId}}_lptv' class='gsp_lpalbumtree'></div>
{{if App.LatestUrl}}<p class='gsp_lplatest'><a href='{{:App.LatestUrl}}' class='jstree-anchor'><i class='jstree-icon'></i>{{:Resource.LpRecent}}</a></p>{{/if}}
{{if App.TopRatedUrl}}<p class='gsp_lptoprated'><a href='{{:App.TopRatedUrl}}' class='jstree-anchor'><i class='jstree-icon'></i>{{:Resource.LpTopRated}}</a></p>{{/if}}";

				uiTmpl.HtmlTemplate = uiTmpl.HtmlTemplate.Replace(srcText, replText);
			}

			// Task 727: Update jQuery 1.10.2 to 1.11.1. Update jQuery UI 1.10.3 to 1.10.4.
			var appSetting = ctx.AppSettings.FirstOrDefault(a => a.SettingName == "JQueryScriptPath");

			if (appSetting != null && appSetting.SettingValue == "//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js")
			{
				appSetting.SettingValue = "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js";
			}

			appSetting = ctx.AppSettings.FirstOrDefault(a => a.SettingName == "JQueryUiScriptPath");

			if (appSetting != null && appSetting.SettingValue == "//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js")
			{
				appSetting.SettingValue = "//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js";
			}

			// Task 722: Now that {MediaObjectUrl} is an absolute URL, remove any instances of {HostUrl} that precede it.
			// Under default settings this only affects the HTML portion of two video/divx records, but we're making it more 
			// generic to catch any customizations an admin may have done.
			foreach (var tmpl in ctx.MediaTemplates.Where(mt => mt.HtmlTemplate.Contains("{HostUrl}{MediaObjectUrl}") || mt.ScriptTemplate.Contains("{HostUrl}{MediaObjectUrl}")))
			{
				tmpl.HtmlTemplate = tmpl.HtmlTemplate.Replace("{HostUrl}{MediaObjectUrl}", "{MediaObjectUrl}");
				tmpl.ScriptTemplate = tmpl.ScriptTemplate.Replace("{HostUrl}{MediaObjectUrl}", "{MediaObjectUrl}");
			}

			// Update data schema version to 3.2.0
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.2.0";

			ctx.SaveChanges();

			// Task 674: Add/update Enterprise UI templates introduced in 3.2.0
			var pkRow = ctx.AppSettings.Single(a => a.SettingName == "ProductKey");

			if (pkRow.SettingValue == Constants.ProductKeyEnterprise)
			{
				SeedController.InsertEnterpriseTemplates();
			}
		}

		// Note for next migration. I forgot the left pane JS replacement in the section "Task 671: Improve touchscreen support"
		// above in the initial release of 3.2.0. It was added on May 19, 4 days after the 3.2.0 release.
		// As an extra precaution, we should re-apply this fix in the next migration as well, so anyone who didn't manually
		// update their template still gets the fix.

		/// <summary>
		/// Upgrades the 3.2.0 data to the 3.2.1 data. Applies to data such as app settings, gallery settings, templates, etc.
		/// Does not contain data structure changes such as new columns.
		/// </summary>
		/// <param name="ctx">Context to be used for updating data.</param>
		private static void UpgradeTo321(GalleryDb ctx)
		{
			foreach (var uiTmpl in ctx.UiTemplates.Where(t => t.TemplateType == UiTemplateType.LeftPane))
			{
				// Task 671: Improve touchscreen support.
				// NOTE: This should have been in the 3.2.0 upgrade, but it was forgotten.
				const string srcText = @"// Render the left pane, but not for touchscreens UNLESS the left pane is the only visible pane
var isTouch = window.Gsp.isTouchScreen();
var renderLeftPane = !isTouch  || (isTouch && ($('.gsp_tb_s_CenterPane:visible, .gsp_tb_s_RightPane:visible').length == 0));

if (renderLeftPane ) {
 $('#{{:Settings.LeftPaneClientId}}').html( $.render [ '{{:Settings.LeftPaneTmplName}}' ]( window.{{:Settings.ClientId}}.gspData ));";

				const string replText = @"// Render the left pane if it exists
var $lp = $('#{{:Settings.LeftPaneClientId}}');

if ($lp.length > 0) {
 $lp.html( $.render [ '{{:Settings.LeftPaneTmplName}}' ]( window.{{:Settings.ClientId}}.gspData ));";

				uiTmpl.ScriptTemplate = uiTmpl.ScriptTemplate.Replace(srcText, replText);
			}

			// Update data schema version to 3.2.1
			var asDataSchema = ctx.AppSettings.First(a => a.SettingName == "DataSchemaVersion");
			asDataSchema.SettingValue = "3.2.1";

			ctx.SaveChanges();
		}

		#endregion
	}
}