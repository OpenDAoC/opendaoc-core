using System;
using System.Collections;
using log4net;
using DOL.Database;

namespace DOL.GS.DatabaseUpdate
{
	/// <summary>
	/// Checks and updates the new SalvageYields table
	/// </summary>
	[DatabaseUpdate]
	public class SalvageYieldsUpdate : IDatabaseUpdater
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// If new SalvageYield table is empty then copy values from old Salvage table
		/// </summary>
		public void Update()
		{
			int count = 0;
			var newSalvage = GameServer.Database.SelectAllObjects<DbSalvageYields>();

			if (newSalvage == null || newSalvage.Count == 0)
			{
				log.InfoFormat("Updating the SalvageYield table...", count);

				var oldSalvage = GameServer.Database.SelectAllObjects<DbSalvage>();

				foreach (DbSalvage salvage in oldSalvage)
				{
					DbSalvageYields salvageYield = new DbSalvageYields();
					salvageYield.ID = ++count; // start at 1
					salvageYield.ObjectType = salvage.ObjectType;
					salvageYield.SalvageLevel = salvage.SalvageLevel;
					salvageYield.MaterialId_nb = salvage.Id_nb;
					salvageYield.Count = 0;
					salvageYield.Realm = salvage.Realm;
					salvageYield.PackageID = DbSalvageYields.LEGACY_SALVAGE_ID;
					GameServer.Database.AddObject(salvageYield);
				}
			}

			if (count > 0)
			{
				log.InfoFormat("Copied {0} entries from Salvage to SalvageYield.", count);
			}
		}
	}
}
