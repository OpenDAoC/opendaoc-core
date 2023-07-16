using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&gu",
		new string[] {"&guild"},
		ePrivLevel.Player,
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

			if (!client.Player.Guild.HasRank(client.Player, Guild.eRank.GcSpeak))
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
			client.Player.Guild.SendMessageToGuildMembers(message, eChatType.CT_Guild, eChatLoc.CL_ChatWindow);
		}
	}

	[CmdAttribute(
		"&o",
		new string[] {"&osend"},
		ePrivLevel.Player,
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

			if (!client.Player.Guild.HasRank(client.Player, Guild.eRank.OcSpeak))
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
				if (!client.Player.Guild.HasRank(ply, Guild.eRank.OcHear))
				{
					continue;
				}
				ply.Out.SendMessage(message, eChatType.CT_Officer, eChatLoc.CL_ChatWindow);
			}
		}
	}

	[CmdAttribute(
		"&as",
		new string[] {"&asend"},
		ePrivLevel.Player,
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

			if (!client.Player.Guild.HasRank(client.Player, Guild.eRank.AcSpeak))
			{
				DisplayMessage(client, "You can not speak on alliance chan.");
				return;
			}

			if (client.Player.IsMuted)
			{
				client.Player.Out.SendMessage("You have been muted and are not allowed to speak in this channel.", eChatType.CT_Staff, eChatLoc.CL_SystemWindow);
				return;
			}

			string message = "[Alliance] " + client.Player.Name + ": \"" + string.Join(" ", args, 1, args.Length - 1) + "\"";
			foreach (Guild gui in client.Player.Guild.alliance.Guilds)
			{
				foreach (GamePlayer ply in gui.GetListOfOnlineMembers())
				{
					if (!gui.HasRank(ply, Guild.eRank.AcHear))
					{
						continue;
					}
					ply.Out.SendMessage(message, eChatType.CT_Alliance, eChatLoc.CL_ChatWindow);
				}
			}
		}
	}
}