﻿using System;
using System.Reflection;
using Core.Events;
using Core.GS.Enums;
using Core.GS.PacketHandler;
using log4net;

namespace Core.GS.Scripts
{
    public class InstantLevelNpc : GameNpc
    {
        private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
		public override bool AddToWorld()
		{
			Name = "Free Levels";
            Model = 1198;
            Size = 70;
			Flags |= ENpcFlags.PEACE;
			Level = 75;

			return base.AddToWorld();
		}

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            if (log.IsInfoEnabled)
                log.Info("InstantLevelNPC is loading...");
		}
        public void SendReply(GamePlayer player, string msg)
        {
            player.Out.SendMessage(msg, EChatType.CT_System, EChatLoc.CL_PopupWindow);
        }
        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player))
                return false;
           
            player.Out.SendMessage("Hello "+player.Name+", during the alpha test I am able to help your character with experience.\n\n Would you like to be [10], [20], [30], [40] or [50]?\n\n\nI've also been given the power to [reset] your character level to 1, should you ask me to.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);

            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str))
                return false;

            GamePlayer player = source as GamePlayer;

            if (player == null)
                return false;

            switch(str)
            {
                case "10":
                    if (player.Level >= 10) {
                       player.Out.SendMessage("My Calculus 3 spell suggest your level is already higher.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                       return false;
                    }
                    else {
                        player.Out.SendMessage("I have given you enough experience to reach level 10!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Level = 10;
                        return true;
                    }
                case "20":
                    if (player.Level >= 20) {
                       player.Out.SendMessage("My Calculus 3 spell suggest your level is already higher.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                       return false;
                    }
                    else {
                        player.Out.SendMessage("I have given you enough experience to reach level 20!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Level = 20;
                        return true;
                    }
                case "30":
                    if (player.Level >= 30) {
                       player.Out.SendMessage("My Calculus 3 spell suggest your level is already higher.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                       return false;
                    }
                    else {
                        player.Out.SendMessage("I have given you enough experience to reach level 30!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Level = 30;
                        return true;
                    }
                case "40":
                    if (player.Level >= 40) {
                       player.Out.SendMessage("My Calculus 3 spell suggest your level is already higher.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                       return false;
                    }
                    else {
                        player.Out.SendMessage("I have given you enough experience to reach level 40!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Level = 40;
                        return true;
                    }
                case "50":
                    if (player.Level >= 50) {
                       player.Out.SendMessage("My Calculus 3 spell suggest your level is already higher.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                       return false;
                    }
                    else {
                        player.Out.SendMessage("I have given you enough experience to reach level 50!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Level = 50;
                        return true;
                    }
                case "reset":
                    player.Out.SendMessage("I have reset all your experience.\n\n Please relog your character to apply the change.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                    player.Level = 1;
                    return true;

                default: 
                    return false;

                return true;
            }
        }

    }
}