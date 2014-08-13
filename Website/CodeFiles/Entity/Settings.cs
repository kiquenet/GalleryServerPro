using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Web.Entity
{
	/// <summary>
	/// A client-optimized object that stores properties that affect the user experience.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Gets the gallery id.
		/// </summary>
		public int GalleryId { get; set; }

		/// <summary>
		/// Gets the client ID for the current Gallery control. An HTML element having this ID will
		/// be present in the web page and can be used by javascript to scope all actions to the 
		/// intended control instance. Example: "gsp_g"
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// Gets the client ID for the DOM element that is to receive the contents of the media
		/// object. Ex: "gsp_g_mediaHtml"
		/// </summary>
		public string MediaClientId { get; set; }

		/// <summary>
		/// Gets the name of the compiled jsRender template for the media object.
		/// Ex: "gsp_g_media_tmpl"
		/// </summary>
		public string MediaTmplName { get; set; }

		/// <summary>
		/// Gets the client ID for the DOM element that is to receive the contents of the gallery
		/// header. Ex: "gsp_g_gHdrHtml"
		/// </summary>
		public string HeaderClientId { get; set; }

		/// <summary>
		/// Gets the name of the compiled jsRender template for the header. Ex: "gsp_g_gallery_header_tmpl"
		/// </summary>
		public string HeaderTmplName { get; set; }

		/// <summary>
		/// Gets the client ID for the DOM element that is to receive the contents of album thumbnail 
		/// images. Ex: "gsp_g_thmbHtml"
		/// </summary>
		public string ThumbnailClientId { get; set; }

		/// <summary>
		/// Gets the name of the compiled jsRender template for the album thumbnail images.
		/// Ex: "gsp_g_thumbnail_tmpl"
		/// </summary>
		public string ThumbnailTmplName { get; set; }

		/// <summary>
		/// Gets the client ID for the DOM element that is to receive the contents of the left pane
		/// of the media view page. Ex: "gsp_g_lpHtml"
		/// </summary>
		public string LeftPaneClientId { get; set; }

		/// <summary>
		/// Gets the name of the compiled jsRender template for the left pane of the media view page.
		/// Ex: "gsp_g_lp_tmpl"
		/// </summary>
		public string LeftPaneTmplName { get; set; }

		/// <summary>
		/// Gets the client ID for the DOM element that is to receive the contents of the right pane
		/// of the media view page. Ex: "gsp_g_rpHtml"
		/// </summary>
		public string RightPaneClientId { get; set; }

		/// <summary>
		/// Gets the name of the compiled jsRender template for the right pane of the media view page.
		/// Ex: "gsp_g_rp_tmpl"
		/// </summary>
		public string RightPaneTmplName { get; set; }

		/// <summary>
		/// Gets a value indicating whether to show the header
		/// </summary>
		public bool ShowHeader { get; set; }

		/// <summary>
		/// Gets a value indicating whether show the login functionality.
		/// </summary>
		public bool ShowLogin { get; set; }

		/// <summary>
		/// Gets a value indicating whether show the search functionality.
		/// </summary>
		public bool ShowSearch { get; set; }

		/// <summary>
		/// Indicates whether anonymous users are allowed to create accounts.
		/// </summary>
		public bool EnableSelfRegistration { get; set; }

		/// <summary>
		/// Indicates whether the user album feature is enabled.</summary>
		public bool EnableUserAlbum { get; set; }

		/// <summary>
		/// Indicates whether to allow a logged-on user to manage their account.
		/// </summary>
		public bool AllowManageOwnAccount { get; set; }

		/// <summary>
		/// Gets the header text that appears at the top of each web page.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets the URL the header text links to.
		/// </summary>
		public string TitleUrl { get; set; }

		/// <summary>
		/// Gets the tooltip for the <see cref="TitleUrl" />.
		/// </summary>
		public string TitleUrlTooltip { get; set; }

		/// <summary>
		/// Gets a value indicating whether the title is displayed beneath individual media objects.
		/// </summary>
		public bool ShowMediaObjectTitle { get; set; }

		/// <summary>
		/// Gets a value indicating whether the next and previous buttons are rendered for individual media objects.
		/// </summary>
		public bool ShowMediaObjectNavigation { get; set; }

		/// <summary>
		/// Gets a value indicating whether to display the relative position of a media object within an album (example: (3 of 24)). 
		/// </summary>
		public bool ShowMediaObjectIndexPosition { get; set; }

		/// <summary>
		/// Indicates the number of thumbnails to display at a time. A value of zero indicates paging
		/// is disabled (all items will be shown).
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the location for the pager used to navigate thumbnails. Will be one of the 
		/// following values: Top, Bottom, TopAndBottom.
		/// </summary>
		public string PagerLocation { get; set; }

		/// <summary>
		/// Specifies the visual transition effect to use when moving from one media object to another.
		/// </summary>
		public string TransitionType { get; set; }

		/// <summary>
		/// The duration of the transition effect, in milliseconds, when navigating between media 
		/// objects. This setting has no effect when <see cref="TransitionType" /> is null or empty.
		/// </summary>
		public int TransitionDurationMs { get; set; }

		/// <summary>
		/// Gets a value indicating whether the toolbar is rendered above individual media objects. 
		/// </summary>
		public bool ShowMediaObjectToolbar { get; set; }

		/// <summary>
		/// Gets a value indicating whether the user is allowed to the media object.
		/// </summary>
		public bool AllowDownload { get; set; }

		/// <summary>
		/// Gets a value indicating whether the user is allowed to download media objects and albums in a ZIP file. 
		/// </summary>
		public bool AllowZipDownload { get; set; }

		/// <summary>
		/// Gets a value indicating whether the show urls button is visible above a media object. 
		/// </summary>
		public bool ShowUrlsButton { get; set; }

		/// <summary>
		/// Gets a value indicating whether the play/pause slide show button is visible above a media 
		/// object. When <see cref="ShowMediaObjectToolbar" />=<c>false</c>, this property is ignored.
		/// </summary>
		public bool ShowSlideShowButton { get; set; }

		/// <summary>
		/// Gets a value indicating whether a slide show of image media objects automatically starts 
		/// playing when the page loads.
		/// </summary>
		public bool SlideShowIsRunning { get; set; }

		/// <summary>
		/// Gets the type of the slide show. Example: "Inline", "FullScreen"
		/// </summary>
		public string SlideShowType { get; set; }

		/// <summary>
		/// The delay, in milliseconds, between images during a slide show.
		/// </summary>
		public int SlideShowIntervalMs { get; set; }

		/// <summary>
		/// Gets a value indicating whether the transfer media object button is visible above a media 
		/// object. When <see cref="ShowMediaObjectToolbar" />=<c>false</c>, this property is ignored.
		/// </summary>
		public bool ShowTransferMediaObjectButton { get; set; }

		/// <summary>
		/// Gets a value indicating whether the copy media object button is visible above a media 
		/// object. When <see cref="ShowMediaObjectToolbar" />=<c>false</c>, this property is ignored.
		/// </summary>
		public bool ShowCopyMediaObjectButton { get; set; }

		/// <summary>
		/// Gets a value indicating whether the rotate media object button is visible above a media 
		/// object. When <see cref="ShowMediaObjectToolbar" />=<c>false</c>, this property is ignored.
		/// </summary>
		public bool ShowRotateMediaObjectButton { get; set; }

		/// <summary>
		/// Gets a value indicating whether the delete media object button is visible above a media 
		/// object. When <see cref="ShowMediaObjectToolbar" />=<c>false</c>, this property is ignored.
		/// </summary>
		public bool ShowDeleteMediaObjectButton { get; set; }

		/// <summary>
		/// Maximum # of characters to display when showing the title of a thumbnail item.
		/// </summary>
		public int MaxThmbTitleDisplayLength { get; set; }

		/// <summary>
		/// Specifies whether anonymous users are allowed to rate gallery objects.
		/// </summary>
		public bool AllowAnonymousRating { get; set; }

		/// <summary>
		/// Specifies whether anonymous users are allowed to browse the gallery.
		/// </summary>
		public bool AllowAnonBrowsing { get; set; }

		/// <summary>
		/// Specifies whether the current gallery is read only. Will be true when 
		/// <see cref="IGallerySettings.MediaObjectPathIsReadOnly" /> is <c>true</c>
		/// </summary>
		public bool IsReadOnlyGallery { get; set; }
	}
}