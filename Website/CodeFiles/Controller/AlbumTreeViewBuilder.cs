using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Web.Entity;

namespace GalleryServerPro.Web.Controller
{
  /// <summary>
  /// Contains functionality for creating <see cref="TreeView" /> instances.
  /// </summary>
  public class AlbumTreeViewBuilder
  {
    #region Fields

    private readonly TreeViewOptions _tvOptions;
    private TreeView _tv;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumTreeViewBuilder"/> class.
    /// </summary>
    /// <param name="tvOptions">The treeview options.</param>
    private AlbumTreeViewBuilder(TreeViewOptions tvOptions)
    {
      _tvOptions = tvOptions;
      _tv = new TreeView();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Generates a <see cref="TreeView" /> instance corresponding to the settings specified in <paramref name="tvOptions" />.
    /// </summary>
    /// <param name="tvOptions">The treeview options.</param>
    /// <returns>An instance of <see cref="TreeView" />.</returns>
    public static TreeView GetAlbumsAsTreeView(TreeViewOptions tvOptions)
    {
      AlbumTreeViewBuilder tvBuilder = new AlbumTreeViewBuilder(tvOptions);
      return tvBuilder.Generate();
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Render the treeview with the first two levels of albums that are viewable to the logged on user.
    /// </summary>
    private TreeView Generate()
    {
      _tv = new TreeView
            {
              EnableCheckBoxPlugin = _tvOptions.EnableCheckboxPlugin
            };

      foreach (IAlbum rootAlbum in GetRootAlbums())
      {
        // Add root node.
        TreeNode rootNode = new TreeNode();

        string albumTitle = GetRootAlbumTitle(rootAlbum);
        rootNode.Text = albumTitle;
        rootNode.ToolTip = albumTitle;
        rootNode.Id = String.Concat("tv_", rootAlbum.Id.ToString(CultureInfo.InvariantCulture));
        rootNode.DataId = rootAlbum.Id.ToString(CultureInfo.InvariantCulture);
        rootNode.Expanded = true;
        rootNode.AddCssClass("jstree-root-node");

        if (!String.IsNullOrEmpty(_tvOptions.NavigateUrl))
        {
          var url = rootAlbum.IsVirtualAlbum ? _tvOptions.NavigateUrl : Utils.AddQueryStringParameter(_tvOptions.NavigateUrl, String.Concat("aid=", rootAlbum.Id.ToString(CultureInfo.InvariantCulture)));
          rootNode.NavigateUrl = url;
        }

        if (_tvOptions.EnableCheckboxPlugin)
        {
          rootNode.ShowCheckBox = !rootAlbum.IsVirtualAlbum && Utils.IsUserAuthorized(_tvOptions.RequiredSecurityPermissions, RoleController.GetGalleryServerRolesForUser(), rootAlbum.Id, rootAlbum.GalleryId, rootAlbum.IsPrivate, SecurityActionsOption.RequireOne, rootAlbum.IsVirtualAlbum);
          rootNode.Selectable = rootNode.ShowCheckBox;
        }
        else
        {
          rootNode.Selectable = true;
        }
        //if (!rootNode.Selectable) rootNode.HoverCssClass = String.Empty;

        // Select and check this node if needed.
        if (_tvOptions.SelectedAlbumIds.Contains(rootAlbum.Id))
        {
          rootNode.Selected = true;
        }

        _tv.Nodes.Add(rootNode);

        // Add the first level of albums below the root album.
        BindAlbumToTreeview(rootAlbum.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).ToSortedList(), rootNode, false);

        // Only display the root node if it is selectable or we added any children to it; otherwise, remove it.
        if (!rootNode.Selectable && rootNode.Nodes.Count == 0)
        {
          _tv.Nodes.Remove(rootNode);
        }
      }

      // Make sure all specified albums are visible and checked.
      foreach (int albumId in _tvOptions.SelectedAlbumIds)
      {
        IAlbum album = AlbumController.LoadAlbumInstance(albumId, false);
        if (Utils.IsUserAuthorized(_tvOptions.RequiredSecurityPermissions, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, album.IsPrivate, SecurityActionsOption.RequireOne, album.IsVirtualAlbum))
        {
          BindSpecificAlbumToTreeview(album);
        }
      }

      return _tv;
    }

    /// <summary>
    /// Add the collection of albums to the specified treeview node.
    /// </summary>
    /// <param name="albums">The collection of albums to add the the treeview node.</param>
    /// <param name="parentNode">The treeview node that will receive child nodes representing the specified albums.</param>
    /// <param name="expandNode">Specifies whether the nodes should be expanded.</param>
    private void BindAlbumToTreeview(IEnumerable<IGalleryObject> albums, TreeNode parentNode, bool expandNode)
    {
      foreach (IAlbum album in albums)
      {
        TreeNode node = new TreeNode();
        string albumTitle = Utils.RemoveHtmlTags(album.Title);
        node.Text = albumTitle;
        node.ToolTip = albumTitle;
        node.Id = String.Concat("tv_", album.Id.ToString(CultureInfo.InvariantCulture));
        node.DataId = album.Id.ToString(CultureInfo.InvariantCulture);
        node.Expanded = expandNode;

        if (!String.IsNullOrEmpty(_tvOptions.NavigateUrl))
        {
          node.NavigateUrl = Utils.AddQueryStringParameter(_tvOptions.NavigateUrl, String.Concat("aid=", album.Id.ToString(CultureInfo.InvariantCulture)));
        }

        if (_tvOptions.EnableCheckboxPlugin && !parentNode.ShowCheckBox)
        {
          node.ShowCheckBox = !album.IsVirtualAlbum && Utils.IsUserAuthorized(_tvOptions.RequiredSecurityPermissions, RoleController.GetGalleryServerRolesForUser(), album.Id, album.GalleryId, album.IsPrivate, SecurityActionsOption.RequireOne, album.IsVirtualAlbum);
          node.Selectable = node.ShowCheckBox;
        }
        else
        {
          node.Selectable = true;
        }

        if (album.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).Any())
        {
          node.HasChildren = true;
        }

        // Select and check this node if needed.
        if (_tvOptions.SelectedAlbumIds.Contains(album.Id))
        {
          node.Expanded = true;
          node.Selected = true;
          // Expand the child of the selected album.
          BindAlbumToTreeview(album.GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).ToSortedList(), node, false);
        }

        parentNode.Nodes.Add(node);
      }
    }

