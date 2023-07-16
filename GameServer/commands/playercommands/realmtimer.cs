﻿using DOL.GS.PacketHandler;
using System.Collections;
using System.Collections.Generic;
using DOL.Language;
using DOL.GS.ServerRules;
using System;
using DOL.GS.Utils;

namespace DOL.GS.Commands
{
	[CmdAttribute(
	   "&realmtimer",
	   ePrivLevel.Player,
		 "Displays the players current realmtimer Status", "/realmtimer")]
	public class RealmTimerCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "realmtimer"))
				return;

			string realmname = "None";
			switch ((eRealm)RealmTimer.CurrentRealm(client.Player))
			{
				case eRealm.Albion: 
					realmname = "Albion";
					break;
				case eRealm.Midgard:
				 	realmname = "Midgard";
					break;
				case eRealm.Hibernia:
				 	realmname = "Hibernia";
					break;
				default: 
					realmname = "None";
					break;

			}

			TimeSpan realmtimerminutes = TimeSpan.FromMinutes(RealmTimer.TimeLeftOnTimer(client.Player));
			DisplayMessage(client, "Realm Timer Status. Realm: " + realmname + " Time Left: " + realmtimerminutes.Hours + "h " + realmtimerminutes.Minutes + "m");
			
		}

	}
}
