using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GalleryServerPro.Business.Metadata;

namespace GalleryServerPro.Data
{
	[Table("Album", Schema = "gsp")]
	public class AlbumDto
	{
		[Key]
		public virtual int AlbumId
		{
			get;
			set;
		}

		[Required]
		public virtual int FKGalleryId
		{
			get;
			set;
		}

		[ForeignKey("FKGalleryId")]
		public virtual GalleryDto Gallery
		{
			get;
			set;
		}

		public virtual int? FKAlbumParentId
		{
			get;
			set;
		}

		[ForeignKey("FKAlbumParentId")]
		public virtual AlbumDto AlbumParent
		{
			get;
			set;
		}

		[Required(AllowEmptyStrings = true), MaxLength(255)]
		public virtual string DirectoryName
		{
			get;
			set;
		}

		[Required]
		public virtual int ThumbnailMediaObjectId
		{
			get;
			set;
		}

		[Required]
		public virtual MetadataItemName SortByMetaName
		{
			get;
			set;
		}

		[Required]
		public virtual bool SortAscending
		{
			get;
			set;
		}

		[Required]
		public virtual int Seq
		{
			get;
			set;
		}

		public virtual System.DateTime? DateStart
		{
			get;
			set;
		}

		public virtual System.DateTime? DateEnd
		{
			get;
			set;
		}

		[Required]
		public virtual System.DateTime DateAdded
		{
			get;
			set;
		}

		[Required, MaxLength(256)]
		public virtual string CreatedBy
		{
			get;
			set;
		}

		[Required, MaxLength(256)]
		public virtual string LastModifiedBy
		{
			get;
			set;
		}

		[Required]
		public virtual System.DateTime DateLastModified
		{
			get;
			set;
		}

		[Required(AllowEmptyStrings = true), MaxLength(256)]
		public virtual string OwnedBy
		{
			get;
			set;
		}

		[Required(AllowEmptyStrings = true), MaxLength(256)]
		public virtual string OwnerRoleName
		{
			get;
			set;
		}

		[Required]
		public virtual bool IsPrivate
		{
			get;
			set;
		}

		public virtual ICollection<MetadataDto> Metadata
		{
			get;
			set;
		}

		public virtual ICollection<UiTemplateAlbumDto> UiTemplates
		{
			get;
			set;
		}
	}
}
