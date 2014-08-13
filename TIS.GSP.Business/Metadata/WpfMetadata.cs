using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business.Metadata
{
	/// <summary>
	/// Contains functionality for interacting with a file's metadata through the WPF classes.
	/// Essentially it is a wrapper for the <see cref="BitmapMetadata" /> class.
	/// </summary>
	internal class WpfMetadata : IWpfMetadata
	{
		private BitmapMetadata Metadata { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WpfMetadata" /> class.
		/// </summary>
		/// <param name="bitmapMetadata">An object containing the metadata.</param>
		public WpfMetadata(BitmapMetadata bitmapMetadata)
		{
			Metadata = bitmapMetadata;
		}

		/// <summary>
		/// Gets or sets a value that indicates the date that the image was taken.
		/// </summary>
		/// <value>A string.</value>
		public string DateTaken { get { return Metadata.DateTaken; } set { Metadata.DateTaken = value; } }

		/// <summary>
		/// Gets or sets a value that indicates the title of an image file.
		/// </summary>
		/// <value>A string.</value>
		public string Title { get { return Metadata.Title; } set { Metadata.Title = value; } }

		/// <summary>
		/// Gets or sets a value that indicates the author of an image.
		/// </summary>
		/// <value>A collection.</value>
		public ReadOnlyCollection<string> Author { get { return Metadata.Author; } set { Metadata.Author = value; } }

		/// <summary>
		/// Gets or sets a value that identifies the camera model that was used to capture the image.
		/// </summary>
		/// <value>A string.</value>
		public string CameraModel { get { return Metadata.CameraModel; } set { Metadata.CameraModel = value; } }

		/// <summary>
		/// Gets or sets a value that identifies the camera manufacturer that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		public string CameraManufacturer { get { return Metadata.CameraManufacturer; } set { Metadata.CameraManufacturer = value; } }

		/// <summary>
		/// Gets or sets a collection of keywords that describe the image.
		/// </summary>
		/// <value>A collection.</value>
		public ReadOnlyCollection<string> Keywords { get { return Metadata.Keywords; } set { Metadata.Keywords = value; } }

		/// <summary>
		/// Gets or sets a value that identifies the image rating.
		/// </summary>
		/// <value>An integer.</value>
		public int Rating { get { return Metadata.Rating; } set { Metadata.Rating = value; } }

		/// <summary>
		/// Gets or sets a value that identifies a comment that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		public string Comment { get { return Metadata.Comment; } set { Metadata.Comment = value; } }

		/// <summary>
		/// Gets or sets a value that identifies copyright information that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		public string Copyright { get { return Metadata.Copyright; } set { Metadata.Copyright = value; } }

		/// <summary>
		/// Gets or sets a value that indicates the subject matter of an image.
		/// </summary>
		/// <value>A string.</value>
		public string Subject { get { return Metadata.Subject; } set { Metadata.Subject = value; } }

		/// <summary>
		/// Provides access to a metadata query reader that can extract metadata from a bitmap image file.
		/// </summary>
		/// <param name="query">Identifies the string that is being queried in the current object.</param>
		/// <returns>The metadata at the specified query location.</returns>
		/// <exception cref="ArgumentNullException">Thrown when query is null.</exception>
		public object GetQuery(string query)
		{
			return Metadata.GetQuery(query);
		}
	}
}