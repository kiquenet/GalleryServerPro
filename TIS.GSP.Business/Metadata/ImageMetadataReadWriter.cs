using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Properties;
using GalleryServerPro.Events;

namespace GalleryServerPro.Business.Metadata
{
	/// <summary>
	/// Provides functionality for reading and writing metadata to or from a gallery object.
	/// </summary>
	public class ImageMetadataReadWriter : MediaObjectMetadataReadWriter
	{
		#region Fields

		private enum MetaPersistAction
		{
			Delete = 1,
			Save = 2
		}

		private const uint MetadataPaddingInBytes = 2048;

		private PropertyItem[] _propertyItems;
		private IWpfMetadata _wpfMetadata;
		private int _width, _height;
		private Dictionary<RawMetadataItemName, MetadataItem> _rawMetadata;
		private GpsLocation _gpsLocation;
		private static Dictionary<MetadataItemName, string> _iptcQueryParameters;
		private static Dictionary<MetadataItemName, string> _updatableMetaItems;
		private static readonly object _sharedLock = new Object();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the property items associated with the image file. Guaranteed to not return null.
		/// </summary>
		/// <value>An array of <see cref="PropertyItem" /> instances.</value>
		private IEnumerable<PropertyItem> PropertyItems
		{
			get { return _propertyItems ?? (_propertyItems = GetImagePropertyItems()); }
		}

		/// <summary>
		/// Gets the raw metadata associated with the current image file. Guaranteed to not return null.
		/// </summary>
		/// <value>The raw metadata associated with the current image file.</value>
		private Dictionary<RawMetadataItemName, MetadataItem> RawMetadata
		{
			get { return _rawMetadata ?? (_rawMetadata = GetRawMetadataDictionary()); }
		}

		/// <summary>
		/// Gets an object that can extract metadata from a media file using the .NET WPF classes.
		/// Guaranteed to not return null.
		/// </summary>
		/// <value>An instance of <see cref="WpfMetadata" /> when possible; otherwise 
		/// <see cref="NullObjects.NullWpfMetadata" />.</value>
		private IWpfMetadata WpfMetadata
		{
			get { return _wpfMetadata ?? (_wpfMetadata = GetBitmapMetadata()); }
		}

		/// <summary>
		/// Gets an object that can retrieve GPS-related data from a media file.
		/// </summary>
		/// <value>An instance of <see cref="GpsLocation" />.</value>
		private GpsLocation GpsLocation
		{
			get { return _gpsLocation ?? (_gpsLocation = GpsLocation.Parse(WpfMetadata)); }
		}

		/// <summary>
		/// Gets the query format string to be used for extracting IPTC data from a media file.
		/// Example: "/app13/irb/8bimiptc/iptc/{{str={0}}}"
		/// </summary>
		/// <value>A string.</value>
		private static string IptcQueryFormatString
		{
			get { return "/app13/irb/8bimiptc/iptc/{{str={0}}}"; }
		}

		/// <summary>
		/// Gets a collection of query parameters for extracting IPTC data from a media file.
		/// The key identifies the metadata item. The value is the query identifier that is combined
		/// with the <see cref="IptcQueryFormatString" /> to create a query string that can be
		/// passed to the <see cref="BitmapMetadata.GetQuery" /> method.
		/// </summary>
		/// <value>A Dictionary object.</value>
		private static Dictionary<MetadataItemName, string> IptcQueryParameters
		{
			get
			{
				if (_iptcQueryParameters == null)
				{
					lock (_sharedLock)
					{
						if (_iptcQueryParameters == null)
						{
							var tmp = new Dictionary<MetadataItemName, string>();

							tmp.Add(MetadataItemName.IptcByline, "By-Line");
							tmp.Add(MetadataItemName.IptcBylineTitle, "By-line Title");
							tmp.Add(MetadataItemName.IptcCaption, "Caption");
							tmp.Add(MetadataItemName.IptcCity, "City");
							tmp.Add(MetadataItemName.IptcCopyrightNotice, "Copyright Notice");
							tmp.Add(MetadataItemName.IptcCountryPrimaryLocationName, "Country/Primary Location Name");
							tmp.Add(MetadataItemName.IptcCredit, "Credit");
							tmp.Add(MetadataItemName.IptcDateCreated, "Date Created");
							tmp.Add(MetadataItemName.IptcHeadline, "Headline");
							tmp.Add(MetadataItemName.IptcKeywords, "Keywords");
							tmp.Add(MetadataItemName.IptcObjectName, "Object Name");
							tmp.Add(MetadataItemName.IptcOriginalTransmissionReference, "Original Transmission Reference");
							tmp.Add(MetadataItemName.IptcProvinceState, "Province/State");
							tmp.Add(MetadataItemName.IptcRecordVersion, "Record Version");
							tmp.Add(MetadataItemName.IptcSource, "Source");
							tmp.Add(MetadataItemName.IptcSpecialInstructions, "Special Instructions");
							tmp.Add(MetadataItemName.IptcSublocation, "Sub-location");
							tmp.Add(MetadataItemName.IptcWriterEditor, "Writer/Editor");

							Thread.MemoryBarrier();

							_iptcQueryParameters = tmp;
						}
					}
				}

				return _iptcQueryParameters;
			}
		}

