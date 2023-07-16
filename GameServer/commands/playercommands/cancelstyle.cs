using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[CmdAttribute("&cancelstyle", ePrivLevel.Player, "Toggle cancelstyle flag.", "/cancelstyle")]
	public class CancelStyleCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "cancelstyle"))
				return;

			client.Player.styleComponent.CancelStyle = !client.Player.styleComponent.CancelStyle;
			DisplayMessage(client, string.Format(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Cancelstyle.Set",
				(client.Player.styleComponent.CancelStyle ? LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Cancelstyle.On") : LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Cancelstyle.Off")))));
		}
	}
}
