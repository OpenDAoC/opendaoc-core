using System;

namespace DOL.GS.DatabaseUpdate
{
	/// <summary>
	/// Interface for all database updaters
	/// </summary>
	public interface IDatabaseUpdater
	{
		/// <summary>
		/// Converts the database to new version specified in attribute
		/// </summary>
		void Update();
	}
}
