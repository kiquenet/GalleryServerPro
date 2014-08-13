using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using GalleryServerPro.Business;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Api
{
	/// <summary>
	/// Contains methods for Web API access to users.
	/// </summary>
	public class UsersController : ApiController
	{
		/// <summary>
		/// Gets the user with the specified <paramref name="userName" />.
		/// Example: GET /api/users/getbyusername?userName=Admin&amp;galleryId=1
		/// </summary>
		/// <param name="userName">The name of the user to retrieve.</param>
		/// <param name="galleryId">The gallery ID. Required for retrieving the correct user album ID.</param>
		/// <returns>An instance of <see cref="Entity.User" />.</returns>
		/// <exception cref="System.Web.Http.HttpResponseException"></exception>
		/// <exception cref="HttpResponseMessage"></exception>
		[ActionName("GetByUserName")]
		public Entity.User Get(string userName, int galleryId)
		{
			try
			{
				return UserController.GetUserEntity(userName, galleryId);
			}
			catch (GallerySecurityException)
			{
				// This is thrown when the current user does not have view and edit permission to the requested user.
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
			}
			catch (InvalidUserException)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
																					{
																						Content = new StringContent(String.Format("User '{0}' does not exist", userName)),
																						ReasonPhrase = "User Not Found"
																					});
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
		/// Gets a value indicating whether the <paramref name="userName" /> represents an existing user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns><c>true</c> if the user exists, <c>false</c> otherwise</returns>
		[ActionName("Exists")]
		public bool Get(string userName)
		{
			return (UserController.GetUser(userName, false) != null);
		}

		/// <summary>
		/// Persists the <paramref name="user" /> to the data store. The user can be an existing one or a new one to be
		/// created.
		/// </summary>
		/// <param name="user">The role.</param>
		/// <returns>An instance of <see cref="HttpResponseMessage" />.</returns>
		/// <exception cref="System.Web.Http.HttpResponseException">Thrown when the requested action is not successful.</exception>
		public HttpResponseMessage Post(Entity.User user)
		{
			// POST /api/users
			try
			{
				string newPwd = null;

				if (user.IsNew.GetValueOrDefault())
				{
					UserController.CreateUser(user);
				}
				else
				{
					UserController.SaveUser(user, out newPwd);
				}

				var msg = new StringContent(String.Format(CultureInfo.CurrentCulture, "User '{0}' has been saved.{1}",
					Utils.HtmlEncode(user.UserName),
					user.PasswordResetRequested.GetValueOrDefault() ? String.Format(CultureInfo.CurrentCulture, Resources.GalleryServerPro.Admin_Manage_Users_New_Pwd_Text, newPwd) : String.Empty
					));

				return new HttpResponseMessage(HttpStatusCode.OK) { Content = msg };
			}
			catch (GallerySecurityException ex)
			{
				AppEventController.LogError(ex);

				// Just in case we created the user and the exception occured at a later step, like adding the roles, delete the user.
				if (user.IsNew.GetValueOrDefault() && UserController.GetUser(user.UserName, false) != null)
				{
					UserController.DeleteUser(user.UserName);
				}
				
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
				{
					Content = new StringContent(ex.Message),
					ReasonPhrase = "Action Forbidden"
				});
			}
			catch (InvalidUserException ex)
			{
				AppEventController.LogError(ex);

				// Just in case we created the user and the exception occured at a later step, like adding the roles, delete the user.
				if (user.IsNew.GetValueOrDefault() && UserController.GetUser(user.UserName, false) != null)
				{
					UserController.DeleteUser(user.UserName);
				}

				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = new StringContent(ex.Message),
					ReasonPhrase = "Invalid User"
				});
			}
			catch (MembershipCreateUserException ex)
			{
				AppEventController.LogError(ex);

				// Just in case we created the user and the exception occured at a later step, like adding the roles, delete the user,
				// but only if the user exists AND the error wasn't 'DuplicateUserName'.
				if (user.IsNew.GetValueOrDefault() && (ex.StatusCode != MembershipCreateStatus.DuplicateUserName) && (UserController.GetUser(user.UserName, false) != null))
				{
					UserController.DeleteUser(user.UserName);
				}

				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = new StringContent(UserController.GetAddUserErrorMessage(ex.StatusCode)),
					ReasonPhrase = "Cannot Create User"
				});
			}
			catch (Exception ex)
			{
				AppEventController.LogError(ex);

				// Just in case we created the user and the exception occured at a later step, like adding the roles, delete the user.
				if (user.IsNew.GetValueOrDefault() && UserController.GetUser(user.UserName, false) != null)
				{
					UserController.DeleteUser(user.UserName);
				}

				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
				{
					Content = Utils.GetExStringContent(ex),
					ReasonPhrase = "Server Error"
				});
			}
			finally
			{
				HelperFunctions.RemoveCache(CacheItem.GalleryServerRoles);
			}
		}

		/// <summary>
		/// Permanently delete the <paramref name="userName" /> from the data store.
		/// </summary>
		/// <param name="userName">The name of the user to be deleted.</param>
		/// <returns>An instance of <see cref="HttpResponseMessage" />.</returns>
		/// <exception cref="System.Web.Http.HttpResponseException">Thrown when the requested action is not successful.</exception>
		[ActionName("DeleteByUserName")]
		[HttpDelete]
		public HttpResponseMessage Delete(string userName)
		{
			// DELETE /api/users
			try
			{
				// Don't need to check security here because we'll do that in RoleController.DeleteGalleryServerProRole.
				UserController.DeleteGalleryServerProUser(userName, true);

				return new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent(String.Format(CultureInfo.CurrentCulture, "User '{0}' has been deleted", Utils.HtmlEncode(userName)))
				};
			}
			catch (GallerySecurityException ex)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
				{
					Content = new StringContent(ex.Message),
					ReasonPhrase = "Action Forbidden"
				});
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