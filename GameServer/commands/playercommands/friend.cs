using System;
using System.Linq;
using DOL.GS.Friends;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&friend",
		ePrivLevel.Player,
		"Adds/Removes a player to/from your friendlist!",
		"/friend <playerName>")]
	public class FriendCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (args.Length < 2)
			{
				client.Player.SendFriendsListSnapshot();
				return;
			}
			else if (args.Length == 2 && args[1] == "window")
			{
				client.Player.SendFriendsListSocial();
				return;
			}
			
			string name = string.Join(" ", args, 1, args.Length - 1);

			// attempt to remove from friends list now to avoid being unable to do so because of a guessed name from an online player
			if (RemoveFriend(name, client))
				return;

			int result;
			GameClient fclient = WorldMgr.GuessClientByPlayerNameAndRealm(name, 0, false, out result);

			// abort if the returned player is from a hostile realm
			if (fclient != null)
			{
				name = fclient.Player.Name;

				if (!GameServer.ServerRules.IsSameRealm(fclient.Player, client.Player, true))
					result = 1;
			}

			switch (result)
			{
				case 1: // not found
					{
						DisplayMessage(client, "No players with that name, or you cannot add this player.");
						return;
					}
				case 2: // not unique
					{
						DisplayMessage(client, "Character name is not unique.");
						return;
					}
				case 3: // exact match
					{
						if (IsAddingSelf(fclient, client))
							return;

						AddFriend(name, client);
						return;
					}
				case 4: // guessed
					{
						if (IsAddingSelf(fclient, client))
							return;

						if (IsNameInFriendsList(name, client))
						{
							DisplayMessage(client, "Type the full name to remove " + name + " from your list.");
							return;
						}

						AddFriend(name, client);
						return;
					}
			}
		}

		private bool IsAddingSelf(GameClient fclient, GameClient client)
		{
			if (fclient == client)
			{
				DisplayMessage(client, "You can't add yourself!");
				return true;
			}
			return false;
		}

		private bool IsNameInFriendsList(string name, GameClient client, StringComparer comparer = null)
		{
			return client.Player.GetFriends().Contains(name, comparer);
		}

		private bool RemoveFriend(string name, GameClient client)
		{
			if (IsNameInFriendsList(name, client, StringComparer.OrdinalIgnoreCase) && client.Player.RemoveFriend(name))
			{
				DisplayMessage(client, name + " was removed from your friend list!");
				return true;
			}
			return false;
		}

		private void AddFriend(string name, GameClient client)
		{
			if (client.Player.AddFriend(name))
			{
				DisplayMessage(client, name + " was added to your friend list!");
			}
		}
	}
}
