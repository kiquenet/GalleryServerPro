
namespace GalleryServerPro.Business
{
	/// <summary>
	/// Contains constants used throughout Gallery Server Pro.
	/// </summary>
	public static class GlobalConstants
	{
		/// <summary>
		/// The default name for a user when no actual user account is available. For example, this value is used when remotely
		/// invoking a synchronization.
		/// </summary>
		public const string SystemUserName = "System";
		/// <summary>
		/// The default name for a directory when a valid name cannot be generated from the album title. This occurs
		/// when a user enters an album title consisting entirely of characters that are invalid for a directory
		/// name, such as ?, *, :.
		/// </summary>
		public const string DefaultAlbumDirectoryName = "Album";
		
		/// <summary>
		/// Gets the name of the dictionary key that references the <see cref="Interfaces.IGalleryServerRoleCollection" /> item containing
		/// all roles for the current gallery in the cache item named <see cref="CacheItem.GalleryServerRoles" />. Note that other items 
		/// in the dictionary have keys identified by a concatenation of the user's session ID and username.
		/// </summary>
		public const string GalleryServerRoleAllRolesCacheKey = "AllRoles";

		/// <summary>
		/// Gets the string that is used for the beginning of every role name used for album ownership. The role name has
		/// this format: {RoleNamePrefix} - {AlbumOwnerUserName} - {AlbumTitle} (album {AlbumID}) For example:
		/// "Album Owner - rdmartin - rdmartin's album (album 193)" Current value: "Album Owner"
		/// </summary>
		public const string AlbumOwnerRoleNamePrefix = "Album Owner";

		/// <summary>
		/// Gets the name of the role that defines the permissions to use for album ownership roles.
		/// Current value: _Album Owner Template"
		/// </summary>
		public const string AlbumOwnerRoleTemplateName = "_Album Owner Template";

		/// <summary>
		/// Gets the name of the session variable that stores a List&lt;String&gt; of filenames that were skipped
		/// when the user added one or more files to Gallery Server on the Add objects page.
		/// </summary>
		public const string SkippedFilesDuringUploadSessionKey = "SkippedFiles";

		/// <summary>
		/// Gets the name of the thumbnail file that is created to represent an external media object.
		/// </summary>
		public const string ExternalMediaObjectFilename = "external";

		/// <summary>
		/// Gets the maximum number of skipped objects to display to the user after a synchronization. If the number is too high, 
		/// it can take a long time to transmit the data to the browser, or it it can exceed the maxJsonLength value set in web.config,
		/// which causes a "maximum length exceed" error.
		/// </summary>
		public const int MaxNumberOfSkippedObjectsToDisplayAfterSynch = 500;

		/// <summary>
		/// Gets the maximum number of users to display in a list on the manage users page. When the number of users exceeds
		/// this number, the layout of the page changes to be more efficient with large numbers of users.
		/// </summary>
		public const int MaxNumberOfUsersToDisplayOnManageUsersPage = 1000;

		/// <summary>
		/// Gets the path, relative to the web application root, where files may be temporarily persisted. Ex: "App_Data\\_Temp"
		/// </summary>
		public const string TempUploadDirectory = "App_Data\\_Temp";

		/// <summary>
		/// Gets the path, relative to the web application root, of the application data directory. Ex: "App_Data"
		/// </summary>
		public const string AppDataDirectory = "App_Data";

		/// <summary>
		/// Gets the name of the file that, when present in the App_Data directory, causes the Install Wizard to automatically run.
		/// Ex: "install.txt"
		/// </summary>
		public const string InstallTriggerFileName = "install.txt";

		/// <summary>
		/// Gets the name of the Active Directory membership provider.
		/// </summary>
		public const string ActiveDirectoryMembershipProviderName = "System.Web.Security.ActiveDirectoryMembershipProvider";

		/// <summary>
		/// Gets the number of days Gallery Server Pro is fully functional before it requires a product key to be entered.
		/// Default value = 30.
		/// </summary>
		public const int TrialNumberOfDays = 30;

		/// <summary>
		/// The maximum allowed length for an album directory name.
		/// </summary>
		public const int AlbumDirectoryNameLength = 255;

		/// <summary>
		/// The maximum allowed length for a media object file name.
		/// </summary>
		public const int MediaObjectFileNameLength = 255;
	}
}