		/// <summary>
		/// Gets a collection of meta items that can be updated in the orginal file. The key identifies the 
		/// metadata item. The value is the query that can be passed to any of the <see cref="BitmapMetadata" /> methods.
		/// </summary>
		/// <value>A Dictionary object.</value>
		private static Dictionary<MetadataItemName, string> UpdatableMetaItems
		{
			get
			{
				if (_updatableMetaItems == null)
				{
					lock (_sharedLock)
					{
						if (_updatableMetaItems == null)
						{
							var tmp = new Dictionary<MetadataItemName, string>
								          {
									          { MetadataItemName.Orientation, "/app1/ifd/{ushort=274}" }
								          };

							Thread.MemoryBarrier();

							_updatableMetaItems = tmp;
						}
					}
				}

				return _updatableMetaItems;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaObjectMetadataReadWriter" /> class.
		/// </summary>
		/// <param name="mediaObject">The media object.</param>
		public ImageMetadataReadWriter(IGalleryObject mediaObject)
			: base(mediaObject)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the metadata value for the specified <paramref name="metaName" />. May return null.
		/// </summary>
		/// <param name="metaName">Name of the metadata item to retrieve.</param>
		/// <returns>An instance that implements <see cref="IMetaValue" />.</returns>
		public override IMetaValue GetMetaValue(MetadataItemName metaName)
		{
			switch (metaName)
			{
				case MetadataItemName.Title: return GetTitle();
				case MetadataItemName.DatePictureTaken: return GetDatePictureTaken();
				case MetadataItemName.Author: return GetAuthor();
				case MetadataItemName.CameraModel: return GetCameraModel();
				case MetadataItemName.EquipmentManufacturer: return GetCameraManufacturer();
				case MetadataItemName.Tags: return GetKeywords();
				case MetadataItemName.Rating: return GetRating();
				case MetadataItemName.Comment: return GetComment();
				case MetadataItemName.Copyright: return GetCopyright();
				case MetadataItemName.Subject: return GetSubject();
				case MetadataItemName.ColorRepresentation: return GetColorRepresentation();
				case MetadataItemName.Description: return GetDescription();
				case MetadataItemName.Dimensions: return GetDimensions();
				case MetadataItemName.ExposureCompensation: return GetExposureCompensation();
				case MetadataItemName.ExposureProgram: return GetExposureProgram();
				case MetadataItemName.Orientation: return GetOrientation();
				case MetadataItemName.ExposureTime: return GetExposureTime();
				case MetadataItemName.FlashMode: return GetFlashMode();
				case MetadataItemName.FNumber: return GetFNumber();
				case MetadataItemName.FocalLength: return GetFocalLength();
				case MetadataItemName.Height: return GetHeight();
				case MetadataItemName.HorizontalResolution: return GetHorizontalResolution();
				case MetadataItemName.IsoSpeed: return GetIsoSpeed();
				case MetadataItemName.LensAperture: return GetLensAperture();
				case MetadataItemName.LightSource: return GetLightSource();
				case MetadataItemName.MeteringMode: return GetMeteringMode();
				case MetadataItemName.SubjectDistance: return GetSubjectDistance();
				case MetadataItemName.VerticalResolution: return GetVerticalResolution();
				case MetadataItemName.Width: return GetWidth();

				case MetadataItemName.GpsVersion:
				case MetadataItemName.GpsLocation:
				case MetadataItemName.GpsLatitude:
				case MetadataItemName.GpsLongitude:
				case MetadataItemName.GpsAltitude:
				case MetadataItemName.GpsDestLocation:
				case MetadataItemName.GpsDestLatitude:
				//case MetadataItemName.GpsLocationWithMapLink: // Built from template, nothing to extract
				//case MetadataItemName.GpsDestLocationWithMapLink: // Built from template, nothing to extract
				case MetadataItemName.GpsDestLongitude: return GetGpsValue(metaName);

				case MetadataItemName.IptcByline:
				case MetadataItemName.IptcBylineTitle:
				case MetadataItemName.IptcCaption:
				case MetadataItemName.IptcCity:
				case MetadataItemName.IptcCopyrightNotice:
				case MetadataItemName.IptcCountryPrimaryLocationName:
				case MetadataItemName.IptcCredit:
				case MetadataItemName.IptcDateCreated:
				case MetadataItemName.IptcHeadline:
				case MetadataItemName.IptcKeywords:
				case MetadataItemName.IptcObjectName:
				case MetadataItemName.IptcOriginalTransmissionReference:
				case MetadataItemName.IptcProvinceState:
				case MetadataItemName.IptcRecordVersion:
				case MetadataItemName.IptcSource:
				case MetadataItemName.IptcSpecialInstructions:
				case MetadataItemName.IptcSublocation:
				case MetadataItemName.IptcWriterEditor: return GetIptcValue(metaName);

				default:
					return base.GetMetaValue(metaName);
			}
		}

		/// <summary>
		/// Persists the meta value identified by <paramref name="metaName" /> to the media file. It is expected the meta item
		/// exists in <see cref="IGalleryObject.MetadataItems" />.
		/// </summary>
		/// <param name="metaName">Name of the meta item to persist.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		public override void SaveMetaValue(MetadataItemName metaName)
		{
			PersistMetaValue(metaName, MetaPersistAction.Save);
		}

		/// <summary>
		/// Permanently removes the meta value from the media file. The item is also removed from
		/// <see cref="IGalleryObject.MetadataItems" />.
		/// </summary>
		/// <param name="metaName">Name of the meta item to delete.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		public override void DeleteMetaValue(MetadataItemName metaName)
		{
			PersistMetaValue(metaName, MetaPersistAction.Delete);
		}

		#endregion

		#region Metadata functions

		private IMetaValue GetTitle()
		{
			// Look in three places for title:
			// 1. The Title property in the WPF BitmapMetadata class.
			// 2. The ImageTitle property of the GDI+ property tags.
			// 3. The filename.
			var wpfTitle = GetWpfTitle();

			if (wpfTitle != null)
				return wpfTitle;

			var title = GetStringMetadataItem(RawMetadataItemName.ImageTitle);

			return !String.IsNullOrWhiteSpace(title) ? new MetaValue(title, title) : new MetaValue(GalleryObject.Original.FileName);
		}

		private IMetaValue GetWpfTitle()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.Title;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (!String.IsNullOrWhiteSpace(wpfValue) ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetDatePictureTaken()
		{
			return GetDatePictureTakenWpf() ?? GetDatePictureTakenGdi();
		}

		private IMetaValue GetAuthor()
		{
			try
			{
				var author = ConvertStringCollectionToDelimitedString(WpfMetadata.Author);

				return new MetaValue(author, author);
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return null;
		}

		private IMetaValue GetCameraModel()
		{
			var wpfCameraModel = GetWpfCameraModel();

			if (wpfCameraModel != null)
				return wpfCameraModel;

			var cameraModel = GetStringMetadataItem(RawMetadataItemName.EquipModel);

			return !String.IsNullOrWhiteSpace(cameraModel) ? new MetaValue(cameraModel, cameraModel) : null;
		}

		private IMetaValue GetWpfCameraModel()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.CameraModel;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (wpfValue != null ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetCameraManufacturer()
		{
			var wpfCameraManufacturer = GetWpfCameraManufacturer();

			if (wpfCameraManufacturer != null)
				return wpfCameraManufacturer;

			var cameraMfg = GetStringMetadataItem(RawMetadataItemName.EquipMake);

			return !String.IsNullOrWhiteSpace(cameraMfg) ? new MetaValue(cameraMfg, cameraMfg) : null;
		}

		private IMetaValue GetWpfCameraManufacturer()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.CameraManufacturer;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (wpfValue != null ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetKeywords()
		{
			try
			{
				var keywords = ConvertStringCollectionToDelimitedString(WpfMetadata.Keywords);

				return new MetaValue(keywords, keywords);
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return null;
		}

		private IMetaValue GetRating()
		{
			try
			{
				var rating = WpfMetadata.Rating;
				return (rating > 0 ? new MetaValue(rating.ToString(CultureInfo.InvariantCulture), rating.ToString(CultureInfo.InvariantCulture)) : null);
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return null;
		}

		private IMetaValue GetComment()
		{
			var wpfComment = GetWpfComment();

			if (wpfComment != null)
				return wpfComment;

			var comment = GetStringMetadataItem(RawMetadataItemName.ExifUserComment);

			return !String.IsNullOrWhiteSpace(comment) ? new MetaValue(comment, comment) : null;
		}

		private IMetaValue GetWpfComment()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.Comment;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (wpfValue != null ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetCopyright()
		{
			var wpfCopyright = GetWpfCopyright();

			if (wpfCopyright != null)
				return wpfCopyright;

			var copyright = GetStringMetadataItem(RawMetadataItemName.Copyright);

			return !String.IsNullOrWhiteSpace(copyright) ? new MetaValue(copyright, copyright) : null;
		}

		private IMetaValue GetWpfCopyright()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.Copyright;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (wpfValue != null ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetSubject()
		{
			var wpfSubject = GetWpfSubject();

			if (wpfSubject != null)
				return wpfSubject;
			else
				return null;
		}

		private IMetaValue GetWpfSubject()
		{
			string wpfValue = null;

			try
			{
				wpfValue = WpfMetadata.Subject;
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return (wpfValue != null ? new MetaValue(wpfValue.Trim(), wpfValue) : null);
		}

		private IMetaValue GetColorRepresentation()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifColorSpace, out rawMdi))
			{
				string value = rawMdi.Value.ToString().Trim();

				if (value == "1")
					return new MetaValue(Resources.Metadata_ColorRepresentation_sRGB, value);
				else
					return new MetaValue(Resources.Metadata_ColorRepresentation_Uncalibrated, value);
			}

			return null;
		}

		private IMetaValue GetDescription()
		{
			var desc = GetStringMetadataItem(RawMetadataItemName.ImageDescription);

			return (desc != null ? new MetaValue(desc, desc) : null);
		}

		private IMetaValue GetDimensions()
		{
			int width = GetWidthAsInt();
			int height = GetHeightAsInt();

			if ((width > 0) && (height > 0))
			{
				return new MetaValue(String.Concat(width, " x ", height));
			}

			return null;
		}

		private IMetaValue GetExposureCompensation()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifExposureBias, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					float value = ((Fraction)rawMdi.Value).ToSingle();

					return new MetaValue(
						String.Concat(value.ToString("##0.# ", CultureInfo.InvariantCulture), Resources.Metadata_ExposureCompensation_Suffix),
						value.ToString(CultureInfo.InvariantCulture));
				}
			}
			return null;
		}

		private IMetaValue GetExposureProgram()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifExposureProg, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					var expProgram = (ExposureProgram)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidExposureProgram(expProgram))
					{
						return new MetaValue(expProgram.ToString(), ((ushort)expProgram).ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			return null;
		}

		private IMetaValue GetOrientation()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.Orientation, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					var orientation = (Orientation)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidOrientation(orientation))
					{
						return new MetaValue(orientation.GetDescription(), ((ushort)orientation).ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			return null;
		}

		private IMetaValue GetExposureTime()
		{
			MetadataItem rawMdi;
			const Single numSeconds = 1; // If the exposure time is less than this # of seconds, format as fraction (1/350 sec.); otherwise convert to Single (2.35 sec.)
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifExposureTime, out rawMdi))
			{
				string exposureTime;
				if ((rawMdi.ExtractedValueType == ExtractedValueType.Fraction) && ((Fraction)rawMdi.Value).ToSingle() > numSeconds)
				{
					exposureTime = Math.Round(((Fraction)rawMdi.Value).ToSingle(), 2).ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					exposureTime = rawMdi.Value.ToString();
				}

				return new MetaValue(String.Concat(exposureTime, " ", Resources.Metadata_ExposureTime_Units), exposureTime);
			}
			return null;
		}

		private IMetaValue GetFlashMode()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifFlash, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					var flashMode = (FlashMode)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidFlashMode(flashMode))
					{
						return new MetaValue(flashMode.GetDescription(), ((ushort)flashMode).ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			return null;
		}

		private IMetaValue GetFNumber()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifFNumber, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					float value = ((Fraction)rawMdi.Value).ToSingle();
					return new MetaValue(value.ToString("f/##0.#", CultureInfo.InvariantCulture), value.ToString(CultureInfo.InvariantCulture));
				}
			}
			return null;
		}

