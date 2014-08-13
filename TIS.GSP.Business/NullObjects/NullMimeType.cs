using System;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business.NullObjects
{
	/// <summary>
	/// Represents a <see cref="IMimeType" /> that is equivalent to null. This class is used instead of null to prevent 
	/// <see cref="NullReferenceException" /> errors if the calling code accesses a property or executes a method.
	/// </summary>
	public class NullMimeType : IMimeType
  {
		public int MimeTypeId
		{
			get { return int.MinValue; }
			set { }
		}

		public int MimeTypeGalleryId
		{
			get { return int.MinValue; }
			set {  }
		}

		public int GalleryId
		{
			get { return int.MinValue; }
			set {  }
		}

		public string Extension
    {
      get { return string.Empty; }
    }

    public string FullType
    {
      get { return string.Empty; }
		}

		public string MajorType
		{
			get { return string.Empty; }
		}

		public string Subtype
    {
      get { return string.Empty; }
    }

    public MimeTypeCategory TypeCategory
    {
      get { return MimeTypeCategory.NotSet; }
    }

		public string BrowserMimeType
		{
			get { return string.Empty; }
		}

		public bool AllowAddToGallery
		{
			get { return false; }
			set {  }
		}

		public IMediaTemplateCollection MediaTemplates
		{
			get { return new MediaTemplateCollection(); }
		}

		public IMimeType Copy()
		{
			return new NullMimeType();
		}

		public void Save()
		{
		}

		public IMediaTemplate GetMediaTemplate(Array browserIds)
		{
			return null;
		}
  }
}
