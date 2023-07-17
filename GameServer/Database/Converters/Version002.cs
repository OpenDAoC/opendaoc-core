using System;
using log4net;
using DOL.Database;

namespace DOL.GS.DatabaseConverters
{
	/// <summary>
	/// Converts the database format to the version 2
	/// </summary>
	[DatabaseConverter(2)]
	public class Version002 : IDatabaseConverter
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// style icon field added this should copy the ID value
		/// realm 6 should be peace flag and realm changed
		/// </summary>
		public void ConvertDatabase()
		{
			log.Info("Database Version 2 Convert Started");

			log.Info("Converting Styles");
			var styles = GameServer.Database.SelectAllObjects<DBStyle>();
			foreach (DBStyle style in styles)
			{
				style.Icon = style.ID;

				GameServer.Database.SaveObject(style);
			}
			log.Info(styles.Count + " Styles Processed");

			log.Info("Converting Mobs");
			var mobs = CoreDb<DbMobs>.SelectObjects(DB.Column("Realm").IsEqualTo(6));
			foreach (DbMobs mob in mobs)
			{
				if ((mob.Flags & (uint)GameNPC.eFlags.PEACE) == 0)
				{
					mob.Flags ^= (uint)GameNPC.eFlags.PEACE;
				}

				Region region = WorldMgr.GetRegion(mob.Region);
				if (region != null)
				{
					Zone zone = region.GetZone(mob.X, mob.Y);
					if (zone != null)
					{
						mob.Realm = (byte)zone.Realm;
					}
				}

				GameServer.Database.SaveObject(mob);
			}
			log.Info(mobs.Count + " Mobs Processed");

			log.Info("Database Version 2 Convert Finished");
		}
	}
}
