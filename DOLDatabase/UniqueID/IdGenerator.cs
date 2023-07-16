using System;

namespace DOL.Database.UniqueID
{
	/// <summary>
	/// Generates an UniqeID for every Object.
	/// </summary>
	public static class IDGenerator
	{
		/// <summary>
		/// Generate a new GUID String
		/// </summary>
		/// <returns>a new unique Key</returns>
		public static string GenerateID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}