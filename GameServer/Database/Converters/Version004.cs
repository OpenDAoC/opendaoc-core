using System;
using System.Collections;
using log4net;
using DOL.Database;

namespace DOL.GS.DatabaseConverters
{
	/// <summary>
	/// Converts the database format to the version 3
	/// </summary>
	[DatabaseConverter(4)]
	public class Version004 : IDatabaseConverter
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// we need to make use of the new poison fields
		/// </summary>
		public void ConvertDatabase()
		{
			log.Info("Database Version 4 Convert Started");

			if (GameServer.Instance.Configuration.DBType == DOL.Database.Connection.EConnectionType.DATABASE_XML)
			{
				log.Info("You have an XML database loaded, this converter will only work with MySQL, skipping");
				return;
			}

			var mobs = CoreDb<DbMobs>.SelectObjects(DB.Column("ClassType").IsEqualTo("DOL.GS.GameMob"));

			int count = 0;
			foreach (DbMobs mob in mobs)
			{
				mob.ClassType = "DOL.GS.GameNPC";
				GameServer.Database.SaveObject(mob);
				count++;
			}

			log.Info("Converted " + count + " mobs");

			log.Info("Database Version 4 Convert Finished");
		}
	}
}
