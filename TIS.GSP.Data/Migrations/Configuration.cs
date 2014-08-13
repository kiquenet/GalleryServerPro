using System;
using System.Data.Entity.Migrations;

namespace GalleryServerPro.Data.Migrations
{
	/// <summary>
	/// Configuration related to the use of Code First data migrations for the gallery database.
	/// </summary>
	/// <remarks>
	/// To add a new migration, run Add-Migration v3.0.1 (replace 3.0.1 with the new version).
	/// To re-scaffold a migration, run 'Add-Migration 201306241655425_v3.0.1' (replace name with auto-generated name)
	/// </remarks>
	public sealed class GalleryDbMigrationConfiguration : DbMigrationsConfiguration<GalleryDb>
	{
		public GalleryDbMigrationConfiguration()
		{
		}

		public GalleryDbMigrationConfiguration(Business.ProviderDataStore galleryDataStore)
		{
			if (galleryDataStore == Business.ProviderDataStore.SqlServer)
			{
				// Increase the timeout used to apply migration changes, but only for SQL Server (SQL CE will throw an exception).
				CommandTimeout = 3600;
			}
			
			//AutomaticMigrationsEnabled = true;
			//AutomaticMigrationDataLossAllowed = true;
		}

		/// <summary>
		/// Runs after upgrading to the latest migration to allow seed data to be updated. Use this opportunity to apply bug
		/// fixes requiring updates to database values and to update the data schema in the AppSetting table.
		/// </summary>
		/// <param name="ctx">Context to be used for updating seed data.</param>
		protected override void Seed(GalleryDb ctx)
		{
			MigrateController.ApplyDbUpdates(ctx);
		}
	}
}
