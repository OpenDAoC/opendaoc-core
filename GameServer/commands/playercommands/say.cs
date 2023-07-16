using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&say",
		new string[] {"&s"},
		ePrivLevel.Player,
		"Say something to other players around you",
		"/say <message>")]
	public class SayCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			const string SAY_TICK = "Say_Tick";

			if (args.Length < 2)
			{
				client.Out.SendMessage("You must say something...", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			string message = string.Join(" ", args, 1, args.Length - 1);

			long SayTick = client.Player.TempProperties.getProperty<long>(SAY_TICK);
			if (SayTick > 0 && SayTick - client.Player.CurrentRegion.Time <= 0)
			{
				client.Player.TempProperties.removeProperty(SAY_TICK);
			}

			long changeTime = client.Player.CurrentRegion.Time - SayTick;
			if (changeTime < 500 && SayTick > 0)
			{
				client.Player.Out.SendMessage("Slow down! Think before you say each word!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
            if (client.Player.IsMuted)
            {
                client.Player.Out.SendMessage("You have been muted. You cannot talk.", eChatType.CT_Staff, eChatLoc.CL_SystemWindow);
                return;
            }

			client.Player.Say(message);
			client.Player.TempProperties.setProperty(SAY_TICK, client.Player.CurrentRegion.Time);
		}
	}
}