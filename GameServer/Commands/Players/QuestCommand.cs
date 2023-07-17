using DOL.GS.PacketHandler;
using DOL.GS.Quests;
using System;

namespace DOL.GS.Commands
{
	[Command(
		"&quest",
		new string[] {"&quests"},
		EPrivLevel.Player,
		"Display a list of your ongoing and completed quests", "/quest")]
	public class QuestCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "quest"))
				return;

			string message = "";
			if (client.Player.QuestList == null || client.Player.QuestList.Count == 0)
				message += "You have no pending quests currently.";
			else
			{
				if (client.Player.QuestList.Count < 10)
				{
					message += "You are currently working on " + client.Player.QuestList.Count + " quest(s), including:";

					foreach (AbstractQuest questC in client.Player.QuestList)
					{
						// Need to protect from too long a list
						// We'll do an easy sloppy chop at 1500 characters (packet limit is 2048)
						if (message.Length < 1500)
							message += String.Format("\n{0}", questC.Name);
						else
							message += "";
					}
				}
				else
					message += "You are currently working on " + client.Player.QuestList.Count + " quests.";
			}
			if (client.Player.QuestListFinished == null || client.Player.QuestListFinished.Count == 0)
				message += "\nYou have not yet completed any quests.";
			else
			{
				if (client.Player.QuestListFinished.Count < 10)
				{
					message += "\nYou have completed the following quest(s):";

					foreach (AbstractQuest questF in client.Player.QuestListFinished)
					{
						// Need to protect from too long a list
						// We'll do an easy sloppy chop at 1500 characters (packet limit is 2048)
						if (message.Length < 1500)
							message += String.Format("\n{0}", questF.Name);
						else
							message += "";
					}
				}
				else
					message += "\nYou have completed " + client.Player.QuestListFinished.Count + " quests.";
			}

			message += "\nUse the '/journal' command to view your full ongoing and completed quests.";
			DisplayMessage(client, message);
		}
	}
}