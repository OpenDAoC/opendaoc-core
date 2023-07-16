using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Cmd(
		"&clientlist",
		ePrivLevel.GM,
        "Usage: /clientlist [full] - full option includes IP's and accounts",
		"Show a list of currently playing clients and their ID's")]
	public class ClientListCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			var clients = WorldMgr.GetAllPlayingClients();
			var message = new List<string>();

			foreach (GameClient gc in clients)
			{
				if (gc.Player != null)
				{
                    if (args.Length > 1 && args[1].ToLower() == "full")
                    {
                        message.Add("(" + gc.SessionID + ") " + gc.TcpEndpointAddress + ", " + gc.Account.Name + ", " + gc.Player.Name + " " + gc.Version);
                    }
                    else
                    {
                        message.Add("(" + gc.SessionID + ") " + gc.Player.Name);
                    }
				}
			}

			client.Out.SendCustomTextWindow("[ Playing Client List ]", message);
			return;
		}
	}
}
