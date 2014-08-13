using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Business.Metadata;
using GalleryServerPro.Events.CustomExceptions;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages.Task
{
	/// <summary>
	/// A page-like user control that handles the Rotate images task.
	/// </summary>
	public partial class rotateimages : Pages.TaskPage
	{
		#region Properties

		protected static string Rotate0Text
		{
			get
			{
				return Resources.GalleryServerPro.Task_Rotate_Image_0_Rotate_Text;
			}
		}

		protected static string Rotate90Text
		{
			get
			{
				return Resources.GalleryServerPro.Task_Rotate_Image_90_Rotate_Text;
			}
		}

		protected static string Rotate180Text
		{
			get
			{
				return Resources.GalleryServerPro.Task_Rotate_Image_180_Rotate_Text;
			}
		}

		protected static string Rotate270Text
		{
			get
			{
				return Resources.GalleryServerPro.Task_Rotate_Image_270_Rotate_Text;
			}
		}
		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.TaskHeaderPlaceHolder = phTaskHeader;
			this.TaskFooterPlaceHolder = phTaskFooter;
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (GallerySettings.MediaObjectPathIsReadOnly)
				RedirectToAlbumViewPage("msg={0}", ((int)MessageType.CannotEditGalleryIsReadOnly).ToString(CultureInfo.InvariantCulture));

			this.CheckUserSecurity(SecurityActions.EditMediaObject);

			if (!IsPostBack)
			{
				ConfigureControls();
			}
		}

		/// <summary>
		/// Determines whether the event for the server control is passed up the page's UI server control hierarchy.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="args">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		/// <returns>
		/// true if the event has been cancelled; otherwise, false. The default is false.
		/// </returns>
		protected override bool OnBubbleEvent(object source, EventArgs args)
		{
			//An event from a control has bubbled up.  If it's the Ok button, then run the
			//code to synchronize; otherwise ignore.
			var btn = source as Button;
			if ((btn != null) && (((btn.ID == "btnOkTop") || (btn.ID == "btnOkBottom"))))
			{
				var msg = btnOkClicked();

				if (msg > int.MinValue)
					this.RedirectToAlbumViewPage("msg={0}", msg.ToString(CultureInfo.InvariantCulture));
				else
					this.RedirectToAlbumViewPage("msg={0}", ((int)MessageType.ObjectsSuccessfullyRotated).ToString(CultureInfo.InvariantCulture));
			}

			return true;
		}

		#endregion

		#region Functions

		private void ConfigureControls()
		{
			this.TaskHeaderText = Resources.GalleryServerPro.Task_Rotate_Images_Header_Text;
			this.TaskBodyText = Resources.GalleryServerPro.Task_Rotate_Images_Body_Text;
			this.OkButtonText = Resources.GalleryServerPro.Task_Rotate_Images_Ok_Button_Text;
			this.OkButtonToolTip = Resources.GalleryServerPro.Task_Rotate_Images_Ok_Button_Tooltip;

			this.PageTitle = Resources.GalleryServerPro.Task_Rotate_Image_Page_Title;

			var rotatableMediaObjects = this.GetAlbum().GetChildGalleryObjects(GalleryObjectType.Image);

			var videos = this.GetAlbum().GetChildGalleryObjects(GalleryObjectType.Video);
			if (!String.IsNullOrEmpty(AppSetting.Instance.FFmpegPath))
			{
				// Only include videos when FFmpeg is installed.
				rotatableMediaObjects.AddRange(videos);
			}

			var albumChildren = rotatableMediaObjects.ToSortedList();

			if (albumChildren.Any())
			{
				rptr.DataSource = albumChildren;
				rptr.DataBind();
			}
			else
			{
				this.RedirectToAlbumViewPage("msg={0}", ((int)MessageType.CannotRotateNoRotatableObjectsExistInAlbum).ToString(CultureInfo.InvariantCulture));
			}
		}

		private int btnOkClicked()
		{
			return RotateImages();
		}

		private int RotateImages()
		{
			// Rotate any images on the hard drive according to the user's wish.
			var returnValue = int.MinValue;

			var imagesToRotate = RetrieveUserSelections();

			try
			{
				HelperFunctions.BeginTransaction();
				foreach (var kvp in imagesToRotate)
				{
					IGalleryObject mo;
					try
					{
						mo = Factory.LoadMediaObjectInstance(kvp.Key, true);
					}
					catch (InvalidMediaObjectException)
					{
						continue; // Image may have been deleted by someone else, so just skip it.
					}

					IGalleryObjectMetadataItem metaOrientation;
					if (kvp.Value == MediaObjectRotation.Rotate0 && !mo.MetadataItems.TryGetMetadataItem(MetadataItemName.Orientation, out metaOrientation))
					{
						continue;
					}

					mo.Rotation = kvp.Value;

					try
					{
						GalleryObjectController.SaveGalleryObject(mo);
					}
					catch (UnsupportedImageTypeException)
					{
						returnValue = (int)MessageType.CannotRotateInvalidImage;
					}
				}
				HelperFunctions.CommitTransaction();
			}
			catch
			{
				HelperFunctions.RollbackTransaction();
				throw;
			}

			HelperFunctions.PurgeCache();

			return returnValue;
		}

		private Dictionary<int, MediaObjectRotation> RetrieveUserSelections()
		{
			// Iterate through all the objects, retrieving the orientation of each image. If the
			// orientation has changed (it is no longer set to 't' for top), then add it to an array.
			// The media object IDs are stored in a hidden input tag.

			var imagesToRotate = new Dictionary<int, MediaObjectRotation>();

			foreach (RepeaterItem rptrItem in rptr.Items)
			{
				var rotateTag = (HtmlInputHidden)rptrItem.FindControl("hdnSelectedSide");

				if (rotateTag.Value.Trim().Length < 1)
					continue;

				var newOrientation = Convert.ToChar(rotateTag.Value.Trim().Substring(0, 1), CultureInfo.InvariantCulture);
				// If the orientation value isn't valid, throw an exception.
				if ((newOrientation != 't') && (newOrientation != 'r') && (newOrientation != 'b') && (newOrientation != 'l'))
					throw new UnexpectedFormValueException();

				// User selected an orientation other than t(top). Add to array.
				MediaObjectRotation r;
				switch (newOrientation)
				{
					case 't': r = MediaObjectRotation.Rotate0; break;
					case 'r': r = MediaObjectRotation.Rotate270; break;
					case 'b': r = MediaObjectRotation.Rotate180; break;
					case 'l': r = MediaObjectRotation.Rotate90; break;
					default: r = MediaObjectRotation.Rotate0; break; // Should never get here because of our if condition above, but let's be safe
				}

				// User selected an orientation other than t(top). Add to dictionary.
				var moidTag = (HtmlInputHidden)rptrItem.FindControl("moid");
				int moid;
				if (Int32.TryParse(moidTag.Value, out moid))
				{
					imagesToRotate.Add(Convert.ToInt32(moidTag.Value, CultureInfo.InvariantCulture), r);
				}
				else
					throw new UnexpectedFormValueException();
			}
			return imagesToRotate;
		}

		#endregion
	}
}