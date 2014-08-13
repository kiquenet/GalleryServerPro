using System;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business
{
	/// <summary>
	/// An object for passing information between an instance raising the 
	/// <see cref="GalleryObject.BeforeAddMetaItem" /> event and an instance consuming it.
	/// </summary>
	public class AddMetaEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AddMetaEventArgs"/> class.
		/// </summary>
		/// <param name="metaItem"> </param>
		public AddMetaEventArgs(IGalleryObjectMetadataItem metaItem)
		{
			MetaItem = metaItem;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the meta item should not be added to the
		/// gallery object.
		/// </summary>
		/// <value>
		///   <c>true</c> if cancelled; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }

		/// <summary>
		/// Gets or sets a metadata item for a gallery object.
		/// </summary>
		/// <value>An instance of <see cref="IGalleryObjectMetadataItem" />.</value>
		public IGalleryObjectMetadataItem MetaItem { get; set; }
	}
}