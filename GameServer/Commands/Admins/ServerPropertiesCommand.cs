using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[Command(
		"&serverproperties",
		ePrivLevel.Admin,
		"AdminCommands.ServerProperties.Description",
		"AdminCommands.ServerProperties.Usage")]
	public class ServerPropertiesCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (GameServer.Instance.Configuration.DBType == DOL.Database.Connection.EConnectionType.DATABASE_XML)
			{
				DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "AdminCommands.ServerProperties.DataBaseXML"));
				return;
			}
			ServerProperties.Properties.Refresh();
			DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "AdminCommands.ServerProperties.PropertiesRefreshed"));
		}
	}
}