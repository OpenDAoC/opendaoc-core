
using System.Linq;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command(
		"&invite",
		EPrivLevel.Player,
		"Invite a specified or targeted player to join your group", "/invite <player>")]
	public class InviteCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.Group != null && client.Player.Group.Leader != client.Player)
			{
				client.Out.SendMessage("You are not the leader of your group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				return;
			}

			if (IsSpammingCommand(client.Player, "invite"))
				return;

			string targetName = string.Join(" ", args, 1, args.Length - 1);
			GamePlayer target;

			if (args.Length < 2)
			{ // Inviting by target
				if (client.Player.TargetObject == null || client.Player.TargetObject == client.Player)
				{
					client.Out.SendMessage("You have not selected a valid player as your target.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}

				if (!(client.Player.TargetObject is GamePlayer))
				{
					client.Out.SendMessage("You have not selected a valid player as your target.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}
				target = (GamePlayer) client.Player.TargetObject;
				
				if (!GameServer.ServerRules.IsAllowedToGroup(client.Player, target, false))
				{
					return;
				}
				
				if (target.NoHelp)
				{
					client.Out.SendMessage(target.Name + "has chosen the solo path and can't join your group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}
				
			}
			else
			{ // Inviting by name
				var matchingClients = WorldMgr.GetClientByPlayerNameAndRealm(targetName, client.Player.Realm, true);
				GameClient targetClient = null;

				if (matchingClients != null)
				{
					if (matchingClients.Count == 1) targetClient = matchingClients.First();
					else
					{
						client.Out.SendMessage("More than one online player matches that name.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
						return;
					}
				}

				if (targetClient == null)
					target = null;
				else
					target = targetClient.Player;
				if (target == null || !GameServer.ServerRules.IsAllowedToGroup(client.Player, target, true))
				{ // Invalid target or realm restriction
					client.Out.SendMessage("No players online with that name.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}
				if (target == client.Player)
				{
					client.Out.SendMessage("You can't invite yourself.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}
				if (target.NoHelp)
				{
					client.Out.SendMessage(target.Name + "has chosen the solo path and can't join your group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}
			}

			if (target.Group != null)
			{
				client.Out.SendMessage("The player is still in a group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				return;
			}

			if (client.Account.PrivLevel > target.Client.Account.PrivLevel)
			{
				// you have no choice!

				if (client.Player.Group == null)
				{
					Group group = new Group(client.Player);
					GroupMgr.AddGroup(group);
					group.AddMember(client.Player);
					group.AddMember(target);
				}
				else if (target.NoHelp)
				{
					client.Out.SendMessage("Grouping this player would void their SOLO challenge", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
				} else
				{
					client.Player.Group.AddMember(target);
				}

				client.Out.SendMessage("(GM) You have added " + target.Name + " to your group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				target.Out.SendMessage("GM " + client.Player.Name + " has added you to " + client.Player.GetPronoun(1, false) + " group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
			}
			else
			{
				client.Out.SendMessage("You have invited " + target.Name + " to join your group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				target.Out.SendGroupInviteCommand(client.Player, client.Player.Name + " has invited you to join\n" + client.Player.GetPronoun(1, false) + " group. Do you wish to join?");
				target.Out.SendMessage(client.Player.Name + " has invited you to join " + client.Player.GetPronoun(1, false) + " group.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
			}
		}
	}
}