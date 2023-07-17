using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    /// <summary>
    /// Command handler for the /ignore command
    /// </summary>
    [Command(
        "&ignore",
        EPrivLevel.Player,
        "Adds/Removes a player to/from your Ignorelist!",
        "/ignore <playerName>")]
    public class IgnoreCommand : AbstractCommandHandler, ICommandHandler
    {
        /// <summary>
        /// Method to handle the command and any arguments
        /// </summary>
        /// <param name="client"></param>
        /// <param name="args"></param>
        public void OnCommand(GameClient client, string[] args)
        {
            if (args.Length < 2)
            {
                string[] ignores = client.Player.SerializedIgnoreList;
                client.Out.SendCustomTextWindow("Ignore List (snapshot)", ignores);
                return;
            }

            string name = string.Join(" ", args, 1, args.Length - 1);

            int result = 0;
            GameClient fclient = WorldMgr.GuessClientByPlayerNameAndRealm(name, 0, false, out result);

            if (fclient == null)
            {
                name = args[1];
                if (client.Player.IgnoreList.Contains(name))
                {
                    client.Player.ModifyIgnoreList(name, true);
                    return;
                }
                else
                {
                    // nothing found
                    DisplayMessage(client, "No players online with that name.");
                    return;
                }
            }

            switch (result)
            {
                case 2:
                    {
                        // name not unique
                        DisplayMessage(client, "Character name is not unique.");
                        break;
                    }
                case 3: // exact match
                case 4: // guessed name
                    {
                        if (fclient == client)
                        {
                            DisplayMessage(client, "You can't add yourself!");
                            return;
                        }

                        name = fclient.Player.Name;
                        if (client.Player.IgnoreList.Contains(name))
                        {
                           client.Player.ModifyIgnoreList(name, true);
                        }
                        else
                        {
                            client.Player.ModifyIgnoreList(name, false);
                        }
                        break;
                    }
            }
        }
    }
}