    /// <summary>
    /// Bind the specified album to the treeview. This method assumes the treeview has at least the root node already
    /// built. The specified album can be at any level in the hierarchy. Nodes between the album and the existing top node
    /// are automatically created so that the full node path to the album is shown.
    /// </summary>
    /// <param name="album">An album to be added to the treeview.</param>
    private void BindSpecificAlbumToTreeview(IAlbum album)
    {
      if (_tv.FindNodeByDataId(album.Id.ToString(CultureInfo.InvariantCulture)) == null)
      {
        // Get a stack of albums that go from the current album to the top level album.
        // Once the stack is built we'll then add these albums to the treeview so that the full heirarchy
        // to the current album is shown.
        TreeNode existingParentNode;
        Stack<IAlbum> albumParents = GetAlbumsBetweenTopLevelNodeAndAlbum(album, out existingParentNode);

        if (existingParentNode == null)
          return;

        BindSpecificAlbumToTreeview(existingParentNode, albumParents);
      }
    }

    /// <summary>
    /// Bind the hierarchical list of albums to the specified treeview node.
    /// </summary>
    /// <param name="existingParentNode">The treeview node to add the first album in the stack to.</param>
    /// <param name="albumParents">A list of albums where the first album should be a child of the specified treeview
    /// node, and each subsequent album is a child of the previous album.</param>
    private void BindSpecificAlbumToTreeview(TreeNode existingParentNode, Stack<IAlbum> albumParents)
    {
      // Assumption: The first album in the stack is a child of the existingParentNode node.
      existingParentNode.Expanded = true;

      // For each album in the heirarchy of albums to the current album, add the album and all its siblings to the 
      // treeview.
      foreach (IAlbum album in albumParents)
      {
        if (existingParentNode.Nodes.Count == 0)
        {
          // Add all the album's siblings to the treeview.
          var childAlbums = AlbumController.LoadAlbumInstance(Convert.ToInt32(existingParentNode.DataId, CultureInfo.InvariantCulture), true).GetChildGalleryObjects(GalleryObjectType.Album, !Utils.IsAuthenticated).ToSortedList();
          BindAlbumToTreeview(childAlbums, existingParentNode, false);
        }

        // Now find the album in the siblings we just added that matches the current album in the stack.
        // Set that album as the new parent and expand it.
        TreeNode nodeInAlbumHeirarchy = null;
        foreach (TreeNode node in existingParentNode.Nodes)
        {
          if (node.DataId.Equals(album.Id.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
          {
            nodeInAlbumHeirarchy = node;
            nodeInAlbumHeirarchy.Expanded = true;
            break;
          }
        }

        if (nodeInAlbumHeirarchy == null)
          throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Album ID {0} is not a child of the treeview node representing album ID {1}.", album.Id, Convert.ToInt32(existingParentNode.DataId, CultureInfo.InvariantCulture)));

        existingParentNode = nodeInAlbumHeirarchy;
      }
      existingParentNode.Expanded = false;
    }

    /// <summary>
    /// Retrieve a list of albums that are in the heirarchical path between the specified album and a node in the treeview.
    /// The node that is discovered as the ancestor of the album is assigned to the existingParentNode parameter.
    /// </summary>
    /// <param name="album">An album. This method navigates the ancestors of this album until it finds a matching node in the treeview.</param>
    /// <param name="existingParentNode">The existing node in the treeview that is an ancestor of the specified album is assigned to
    /// this parameter.</param>
    /// <returns>Returns a list of albums where the first album (the one returned by calling Pop) is a child of the album 
    /// represented by the existingParentNode treeview node, and each subsequent album is a child of the previous album.
    /// The final album is the same album specified in the album parameter.</returns>
    private Stack<IAlbum> GetAlbumsBetweenTopLevelNodeAndAlbum(IAlbum album, out TreeNode existingParentNode)
    {
      if (_tv.Nodes.Count == 0)
        throw new ArgumentException("The treeview must have at least one top-level node before calling the function GetAlbumsBetweenTopLevelNodeAndAlbum().");

      Stack<IAlbum> albumParents = new Stack<IAlbum>();
      albumParents.Push(album);

      IAlbum parentAlbum = (IAlbum) album.Parent;

      albumParents.Push(parentAlbum);

      // Navigate up from the specified album until we find an album that exists in the treeview. Remember,
      // the treeview has been built with the root node and the first level of albums, so eventually we
      // should find an album. If not, just return without showing the current album.
      while ((existingParentNode = _tv.FindNodeByDataId(parentAlbum.Id.ToString(CultureInfo.InvariantCulture))) == null)
      {
        parentAlbum = parentAlbum.Parent as IAlbum;

        if (parentAlbum == null)
          break;

        albumParents.Push(parentAlbum);
      }

      // Since we found a node in the treeview we don't need to add the most recent item in the stack. Pop it off.
      albumParents.Pop();

      return albumParents;
    }

    /// <summary>
    /// Gets a list of top-level albums to display in the treeview. There will be a maximum of one for each gallery.
    /// If the <see cref="TreeViewOptions.RootAlbumId" /> property is assigned, that album is returned and the <see cref="Galleries" /> property is 
    /// ignored.
    /// </summary>
    /// <returns>Returns a list of top-level albums to display in the treeview.</returns>
    private IEnumerable<IAlbum> GetRootAlbums()
    {
      List<IAlbum> rootAlbums = new List<IAlbum>(1);

      if (_tvOptions.RootAlbumId > 0)
      {
        rootAlbums.Add(AlbumController.LoadAlbumInstance(_tvOptions.RootAlbumId, true));
      }
      else
      {
        foreach (IGallery gallery in _tvOptions.Galleries)
        {
          var rootAlbum = Factory.LoadRootAlbum(gallery.GalleryId, RoleController.GetGalleryServerRolesForUser(), Utils.IsAuthenticated);

          if (rootAlbum != null)
            rootAlbums.Add(rootAlbum);
        }
      }

      return rootAlbums;
    }

    private string GetRootAlbumTitle(IAlbum rootAlbum)
    {
      IGallery gallery = Factory.LoadGallery(rootAlbum.GalleryId);
      string rootAlbumPrefix = _tvOptions.RootAlbumPrefix.Replace("{GalleryId}", gallery.GalleryId.ToString(CultureInfo.InvariantCulture)).Replace("{GalleryDescription}", gallery.Description);
      return Utils.RemoveHtmlTags(String.Concat(rootAlbumPrefix, rootAlbum.Title));
    }

    #endregion

  }
}