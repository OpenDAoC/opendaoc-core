using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&debug",
		ePrivLevel.GM,
		"GMCommands.Debug.Description",
		"GMCommands.Debug.Usage")]
	public class DebugCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client == null || client.Player == null)
			{
				return;
			}

			if (IsSpammingCommand(client.Player, "Debug"))
			{
				return;
			}

			// extra check to disallow all but server GM's
			if (client.Account.PrivLevel < 2)
				return;

			if (args.Length < 2)
			{
				DisplaySyntax(client);
				return;
			}
			if (args[1].ToLower().Equals("on"))
			{
				client.Player.TempProperties.setProperty(GamePlayer.DEBUG_MODE_PROPERTY, true);
				client.Player.IsAllowedToFly = true;
				client.Out.SendDebugMode(true);
				DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Debug.ModeON"));
			}
			else if (args[1].ToLower().Equals("off"))
			{
				client.Player.TempProperties.removeProperty(GamePlayer.DEBUG_MODE_PROPERTY);
				client.Out.SendDebugMode(false);
				client.Player.IsAllowedToFly = false;
				DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Debug.ModeOFF"));
			}
		}
	}
}