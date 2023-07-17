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
    "&diag",
    EPrivLevel.GM,
    "Toggle server logging of performance diagnostics.",
    "/diag perf <on|off> to toggle performance diagnostics logging on server.",
    "/diag notify <on|off> <interval> to toggle GameEventMgr Notify profiling, where interval is the period of time in milliseconds during which to accumulate stats.",
    "/diag timer <tickcount> enables debugging of the TimerService for <tickcount> ticks and outputs to the server Console.",
    "/diag think <tickcount> enables debugging of the NPCThinkService for <tickcount> ticks and outputs to the server Console.",
    "/diag currentservicetick - returns the current service the gameloop tick is on; useful for debugging lagging/frozen server.")]
    public class ECSDiagnosticsCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client == null || client.Player == null)
                return;

            if (IsSpammingCommand(client.Player, "Diag"))
                return;

            if (client.Account.PrivLevel < 2)
                return;

            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }

            if (args[1].ToLower().Equals("currentservicetick"))
            {
                DisplayMessage(client, "Gameloop CurrentService Tick: " + GameLoop.CurrentServiceTick);
                return;
            }

            if (args.Length < 3)
            {
                DisplaySyntax(client);
                return;
            }

            if (args[1].ToLower().Equals("perf"))
            {
                if (args[2].ToLower().Equals("on"))
                {
                    Diagnostics.TogglePerfCounters(true);
                    DisplayMessage(client, "Performance diagnostics logging turned on. WARNING: This will spam the server logs.");
                }
                else if (args[2].ToLower().Equals("off"))
                {
                    Diagnostics.TogglePerfCounters(false);
                    DisplayMessage(client, "Performance diagnostics logging turned off.");
                }
            }

            if (args[1].ToLower().Equals("notify"))
            {
                if (args[2].ToLower().Equals("on"))
                {
                    int interval = int.Parse(args[3]);
                    if (interval <= 0)
                    {
                        DisplayMessage(client, "Invalid interval argument. Please specify a value in milliseconds.");
                        return;
                    }

                    Diagnostics.StartGameEventMgrNotifyTimeReporting(interval);
                    DisplayMessage(client, "GameEventMgr Notify() logging turned on. WARNING: This will spam the server logs.");
                }
                else if (args[2].ToLower().Equals("off"))
                {
                    Diagnostics.StopGameEventMgrNotifyTimeReporting();
                    DisplayMessage(client, "GameEventMgr Notify() logging turned off.");
                }
            }

            if (args[1].ToLower().Equals("timer"))
            {
                int tickcount = int.Parse(args[2]);
                if (tickcount <= 0)
                {
                    DisplayMessage(client, "Invalid tickcount argument. Please specify a positive integer value.");
                    return;
                }

                TimerService.DebugTickCount = tickcount;
                DisplayMessage(client, "Debugging next " + tickcount + " TimerService tick(s)");
            }

            if (args[1].ToLower().Equals("think"))
            {
                int tickcount = int.Parse(args[2]);
                if (tickcount <= 0)
                {
                    DisplayMessage(client, "Invalid tickcount argument. Please specify a positive integer value.");
                    return;
                }

                NpcService.DebugTickCount = tickcount;
                DisplayMessage(client, "Debugging next " + tickcount + " NPCThinkService tick(s)");
            }
        }
    }
}
