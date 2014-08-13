using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business
{
  /// <summary>
  /// An object that specifies options for retrieving gallery objects. Used in conjunction with the
  /// <see cref="TagSearcher" /> class.
  /// </summary>
  public class TagSearchOptions
  {
    /// <summary>
    /// Identifies a particular property in the <see cref="Business.Entity.Tag" /> class.
    /// </summary>
    public enum TagProperty
    {
      /// <summary>
      /// Indicates that no tag property has been specified.
      /// </summary>
      NotSpecified = 0,

      /// <summary>
      /// The <see cref="Business.Entity.Tag.Value" /> property.
      /// </summary>
      Value,

      /// <summary>
      /// The <see cref="Business.Entity.Tag.Count" /> property.
      /// </summary>
      Count,
    }

    /// <summary>
    /// Specifies the type of tag search.
    /// </summary>
    public TagSearchType SearchType;

    /// <summary>
    /// The gallery ID. Only items in this gallery are returned.
    /// </summary>
    public int GalleryId;

    public string SearchTerm;

    /// <summary>
    /// The roles the current user belongs to. Required when <see cref="IsUserAuthenticated" />=<c>true</c>; 
    /// otherwise, the value can be left null.
    /// </summary>
    public IGalleryServerRoleCollection Roles;

    /// <summary>
    /// Indicates whether the current user has been authenticated.
    /// </summary>
    public bool IsUserAuthenticated;

    /// <summary>
    /// Indicates the number tags to retrieve. Values less than zero are treated the same as zero. Specify 
    /// <see cref="int.MaxValue" /> to return all tags.
    /// </summary>
    public int NumTagsToRetrieve;

    /// <summary>
    /// Indicates which property of the <see cref="Business.Entity.Tag" /> class to sort by.
    /// </summary>
    public TagProperty SortProperty;

    /// <summary>
    /// Indicates whether to sort the tags in ascending order. When <c>false</c>, tags are sorted in
    /// descending order on the property specified by <see cref="SortProperty" />.
    /// </summary>
    public bool SortAscending;

    /// <summary>
    /// Indicates whether the tag tree is shown expanded or collapsed.
    /// </summary>
    public bool TagTreeIsExpanded;
  }
}
