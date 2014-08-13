namespace GalleryServerPro.Web.Entity
{
	/// <summary>
	/// A client-optimized object that stores application-level properties for the gallery.
	/// </summary>
	public class App
	{
		/// <summary>
		/// Gets the path, relative to the current application, to the directory containing the Gallery Server Pro
		/// resources such as images, user controls, scripts, etc. Examples: "gs", "GalleryServerPro\resources"
		/// </summary>
		/// <value>
		/// The gallery resources path.
		/// </value>
		public string GalleryResourcesPath { get; set; }

		/// <summary>
		/// Gets the path, relative to the current application, to the directory containing the Gallery Server Pro
		/// skin resources for the currently selected skin. Examples: "gs/skins/simple-grey", "/dev/gallery/gsp/skins/simple-grey"
		/// </summary>
		/// <value>The skin path.</value>
		public string SkinPath { get; set; }

		/// <summary>
		/// Gets the URL, relative to the website root and without any query string parameters, 
		/// to the current page. Example: "/dev/gs/gallery.aspx"
		/// </summary>
		/// <value>
		/// The current page URL.
		/// </value>
		public string CurrentPageUrl { get; set; }

		/// <summary>
		/// Get the URI scheme, DNS host name or IP address, and port number for the current application. 
		/// Examples: http://www.site.com, http://localhost, http://127.0.0.1, http://godzilla
		/// </summary>
		/// <value>The URL to the current web host.</value>
		public string HostUrl { get; set; }

		/// <summary>
		/// Gets the URL to the current web application. Does not include the containing page or the trailing slash. 
		///  Example: If the gallery is installed in a virtual directory 'gallery' on domain 'www.site.com', this 
		/// returns 'http://www.site.com/gallery'.
		/// </summary>
		/// <value>The URL to the current web application.</value>
		public string AppUrl { get; set; }

		/// <summary>
		/// Gets the URL to the list of recently added media objects. Requires running an Enterprise license; 
		/// value will be null when running in trial mode or under other licenses. Ex: http://site.com/gallery/default.aspx?latest=50
		/// </summary>
		/// <value>The URL to the list of recently added media objects.</value>
		public string LatestUrl { get; set; }

		/// <summary>
		/// Gets the URL to the list of top rated media objects. Requires running an Enterprise license; 
		/// value will be null when running in trial mode or under other licenses. Ex: http://site.com/gallery/default.aspx?latest=50
		/// </summary>
		/// <value>The URL to the list of top rated media objects.</value>
		public string TopRatedUrl { get; set; }

		/// <summary>
		/// Gets a value indicating whether the initial 30-day trial for the application has expired and no valid product key 
		/// has been entered.
		/// </summary>
		/// <value><c>true</c> if the application is in the trial period; otherwise, <c>false</c>.</value>
		public bool IsInReducedFunctionalityMode { get; set; }
	}
}