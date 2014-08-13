using System;
using System.Diagnostics;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business.Metadata
{
	/// <summary>
	/// Represents the definition of a type of metadata that is associated with media objects. Note that this is not an actual
	/// piece of metadata, but rather defines the behavior of metadata stored in <see cref="IGalleryObjectMetadataItem" />.
	/// </summary>
	[DebuggerDisplay("\"{MetadataItem}\", VisibleForGalleryObject={IsVisibleForGalleryObject}, Seq={Sequence}")]
	public class MetadataDefinition : IMetadataDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataDefinition" /> class.
		/// </summary>
		/// <param name="metadataItem">The metadata item.</param>
		/// <param name="displayName">The user-friendly name that describes this metadata item (e.g. "Date picture taken")</param>
		/// <param name="isVisibleForAlbum">If set to <c>true</c> metadata items belonging to albums are visible
		/// in the user interface.</param>
		/// <param name="isVisibleForGalleryObject">If set to <c>true</c> metadata items belonging to media
		/// objects are visible in the user interface.</param>
		/// <param name="isEditable">If set to <c>true</c> metadata items of this type are editable by the user.</param>
		/// <param name="sequence">Indicates the display order of the metadata item.</param>
		/// <param name="defaultValue">The template to use when adding a metadata item for a new album or media object.</param>
		public MetadataDefinition(MetadataItemName metadataItem, string displayName, bool isVisibleForAlbum, bool isVisibleForGalleryObject, bool isEditable, int sequence, string defaultValue)
		{
			MetadataItem = metadataItem;
			DisplayName = displayName;
			IsVisibleForAlbum= isVisibleForAlbum;
			IsVisibleForGalleryObject = isVisibleForGalleryObject;
			IsEditable = isEditable;
			Sequence = sequence;
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets or sets the name of the metadata item.
		/// </summary>
		/// <value>The metadata item.</value>
		public MetadataItemName MetadataItem { get; set; }

		/// <summary>
		/// Gets the string representation of the <see cref="MetadataItem" /> property.
		/// </summary>
		/// <value>A string.</value>
		public string Name { get { return MetadataItem.ToString(); } }

		/// <summary>
		/// Gets or sets the user-friendly name to apply to this metadata item.
		/// </summary>
		/// <value>A string.</value>
		public string DisplayName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether metadata items of this type are visible for albums.
		/// </summary>
		/// <value><c>true</c> if metadata items of this type are visible for albums; otherwise, <c>false</c>.</value>
		public bool IsVisibleForAlbum { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether metadata items of this type are visible in the gallery.
		/// </summary>
		/// <value><c>true</c> if metadata items of this type are visible in the gallery; otherwise, <c>false</c>.</value>
		public bool IsVisibleForGalleryObject { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this metadata item can be edited by the user. The user
		/// must also have permission to edit the album or media object.
		/// </summary>
		/// <value><c>true</c> if this metadata item can be edited by the user; otherwise, <c>false</c>.</value>
		public bool IsEditable { get; set; }

		/// <summary>
		/// Gets or sets the template to use when adding a metadata item for a new album or media object.
		/// Values of the <see cref="MetadataItemName" /> can be used as replacement parameters.
		/// Example: "{IsoSpeed} - {LensAperture}"
		/// </summary>
		/// <value>A string.</value>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the order this metadata item is to be displayed in relation to other metadata items.
		/// </summary>
		/// <value>The order this metadata item is to be displayed in relation to other metadata items.</value>
		public int Sequence { get; set; }

		/// <summary>
		/// Gets or sets the gallery ID this metadata definition is associated with.
		/// </summary>
		/// <value>The gallery ID this metadata definition is associated with.</value>
		//[Newtonsoft.Json.JsonIgnore]
		//public int GalleryId { get; set; }

		/// <summary>
		/// Gets the data type of the metadata item. Returns either <see cref="DateTime" /> or <see cref="System.String" />.
		/// </summary>
		/// <value>The type of the metadata item.</value>
		[Newtonsoft.Json.JsonIgnore]
		public Type DataType
		{
			get
			{
				switch (MetadataItem)
				{
					case MetadataItemName.DateAdded:
					case MetadataItemName.DateFileCreated:
					case MetadataItemName.DateFileCreatedUtc:
					case MetadataItemName.DateFileLastModified:
					case MetadataItemName.DateFileLastModifiedUtc:
					case MetadataItemName.DatePictureTaken:
						return typeof(DateTime);
					default:
						return typeof(String);
				}
			}
		}

		#region IComparable

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
		/// </returns>
		public int CompareTo(IMetadataDefinition other)
		{
			if (other == null)
				return 1;
			else
			{
				return Sequence.CompareTo(other.Sequence);
			}
		}

		#endregion

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="MetadataDefinition"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return MetadataItem.GetHashCode();
		}
	}
}
