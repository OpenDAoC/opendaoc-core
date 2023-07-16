using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&dismount",
		ePrivLevel.Player,
		"Dismount your steed",
		"/dismount")]
	public class RideDismountCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (!client.Player.IsRiding)
			{
				if (client.Player.IsOnHorse)
					client.Player.IsOnHorse = false;
				else
					DisplayMessage(client, (LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Dismount")));
			}
			else 
				client.Player.DismountSteed(false);
		}
	}
}
