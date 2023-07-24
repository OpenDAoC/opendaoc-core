/*
 *  Script by clait
 *  
 *  This NPC will level the player to 10, 20, 30, 40 or 50
 * 
 */

using System;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Scripts
{
    public class BoostNpc : GameNpc
    {
        private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static int EventLVCap = ServerProperties.ServerProperties.EVENT_LVCAP;
        public static int realmPoints = ServerProperties.ServerProperties.EVENT_START_RP;

        public override bool AddToWorld()
		{
			Name = "Booster";
            GuildName = "Server";
            Model = 1198;
            Level = 50;
            Model = 2026;
            Size = 60;
            Flags |= eFlags.PEACE;
            return base.AddToWorld();
		}

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            if (log.IsInfoEnabled)
                log.Info("Boost NPC is loading...");
        }
        public override bool Interact(GamePlayer player)
        {

            if (!base.Interact(player))
                return false;
            
            if(player.HCFlag || player.NoHelp){
                player.Out.SendMessage($"I'm sorry {player.Name}, you have chosen a different path and are not allowed to use my services.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                return false;
            }
            
            if (EventLVCap != 0)
            { player.Out.SendMessage($"Hello {player.Name},\n\n I have been told to give you enough [experience] to reach level {EventLVCap}.",
                EChatType.CT_Say, EChatLoc.CL_PopupWindow);
            }
            
            if (realmPoints != 0)
            {
                player.Out.SendMessage("\nAdditionally, you might be interested in a small [realm level] boost.",
                        EChatType.CT_Say, EChatLoc.CL_PopupWindow);
            }
            
            if (EventLVCap == 0 && realmPoints == 0)
            {
                player.Out.SendMessage("I'm sorry, I can't help you at this time.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
            }
            
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str))
                return false;
            int targetLevel = EventLVCap;

            GamePlayer player = source as GamePlayer;

            if (player == null)
                return false;

            if (player.HCFlag || player.NoHelp)
            {
                return false;
            }

            switch(str)
            {
                case "experience":
                    if (player.Level > 1)
                    {
                        player.Out.SendMessage("You need to be Level 1 to receive my training.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        return false;
                    }
                        
                    
                    if (player.Level < EventLVCap)
                    {
                        string customKey = "BoostedLevel-" + EventLVCap;
                        var boosterKey = CoreDb<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId").IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo(customKey)));
                        
                        player.Out.SendMessage("I have given you enough experience to fight, now speak with the quartermaster and go make your Realm proud!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Boosted = true;
                        player.CanGenerateNews = false;
                        player.Level = (byte)targetLevel;
                        player.Health = player.MaxHealth;
                        
                        
                        if (boosterKey == null)
                        {
                            DOLCharactersXCustomParam boostedLevel = new DOLCharactersXCustomParam();
                            boostedLevel.DOLCharactersObjectId = player.ObjectId;
                            boostedLevel.KeyName = customKey;
                            boostedLevel.Value = "1";
                            GameServer.Database.AddObject(boostedLevel);
                        }

                        return true;
                    }
                    player.Out.SendMessage("You are a veteran already, go fight for your Realm!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                    return false;

                case "realm level":
                    if (player.RealmPoints < realmPoints)
                    {
                        var rate = ServerProperties.ServerProperties.RP_RATE;
                        var realmPointsToGive = Math.Floor((realmPoints - player.RealmPoints)/rate);
                        player.CanGenerateNews = false;
                        player.GainRealmPoints((long)realmPointsToGive);
                        player.Out.SendMessage($"I have given you {realmPointsToGive} RPs, now go get some more yourself!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        return true;
                    }

                    player.Out.SendMessage("You have killed enough enemies already, go kill more!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                    return false;
                
                default:
                    return false;
            }
        }

    }
}