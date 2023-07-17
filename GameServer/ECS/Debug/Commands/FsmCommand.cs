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
    "&fsm",
    ePrivLevel.GM,
    "Toggle server logging of mob FSM states.",
    "/fsm debug <on|off> to toggle performance diagnostics logging on server.")]
    public class FsmCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client == null || client.Player == null)
                return;

            if (IsSpammingCommand(client.Player, "fsm"))
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
                    Diagnostics.ToggleStateMachineDebug(true);
                    DisplayMessage(client, "Mob state logging turned on.");
                }
                else if (args[2].ToLower().Equals("off"))
                {
                    Diagnostics.ToggleStateMachineDebug(false);
                    DisplayMessage(client, "Mob state logging turned off.");
                }
            }
        }
    }
}
