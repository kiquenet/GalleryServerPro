using System;
using System.Globalization;
using System.Web.UI.WebControls;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Web.Controller;

namespace GalleryServerPro.Web.Pages.Task
{
	/// <summary>
	/// A page-like user control that handles the Create album task.
	/// </summary>
	public partial class createalbum : Pages.TaskPage
	{
		#region Private Fields

		private int _currentAlbumId = int.MinValue;

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
			//An event from the control has bubbled up.  If it's the Ok button, then run the
			//code to save the data to the database; otherwise ignore.
			Button btn = source as Button;
			if ((btn != null) && (((btn.ID == "btnOkTop") || (btn.ID == "btnOkBottom"))))
			{
				if (Page.IsValid)
				{
					int newAlbumID = btnOkClicked();

					Utils.Redirect(PageId.album, "aid={0}", newAlbumID);
				}
			}

			return true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the album ID for the current album.
		/// </summary>
		public int CurrentAlbumId
		{
			get
			{
				if (this._currentAlbumId == int.MinValue)
					this._currentAlbumId = this.GetAlbumId();

				return this._currentAlbumId;
			}
		}

		#endregion

		#region Private Methods

		private void ConfigureControls()
		{
			this.TaskHeaderText = Resources.GalleryServerPro.Task_Create_Album_Header_Text;
			this.TaskBodyText = Resources.GalleryServerPro.Task_Create_Album_Body_Text;
			this.OkButtonText = Resources.GalleryServerPro.Task_Create_Album_Ok_Button_Text;
			this.OkButtonToolTip = Resources.GalleryServerPro.Task_Create_Album_Ok_Button_Tooltip;

			this.PageTitle = Resources.GalleryServerPro.Task_Create_Album_Page_Title;

			tvUC.RequiredSecurityPermissions = SecurityActions.AddChildAlbum;
			IAlbum albumToSelect = this.GetAlbum();
			if (albumToSelect.IsVirtualAlbum || !IsUserAuthorized(SecurityActions.AddChildAlbum, albumToSelect))
			{
				albumToSelect = AlbumController.GetHighestLevelAlbumWithCreatePermission(GalleryId);
			}

			tvUC.SelectedAlbumIds.Add(albumToSelect.Id);

			this.Page.Form.DefaultFocus = txtTitle.ClientID;
		}

		private int btnOkClicked()
		{
			//User clicked 'Create album'. Create the new album and return the new album ID.
			var parentAlbum = Factory.LoadAlbumInstance(tvUC.SelectedAlbum.Id, true, true);

			this.CheckUserSecurity(SecurityActions.AddChildAlbum, parentAlbum);

			int newAlbumId;

			if (parentAlbum.Id > 0)
			{
				IAlbum newAlbum = Factory.CreateEmptyAlbumInstance(parentAlbum.GalleryId);
				newAlbum.Title = Utils.CleanHtmlTags(txtTitle.Text.Trim(), GalleryId);
				//newAlbum.ThumbnailMediaObjectId = 0; // not needed
				newAlbum.Parent = parentAlbum;
				newAlbum.IsPrivate = parentAlbum.IsPrivate;
				GalleryObjectController.SaveGalleryObject(newAlbum);
				newAlbumId = newAlbum.Id;

				// Re-sort the items in the album. This will put the media object in the right position relative to its neighbors.
				((IAlbum)newAlbum.Parent).Sort(true, Utils.UserName);

				HelperFunctions.PurgeCache();
			}
			else
				throw new GalleryServerPro.Events.CustomExceptions.InvalidAlbumException(parentAlbum.Id);

			return newAlbumId;
		}

		#endregion
	}
}