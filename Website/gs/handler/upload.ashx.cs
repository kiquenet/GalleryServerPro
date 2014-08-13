using System;
using System.IO;
using System.Linq;
using System.Web.SessionState;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Handler
{
  /// <summary>
  /// Defines a handler for receiving files sent from a browser and saving them to a temporary directory.
  /// Specifically, it is designed to receive files sent from Plupload (http://www.plupload.com).
  /// </summary>
  /// <remarks>
  /// This implements the upload handler by Rick Strahl: https://github.com/RickStrahl/Westwind.plUploadHandler
  /// </remarks>
  public class upload : UploadHandler, IReadOnlySessionState
  {
    #region Fields

    private IAlbum _album;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the destination album that will eventually host the uploaded media file. This value is determined by inspecting the
    /// 'aid' query string parameter sent by the browser.
    /// </summary>
    /// <value>The destination album.</value>
    private IAlbum DestinationAlbum
    {
      get
      {
        try
        {
          return _album ?? (_album = Factory.LoadAlbumInstance(Utils.GetQueryStringParameterInt32("aid"), false));
        }
        catch (InvalidAlbumException)
        {
          return null;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current user is authorized to upload a file to the specified <see cref="DestinationAlbum" />.
    /// </summary>
    /// <value><c>true</c> if the current user is authorized; otherwise, <c>false</c>.</value>
    private bool IsUserAuthorized
    {
      get
      {
        if (DestinationAlbum != null)
        {
          return Utils.IsUserAuthorized(SecurityActions.AddMediaObject, DestinationAlbum.Id, DestinationAlbum.GalleryId, DestinationAlbum.IsPrivate, DestinationAlbum.IsVirtualAlbum);
        }

        return false;
      }
    }

    /// <summary>
    /// Gets the gallery ID the uploaded media file is intended for.
    /// </summary>
    /// <value>The gallery ID.</value>
    private int GalleryId
    {
      get
      {
        return (DestinationAlbum != null ? DestinationAlbum.GalleryId : int.MinValue);
      }
    }

    /// <summary>
    /// Gets the gallery settings.
    /// </summary>
    /// <value>The gallery settings.</value>
    private IGallerySettings GallerySettings
    {
      get
      {
        try
        {
          return Factory.LoadGallerySetting(GalleryId);
        }
        catch (ArgumentOutOfRangeException)
        {
          return null;
        }
      }
    }

    #endregion

    #region Methods

    protected override void Initialize()
    {
      if (!GalleryController.IsInitialized)
      {
        GalleryController.InitializeGspApplication();
      }

      if (!IsUserAuthorized)
      {
        WriteErrorResponse("You do not have authorization to perform the requested action.", 403);
        return;
      }

      // Normally you'd set these values from config values
      FileUploadPhysicalPath = Path.Combine(AppSetting.Instance.PhysicalApplicationPath, GlobalConstants.TempUploadDirectory);
      MaxUploadSize = GallerySettings.MaxUploadSize * 1024;

      AllowedExtensions = GallerySettings.AllowUnspecifiedMimeTypes ?
        ".*" :
        String.Join(",", Factory.LoadMimeTypes(GalleryId).Where(mt => mt.AllowAddToGallery).Select(mt => mt.Extension));
    }

    /// <summary>
    /// Completion handler called when the download completes
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    protected override void OnUploadCompleted(string fileName)
    {
      WriteUploadCompletedMessage("Success");
    }

    #endregion
  }
}