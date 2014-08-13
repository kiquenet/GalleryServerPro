
using System;
using GalleryServerPro.Data;
using GalleryServerPro.Events;
using GalleryServerPro.Events.CustomExceptions;

namespace GalleryServerPro.Business
{
	/// <summary>
	/// Contains functionality for performing administrative and maintenance tasks of the database.
	/// </summary>
	public static class DbManager
	{
		#region Methods

		/// <summary>
		/// Compacts and, if necessary, repairs the database. Applies only to SQL CE. A detailed message describing 
		/// the result of the operation is assigned to <paramref name="message" />.
		/// </summary>
		/// <param name="message">A detailed message describing the result of the operation.</param>
		/// <returns>Returns <c>true</c> if the operation is successful; otherwise returns <c>false</c>.</returns>
		public static bool CompactAndRepairSqlCeDatabase(out string message)
		{
			var sqlCeController = new SqlCeController(Factory.GetConnectionStringSettings().ConnectionString);

			var compactSuccessful = false;
			var repairNeeded = false;
			var repairSuccessful = false;
			Exception ex = null;

			try
			{
				if (!sqlCeController.Verify())
				{
					repairNeeded = true;
					sqlCeController.Repair();

					repairSuccessful = sqlCeController.Verify();
				}

				sqlCeController.Compact();
				compactSuccessful = true;
			}
			catch (Exception exception)
			{
				ex = exception;
				EventController.RecordError(ex, AppSetting.Instance);
			}
			message = GetCompactAndRepairMessage(ex, compactSuccessful, repairNeeded, repairSuccessful);

			return (compactSuccessful && (!repairNeeded || repairSuccessful));
		}

		#endregion

		#region Functions

		private static string GetCompactAndRepairMessage(Exception ex, bool compactSuccessful, bool repairNeeded, bool repairSuccessful)
		{
			string msg = null;

			if (ex != null) // An exception occurred.
			{
				if (!compactSuccessful)
					msg = String.Concat("The following error occurred while compacting the database: ", EventController.GetExceptionDetails(ex));
				else if (compactSuccessful && !repairNeeded)
					msg = String.Concat("The database was successfully compacted but the following error occurred while checking the database for errors: ", EventController.GetExceptionDetails(ex));
				else if (compactSuccessful && repairNeeded && !repairSuccessful)
					msg = String.Concat("The database was successfully compacted. However, data corruption was found and the following error occurred while attempting to fix the errors: ", EventController.GetExceptionDetails(ex));
				else
					msg = String.Concat("The following error occurred: ", EventController.GetExceptionDetails(ex)); // This should never execute unless a dev changed the logic in CompactAndRepairSqlCe()
			}
			else // No exception occurred, compactSuccessful is guaranteed to be true
			{
				if (compactSuccessful && !repairNeeded)
					msg = "The SQL CE database was successfully compacted. No corruption was found.";
				else if (compactSuccessful && repairNeeded && !repairSuccessful)
					msg = "The SQL CE database was successfully compacted. Data corruption was found but could not be automatically repaired. Consider using the backup function to back up your data and restore to a new instance of your gallery.";
				else if (compactSuccessful && repairNeeded && repairSuccessful)
					msg = "The SQL CE database was successfully compacted. Data corruption was found and automatically repaired.";
				else
					throw new BusinessException(String.Format("An unexpected combination of parameters was passed to GetCompactAndRepairMessage(). ex != null; compactSuccessful={0}; repairNeeded={1}; repairSuccessful={2}", compactSuccessful, repairNeeded, repairSuccessful));
			}

			return msg;
		}

		#endregion
	}
}
