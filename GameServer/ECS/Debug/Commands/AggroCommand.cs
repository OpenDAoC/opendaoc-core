using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using ECS.Debug;

namespace DOL.GS.Commands
{
    [Command(
    "&aggro",
    EPrivLevel.GM,
    "Toggle server logging of mob aggro tables.",
    "/aggro debug <on|off> to toggle mob aggro logging on server.")]
    public class AggroCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client == null || client.Player == null)
                return;

            if (IsSpammingCommand(client.Player, "aggro"))
                return;

            if (client.Account.PrivLevel < 2)
                return;

            if (args.Length < 3)
            {
                DisplaySyntax(client);
                return;
            }

            if (args[1].ToLower().Equals("debug"))
            {
                if (args[2].ToLower().Equals("on"))
                {
                    Diagnostics.ToggleAggroDebug(true);
                    DisplayMessage(client, "Mob aggro logging turned on.");
                }
                else if (args[2].ToLower().Equals("off"))
                {
                    Diagnostics.ToggleAggroDebug(false);
                    DisplayMessage(client, "Mob aggro logging turned off.");
                }
            }
        }
    }
}
