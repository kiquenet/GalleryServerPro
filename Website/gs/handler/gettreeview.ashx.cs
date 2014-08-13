using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Web.Controller;
using GalleryServerPro.Web.Entity;

namespace GalleryServerPro.Web.Handler
{
  /// <summary>
  /// Defines a handler that returns JSON in a format that is consumable by the JsTree jQuery plug-in.
  /// This can be called when a user clicks on a treeview node to dynamically load that node's contents.
  /// JsTree home page: http://www.jstree.com
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class gettreeview : IHttpHandler
  {
    #region Private Fields

    private int _albumId;
    private SecurityActions _securityAction;
    private bool _showCheckbox;
    private string _navigateUrl;

    #endregion

    #region Public Methods

    /// <summary>
    /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
    /// </summary>
    /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
    public void ProcessRequest(HttpContext context)
    {
      try
      {
        if (!GalleryController.IsInitialized)
        {
          GalleryController.InitializeGspApplication();
        }

        if (InitializeVariables(context))
        {
          string tvXml = GenerateTreeviewJson();
          context.Response.ContentType = "text/json";
          context.Response.Cache.SetCacheability(HttpCacheability.NoCache); // Needed for IE 7
          context.Response.Write(tvXml);
        }
        else
          context.Response.End();
      }
      catch (System.Threading.ThreadAbortException)
      {
        throw; // We don't want these to fall into the generic catch because we don't want them logged.
      }
      catch (Exception ex)
      {
        AppEventController.LogError(ex);
        throw;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
    /// </summary>
    /// <value></value>
    /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
    /// </returns>
    public bool IsReusable
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region Private Methods

    private string GenerateTreeviewJson()
    {
      TreeView tv = GenerateTreeview();
      return tv.ToJson();
    }

    private TreeView GenerateTreeview()
    {
      // We'll use a TreeView instance to generate the appropriate JSON structure 
      TreeView tv = new TreeView();
      
      IAlbum parentAlbum = AlbumController.LoadAlbumInstance(this._albumId, true);

      foreach (IAlbum childAlbum in parentAlbum.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).ToSortedList())
      {
        TreeNode node = new TreeNode();
        node.Id = String.Concat("tv_", childAlbum.Id.ToString(CultureInfo.InvariantCulture));
        node.Text = Utils.RemoveHtmlTags(childAlbum.Title);
        node.DataId = childAlbum.Id.ToString(CultureInfo.InvariantCulture);

        if (!String.IsNullOrEmpty(_navigateUrl))
        {
          node.NavigateUrl = Utils.AddQueryStringParameter(_navigateUrl, String.Concat("aid=", childAlbum.Id.ToString(CultureInfo.InvariantCulture)));
        }

        bool isUserAuthorized = true;
        if (SecurityActionEnumHelper.IsValidSecurityAction(this._securityAction))
        {
          isUserAuthorized = Utils.IsUserAuthorized(_securityAction, RoleController.GetGalleryServerRolesForUser(), childAlbum.Id, childAlbum.GalleryId, childAlbum.IsPrivate, childAlbum.IsVirtualAlbum);
        }
        
        node.ShowCheckBox = isUserAuthorized && _showCheckbox;
        node.Selectable = isUserAuthorized;

        if (childAlbum.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).Any())
        {
          node.HasChildren = true;
        }

        tv.Nodes.Add(node);
      }
      
      return tv;
    }

    /// <summary>
    /// Initialize the class level variables with information from the query string. Returns false if the variables cannot 
    /// be properly initialized.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>Returns true if all variables were initialized; returns false if there was a problem and one or more variables
    /// could not be set.</returns>
    private bool InitializeVariables(HttpContext context)
    {
      if (!ExtractQueryStringParms(context.Request.Url.Query))
        return false;

      if (_albumId > 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Extract information from the query string and assign to our class level variables. Return false if something goes wrong
    /// and the variables cannot be set. This will happen when the query string is in an unexpected format.
    /// </summary>
    /// <param name="queryString">The query string for the current request. Can be populated with HttpContext.Request.Url.Query.</param>
    /// <returns>Returns true if all relevant variables were assigned from the query string; returns false if there was a problem.</returns>
    private bool ExtractQueryStringParms(string queryString)
    {
      if (String.IsNullOrEmpty(queryString)) return false;

      if (queryString.StartsWith("?", StringComparison.Ordinal)) queryString = queryString.Remove(0, 1);

      //id={0}&secaction={1}&sc={2}&navurl={3}
      foreach (string nameValuePair in queryString.Split(new char[] { '&' }))
      {
        string[] nameOrValue = nameValuePair.Split(new char[] { '=' });

        if (nameOrValue.Length < 2)
        {
          return false;
        }

        switch (nameOrValue[0])
        {
          case "id":
            {
              int aid;
              if (Int32.TryParse(nameOrValue[1], out aid))
                _albumId = aid;
              else
                return false;
              break;
            }
          case "secaction":
            {
              int secActionInt;
              if (Int32.TryParse(nameOrValue[1], out secActionInt))
              {
                if (SecurityActionEnumHelper.IsValidSecurityAction((SecurityActions)secActionInt))
                {
                  _securityAction = (SecurityActions)secActionInt; break;
                }
                else
                  return false;
              }
              else
                return false;
            }
          case "sc":
            {
              bool showCheckbox;
              if (Boolean.TryParse(nameOrValue[1], out showCheckbox))
                _showCheckbox = showCheckbox;
              else
                return false;
              break;
            }
          case "navurl":
            {
              _navigateUrl = Utils.UrlDecode(nameOrValue[1]).Trim();
              break;
            }
          default: return false; // Unexpected query string parm. Return false so execution is aborted.
        }
      }

      return true;
    }

    #endregion
  }
}
