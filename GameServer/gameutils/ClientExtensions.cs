﻿
using System;

using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// Client Extensions Utils.
	/// </summary>
	public static class ClientExtensions
	{
        
        #region Account Util
        public static void BanAccount(this GameClient client, string reason)
        {
			DBBannedAccount b = new DBBannedAccount();
			b.Author = "SERVER";
			b.Ip = client.TcpEndpointAddress;
			b.Account = client.Account.Name;
			b.DateBan = DateTime.Now;
			b.Type = "B";
			b.Reason = reason;
			GameServer.Database.AddObject(b);
			GameServer.Database.SaveObject(b);
			GameServer.Instance.LogCheatAction(string.Format("{1}. Client Account: {0}", client.Account.Name, b.Reason));
        }
        #endregion
	}
}
