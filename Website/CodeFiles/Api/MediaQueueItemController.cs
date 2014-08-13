using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GalleryServerPro.Web.Api
{
	/// <summary>
	/// Contains methods for Web API access to the media processing queue.
	/// </summary>
	public class MediaQueueItemController : ApiController
	{
		/// <summary>
		/// Permanently deletes the specified queue items from the data store. Current user must be
		/// a gallery administrator for the gallery the media item belongs to; otherwise no action 
		/// is taken.
		/// </summary>
		/// <param name="mediaQueueIds">The media queue IDs.</param>
		public HttpResponseMessage Delete(int[] mediaQueueIds)
		{
			try
			{
				foreach (int mediaQueueId in mediaQueueIds)
				{
					Delete(mediaQueueId);
				}

				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully deleted...") };
			}
			catch (InvalidMediaObjectException)
			{
				// HTTP specification says the DELETE method must be idempotent, so deleting a nonexistent item must have 
				// the same effect as deleting an existing one. So we do nothing here and let the method return HttpStatusCode.OK.
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Successfully deleted...") };
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
		/// Permanently deletes the specified queue item from the data store. Current user must be
		/// a gallery administrator for the gallery the media item belongs to; otherwise no action 
		/// is taken.
		/// </summary>
		/// <param name="id">The media queue ID.</param>
		private void Delete(int id)
		{
			var item = MediaConversionQueue.Instance.Get(id);

			if (item == null)
				return;

			IGalleryObject mo = Factory.LoadMediaObjectInstance(item.MediaObjectId);

			if (Utils.IsCurrentUserGalleryAdministrator(mo.GalleryId))
			{
				MediaConversionQueue.Instance.RemoveMediaQueueItem(id);
			}
			else
			{
				throw new GallerySecurityException();
			}
		}
	}
}