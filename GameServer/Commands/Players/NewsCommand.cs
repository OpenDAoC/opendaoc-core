using System;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command(
		"&news",
		EPrivLevel.Player,
		"Show news on social interface",
		"/news")]
	public class NewsCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player == null)
				return;

			if (IsSpammingCommand(client.Player, "news"))
				return;

			NewsMgr.DisplayNews(client);
		}
	}
}