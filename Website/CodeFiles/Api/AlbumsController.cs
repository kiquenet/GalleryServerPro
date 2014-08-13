using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Api
{
  /// <summary>
  /// Contains methods for Web API access to albums.
  /// </summary>
  public class AlbumsController : ApiController
  {
    /// <summary>
    /// Gets the album with the specified <paramref name="id" />. The properties 
    /// <see cref="Entity.Album.GalleryItems" /> and <see cref="Entity.Album.MediaItems" /> 
    /// are set to null to keep the instance small. Example: api/albums/4/
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <returns>An instance of <see cref="Entity.Album" />.</returns>
    /// <exception cref="System.Web.Http.HttpResponseException"></exception>
    public Entity.Album Get(int id)
    {
      IAlbum album = null;
      try
      {
        album = AlbumController.LoadAlbumInstance(id, true);
        SecurityManager.ThrowIfUserNotAuthorized(SecurityActions.ViewAlbumOrMediaObject, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, Utils.IsAuthenticated, album.IsPrivate, album.IsVirtualAlbum);
        var permissionsEntity = new Entity.Permissions();

        return AlbumController.ToAlbumEntity(album, permissionsEntity, new Entity.GalleryDataLoadOptions());
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex, (album != null ? album.GalleryId : new int?()));

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Gets a comprehensive set of data about the specified album.
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <param name="top">Specifies the number of child gallery objects to retrieve. Specify 0 to retrieve all items.</param>
    /// <param name="skip">Specifies the number of child gallery objects to skip.</param>
    /// <returns>An instance of <see cref="Entity.GalleryData" />.</returns>
    /// <exception cref="System.Web.Http.HttpResponseException">
    /// </exception>
    /// <exception cref="HttpResponseMessage">
    /// </exception>
    /// <exception cref="StringContent"></exception>
    [ActionName("Inflated")]
    public Entity.GalleryData GetInflatedAlbum(int id, int top = 0, int skip = 0)
    {
      // GET /api/albums/12/inflated // Return data for album # 12
      IAlbum album = null;
      try
      {
        album = Factory.LoadAlbumInstance(id, true);
        var loadOptions = new Entity.GalleryDataLoadOptions
          {
            LoadGalleryItems = true,
            NumGalleryItemsToRetrieve = top,
            NumGalleryItemsToSkip = skip
          };

        return GalleryController.GetGalleryDataForAlbum(album, loadOptions);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex, (album != null ? album.GalleryId : new int?()));

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Gets the gallery items for the specified album, optionally sorting the results.
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <param name="sortByMetaNameId">The name of the metadata item to sort on.</param>
    /// <param name="sortAscending">If set to <c>true</c> sort in ascending order.</param>
    /// <returns>IQueryable{Entity.GalleryItem}.</returns>
    /// <exception cref="System.Web.Http.HttpResponseException"></exception>
    [ActionName("GalleryItems")]
    public IQueryable<Entity.GalleryItem> GetGalleryItemsForAlbumId(int id, int sortByMetaNameId = int.MinValue, bool sortAscending = true)
    {
      // GET /api/albums/12/galleryitems?sortByMetaNameId=11&sortAscending=true - Gets gallery items for album #12
      try
      {
        return GalleryObjectController.GetGalleryItemsInAlbum(id, (MetadataItemName)sortByMetaNameId, sortAscending);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Gets the media items for the specified album.
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <param name="sortByMetaNameId">The name of the metadata item to sort on.</param>
    /// <param name="sortAscending">If set to <c>true</c> sort in ascending order.</param>
    /// <returns>IQueryable{Entity.MediaItem}.</returns>
    /// <exception cref="System.Web.Http.HttpResponseException"></exception>
    [ActionName("MediaItems")]
    public IQueryable<Entity.MediaItem> GetMediaItemsForAlbumId(int id, int sortByMetaNameId = int.MinValue, bool sortAscending = true)
    {
      // GET /api/albums/12/mediaitems - Gets media items for album #12
      try
      {
        return Controller.GalleryObjectController.GetMediaItemsInAlbum(id, (MetadataItemName)sortByMetaNameId, sortAscending);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Gets the meta items for the specified album <paramref name="id" />.
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <returns></returns>
    /// <exception cref="System.Web.Http.HttpResponseException"></exception>
    [ActionName("Meta")]
    public IQueryable<Entity.MetaItem> GetMetaItemsForAlbumId(int id)
    {
      // GET /api/albums/12/meta - Gets metadata items for album #12
      try
      {
        return AlbumController.GetMetaItemsForAlbum(id).AsQueryable();
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Persists the <paramref name="album" /> to the data store. Only the following properties are persisted: 
    /// <see cref="Entity.Album.DateStart" />, <see cref="Entity.Album.DateEnd" />, <see cref="Entity.Album.SortById" />,
    /// <see cref="Entity.Album.SortUp" />, <see cref="Entity.Album.IsPrivate" />, <see cref="Entity.Album.Owner" />
    /// </summary>
    /// <param name="album">The album to persist.</param>
    /// <exception cref="System.Web.Http.HttpResponseException">Thrown when the album isn't found in the data store,
    /// the current user doesn't have permission to edit the album, or some other error occurs.
    /// </exception>
    public void Post(Entity.Album album)
    {
      try
      {
        AlbumController.UpdateAlbumInfo(album);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", album.Id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (NotSupportedException ex)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = new StringContent(ex.Message),
          ReasonPhrase = "Business Rule Violation"
        });
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex, album.GalleryId);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Deletes the album with the specified <paramref name="id" /> from the data store.
    /// </summary>
    /// <param name="id">The ID of the album to delete.</param>
    /// <returns>An instance of <see cref="HttpResponseMessage" />.</returns>
    /// <exception cref="System.Web.Http.HttpResponseException">Thrown when the current user doesn't have
    /// permission to delete the album, deleting the album would violate a business rule, or some other
    /// error occurs.
    /// </exception>
    public HttpResponseMessage Delete(int id)
    {
      try
      {
        AlbumController.DeleteAlbum(id);

        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(String.Format("Album {0} deleted...", id)) };
      }
      catch (InvalidAlbumException)
      {
        // HTTP specification says the DELETE method must be idempotent, so deleting a nonexistent item must have 
        // the same effect as deleting an existing one. So we simply return HttpStatusCode.OK.
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(String.Format("Album with ID = {0} does not exist.", id)) };
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (CannotDeleteAlbumException ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Sorts the <paramref name="galleryItems" /> in the order in which they are passed.
    /// This method is used when a user is manually sorting an album and has dragged an item to a new position.
    /// The operation occurs asynchronously and returns immediately.
    /// </summary>
    /// <param name="galleryItems">The gallery objects to sort. Their position in the array indicates the desired
    /// sequence. Only <see cref="Entity.GalleryItem.Id" /> and <see cref="Entity.GalleryItem.ItemType" /> need be 
    /// populated.</param>
    [HttpPost]
    [ActionName("SortGalleryObjects")]
    public void Sort(Entity.GalleryItem[] galleryItems)
    {
      try
      {
        var userName = Utils.UserName;
        Task.Factory.StartNew(() => AlbumController.Sort(galleryItems, userName));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Re-sort the items in the album according to the criteria and store this updated sequence in the
    /// database. Callers must have <see cref="SecurityActions.EditAlbum" /> permission.
    /// </summary>
    /// <param name="id">The album ID.</param>
    /// <param name="sortByMetaNameId">The name of the metadata item to sort on.</param>
    /// <param name="sortAscending">If set to <c>true</c> sort in ascending order.</param>
    /// <exception cref="System.Web.Http.HttpResponseException"></exception>
    [HttpPost]
    [ActionName("SortAlbum")]
    public void Sort(int id, int sortByMetaNameId, bool sortAscending)
    {
      try
      {
        AlbumController.Sort(id, sortByMetaNameId, sortAscending);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }

    /// <summary>
    /// Sorts the gallery items passed to this method and return. No changes are made to the data store.
    /// When the album is virtual, the <see cref="Entity.AlbumAction.Album.GalleryItems" /> property
    /// must be populated with the items to sort. For non-virtual albums (those with a valid ID), the 
    /// gallery objects are retrieved based on the ID and then sorted. The sort preference is saved to 
    /// the current user's profile, except when the album is virtual. The method incorporates security to
    /// ensure only authorized items are returned to the user.
    /// </summary>
    /// <param name="albumAction">An instance containing the album to sort and the sort preferences.</param>
    /// <returns>IQueryable{Entity.GalleryItem}.</returns>
    [HttpPost]
    [ActionName("GetSortedAlbum")]
    public IQueryable<Entity.GalleryItem> Sort(Entity.AlbumAction albumAction)
    {
      // POST /api/albums/getsortedalbum -
      try
      {
        return GalleryObjectController.SortGalleryItems(albumAction);
      }
      catch (InvalidAlbumException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = new StringContent(String.Format("Could not find album with ID = {0}", albumAction.Album.Id)),
          ReasonPhrase = "Album Not Found"
        });
      }
      catch (GallerySecurityException)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);

        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = Utils.GetExStringContent(ex),
          ReasonPhrase = "Server Error"
        });
      }
    }
  }
}