using GalleryServerPro.Business.Metadata;

namespace GalleryServerPro.Web.Entity
{
	/// <summary>
	/// A simple object for specifying actions to take on an album.
	/// </summary>
	public class AlbumAction
	{
		public Entity.Album Album { get; set; }
		public MetadataItemName SortByMetaNameId { get; set; }
		public bool SortAscending { get; set; }
	}
}