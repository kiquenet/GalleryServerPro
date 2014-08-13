using GalleryServerPro.Web.Entity;

namespace GalleryServerPro.Web.Controller
{
	/// <summary>
	/// Contains functionality for returning resources in a form that can be consumed on the client.
	/// </summary>
	public static class ResourceController
	{
		/// <summary>
		/// Gets an object containing string resources. The object can be JSON parsed and sent to the client.
		/// </summary>
		/// <returns>An instance of <see cref="Resource" />.</returns>
		public static Resource GetResourceEntity()
		{
			return new Resource
			{
				HdrSearchButtonTt = Resources.GalleryServerPro.Search_Button_Tooltip,
				HdrUserAlbumLinkTt = Resources.GalleryServerPro.Header_Go_To_Home_Album_Link_Tooltip,
				HdrMyAccountTt = Resources.GalleryServerPro.Login_My_Account_Link_Text,
				HdrLoginLinkText = Resources.GalleryServerPro.Login_Button_Text,
				HdrLogoutTt = Resources.GalleryServerPro.Login_Logout_Text,
				HdrCreateAccountLinkText = Resources.GalleryServerPro.Login_Create_Account_Text,
				LpRecent = Resources.GalleryServerPro.UC_LeftPane_RecentlyAdded_Text,
				LpTopRated = Resources.GalleryServerPro.UC_LeftPane_TopRated_Text,
				LpTags = Resources.GalleryServerPro.UC_LeftPane_Tags_Text,
				LpPeople = Resources.GalleryServerPro.UC_LeftPane_People_Text,
				AbmPgrFirstTt = Resources.GalleryServerPro.UC_ThumbnailView_Pager_First_Tooltip,
				AbmPgrLastTt = Resources.GalleryServerPro.UC_ThumbnailView_Pager_Last_Tooltip,
				AbmPfx = Resources.GalleryServerPro.Site_Album_Lbl,
				AbmIsPvtTt = Resources.GalleryServerPro.UC_Album_Is_Private_Tt,
				AbmNotPvtTt = Resources.GalleryServerPro.UC_Album_Not_Private_Tt,
				AbmAnonDisabledTt = Resources.GalleryServerPro.UC_Album_Anon_Disabled_Tt,
				AbmAnonDisabledTitle = Resources.GalleryServerPro.UC_Album_Anon_Disabled_Title,
				AbmAnonDisabledMsg = Resources.GalleryServerPro.UC_Album_Anon_Disabled_Msg,
				AbmPvtChngd = Resources.GalleryServerPro.UC_Album_Visibility_Changed_Tt,
				AbmOwnrTt = Resources.GalleryServerPro.UC_Album_Assign_Owner_Tt,
				AbmOwnr = Resources.GalleryServerPro.UC_Album_Owner_Tt,
				AbmOwnrDtl = Resources.GalleryServerPro.UC_Album_Owner_Dtl,
				AbmOwnrLbl = Resources.GalleryServerPro.UC_Album_Owner_Lbl,
				AbmOwnrInhtd = Resources.GalleryServerPro.UC_Album_Inherited_Owners_Lbl,
				AbmOwnrChngd = Resources.GalleryServerPro.UC_Album_Owner_Changed_Hdr,
				AbmOwnrClrd = Resources.GalleryServerPro.UC_Album_Owner_Removed,
				AbmOwnrChngdDtl = Resources.GalleryServerPro.UC_Album_Owner_Changed_Dtl,
				AbmOwnrTtDtl = Resources.GalleryServerPro.UC_Album_Owner_Tt_Dtl,
				AbmRssTt = Resources.GalleryServerPro.UC_Album_Rss_Tt,
				AbmPgrNextTt = Resources.GalleryServerPro.UC_ThumbnailView_Pager_Next_Tooltip,
				AbmPgrPrevTt = Resources.GalleryServerPro.UC_ThumbnailView_Pager_Previous_Tooltip,
				AbmPgrStatus = Resources.GalleryServerPro.UC_ThumbnailView_Pager_Status,
				AbmNumObjSuffix = Resources.GalleryServerPro.UC_Album_Num_Obj_Suffix,
				AbmShareAlbum = Resources.GalleryServerPro.UC_Album_Share_Hdr,
				AbmLinkToAlbum = Resources.GalleryServerPro.UC_Album_Link_Hdr,
				AbmDwnldZip = Resources.GalleryServerPro.UC_ThumbnailView_Album_Download_Zip_Tooltip,
				AbmRvsSortTt = Resources.GalleryServerPro.UC_Album_Reverse_Sort_Tt,
				AbmSortbyTt = Resources.GalleryServerPro.UC_Album_Sort_By_Tt,
				AbmSortbyCustom = Resources.GalleryServerPro.UC_Album_Sort_By_Custom,
				AbmSortbyTitle = Resources.GalleryServerPro.UC_Album_Sort_By_Title,
				AbmSortbyRating= Resources.GalleryServerPro.UC_Album_Sort_By_Rating,
				AbmSortbyDatePictureTaken = Resources.GalleryServerPro.UC_Album_Sort_By_DatePictureTaken,
				AbmSortbyDateAdded = Resources.GalleryServerPro.UC_Album_Sort_By_DateAdded,
				AbmSortbyFilename = Resources.GalleryServerPro.UC_Album_Sort_By_Filename,
				AbmNoObj = Resources.GalleryServerPro.UC_ThumbnailView_Intro_Text_No_Objects,
				AbmAddObj = Resources.GalleryServerPro.UC_ThumbnailView_Intro_Text_Add_Objects,

				MoPrev = Resources.GalleryServerPro.UC_MoView_Prev_Tt,
				MoNext = Resources.GalleryServerPro.UC_MoView_Next_Tt,
				MoTbEmbed = Resources.GalleryServerPro.UC_MoView_Tb_Download_Tt,
				MoTbSsStart = Resources.GalleryServerPro.UC_MoView_Tb_Ss_Start_Tt,
				MoTbSsStop = Resources.GalleryServerPro.UC_MoView_Tb_Ss_Pause_Tt,
				MoTbMove = Resources.GalleryServerPro.UC_MoView_Tb_Move_Tt,
				MoTbCopy = Resources.GalleryServerPro.UC_MoView_Tb_Copy_Tt,
				MoTbRotate = Resources.GalleryServerPro.UC_MoView_Tb_Rotate_Tt,
				MoTbDelete = Resources.GalleryServerPro.UC_MoView_Tb_Delete_Tt,
				MoPosSptr = Resources.GalleryServerPro.UC_MediaObjectView_Position_Separator_Text,
				MoShare = Resources.GalleryServerPro.UC_MoView_Tb_Share_Tt,
				MoShareThisPage = Resources.GalleryServerPro.UC_MoView_Tb_Share_Url_Lbl,
				MoShareHtml = Resources.GalleryServerPro.UC_MoView_Tb_Share_Embed_Lbl,
				MoShareDwnld = Resources.GalleryServerPro.UC_MoView_Tb_Share_Download_Lbl,
				MoShareSlctThmb = Resources.GalleryServerPro.UC_MoView_Tb_Share_Thmb_Lbl,
				MoShareSlctOpt = Resources.GalleryServerPro.UC_MoView_Tb_Share_Opt_Lbl,
				MoShareSlctOrg = Resources.GalleryServerPro.UC_MoView_Tb_Share_Org_Lbl,
				MoShareDwnldFile = Resources.GalleryServerPro.UC_MoView_Tb_Share_Download_Link,
				MoShareDwnldZip = Resources.GalleryServerPro.UC_MoView_Tb_Share_DownloadAlbum_Link,
				MoShareDwnldZipTt = Resources.GalleryServerPro.UC_MoView_Tb_Share_DownloadAlbum_Link_Tt,
				MoNoSsHdr = Resources.GalleryServerPro.UC_MoView_NoSs_Hdr,
				MoNoSsBdy = Resources.GalleryServerPro.UC_MoView_NoSs_Bdy,

				MediaCaptionEditSave = Resources.GalleryServerPro.UC_MoView_Save,
				MediaCaptionEditCancel = Resources.GalleryServerPro.UC_MoView_Cancel,
				MediaCaptionEditSaving = Resources.GalleryServerPro.UC_MoView_Saving,
				MediaCaptionEditTt = Resources.GalleryServerPro.UC_MoView_Edit_Tt,
				MediaDeleteConfirm = Resources.GalleryServerPro.UC_MoView_Tb_Delete_Confirm,
				MetaEditPlaceholder = Resources.GalleryServerPro.UC_MoView_Meta_Edit_Placeholder,
				SyncStarting = Resources.GalleryServerPro.Task_Synch_Progress_SynchStarting,
				SyncAborting = Resources.GalleryServerPro.Task_Synch_Progress_SynchCanceling,
				SyncAbort = Resources.GalleryServerPro.Task_Synch_Cancel_Button_Text
			};
		}
	}
}