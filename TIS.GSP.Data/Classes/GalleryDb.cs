using System.Data.Entity;
using GalleryServerPro.Business;

namespace GalleryServerPro.Data
{
	public class GalleryDb : DbContext
	{
		public GalleryDb()
		{
			this.Configuration.LazyLoadingEnabled = false;
		}

		// Uncomment if access to the underlying ObjectContext is needed.
		//public System.Data.Objects.ObjectContext ObjectContext
		//{
		//  get { return ((System.Data.Entity.Infrastructure.IObjectContextAdapter) this).ObjectContext; }
		//}

		public DbSet<AlbumDto> Albums { get; set; }
		public DbSet<EventDto> Events { get; set; }
		public DbSet<AppSettingDto> AppSettings { get; set; }
		public DbSet<MediaTemplateDto> MediaTemplates { get; set; }
		public DbSet<GalleryControlSettingDto> GalleryControlSettings { get; set; }
		public DbSet<GalleryDto> Galleries { get; set; }
		public DbSet<GallerySettingDto> GallerySettings { get; set; }
		public DbSet<MediaObjectDto> MediaObjects { get; set; }
		public DbSet<MetadataDto> Metadatas { get; set; }
		public DbSet<MimeTypeDto> MimeTypes { get; set; }
		public DbSet<MimeTypeGalleryDto> MimeTypeGalleries { get; set; }
		public DbSet<RoleDto> Roles { get; set; }
		public DbSet<SynchronizeDto> Synchronizes { get; set; }
		public DbSet<RoleAlbumDto> RoleAlbums { get; set; }
		public DbSet<UserGalleryProfileDto> UserGalleryProfiles { get; set; }
		public DbSet<MediaQueueDto> MediaQueues { get; set; }
		public DbSet<UiTemplateDto> UiTemplates { get; set; }
		public DbSet<UiTemplateAlbumDto> UiTemplateAlbums { get; set; }
		public DbSet<TagDto> Tags { get; set; }
		public DbSet<MetadataTagDto> MetadataTags { get; set; }

		/// <summary>
		/// Gets the version of the current data schema.
		/// </summary>
		/// <value>The data schema version.</value>
		public static GalleryDataSchemaVersion DataSchemaVersion
		{
			get
			{
				return GalleryDataSchemaVersion.V3_2_1;
			}
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Set up relationship to enforce cascade delete between media objects and their metadata (by default it is set to NO ACTION)
			modelBuilder.Entity<MetadataDto>()
				.HasOptional(t => t.MediaObject)
				.WithMany(t => t.Metadata)
				.HasForeignKey(t => t.FKMediaObjectId)
				.WillCascadeOnDelete(true);

			// Can't create a cascade delete between albums and their metadata, as we get this error when we try:
			// "Introducing FOREIGN KEY constraint 'FK_dbo.gsp_Metadata_dbo.gsp_Album_FKAlbumId' on table 'gsp_Metadata' may cause cycles or multiple cascade paths."
			// We just have to make sure the app deletes 
			//modelBuilder.Entity<MetadataDto>()
			//  .HasOptional(t => t.Album)
			//  .WithMany(t => t.Metadata)
			//  .HasForeignKey(t => t.FKAlbumId)
			//  .WillCascadeOnDelete(true);
		}
	}
}
