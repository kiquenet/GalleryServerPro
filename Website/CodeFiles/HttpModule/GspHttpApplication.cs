using System;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.HttpModule
{
	/// <summary>
	/// An HTTP module that implements application-wide functionality in Gallery Server Pro.
	/// </summary>
	public class GspHttpApplication : IHttpModule
	{
		#region Fields

		private static volatile bool _appInitialized;
		private static readonly object _appInitializedLock = new object();

		#endregion

		#region IHttpModule Members

		/// <summary>
		/// Initializes the specified module.
		/// </summary>
		/// <param name="context">The application context that instantiated and will be running this module.</param>
		public void Init(HttpApplication context)
		{
			context.PostAuthorizeRequest += (sender, e) =>
			{
				if (Utils.IsWebApiRequest())
				{
					// Some Web.API requests need access to session state, so turn it on.
					HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
				}
			};

			if (!_appInitialized)
			{
				lock (_appInitializedLock)
				{
					if (!_appInitialized)
					{
						// This will run only once per application start
						OnStart(context);

						_appInitialized = true;
					}
				}
			}

			OnInit(context);
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion

		#region Functions

		/// <summary>Initializes any data/resources on application start.</summary>
		/// <param name="context">The application context that instantiated and will be running this module.</param>
		private void OnStart(HttpApplication context)
		{
			RegisterRoutes(RouteTable.Routes);

			GalleryController.InitializeGspApplication();
		}

		/// <summary>Initializes any data/resources on HTTP module start.</summary>
		/// <param name="context">The application context that instantiated and will be running this module.</param>
		private void OnInit(HttpApplication context)
		{
			// This will run on every HttpApplication initialization in the application pool.
		}

		private void RegisterRoutes(RouteCollection routes)
		{
			// Supported routes:
			// api/albums/4/ - Gets album #4 with album-only properties set
			// api/albums/4/galleryitems - Gets all gallery objects in album #4
			// api/albums/4/mediaitems - Gets all media objects in album #4
			// api/mediaitems/12 - Gets media object #12
			// api/albums/4/inflated - Gets a GalleryData instance for album #4. Includes all items
			// api/albums/4/inflated?top=10&skip=30 - Same as previous, except gets items 31-40
			// api/mediaitems/12/inflated - Gets a GalleryData instance for media object #12
			// api/mediaitems/12/meta - Gets metadata items for media object #12
			// api/meta - (PUT) Saves metadata item

			// We may want the following validation to prevent duplicate registering, but we'll
			// leave it commented out for now in case a user is registering routes from another place.
			//if (RouteTable.Routes.Count == 0)
			//{
			//	return;
			//}

			routes.MapHttpRoute(
				name: "GalleryApi1",
				routeTemplate: "api/{controller}"
				);

			routes.MapHttpRoute(
				name: "GalleryApi2",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { },
				constraints: new
					{
						id = @"\d*",
					}
				);

			//routes.MapHttpRoute(
			//  name: "GalleryApi2",
			//  routeTemplate: "api/{controller}/{id}",
			//  defaults: new
			//    {
			//      Action = "Get"
			//    },
			//  constraints: new
			//    {
			//      id = @"\d*",
			//    }
			//  );

			routes.MapHttpRoute(
				name: "GalleryApi3",
				routeTemplate: "api/{controller}/{id}/{action}",
				defaults: new
					{
					},
				constraints: new
					{
						id = @"\d*"
					}
				);

			// Add route to support things like api/meta/galleryitems/
			routes.MapHttpRoute(
				name: "GalleryApi4",
				routeTemplate: "api/{controller}/{action}",
				defaults: new
					{
					},
				constraints: new
					{
					}
				);
			//_config.Routes.MapHttpRoute("UpdateOrderApiUrl", "api/orders/{orderId}",
			//															new { controller = "OrdersApi", action = "UpdateOrder" }, new
			//															{
			//																httpMethod = new HttpMethodConstraint(HttpMethod.Post)
			//															});
			//routes.MapHttpRoute("GalleryApi4", "api/meta",
			//	new { controller = "Meta", action = "AddTag" },
			//	new { httpMethod = new HttpMethodConstraint(new string[]{ "PUT" }) });
		}

		#endregion
	}
}