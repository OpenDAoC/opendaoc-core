using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command(
		"&gu",
		new string[] {"&guild"},
		EPrivLevel.Player,
		"Guild Chat command",
		"/gu <text>")]
	public class GuildChatCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.Guild == null)
			{
				DisplayMessage(client, "You don't belong to a player guild.");
				return;
			}

			if (!client.Player.Guild.HasRank(client.Player, GuildUtil.EGuildRank.GcSpeak))
			{
				DisplayMessage(client, "You don't have permission to speak on the on guild line.");
				return;
			}

			if (IsSpammingCommand(client.Player, "guildchat", 500))
			{
				DisplayMessage(client, "Slow down! Think before you say each word!");
				return;
			}

			string message = "[Guild] " + client.Player.Name + ": \"" + string.Join(" ", args, 1, args.Length - 1) + "\"";
			client.Player.Guild.SendMessageToGuildMembers(message, EChatType.CT_Guild, EChatLoc.CL_ChatWindow);
		}
	}

	[Command(
		"&o",
		new string[] {"&osend"},
		EPrivLevel.Player,
		"Speak in officer chat (Must be a guild officer)",
		"/o <text>")]
	public class OfficerGuildChatCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.Guild == null)
			{
				DisplayMessage(client, "You don't belong to a player guild.");
				return;
			}

			if (!client.Player.Guild.HasRank(client.Player, GuildUtil.EGuildRank.OcSpeak))
			{
				DisplayMessage(client, "You don't have permission to speak on the officer line.");
				return;
			}

			if (IsSpammingCommand(client.Player, "osend", 500))
			{
				DisplayMessage(client, "Slow down! Think before you say each word!");
				return;
			}

			string message = "[Officers] " + client.Player.Name + ": \"" + string.Join(" ", args, 1, args.Length - 1) + "\"";
			foreach (GamePlayer ply in client.Player.Guild.GetListOfOnlineMembers())
			{
				if (!client.Player.Guild.HasRank(ply, GuildUtil.EGuildRank.OcHear))
				{
					continue;
				}
				ply.Out.SendMessage(message, EChatType.CT_Officer, EChatLoc.CL_ChatWindow);
			}
		}
	}

	[Command(
		"&as",
		new string[] {"&asend"},
		EPrivLevel.Player,
		"Sends a message to the alliance chat",
		"/as <text>")]
	public class AllianceGuildChatCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.Guild == null)
			{
				DisplayMessage(client, "You don't belong to a player guild.");
				return;
			}

			if (client.Player.Guild.alliance == null)
			{
				DisplayMessage(client, "Your guild doesn't belong to any alliance.");
				return;
			}

			if (!client.Player.Guild.HasRank(client.Player, GuildUtil.EGuildRank.AcSpeak))
			{
				DisplayMessage(client, "You can not speak on alliance chan.");
				return;
			}

			if (client.Player.IsMuted)
			{
				client.Player.Out.SendMessage("You have been muted and are not allowed to speak in this channel.", EChatType.CT_Staff, EChatLoc.CL_SystemWindow);
				return;
			}

			string message = "[Alliance] " + client.Player.Name + ": \"" + string.Join(" ", args, 1, args.Length - 1) + "\"";
			foreach (GuildUtil gui in client.Player.Guild.alliance.Guilds)
			{
				foreach (GamePlayer ply in gui.GetListOfOnlineMembers())
				{
					if (!gui.HasRank(ply, GuildUtil.EGuildRank.AcHear))
					{
						continue;
					}
					ply.Out.SendMessage(message, EChatType.CT_Alliance, EChatLoc.CL_ChatWindow);
				}
			}
		}
	}
}