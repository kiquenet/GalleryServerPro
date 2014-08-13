using System;
using System.Collections.ObjectModel;

namespace GalleryServerPro.Business.Interfaces
{
	/// <summary>
	/// Contains functionality for interacting with a file's metadata through the WPF classes.
	/// </summary>
	public interface IWpfMetadata
	{
		/// <summary>
		/// Gets or sets a value that indicates the date that the image was taken.
		/// </summary>
		/// <value>A string.</value>
		string DateTaken { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates the title of an image file.
		/// </summary>
		/// <value>A string.</value>
		string Title { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates the author of an image.
		/// </summary>
		/// <value>A collection.</value>
		ReadOnlyCollection<string> Author { get; set; }

		/// <summary>
		/// Gets or sets a value that identifies the camera model that was used to capture the image.
		/// </summary>
		/// <value>A string.</value>
		string CameraModel { get; set; }

		/// <summary>
		/// Gets or sets a value that identifies the camera manufacturer that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		string CameraManufacturer { get; set; }

		/// <summary>
		/// Gets or sets a collection of keywords that describe the image.
		/// </summary>
		/// <value>A collection.</value>
		ReadOnlyCollection<string> Keywords { get; set; }

		/// <summary>
		/// Gets or sets a value that identifies the image rating.
		/// </summary>
		/// <value>An integer.</value>
		int Rating { get; set; }

		/// <summary>
		/// Gets or sets a value that identifies a comment that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		string Comment { get; set; }

		/// <summary>
		/// Gets or sets a value that identifies copyright information that is associated with an image.
		/// </summary>
		/// <value>A string.</value>
		string Copyright { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates the subject matter of an image.
		/// </summary>
		/// <value>A string.</value>
		string Subject { get; set; }

		/// <summary>
		/// Provides access to a metadata query reader that can extract metadata from a bitmap image file.
		/// </summary>
		/// <param name="query">Identifies the string that is being queried in the current object.</param>
		/// <returns>The metadata at the specified query location.</returns>
		/// <exception cref="ArgumentNullException">Thrown when query is null.</exception>
		object GetQuery(string query);
	}
}