		private IMetaValue GetFocalLength()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifFocalLength, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					float value = ((Fraction)rawMdi.Value).ToSingle();
					return new MetaValue(String.Concat(Math.Round(value), " ", Resources.Metadata_FocalLength_Units), value.ToString(CultureInfo.InvariantCulture));
				}
			}
			return null;
		}

		private IMetaValue GetHeight()
		{
			int height = GetHeightAsInt();

			return (height > 0 ? new MetaValue(String.Concat(height, " ", Resources.Metadata_Height_Units), height.ToString(CultureInfo.InvariantCulture)) : null);
		}

		private IMetaValue GetHorizontalResolution()
		{
			MetadataItem rawMdi;
			string resolutionUnit = String.Empty;

			if (RawMetadata.TryGetValue(RawMetadataItemName.ResolutionXUnit, out rawMdi))
			{
				resolutionUnit = rawMdi.Value.ToString();
			}

			if ((String.IsNullOrWhiteSpace(resolutionUnit)) && (RawMetadata.TryGetValue(RawMetadataItemName.ResolutionUnit, out rawMdi)))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					ResolutionUnit resUnit = (ResolutionUnit)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidResolutionUnit(resUnit))
					{
						resolutionUnit = resUnit.ToString();
					}
				}
			}

			if (RawMetadata.TryGetValue(RawMetadataItemName.XResolution, out rawMdi))
			{
				string xResolution;
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					xResolution = Math.Round(((Fraction)rawMdi.Value).ToSingle(), 2).ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					xResolution = rawMdi.Value.ToString();
				}

				return new MetaValue(String.Concat(xResolution, " ", resolutionUnit), xResolution);
			}

			return null;
		}

		private IMetaValue GetIsoSpeed()
		{
			var iso = GetStringMetadataItem(RawMetadataItemName.ExifISOSpeed);

			return (!String.IsNullOrEmpty(iso) ? new MetaValue(iso, iso) : null);
		}

		private IMetaValue GetLensAperture()
		{
			// The aperture is the same as the F-Number if present; otherwise it is calculated from ExifAperture.
			MetadataItem rawMdi;
			string aperture = String.Empty;
			float apertureRaw = 0;

			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifFNumber, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					apertureRaw = ((Fraction)rawMdi.Value).ToSingle();
					aperture = apertureRaw.ToString("f/##0.#", CultureInfo.InvariantCulture);
				}
			}

			if ((String.IsNullOrWhiteSpace(aperture)) && (RawMetadata.TryGetValue(RawMetadataItemName.ExifAperture, out rawMdi)))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					apertureRaw = ((Fraction)rawMdi.Value).ToSingle();
					var exifFNumber = (float)Math.Round(Math.Pow(Math.Sqrt(2), apertureRaw), 1);
					aperture = exifFNumber.ToString("f/##0.#", CultureInfo.InvariantCulture);
				}
			}

			return (!String.IsNullOrWhiteSpace(aperture) ? new MetaValue(aperture, apertureRaw.ToString(CultureInfo.InvariantCulture)) : null);
		}

		private IMetaValue GetLightSource()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifLightSource, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					var lightSource = (LightSource)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidLightSource(lightSource))
					{
						// Don't bother with it if it is "Unknown"
						if (lightSource != LightSource.Unknown)
						{
							return new MetaValue(lightSource.GetDescription(), ((ushort)lightSource).ToString(CultureInfo.InvariantCulture));
						}
					}
				}
			}
			return null;
		}

		private IMetaValue GetMeteringMode()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifMeteringMode, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					var meterMode = (MeteringMode)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidMeteringMode(meterMode))
					{
						return new MetaValue(meterMode.ToString(), ((ushort)meterMode).ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			return null;
		}

		private IMetaValue GetSubjectDistance()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifSubjectDist, out rawMdi))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					double distance = ((Fraction)rawMdi.Value).ToSingle();

					if (distance > 1)
					{
						distance = Math.Round(distance, 1);
					}

					return new MetaValue(String.Concat(distance.ToString("0.### ", CultureInfo.InvariantCulture), Resources.Metadata_SubjectDistance_Units), distance.ToString("0.### ", CultureInfo.InvariantCulture));
				}
				else
				{
					string value = rawMdi.Value.ToString().Trim().TrimEnd(new[] { '\0' });

					if (!String.IsNullOrWhiteSpace(value))
					{
						return new MetaValue(String.Format(CultureInfo.CurrentCulture, String.Concat("{0} ", Resources.Metadata_SubjectDistance_Units), value), value);
					}
				}
			}

			return null;
		}

		private IMetaValue GetVerticalResolution()
		{
			MetadataItem rawMdi;
			string resolutionUnit = String.Empty;

			if (RawMetadata.TryGetValue(RawMetadataItemName.ResolutionYUnit, out rawMdi))
			{
				resolutionUnit = rawMdi.Value.ToString();
			}

			if ((String.IsNullOrWhiteSpace(resolutionUnit)) && (RawMetadata.TryGetValue(RawMetadataItemName.ResolutionUnit, out rawMdi)))
			{
				if (rawMdi.ExtractedValueType == ExtractedValueType.Int64)
				{
					ResolutionUnit resUnit = (ResolutionUnit)(Int64)rawMdi.Value;
					if (MetadataEnumHelper.IsValidResolutionUnit(resUnit))
					{
						resolutionUnit = resUnit.ToString();
					}
				}
			}

			if (RawMetadata.TryGetValue(RawMetadataItemName.YResolution, out rawMdi))
			{
				string yResolution;
				if (rawMdi.ExtractedValueType == ExtractedValueType.Fraction)
				{
					yResolution = Math.Round(((Fraction)rawMdi.Value).ToSingle(), 2).ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					yResolution = rawMdi.Value.ToString();
				}

				return new MetaValue(String.Concat(yResolution, " ", resolutionUnit), yResolution);
			}

			return null;
		}

		private IMetaValue GetWidth()
		{
			int width = GetWidthAsInt();

			return (width > 0 ? new MetaValue(String.Concat(width, " ", Resources.Metadata_Width_Units), width.ToString(CultureInfo.InvariantCulture)) : null);
		}

		private IMetaValue GetGpsValue(MetadataItemName metaName)
		{
			switch (metaName)
			{
				case MetadataItemName.GpsVersion:
					return (!String.IsNullOrWhiteSpace(GpsLocation.Version) ? new MetaValue(GpsLocation.Version, GpsLocation.Version) : null);

				case MetadataItemName.GpsLocation:
					if ((GpsLocation.Latitude != null) && (GpsLocation.Longitude != null))
					{
						var loc = GpsLocation.ToLatitudeLongitudeDecimalString();
						return new MetaValue(loc, loc);
					}
					else
						return null;

				case MetadataItemName.GpsLatitude:
					if ((GpsLocation.Latitude != null) && (GpsLocation.Longitude != null))
					{
						var lat = GpsLocation.Latitude.ToDouble().ToString("F6", CultureInfo.InvariantCulture);
						return new MetaValue(lat, lat);
					}
					else
						return null;

				case MetadataItemName.GpsLongitude:
					if ((GpsLocation.Latitude != null) && (GpsLocation.Longitude != null))
					{
						var longitude = GpsLocation.Longitude.ToDouble().ToString("F6", CultureInfo.InvariantCulture);
						return new MetaValue(longitude, longitude);
					}
					else
						return null;

				case MetadataItemName.GpsAltitude:
					if (GpsLocation.Altitude.HasValue)
					{
						var altitude = GpsLocation.Altitude.Value.ToString("N0", CultureInfo.CurrentCulture);
						return new MetaValue(String.Concat(altitude, " ", Resources.Metadata_meters), altitude);
					}
					else
						return null;

				case MetadataItemName.GpsDestLocation:
					if ((GpsLocation.DestLatitude != null) && (GpsLocation.DestLongitude != null))
					{
						var loc = GpsLocation.ToDestLatitudeLongitudeDecimalString();
						return new MetaValue(loc, loc);
					}
					else
						return null;

				case MetadataItemName.GpsDestLatitude:
					if ((GpsLocation.DestLatitude != null) && (GpsLocation.DestLongitude != null))
					{
						var lat = GpsLocation.DestLatitude.ToDouble().ToString("F6", CultureInfo.InvariantCulture);
						return new MetaValue(lat, lat);
					}
					else
						return null;

				case MetadataItemName.GpsDestLongitude:
					if ((GpsLocation.DestLatitude != null) && (GpsLocation.DestLongitude != null))
					{
						var longitude = GpsLocation.DestLongitude.ToDouble().ToString("F6", CultureInfo.InvariantCulture);
						return new MetaValue(longitude, longitude);
					}
					else
						return null;

				default:
					throw new ArgumentException(string.Format("The function GetGpsValue() expects a GPS-related parameter; instead the value {0} was passed.", metaName), "metaName");
			}
		}

		private IMetaValue GetIptcValue(MetadataItemName metaName)
		{
			string iptcValue;
			try
			{
				iptcValue = WpfMetadata.GetQuery(String.Format(CultureInfo.InvariantCulture, IptcQueryFormatString, IptcQueryParameters[metaName])) as string;
			}
			catch (ArgumentNullException)
			{
				// Some images throw this exception. When this happens, just exit.
				return null;
			}
			catch (InvalidOperationException)
			{
				// Some images throw this exception. When this happens, just exit.
				return null;
			}

			if (String.IsNullOrWhiteSpace(iptcValue))
				return null;

			var formattedIptcValue = iptcValue;

			// For dates, format to a specific pattern.
			if (metaName == MetadataItemName.IptcDateCreated)
			{
				var dateTaken = TryParseDate(iptcValue);

				if (dateTaken.Year > DateTime.MinValue.Year)
					formattedIptcValue = dateTaken.ToString(DateTimeFormatString, CultureInfo.InvariantCulture);
			}

			return new MetaValue(formattedIptcValue, iptcValue);
		}

		#endregion

		#region Functions

		/// <summary>
		/// Fill the class-level _rawMetadata dictionary with MetadataItem objects created from the
		/// PropertyItems property of the image. Skip any items that are not defined in the 
		/// RawMetadataItemName enumeration. Guaranteed to not return null.
		/// </summary>
		private Dictionary<RawMetadataItemName, MetadataItem> GetRawMetadataDictionary()
		{
			var rawMetadata = new Dictionary<RawMetadataItemName, MetadataItem>();

			foreach (var itemIterator in PropertyItems)
			{
				var metadataName = (RawMetadataItemName)itemIterator.Id;
				if (Enum.IsDefined(typeof(RawMetadataItemName), metadataName))
				{
					if (!rawMetadata.ContainsKey(metadataName))
					{
						var metadataItem = new MetadataItem(itemIterator);
						if (metadataItem.Value != null)
							rawMetadata.Add(metadataName, metadataItem);
					}
				}
			}

			return rawMetadata;
		}

		/// <summary>
		/// Extract the property items of the specified image. Guaranteed to not return null.
		/// </summary>
		private PropertyItem[] GetImagePropertyItems()
		{
			var filePath = GalleryObject.Original.FileNamePhysicalPath;

			if (String.IsNullOrWhiteSpace(filePath))
				return new PropertyItem[0];

			if (AppSetting.Instance.AppTrustLevel == ApplicationTrustLevel.Full)
			{
				return GetPropertyItemsUsingFullTrustTechnique(filePath);
			}
			else
			{
				return GetPropertyItemsUsingLimitedTrustTechnique(filePath);
			}
		}

		private PropertyItem[] GetPropertyItemsUsingFullTrustTechnique(string imageFilePath)
		{
			// This technique is fast but requires full trust. Can only be called when app is running under full trust.
			if (AppSetting.Instance.AppTrustLevel != ApplicationTrustLevel.Full)
				throw new InvalidOperationException("The method MediaObjectMetadataExtractor.GetPropertyItemsUsingFullTrustTechnique can only be called when the application is running under full trust. The application should have already checked for this before calling this method. The developer needs to modify the source code to fix this.");

			using (Stream stream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				try
				{
					using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true, false))
					{
						try
						{
							return image.PropertyItems;
						}
						catch (NotImplementedException)
						{
							// Some images, such as wmf, throw this exception. We'll make a note of it and set our field to an empty array.
							//if (!ex.Data.Contains("Metadata Extraction Error"))
							//{
							//	ex.Data.Add("Metadata Extraction Error", String.Format(CultureInfo.CurrentCulture, "Cannot extract metadata from file \"{0}\".", imageFilePath));
							//}

							//LogError(ex, GalleryObject.GalleryId);
							return new PropertyItem[0];
						}
					}
				}
				catch (ArgumentException)
				{
					//if (!ex.Data.Contains("Metadata Extraction Error"))
					//{
					//	ex.Data.Add("Metadata Extraction Error", String.Format(CultureInfo.CurrentCulture, "Cannot extract metadata from file \"{0}\".", imageFilePath));
					//}

					//LogError(ex, GalleryObject.GalleryId);
					return new PropertyItem[0];
				}
			}
		}

		private PropertyItem[] GetPropertyItemsUsingLimitedTrustTechnique(string imageFilePath)
		{
			// This technique is not as fast as the one in the method GetPropertyItemsUsingFullTrustTechnique() but in works in limited
			// trust environments.
			try
			{
				using (System.Drawing.Image image = new System.Drawing.Bitmap(imageFilePath))
				{
					try
					{
						return image.PropertyItems;
					}
					catch (NotImplementedException ex)
					{
						// Some images, such as wmf, throw this exception. We'll make a note of it and set our field to an empty array.
						LogError(ex, GalleryObject.GalleryId);
						return new PropertyItem[0];
					}
				}
			}
			catch (ArgumentException ex)
			{
				LogError(ex, GalleryObject.GalleryId);
				return new PropertyItem[0];
			}
		}

		/// <summary>
		/// Get a reference to the <see cref="BitmapMetadata" /> object for this image file that contains 
		/// the metadata such as title, keywords, etc. Guaranteed to not return null. Returns an instance 
		/// of <see cref="NullObjects.NullWpfMetadata" /> if an actual <see cref="BitmapMetadata" /> object 
		/// is not available.
		/// </summary>
		/// <returns> Returns a reference to the BitmapMetadata object for this image file that contains 
		/// the metadata such as title, keywords, etc.</returns>
		/// <remarks>A BitmapDecoder object is created from the absolute filepath passed into the constructor. Through trial and
		/// error, the relevant metadata appears to be stored in the first frame in the BitmapDecoder property of the first frame
		/// of the root-level BitmapDecoder object. One might expect the Metadata property of the root-level BitmapDecoder object to
		/// contain the metadata, but it seems to always be null.</remarks>
		private IWpfMetadata GetBitmapMetadata()
		{
			// We can access the BitmapMetadata object in the WPF namespace only when the app is running 
			// in Full Trust. There is also a config setting that enables this functionality, so query 
			// that as well. (The config setting allows it to be disabled due to the reliability issues 
			// found with the WPF classes.)
			if ((AppSetting.Instance.AppTrustLevel < ApplicationTrustLevel.Full)
					|| (!Factory.LoadGallerySetting(GalleryObject.GalleryId).ExtractMetadataUsingWpf))
			{
				return new NullObjects.NullWpfMetadata();
			}

			// Do not use the BitmapCacheOption.Default or None option, as it will hold a lock on the file until garbage collection. I discovered
			// this problem and it has been submitted to MS as a bug. See thread in the managed newsgroup:
			// http://www.microsoft.com/communities/newsgroups/en-us/default.aspx?dg=microsoft.public.dotnet.framework&tid=b694ada2-10c4-4999-81f8-97295eb024a9&cat=en_US_a4ab6128-1a11-4169-8005-1d640f3bd725&lang=en&cr=US&sloc=en-us&m=1&p=1
			// Also do not use BitmapCacheOption.OnLoad as suggested in the thread, as it causes the memory to not be released until 
			// eventually IIS crashes when you do things like synchronize 100 images.
			// BitmapCacheOption.OnDemand seems to be the only option that doesn't lock the file or crash IIS.
			// Update 2007-07-29: OnDemand seems to also lock the file. There is no good solution! Acckkk
			// Update 2007-08-04: After installing VS 2008 beta 2, which also installs .NET 2.0 SP1, I discovered that OnLoad no longer crashes IIS.
			// Update 2008-05-19: The Create method doesn't release the file lock when an exception occurs, such as when the file is a WMF. See:
			// http://www.microsoft.com/communities/newsgroups/en-us/default.aspx?dg=microsoft.public.dotnet.framework&tid=fe3fb82f-0191-40a3-b789-0602cc4445d3&cat=&lang=&cr=&sloc=&p=1
			// Bug submission: https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=344914
			// The workaround is to use a different overload of Create that takes a FileStream.

			var filePath = GalleryObject.Original.FileNamePhysicalPath;

			if (String.IsNullOrWhiteSpace(filePath))
				return new NullObjects.NullWpfMetadata();

			BitmapDecoder fileBitmapDecoder = GetBitmapDecoder(filePath);

			if ((fileBitmapDecoder == null) || (fileBitmapDecoder.Frames.Count == 0))
				return new NullObjects.NullWpfMetadata();

			BitmapFrame fileFirstFrame = fileBitmapDecoder.Frames[0];

			if (fileFirstFrame == null)
				return new NullObjects.NullWpfMetadata();

			BitmapDecoder firstFrameBitmapDecoder = fileFirstFrame.Decoder;

			if ((firstFrameBitmapDecoder == null) || (firstFrameBitmapDecoder.Frames.Count == 0))
				return new NullObjects.NullWpfMetadata();

			BitmapFrame firstFrameInDecoderInFirstFrameOfFile = firstFrameBitmapDecoder.Frames[0];

			// The Metadata property is of type ImageMetadata, so we must cast it to BitmapMetadata.
			var bitmapMetadata = firstFrameInDecoderInFirstFrameOfFile.Metadata as BitmapMetadata;

			if (bitmapMetadata != null)
				return new WpfMetadata(bitmapMetadata);
			else
				return new NullObjects.NullWpfMetadata();
		}

		private BitmapDecoder GetBitmapDecoder(string imageFilePath)
		{
			BitmapDecoder fileBitmapDecoder = null;
			try
			{
				using (Stream stream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					try
					{
						fileBitmapDecoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
						// DO NOT USE: fileBitmapDecoder = BitmapDecoder.Create(new Uri(imageFilePath, UriKind.Absolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
					}
					catch (NotSupportedException) { } // Thrown by some file types such as wmf
					catch (InvalidOperationException) { } // Reported by some users
					catch (ArgumentException) { } // Reported by some users
					catch (FileFormatException) { } // Reported by some users
					catch (IOException) { } // Reported by some users
					catch (OverflowException) { } // Reported by some users
					catch (OutOfMemoryException)
					{
						// The garbage collector will automatically run to try to clean up memory, so let's wait for it to finish and 
						// try again. If it still doesn't work because the image is just too large and the system doesn't have enough
						// memory, just give up.
						GC.WaitForPendingFinalizers();
						try
						{
							fileBitmapDecoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
						}
						catch (NotSupportedException) { }
						catch (InvalidOperationException) { }
						catch (ArgumentException) { }
						catch (OutOfMemoryException) { }
					}
					catch (Exception ex)
					{
						if (!ex.Data.Contains("Note"))
							ex.Data.Add("Note", "This error was silently handled by the application and did not cause user disruption.");

						if (!ex.Data.Contains("Image File path"))
							ex.Data.Add("Image File path", imageFilePath);

						EventController.RecordError(ex, AppSetting.Instance, GalleryObject.GalleryId, Factory.LoadGallerySettings());
					}
				}
			}
			catch (FileNotFoundException) { } // Return null if file not found
			catch (DirectoryNotFoundException) { } // Return null if directory not found
			catch (IOException) { } // Return null if IO problem occurs

			return fileBitmapDecoder;
		}

		private static string ConvertStringCollectionToDelimitedString(System.Collections.ObjectModel.ReadOnlyCollection<string> stringCollection)
		{
			if (stringCollection == null)
				return null;

			// If any of the entries is itself a comma-separated list, parse them. Remove any duplicates.
			var strings = new List<String>();

			foreach (var s in stringCollection)
			{
				strings.AddRange(s.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s1 => s1.Trim()));
			}

			return String.Join(", ", strings.Distinct());
		}

		private static void LogError(Exception ex, int galleryId)
		{
			EventController.RecordError(ex, AppSetting.Instance, galleryId, Factory.LoadGallerySettings());
			HelperFunctions.PurgeCache();
		}

		private string GetStringMetadataItem(RawMetadataItemName sourceRawMetadataName, string formatString = "{0}")
		{
			MetadataItem rawMdi;
			string rawValue = null;

			if (RawMetadata.TryGetValue(sourceRawMetadataName, out rawMdi))
			{
				var unformattedValue = rawMdi.Value.ToString().Trim().TrimEnd(new[] { '\0' });

				rawValue = String.Format(CultureInfo.CurrentCulture, formatString, unformattedValue);
			}

			return rawValue;
		}

		/// <summary>
		/// Try to convert <paramref name="dteRaw" /> to a valid <see cref="DateTime" /> object. If it cannot be converted, return
		/// <see cref="DateTime.MinValue" />.
		/// </summary>
		/// <param name="dteRaw">The string containing the date/time to convert.</param>
		/// <returns>Returns a <see cref="DateTime" /> instance.</returns>
		/// <remarks>The IPTC specs do not define an exact format for the ITPC Date Created field, so it is unclear how to reliably parse
		/// it. However, an analysis of sample photos, including those provided by IPTC (http://www.iptc.org), show that the format
		/// yyyyMMdd is consistently used, so we'll try that if the more generic parsing doesnt work.</remarks>
		private static DateTime TryParseDate(string dteRaw)
		{
			DateTime result;
			if (DateTime.TryParse(dteRaw, out result))
			{
				return result;
			}
			else if (DateTime.TryParseExact(dteRaw, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
			{
				return result;
			}

			return DateTime.MinValue;
		}

		/// <summary>
		/// Convert an EXIF-formatted timestamp to the .NET DateTime type. Returns DateTime.MinValue when the date cannot be parsed.
		/// </summary>
		/// <param name="exifDateTime">An EXIF-formatted timestamp. The format is YYYY:MM:DD HH:MM:SS with time shown 
		/// in 24-hour format and the date and time separated by one blank character (0x2000). The character 
		/// string length is 20 bytes including the NULL terminator.</param>
		/// <returns>Returns the EXIF-formatted timestamp as a .NET DateTime type.</returns>
		private static DateTime ConvertExifDateTimeToDateTime(string exifDateTime)
		{
			DateTime convertedDateTimeValue = DateTime.MinValue;
			const int minCharsReqdToSpecifyDate = 10; // Need at least 10 characters to specify a date (e.g. 2010:10:15)

			if (String.IsNullOrWhiteSpace(exifDateTime) || (exifDateTime.Trim().Length < minCharsReqdToSpecifyDate))
				return convertedDateTimeValue; // No date/time is present; just return

			exifDateTime = exifDateTime.Trim();

			string[] ymdhms = exifDateTime.Split(new[] { ' ', ':' });

			// Default to lowest possible year, first month and first day
			int year = DateTime.MinValue.Year, month = 1, day = 1, hour = 0, minute = 0, second = 0;

			if (ymdhms.Length >= 2)
			{
				Int32.TryParse(ymdhms[0], out year);
				Int32.TryParse(ymdhms[1], out month);
				Int32.TryParse(ymdhms[2], out day);
			}

			if (ymdhms.Length >= 6)
			{
				// The hour, minute and second will default to 0 if it can't be parsed, which is good.
				Int32.TryParse(ymdhms[3], out hour);
				Int32.TryParse(ymdhms[4], out minute);
				Int32.TryParse(ymdhms[5], out second);
			}
			if (year > DateTime.MinValue.Year)
			{
				try
				{
					convertedDateTimeValue = new DateTime(year, month, day, hour, minute, second);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
			}

			return convertedDateTimeValue;
		}

		private IMetaValue GetDatePictureTakenWpf()
		{
			try
			{
				var dateTakenRaw = WpfMetadata.DateTaken;

				if (!String.IsNullOrWhiteSpace(dateTakenRaw))
				{
					var dateTaken = TryParseDate(dateTakenRaw);
					if (dateTaken.Year > DateTime.MinValue.Year)
					{
						return new MetaValue(dateTaken.ToString(DateTimeFormatString, CultureInfo.InvariantCulture), dateTaken.ToString("O", CultureInfo.InvariantCulture));
					}
					else
						return new MetaValue(dateTakenRaw, dateTakenRaw); // We can't parse it so just return it as is
				}
			}
			catch (NotSupportedException) { } // Some image types, such as png, throw a NotSupportedException. Let's swallow them and move on.
			catch (ArgumentException) { }
			catch (InvalidOperationException) { }

			return null;
		}

		private IMetaValue GetDatePictureTakenGdi()
		{
			MetadataItem rawMdi;
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifDTOrig, out rawMdi))
			{
				var convertedDateTimeValue = ConvertExifDateTimeToDateTime(rawMdi.Value.ToString());
				if (convertedDateTimeValue > DateTime.MinValue)
				{
					return new MetaValue(convertedDateTimeValue.ToString(DateTimeFormatString, CultureInfo.InvariantCulture), convertedDateTimeValue.ToString("O", CultureInfo.InvariantCulture));
				}
				else if (!String.IsNullOrWhiteSpace(rawMdi.Value.ToString()))
				{
					return new MetaValue(rawMdi.Value.ToString(), rawMdi.Value.ToString());
				}
			}

			return null;
		}

		/// <summary>
		/// Get the height of the media object. Extracted from RawMetadataItemName.ExifPixXDim for compressed images and
		/// from RawMetadataItemName.ImageHeight for uncompressed images. The value is stored in a private class level variable
		/// for quicker subsequent access.
		/// </summary>
		/// <returns>Returns the height of the media object.</returns>
		private int GetWidthAsInt()
		{
			if (_width > 0)
				return _width;

			MetadataItem rawMdi;
			int width = int.MinValue;
			bool foundWidth = false;

			// Compressed images store their width in ExifPixXDim. Uncompressed images store their width in ImageWidth.
			// First look in ExifPixXDim since most images are likely to be compressed ones. If we don't find that one,
			// look for ImageWidth. If we don't find that one either (which should be unlikely to ever happen), then just give 
			// up and return null.
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifPixXDim, out rawMdi))
			{
				foundWidth = Int32.TryParse(rawMdi.Value.ToString(), out width);
			}

			if ((!foundWidth) && (RawMetadata.TryGetValue(RawMetadataItemName.ImageWidth, out rawMdi)))
			{
				foundWidth = Int32.TryParse(rawMdi.Value.ToString(), out width);
			}

			if (!foundWidth)
			{
				width = this.GalleryObject.Original.Width;
				foundWidth = (width > 0);
			}

			if (foundWidth)
				_width = width;

			return width;
		}

		/// <summary>
		/// Get the width of the media object. Extracted from RawMetadataItemName.ExifPixYDim for compressed images and
		/// from RawMetadataItemName.ImageWidth for uncompressed images. The value is stored in a private class level variable
		/// for quicker subsequent access.
		/// </summary>
		/// <returns>Returns the width of the media object.</returns>
		private int GetHeightAsInt()
		{
			if (_height > 0)
				return _height;

			MetadataItem rawMdi;
			int height = int.MinValue;
			bool foundHeight = false;

			// Compressed images store their width in ExifPixXDim. Uncompressed images store their width in ImageWidth.
			// First look in ExifPixXDim since most images are likely to be compressed ones. If we don't find that one,
			// look for ImageWidth. If we don't find that one either (which should be unlikely to ever happen), then just give 
			// up and return null.
			if (RawMetadata.TryGetValue(RawMetadataItemName.ExifPixYDim, out rawMdi))
			{
				foundHeight = Int32.TryParse(rawMdi.Value.ToString(), out height);
			}

			if ((!foundHeight) && (RawMetadata.TryGetValue(RawMetadataItemName.ImageHeight, out rawMdi)))
			{
				foundHeight = Int32.TryParse(rawMdi.Value.ToString(), out height);
			}

			if (!foundHeight)
			{
				height = this.GalleryObject.Original.Height;
				foundHeight = (height > 0);
			}

			if (foundHeight)
				_height = height;

			return height;
		}

		/// <summary>
		/// Persists the meta value.
		/// </summary>
		/// <param name="metaName">Name of the meta.</param>
		/// <param name="persistAction">The persist action.</param>
		private void PersistMetaValue(MetadataItemName metaName, MetaPersistAction persistAction)
		{
			if (!UpdatableMetaItems.ContainsKey(metaName))
			{
				EventController.RecordEvent(String.Format("This version of Gallery Server Pro does not support modifying the meta value {0} in the original file. The request to save or delete the meta value was ignored.", metaName));
				return;
			}

			lock (_sharedLock)
			{
				bool isSuccessful = false;
				var filePath = GalleryObject.Original.FileNamePhysicalPath;

				using (Stream savedFile = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
				{
					var output = BitmapDecoder.Create(savedFile, BitmapCreateOptions.None, BitmapCacheOption.Default);
					var bitmapMetadata = output.Frames[0].CreateInPlaceBitmapMetadataWriter();

					if (bitmapMetadata != null)
					{
						SetMetadata(bitmapMetadata, metaName, persistAction);

						if (bitmapMetadata.TrySave())
						{
							isSuccessful = true;
						}
					}
				}

				// If the save wasn't successful, try to save another way.
				if (!isSuccessful)
				{
					string tmpFilePath = Path.Combine(AppSetting.Instance.TempUploadDirectory, String.Concat(Guid.NewGuid().ToString(), ".tmp"));

					TryAlternateMethodsOfPersistingMetadata(filePath, tmpFilePath, metaName, persistAction);

					ReplaceFileSafely(tmpFilePath, filePath);
				}
			}
		}

		private void SetMetadata(BitmapMetadata bitmapMetadata, MetadataItemName metaName, MetaPersistAction persistAction)
		{
			if (!UpdatableMetaItems.ContainsKey(metaName))
			{
				throw new ArgumentException(String.Format("This function does not support persisting the meta item {0}.", metaName));
			}

			switch (metaName)
			{
				case MetadataItemName.Orientation:
					SetOrientationMetadata(bitmapMetadata, metaName, persistAction);
					break;

				default:
					throw new InvalidEnumArgumentException(String.Format(CultureInfo.CurrentCulture, "This function is not designed to handle the enumeration value {0}. The function must be updated.", metaName));
			}


			//if (caption != null)
			//{
			//	bitmapMetadata.Comment = caption;
			//}

			//if (dateTaken.HasValue)
			//{
			//	bitmapMetadata.DateTaken = dateTaken.Value.ToString("M/d/yyyy HH:mm:ss");
			//	bitmapMetadata.SetQuery(DATE_TAKEN_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
			//	bitmapMetadata.SetQuery(DIGITIZED_DATE_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
			//	bitmapMetadata.SetQuery(ORIGINAL_DATE_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
			//}

			////-----------tags----------------------
			//List<String> tagsList = new List<String>();

			//foreach (string tag in tags)
			//{
			//	if (tag.Length > 0)
			//		tagsList.Add(tag);
			//}

			//if (tagsList.Count == 0)
			//	tagsList.Add("");

			////XMP
			//bitmapMetadata.Keywords = new System.Collections.ObjectModel.ReadOnlyCollection<string>(tagsList);

			////IPTC
			//string[] iptcTagsList = tagsList.ToArray();
			//bitmapMetadata.SetQuery(IPTC_KEYWORDS_QUERY, iptcTagsList);
			////-----------tags----------------------

		}

		private void SetOrientationMetadata(BitmapMetadata bitmapMetadata, MetadataItemName metaName, MetaPersistAction persistAction)
		{
			switch (persistAction)
			{
				case MetaPersistAction.Delete:
					bitmapMetadata.RemoveQuery(UpdatableMetaItems[metaName]);
					break;

				case MetaPersistAction.Save:
					IGalleryObjectMetadataItem orientationMeta;
					if (GalleryObject.MetadataItems.TryGetMetadataItem(metaName, out orientationMeta))
					{
						ushort orientationRaw;
						if (UInt16.TryParse(orientationMeta.RawValue, out orientationRaw) && MetadataEnumHelper.IsValidOrientation((Orientation)orientationRaw))
						{
							bitmapMetadata.SetQuery(UpdatableMetaItems[metaName], orientationRaw);
						}
					}
					break;

				default:
					throw new InvalidEnumArgumentException(String.Format(CultureInfo.CurrentCulture, "This function is not designed to handle the enumeration value {0}. The function must be updated.", persistAction));
			}
		}

		private void TryAlternateMethodsOfPersistingMetadata(string sourceFileName, string outputFileName, MetadataItemName metaName, MetaPersistAction persistAction)
		{
			// Three alternate attempts to persist the metadata:
			// 1. Use outputFileName parameter and a cloned copy of the file's metadata
			// 2. Use outputFileName parameter and the original file's metadata
			// 3. Rename the file and try again using a cloned copy of the file's metadata
			// Adapted from: https://code.google.com/p/flickrmetasync/source/browse/trunk/FlickrMetadataSync/Picture.cs?spec=svn29&r=29
			bool tryOneLastMethod = false;

			using (Stream originalFile = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				const BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
				BitmapDecoder original = BitmapDecoder.Create(originalFile, createOptions, BitmapCacheOption.None);

				var output = new JpegBitmapEncoder();

				if (original.Frames[0] != null && original.Frames[0].Metadata != null)
				{
					BitmapMetadata bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
					bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", MetadataPaddingInBytes);
					bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", MetadataPaddingInBytes);
					bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", MetadataPaddingInBytes);

					SetMetadata(bitmapMetadata, metaName, persistAction);

					output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
				}

				try
				{
					using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
					{
						output.Save(outputFile);
					}
				}
				catch (Exception e) //System.Exception, NotSupportedException, InvalidOperationException, ArgumentException
				{
					if (e is NotSupportedException || e is ArgumentException)
					{
						output = new JpegBitmapEncoder();

						output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, original.Metadata, original.Frames[0].ColorContexts));

						using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
						{
							output.Save(outputFile);
						}

						tryOneLastMethod = true;
					}
					else
					{
						throw new Exception("Error saving picture.", e);
					}
				}
			}

			if (tryOneLastMethod)
			{
				File.Move(outputFileName, outputFileName + "tmp");

				using (Stream recentlyOutputFile = new FileStream(outputFileName + "tmp", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					const BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
					BitmapDecoder original = BitmapDecoder.Create(recentlyOutputFile, createOptions, BitmapCacheOption.None);
					JpegBitmapEncoder output = new JpegBitmapEncoder();
					if (original.Frames[0] != null && original.Frames[0].Metadata != null)
					{
						BitmapMetadata bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
						bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", MetadataPaddingInBytes);
						bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", MetadataPaddingInBytes);
						bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", MetadataPaddingInBytes);

						SetMetadata(bitmapMetadata, metaName, persistAction);

						output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
					}

					using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
					{
						output.Save(outputFile);
					}
				}
				File.Delete(outputFileName + "tmp");
			}
		}

		/// <summary>
		/// Replaces the <paramref name="destFilePath" /> with <paramref name="sourceFilePath" />. No action - or errors - are thrown
		/// if either file does not exist.
		/// </summary>
		/// <param name="sourceFilePath">The source file path.</param>
		/// <param name="destFilePath">The destination file path.</param>
		/// <returns><c>true</c> if <paramref name="sourceFilePath" /> is successfully moved to <paramref name="destFilePath" />,
		/// <c>false</c> otherwise.</returns>
		private static bool ReplaceFileSafely(string sourceFilePath, string destFilePath)
		{
			if (File.Exists(destFilePath) && File.Exists(sourceFilePath))
			{
				File.Delete(destFilePath);
			}

			if (File.Exists(sourceFilePath))
			{
				File.Move(sourceFilePath, destFilePath);
				return true;
			}

			return false;
		}

		#endregion
	}
}