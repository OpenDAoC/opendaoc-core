﻿using System;
using System.Reflection;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Scripts
{
    public class BgEventTeleporter : GameNPC
	{
		private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		
		public static int TeleportDelay = 40000; //value in milliseconds
		public override bool AddToWorld()
        {
            Model = 2026;
            Name = "Event Teleporter";
            GuildName = "BG Event";
            Level = 50;
            Size = 60;
            Flags |= eFlags.PEACE;
            return base.AddToWorld();
        }
		
		public int GetEnemyCountInArea(GamePlayer t, ushort radius)
		{
			int inArea = 0;
			foreach (GamePlayer NearbyPlayers in t.GetPlayersInRadius(radius))
			{
				if (GameServer.ServerRules.IsAllowedToAttack(this, NearbyPlayers, true));
                {
                    inArea++;
                }
			}
			return inArea;
		}
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;
			TurnTo(player.X, player.Y);
			
			player.Out.SendMessage("Hello " + player.Name + "!\n\n" + "Are you ready to [fight]?", EChatType.CT_Say,EChatLoc.CL_PopupWindow);
			player.Out.SendMessage("If you need so, I can port you back to your Realm's [testing lobby] or to the [battleground]", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
			return true;
		}
		public override bool WhisperReceive(GameLiving source, string str)
		{
			if(!base.WhisperReceive(source,str)) return false;
		  	if(!(source is GamePlayer)) return false;
		    GamePlayer t = (GamePlayer) source;
			TurnTo(t.X,t.Y);
			switch(str)
			{
				case "fight":
					if (t.Level != ServerProperties.ServerProperties.EVENT_LVCAP)
					{
						t.Out.SendMessage("You must be level " + ServerProperties.ServerProperties.EVENT_LVCAP + " to enter the event, speak with my colleague!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
						return false;
					}
					
					//case recently in combat
					if (t.InCombatInLast(TeleportDelay))
					{
	                    t.Out.SendMessage("You need to wait a little longer before porting again.", EChatType.CT_Say,EChatLoc.CL_PopupWindow);
                        return false;
					}
					
					//case solo
					if (t.Group == null)
					{
						log.Info("Solo player");
						int randX = UtilCollection.Random(223000, 235000);
						int randY = UtilCollection.Random(216000, 227000);
						int z = 6000;
						
						t.StartInvulnerabilityTimer(ServerProperties.ServerProperties.TIMER_PVP_TELEPORT*1000,null);
						t.MoveTo(27, randX, randY, z, t.Heading);
						
					}
					//case grouped
					else if (t.Group != null && t.Group.MemberCount > 1)
					{
						log.Info("Player in group");
						foreach (GamePlayer groupMember in t.Group.GetPlayersInTheGroup())
						{
							// checking if any group member is already in the zone and safe
							if (GetDistanceTo(groupMember) > 5000 && !groupMember.InCombat)
							{
								log.Info("Enemies in range: " + GetEnemyCountInArea(groupMember, 2750));
								log.Info("Distance > 5k");
								t.StartInvulnerabilityTimer(ServerProperties.ServerProperties.TIMER_PVP_TELEPORT*1000,null);
								t.MoveTo(groupMember.CurrentRegionID, groupMember.X, groupMember.Y, groupMember.Z, groupMember.Heading);
								return true;
							}
						}
						// fallback if no group member is in the zone or safe
						log.Info("Distance < 5k or too close to enemies");
						int randX = UtilCollection.Random(205000, 253000);
						int randY = UtilCollection.Random(204000, 216000);
						int z = 9000;
						
						t.StartInvulnerabilityTimer(ServerProperties.ServerProperties.TIMER_PVP_TELEPORT*1000,null);
						t.MoveTo(27, randX, randY, z, t.Heading);

					}
					break;

				case "testing lobby":
					switch (t.Realm)
					{
						case ERealm.Albion:
							t.MoveTo(330, 52759, 39528, 4677, 36);
							break;
						case ERealm.Midgard:
							t.MoveTo(334, 52160, 39862, 5472, 46);
							break;
						case ERealm.Hibernia:
							t.MoveTo(335, 52836, 40401, 4672, 441);
							break;
					}
					break;

				case "battleground":
					switch (t.Realm)
					{
						case ERealm.Albion:
							t.MoveTo(252, 38113, 53507, 4160, 3268);
							break;
						case ERealm.Midgard:
							t.MoveTo(252, 53568, 23643, 4530, 3268);
							break;
						case ERealm.Hibernia:
							t.MoveTo(252, 17367, 18248, 4320, 3268);
							break;
					}
					break;

			}
			return true;
		}
		private void SendReply(GamePlayer target, string msg)
			{
				target.Client.Out.SendMessage(
					msg,
					EChatType.CT_Say,EChatLoc.CL_PopupWindow);
			}
		
		[ScriptLoadedEvent]
        public static void OnScriptCompiled(CoreEvent e, object sender, EventArgs args)
        {
            log.Info("Atlas Event Teleporter initialized");
        }	
    }
}