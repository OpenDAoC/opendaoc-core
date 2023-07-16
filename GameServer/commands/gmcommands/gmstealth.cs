﻿using System;
using System.Collections.Generic;
using DOL.GS.Commands;
using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    [CmdAttribute(
        "&gmstealth",
        ePrivLevel.GM,
        "Grants the ability to stealth to a gm/admin character",
        "/gmstealth on : turns the command on",
        "/gmstealth off : turns the command off")]
    public class GMStealthCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
        	if (args.Length != 2) {
        		DisplaySyntax(client);
        	}
        	else if (args[1].ToLower().Equals("on")) {

                if (client.Player.IsStealthed != true)
                {
                   client.Player.Stealth(true);
                   client.Player.CurrentSpeed = 191;
                }
        	}
            else if (args[1].ToLower().Equals("off"))
            {
                    client.Player.Stealth(false);
            }
        }
    }
}
