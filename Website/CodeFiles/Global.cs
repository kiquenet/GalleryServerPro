namespace GalleryServerPro.Web
{
	/// <summary>
	/// Contains constant values used within Gallery Server Pro.
	/// </summary>
	public static class Constants
	{
		public const string APP_NAME = "Gallery Server Pro";
		public const string SAMPLE_IMAGE_FILENAME = "RogerMartin&Family.jpg"; // The name of an embedded resource in the App_GlobalResources directory
		public const string ENCRYPTION_KEY = "mNU-h7:5f_)3=c%@^}#U9Tn*"; // The default encryption key as stored in a new installation. It is updated
																																			// to a new value the first time the application is run.

		// Note this field is also defined in GalleryServerPro.Business.DataConstants. We also define it here because DotNetNuke has a 50-char
		// limit but we don't want to change the value going to the stored procs, so we "override" it here.
		public const int RoleNameLength = 256;
	}

	#region Public Enums

	/// <summary>
	/// Specifies a distinct web page within Gallery Server Pro.
	/// </summary>
	public enum PageId
	{
		none = 0,
		admin_albums = 1,
		admin_backuprestore,
		admin_css,
		admin_eventlog,
		admin_galleries,
		admin_gallerysettings,
		admin_gallerycontrolsettings,
		admin_images,
		admin_manageroles,
		admin_manageusers,
		admin_mediaobjects,
		admin_metadata,
		admin_mediaobjecttypes,
		admin_mediatemplates,
		admin_sitesettings,
		admin_uitemplates,
		admin_usersettings,
		admin_videoaudioother,
		/// <summary>
		/// Represents an album view
		/// </summary>
		album,
		albumtreeview,
		changepassword,
		createaccount,
		error_cannotwritetodirectory,
		error_generic,
		//error_unauthorized, // Removed from use 2009.01.22 (feature # 128)
		install,
		login,
		/// <summary>
		/// Represents the media object view
		/// </summary>
		mediaobject,
		myaccount,
		recoverpassword,
		task_addobjects,
		task_assignthumbnail,
		task_createalbum,
		task_deletealbum,
		task_deletehires,
		task_deleteobjects,
		task_downloadobjects,
		task_editcaptions,
		//task_rearrange,
		task_rotateimage,
		task_rotateimages,
		task_synchronize,
		task_transferobject,
		//search,
		upgrade
	}

	#endregion
}
