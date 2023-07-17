using System.Collections;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command("&target", EPrivLevel.Player, "target a player by name", "/target <playerName>")]
	public class TargetCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "target"))
				return;

			GamePlayer targetPlayer = null;
			if (args.Length == 2)
			{
				int result = 0;
				GameClient targetClient = WorldMgr.GuessClientByPlayerNameAndRealm(args[1], 0, true, out result);
				if (targetClient != null)
				{
					targetPlayer = targetClient.Player;

					if (!client.Player.IsWithinRadius( targetPlayer, WorldMgr.YELL_DISTANCE ) || targetPlayer.IsStealthed || GameServer.ServerRules.IsAllowedToAttack(client.Player, targetPlayer, true))
					{
						client.Out.SendMessage("You don't see " + args[1] + " around here!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						return;
					}

					client.Out.SendChangeTarget(targetPlayer);
					client.Out.SendMessage("You target " + targetPlayer.GetName(0, true) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return;
				}
				if (client.Account.PrivLevel > 1)
				{
					IEnumerator en = client.Player.GetNPCsInRadius(800).GetEnumerator();
					while (en.MoveNext())
					{
						if (((GameObject)en.Current).Name == args[1])
						{
							client.Out.SendChangeTarget((GameObject)en.Current);
							client.Out.SendMessage("[GM] You target " + ((GameObject)en.Current).GetName(0, true) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return;
						}
					}
				}

				client.Out.SendMessage("You don't see " + args[1] + " around here!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			if (client.Account.PrivLevel > 1)
			{
				client.Out.SendMessage("/target <player/mobname>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
			else
			{
				client.Out.SendMessage("/target <playername>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
		}
	}
}