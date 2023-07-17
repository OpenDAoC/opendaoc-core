
/* <--- SendMessage Standardization --->
*  All messages now use translation IDs to both
*  centralize their location and standardize the method
*  of message calls used throughout this project. All messages affected
*  are in English. Other languages are not yet supported.
* 
*  To  find a message at its source location, either use
*  the message body contained in the comment above the return
*  (e.g., // Message: This is a message.) or the
*  translation ID (e.g., "AdminCommands.Account.Description").
* 
*  To perform message changes, take note of your server settings.
*  If the `serverproperty` table setting `use_dblanguage`
*  is set to `True`, you must make your changes from the
*  `languagesystem` DB table.
* 
*  If the `serverproperty` table setting
*  `update_existing_db_system_sentences_from_files` is set to `True`,
*  perform changes to messages using the .txt files locate in "GameServer >
*  language > EN".
*
*  OPTIONAL: After changing a message, paste the new string
*  into the comment above the affected method(s). This is
*  done for ease of reference. */

using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Database;

namespace DOL.GS.Commands
{
	// See the comments above 'using' about SendMessage translation IDs
	[Command( 
	// Enter '/team' to see command syntax messages
	"&team",
	new [] { "&te" },
	// Message: <----- '/team' Commands (plvl 2) ----->
	"GMCommands.Header.Command.Team",
   EPrivLevel.GM,
	"Broadcasts a message to all Atlas server team members (i.e., plvl 2+).",
	// Syntax: '/team <message>' or '/te <message>'
	"GMCommands.Team.Syntax.Team",
	// Message: Broadcasts a message to all Atlas server team members (i.e., plvl 2+).
	"GMCommands.Team.Usage.Team")]

	public class TeamCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			// Lists all '/team' command syntax
			if (args.Length < 2)
			{
				// Message: <----- '/team' Commands (plvl 2) ----->
				ChatUtil.SendHeaderMessage(client, "GMCommands.Header.Command.Team", null);
				// Message: Use the following syntax for this command:
				ChatUtil.SendCommMessage(client, "AllCommands.Command.SyntaxDesc", null);
				// Syntax: '/team <message>' or '/te <message>'
				ChatUtil.SendSyntaxMessage(client, "GMCommands.Team.Syntax.Team", null);
				// Message: Broadcasts a message to all Atlas server team members (i.e., plvl 2+).
				ChatUtil.SendCommMessage(client, "GMCommands.Team.Usage.Team", null);	
				return;
			}

			// Identify message body
			string message = string.Join(" ", args, 1, args.Length - 1);

			foreach (GameClient player in WorldMgr.GetAllPlayingClients())
			{
				// Don't send team messages to Players
				if (player.Account.PrivLevel > 1)
				{
					// Message: [TEAM] {0}: {1}
					ChatUtil.SendTeamMessage(player, "Social.ReceiveMessage.Staff.Channel", client.Player.Name, message);
				}
			}
		}
	}
}
