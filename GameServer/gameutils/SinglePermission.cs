using System.Linq;
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// SinglePermission is special permission of command for player
	/// </summary>
	public class SinglePermission
	{
		protected SinglePermission()
		{

		}

		public static bool HasPermission(GamePlayer player,string command)
		{
			var obj = CoreDb<DBSinglePermission>.SelectObject(DB.Column("Command").IsEqualTo(command).And(DB.Column("PlayerID").IsEqualTo(player.ObjectId).Or(DB.Column("PlayerID").IsEqualTo(player.AccountName))));
			if (obj == null)
				return false;
			return true;
		}

		public static void setPermission(GamePlayer player,string command)
		{
			DBSinglePermission perm = new DBSinglePermission();
			perm.Command = command;
			perm.PlayerID = player.ObjectId;
			GameServer.Database.AddObject(perm);
		}

		public static void setPermissionAccount(GamePlayer player, string command)
		{
			DBSinglePermission perm = new DBSinglePermission();
			perm.Command = command;
			perm.PlayerID = player.AccountName;
			GameServer.Database.AddObject(perm);
		}

		public static bool removePermission(GamePlayer player,string command)
		{
			var obj = CoreDb<DBSinglePermission>.SelectObject(DB.Column("Command").IsEqualTo(command).And(DB.Column("PlayerID").IsEqualTo(player.ObjectId)));
			if (obj == null)
			{
				return false;
			}
			GameServer.Database.DeleteObject(obj);
			return true;
        }

        public static bool removePermissionAccount(GamePlayer player, string command)
        {
            var obj = CoreDb<DBSinglePermission>.SelectObject(DB.Column("Command").IsEqualTo(command).And(DB.Column("PlayerID").IsEqualTo(player.AccountName)));
            if (obj == null)
            {
                return false;
            }
            GameServer.Database.DeleteObject(obj);
            return true;
        }
	}
}