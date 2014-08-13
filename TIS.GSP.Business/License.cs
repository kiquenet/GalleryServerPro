﻿using System;
using GalleryServerPro.Business.Interfaces;
using GalleryServerPro.Data;

namespace GalleryServerPro.Business
{
	/// <summary>
	/// Represents a license for the Gallery Server Pro software.
	/// </summary>
	public class License : ILicense
	{
		/// <summary>
		/// Gets or sets the product key.
		/// </summary>
		/// <value>The product key.</value>
		public string ProductKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the license contained in this instance is legitimate and authorized.
		/// </summary>
		/// <value><c>true</c> if the license is valid; otherwise, <c>false</c>.</value>
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets a value indicating whether the application is currently in the initial 30-day trial period.
		/// </summary>
		/// <value><c>true</c> if the application is in the trial period; otherwise, <c>false</c>.</value>
		public bool IsInTrialPeriod
		{
			get
			{
				return (InstallDate.AddDays(GlobalConstants.TrialNumberOfDays) >= DateTime.Today);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the initial 30-day trial for the application has expired and no valid product key 
		/// has been entered.
		/// </summary>
		/// <value><c>true</c> if the application is in the trial period; otherwise, <c>false</c>.</value>
		public bool IsInReducedFunctionalityMode
		{
			get
			{
				return (!IsInTrialPeriod && !IsValid);
			}
		}

		/// <summary>
		/// Gets or sets a message explaining why the key is invalid. Applies only when <see cref="ILicense.IsValid" /> is <c>false</c>.
		/// </summary>
		/// <value>A string explaining why the key is invalid.</value>
		public string KeyInvalidReason { get; set; }

		/// <summary>
		/// Gets or sets the e-mail the license is assigned to.
		/// </summary>
		/// <value>The e-mail the license is assigned to.</value>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the application version the license applies to. Example: 2.4, 2.5
		/// </summary>
		/// <value>The application version the license applies to.</value>
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets the type of the license applied to the current application.
		/// </summary>
		/// <value>The type of the license.</value>
		public LicenseLevel LicenseType { get; set; }

		/// <summary>
		/// Gets the date/time this application was installed. The timestamp of the oldest gallery's creation date is
		/// considered to be the application install date.
		/// </summary>
		/// <value>The date/time this application was installed.</value>
		public DateTime InstallDate { get; set; }

		/// <summary>
		/// Verify whether the product key is valid. This method updates <see cref="IsValid" />, <see cref="KeyInvalidReason" />,
		/// and <see cref="LicenseType" />, which may be inspected for followup action.
		/// </summary>
		/// <param name="performWebServiceValidation">if set to <c>true</c> perform the validation using a web service.</param>
		public void Validate(bool performWebServiceValidation)
		{
			KeyInvalidReason = String.Empty;

			switch (ProductKey)
			{
				case Constants.ProductKeyGpl:
				case Constants.ProductKeyProfessional:
					if (!IsInTrialPeriod && AppSetting.Instance.ProviderDataStore == ProviderDataStore.SqlServer)
					{
						KeyInvalidReason = "SQL Server detected. The product key you entered requires the use of SQL CE for the data store. You must switch to SQL CE or, if you wish to continue using SQL Server, purchase an Enterprise license. SQL Server offers faster performance and greater reliability. We highly recommend it.";
						IsValid = false;
						LicenseType = LicenseLevel.NotSet;
					}
					else
					{
						IsValid = true;
						LicenseType = (ProductKey.Equals(Constants.ProductKeyProfessional) ? LicenseLevel.Professional : LicenseLevel.Gpl);
					}
					break;

				case Constants.ProductKeyEnterprise:
					IsValid = true;
					LicenseType = LicenseLevel.Enterprise;
					break;

				default:
					KeyInvalidReason = "Product key is not valid";
					IsValid = false;
					LicenseType = LicenseLevel.NotSet;
					break;
			}
		}
	}
}