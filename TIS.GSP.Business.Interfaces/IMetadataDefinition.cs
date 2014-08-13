using System;
using GalleryServerPro.Business.Metadata;

namespace GalleryServerPro.Business.Interfaces
{
	/// <summary>
	/// Represents the definition of a type of metadata that is associated with media objects. Note that this is not an actual
	/// piece of metadata, but rather defines the behavior of metadata stored in <see cref="IGalleryObjectMetadataItem" />.
	/// </summary>
	public interface IMetadataDefinition : IComparable<IMetadataDefinition>
	{
		/// <summary>
		/// Gets or sets the name of the metadata item.
		/// </summary>
		/// <value>The metadata item.</value>
		MetadataItemName MetadataItem { get; set; }

		/// <summary>
		/// Gets the string representation of the <see cref="MetadataItem" /> property.
		/// </summary>
		/// <value>A string.</value>
		string Name { get; }

		/// <summary>
		/// Gets or sets the user-friendly name to apply to this metadata item.
		/// </summary>
		/// <value>A string.</value>
		string DisplayName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether metadata items of this type are visible for albums.
		/// </summary>
		/// <value><c>true</c> if metadata items of this type are visible for albums; otherwise, <c>false</c>.</value>
		bool IsVisibleForAlbum { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether metadata items of this type are visible for gallery objects.
		/// </summary>
		/// <value><c>true</c> if metadata items of this type are visible for gallery objects; otherwise, <c>false</c>.</value>
		bool IsVisibleForGalleryObject { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this metadata item can be edited by the user. The user
		/// must also have permission to edit the album or media object.
		/// </summary>
		/// <value><c>true</c> if this metadata item can be edited by the user; otherwise, <c>false</c>.</value>
		bool IsEditable { get; set; }

		/// <summary>
		/// Gets or sets the template to use when adding a metadata item for a new album or media object.
		/// Values of the <see cref="MetadataItemName" /> can be used as replacement parameters.
		/// Example: "{IsoSpeed} - {LensAperture}"
		/// </summary>
		/// <value>A string.</value>
		string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the order this metadata item is to be displayed in relation to other metadata items.
		/// </summary>
		/// <value>The order this metadata item is to be displayed in relation to other metadata items.</value>
		int Sequence { get; set; }

		/// <summary>
		/// Gets or sets the gallery ID this metadata definition is associated with.
		/// </summary>
		/// <value>The gallery ID this metadata definition is associated with.</value>
		//int GalleryId { get; set; }

		/// <summary>
		/// Gets the data type of the metadata item. Returns either <see cref="DateTime" /> or <see cref="System.String" />.
		/// </summary>
		/// <value>The type of the metadata item.</value>
		Type DataType { get; }
	}
}