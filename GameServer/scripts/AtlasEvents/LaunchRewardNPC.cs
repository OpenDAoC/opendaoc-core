using System;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Scripts
{
    public class LaunchRewardNPC : GameNPC
    {
        private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int ORBS_REWARD = 15000;
        private const string LAUNCH_QUEST_ACHIEVEMENT = "LaunchQuest";
        private const string CUSTOM_KEY = "LaunchQuestOrbs-";
        public override bool AddToWorld()
		{
			Name = "Kay the Nut";
            GuildName = "Royal Rewarder";
            Level = 50;
            Size = 50;
            Flags |= eFlags.PEACE;

            Model = Realm switch
            {
                ERealm.Albion => 8,
                ERealm.Midgard => 213,
                ERealm.Hibernia => 361,
                _ => Model
            };

            return base.AddToWorld();
		}

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            if (log.IsInfoEnabled)
                log.Info("Launch reward NPC loaded");
        }
        public override bool Interact(GamePlayer player)
        {

            if (!base.Interact(player))
                return false;
            
            var LaunchQuestParamKey = $"{CUSTOM_KEY}{Realm}";
            
            var hasRealmCredit = AchievementUtil.CheckPlayerCredit(LAUNCH_QUEST_ACHIEVEMENT, player, (int)Realm);
            
            var hasAccountCredit = AchievementUtil.CheckAccountCredit(LAUNCH_QUEST_ACHIEVEMENT, player);

            if (hasRealmCredit)
            {
                if (!HasCustomParam(player))
                {
                    if (player.Level < 50)
                    {
                        player.Out.SendMessage($"For your efforts, the realm of {RealmName(Realm)} has decided to award you with {ORBS_REWARD} Atlas Orbs, congratulations!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        player.Out.SendMessage("Unfortunately though, the Orbs are too powerful for you at this time. Come back when you reach level 50.\n\n" +
                                               "My dear friend Cruella de Ville has something you can have immediately, go find here! She is around here.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                        return false;
                    }

                    player.Out.SendMessage($"For your efforts, the realm of {RealmName(Realm)} has decided to award you with {ORBS_REWARD} Atlas Orbs, congratulations!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                    player.Out.SendMessage($"Would you like to [receive them] now? \n\n I suggest you storing them in your Account Vault to avoid losing them, I won't be able to give you more.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);

                    return true;
                }
                
                player.Out.SendMessage($"The whole realm of {RealmName(Realm)} thanks you for your help and wish you luck with your adventures! \n \n" +
                                       "I don't have additional rewards for you but don't forget to visit my friend Cruella de Ville.. she can be found around here.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                return true;

            }
            
            if (hasAccountCredit)
            {
                player.Out.SendMessage($"As you haven't fought for {RealmName(Realm)}, I don't have any reward for you. \n\n" +
                                       "My friend Cruella de Ville on the other hand is not as loyal as me and might have something for you. \n" +
                                       "She can be found around here.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                return true;
            }
            
            player.Out.SendMessage($"You are not eligible for any reward on {RealmName(Realm)} at the moment.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);

            
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str))
                return false;

            var player = source as GamePlayer;

            var hasRealmCredit = AchievementUtil.CheckPlayerCredit(LAUNCH_QUEST_ACHIEVEMENT, player, (int)player.Realm);
            
            switch(str)
            {
                case "receive them":
                    if (!hasRealmCredit) return false;
                    if (HasCustomParam(player)) return false;
                    if (player.Level < 50) return false;
                    
                    player.Out.SendMessage($"Here, take {ORBS_REWARD} Atlas Orbs! \n\n" +
                                           $"The whole realm of {RealmName(Realm)} thanks you for your help and wish you luck with your adventures! \n \n" +
                                           "Now, don't forget to visit my friend Cruella de Ville, I'm sure she will have more rewards for you.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
                    
                    RogMgr.GenerateReward(player, ORBS_REWARD);
                    SetCustomParam(player);
                    return true;
                default:
                    return false;
            }
        }
        
        private string RealmName(ERealm realm)
        {
            switch (realm)
            {
                case ERealm.Albion:
                    return "Albion";
                case ERealm.Midgard:
                    return "Midgard";
                case ERealm.Hibernia:
                    return "Hibernia";
            }
            return "";
        }

        private void SetCustomParam(GamePlayer player)
        {

            var LaunchQuestParamKey = $"{CUSTOM_KEY}{Realm}";
            var HasLaunchQuestParam = CoreDb<AccountXCustomParam>.SelectObject(DB.Column("Name").IsEqualTo(player.Client.Account.Name).And(DB.Column("KeyName").IsEqualTo(LaunchQuestParamKey)));

            if (HasLaunchQuestParam != null) return;
            
            var LaunchQuestParam = new AccountXCustomParam();
            LaunchQuestParam.Name = player.Client.Account.Name;
            LaunchQuestParam.KeyName = LaunchQuestParamKey;
            LaunchQuestParam.Value = "1";
            GameServer.Database.AddObject(LaunchQuestParam);
        }

        private bool HasCustomParam(GamePlayer player)
        {
            var LaunchQuestParamKey = $"{CUSTOM_KEY}{Realm}";
            
            return CoreDb<AccountXCustomParam>.SelectObject(DB.Column("Name").IsEqualTo(player.Client.Account.Name).And(DB.Column("KeyName").IsEqualTo(LaunchQuestParamKey))) != null;

        }

    }
}