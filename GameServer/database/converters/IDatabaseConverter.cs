using System;

namespace DOL.GS.DatabaseConverters
{
	/// <summary>
	/// Interface for all database format converters
	/// </summary>
	public interface IDatabaseConverter
	{
		/// <summary>
		/// Converts the database to new version specified in attribute
		/// </summary>
		void ConvertDatabase();
	}
}
