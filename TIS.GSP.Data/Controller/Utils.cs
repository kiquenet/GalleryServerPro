using System;
using GalleryServerPro.Business;

namespace GalleryServerPro.Data
{
	/// <summary>
	/// General purpose utilities to assist the data layer.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Gets the schema-qualified name for the specified <paramref name="rawName" />. For SQL Server, the name is prefixed with the schema "gsp.".
		/// For SQL CE, the value is returned without any changes. An exception is thrown for all other data providers.
		/// </summary>
		/// <param name="rawName">Name of the table.</param>
		/// <param name="galleryDataStore">The data provider for the gallery data.</param>
		/// <param name="schema">The schema. Defaults to "gsp" if not specified.</param>
		/// <returns>Returns the schema qualified name for <paramref name="rawName" />.</returns>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException"></exception>
		public static string GetSqlName(string rawName, ProviderDataStore galleryDataStore, string schema = "gsp")
		{
			switch (galleryDataStore)
			{
				case ProviderDataStore.SqlServer:
					return String.Concat(schema, ".", rawName);
				case ProviderDataStore.SqlCe:
					return rawName;
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException(string.Format("This function is not designed to handle the enum value {0}.", galleryDataStore));
			}
		}
	}
}