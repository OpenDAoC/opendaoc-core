using System;
using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler;
using System.Collections;

namespace DOL.GS.Commands
{
	[Command("&makeleader",
		 new string[] { "&m" },
		 EPrivLevel.Player,
		 "Set a new group leader (can be used by current leader).",
		 "/m <playerName>")]

	public class MakeLeaderCommand : ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.Group == null || client.Player.Group.MemberCount < 2)
			{
				client.Out.SendMessage("You are not part of a group.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				return;
			}
			if(client.Player.Group.Leader != client.Player)
			{
				client.Out.SendMessage("You are not the leader of your group.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				return;
			}

			GamePlayer target;

			if(args.Length<2) // Setting by target
			{
				if(client.Player.TargetObject == null || client.Player.TargetObject == client.Player)
				{
					client.Out.SendMessage("You have not selected a valid player as your target.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return;
				}

				if(!(client.Player.TargetObject is GamePlayer))
				{
					client.Out.SendMessage("You have not selected a valid player as your target.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return;
				}
				target = (GamePlayer)client.Player.TargetObject;
				if(client.Player.Group != target.Group)
				{
					client.Out.SendMessage("You have not selected a valid player as your target.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return;
				}
			}
			else //Setting by name
			{
				string targetName = args[1];
				GameClient targetClient = WorldMgr.GetClientByPlayerName(targetName, false, true);
				if (targetClient == null)
					target = null;
				else target = targetClient.Player;
				if(target==null || client.Player.Group != target.Group)
				{ // Invalid target
					client.Out.SendMessage("No players in group with that name.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return;
				}
				if(target==client.Player)
				{
					client.Out.SendMessage("You are the group leader already.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return;
				}

			}

			client.Player.Group.MakeLeader(target);
		}
	}
}