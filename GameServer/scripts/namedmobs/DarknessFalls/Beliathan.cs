﻿using System;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;

#region Beliathan Inizializator

namespace DOL.GS
{
    public class BeliathanInit : GameNPC
    {
        public BeliathanInit() : base()
        {
        }

        public override bool AddToWorld()
        {
            BeliathanInitBrain hi = new BeliathanInitBrain();
            SetOwnBrain(hi);
            base.AddToWorld();
            return true;
        }

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            GameNPC[] npcs;
            npcs = WorldMgr.GetNPCsByNameFromRegion("Beliathan Initializator", 249, (ERealm) 0);
            if (npcs.Length == 0)
            {
                log.Warn("Beliathan Initializator not found, creating it...");

                log.Warn("Initializing Beliathan Initializator...");
                BeliathanInit CO = new BeliathanInit();
                CO.Name = "Beliathan Initializator";
                CO.GuildName = "DO NOT REMOVE!";
                CO.RespawnInterval = 5000;
                CO.Model = 665;
                CO.Realm = 0;
                CO.Level = 50;
                CO.Size = 50;
                CO.CurrentRegionID = 249;
                CO.Flags ^= eFlags.CANTTARGET;
                CO.Flags ^= eFlags.FLYING;
                CO.Flags ^= eFlags.DONTSHOWNAME;
                CO.Flags ^= eFlags.PEACE;
                CO.Faction = FactionMgr.GetFactionByID(191);
                CO.Faction.AddFriendFaction(FactionMgr.GetFactionByID(191));
                CO.X = 22699;
                CO.Y = 18684;
                CO.Z = 15174;
                BeliathanInitBrain ubrain = new BeliathanInitBrain();
                CO.SetOwnBrain(ubrain);
                CO.AddToWorld();
                CO.SaveIntoDatabase();
                CO.Brain.Start();
            }
            else
                log.Warn(
                    "Beliathan Initializator exists in game, remove it and restart server if you want to add by script code.");
        }
    }
}

#region Initializator Brain

namespace DOL.AI.Brain
{
    public class BeliathanInitBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BeliathanInitBrain()
            : base()
        {
        }

        public override int ThinkInterval => 600000; // 10 min

        public override void Think()
        {
            var princeStatus = WorldMgr.GetNPCsFromRegion(Body.CurrentRegionID);
            var princeCount = 0;
            var beliathan = WorldMgr.GetNPCsByNameFromRegion("Beliathan", 249, (ERealm) 0);
            bool beliSpawned;

            if (beliathan.Length == 0)
            {
                beliSpawned = false;
            }
            else
            {
                beliSpawned = true;
            }

            if (!beliSpawned)
            {
                foreach (GameNPC npc in princeStatus)
                {
                    if (!npc.Name.ToLower().Contains("prince")) continue;
                    princeCount++;
                }

                if (princeCount == 0)
                {
                    SpawnBeliathan();
                }
            }

            base.Think();
        }

        public void SpawnBeliathan()
        {
            BroadcastMessage("The tunnels rumble and shake..");
            Beliathan Add = new Beliathan();
            Add.X = Body.X;
            Add.Y = Body.Y;
            Add.Z = Body.Z;
            Add.CurrentRegion = Body.CurrentRegion;
            Add.Heading = 4072;
            Add.AddToWorld();
        }

        public void BroadcastMessage(String message)
        {
            foreach (GameClient client in WorldMgr.GetClientsOfRegion(Body.CurrentRegionID))
            {
                client.Player.Out.SendMessage(message, eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
            }
        }
    }
}

#endregion

#endregion

namespace DOL.GS
{
    public class Beliathan : GameEpicBoss
    {
        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            GameEventMgr.AddHandler(GameLivingEvent.Dying, new CoreEventHandler(PlayerKilledByBeliathan));
            if (log.IsInfoEnabled)
                log.Info("Beliathan initialized..");
        }

        [ScriptUnloadedEvent]
        public static void ScriptUnloaded(CoreEvent e, object sender, EventArgs args)
        {
        }
        public override double AttackDamage(InventoryItem weapon)
        {
            return base.AttackDamage(weapon) * Strength / 100 * ServerProperties.Properties.EPICS_DMG_MULTIPLIER;
        }
        public override int AttackSpeed(InventoryItem mainWeapon, InventoryItem leftWeapon = null)
        {
            return base.AttackSpeed(mainWeapon, leftWeapon) * 2;
        }
        public override int GetResist(EDamageType damageType)
        {
            switch (damageType)
            {
                case EDamageType.Slash: return 40; // dmg reduction for melee dmg
                case EDamageType.Crush: return 40; // dmg reduction for melee dmg
                case EDamageType.Thrust: return 40; // dmg reduction for melee dmg
                default: return 70; // dmg reduction for rest resists
            }
        }
        public override double GetArmorAF(EArmorSlot slot)
        {
            return 350;
        }
        public override double GetArmorAbsorb(EArmorSlot slot)
        {
            // 85% ABS is cap.
            return 0.20;
        }
        public override int MaxHealth
        {
            get { return 100000; }
        }
        public override short MaxSpeedBase => (short) (191 + Level * 2);
        public override int AttackRange
        {
            get => 180;
            set { }
        }
        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60158351);
            LoadTemplate(npcTemplate);
            MaxDistance = 1500;
            TetherRange = 2000;
            RoamingRange = 400;
            Strength = npcTemplate.Strength;
            Constitution = npcTemplate.Constitution;
            Dexterity = npcTemplate.Dexterity;
            Quickness = npcTemplate.Quickness;
            Empathy = npcTemplate.Empathy;
            Piety = npcTemplate.Piety;
            Intelligence = npcTemplate.Intelligence;
            RespawnInterval = -1;
            BeliathanBrain sBrain = new BeliathanBrain();
            SetOwnBrain(sBrain);
            sBrain.AggroLevel = 100;
            sBrain.AggroRange = 500;

            // demon
            BodyType = 2;

            Faction = FactionMgr.GetFactionByID(191);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(191));

            base.AddToWorld();
            return true;
        }

        public override void Die(GameObject killer)
        {
            base.Die(killer);

            foreach (GameNPC npc in GetNPCsInRadius(4000))
            {
                if (npc.Brain is BeliathanMinionBrain)
                {
                    npc.RemoveFromWorld();
                }
            }
        }
        private static void PlayerKilledByBeliathan(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = sender as GamePlayer;

            if (player == null)
                return;

            DyingEventArgs eArgs = args as DyingEventArgs;

            if (eArgs?.Killer?.Name != "Beliathan")
                return;

            GameNPC beliathan = eArgs.Killer as GameNPC;

            if (beliathan == null)
                return;

            BeliathanMinion sMinion = new BeliathanMinion();
            sMinion.X = player.X;
            sMinion.Y = player.Y;
            sMinion.Z = player.Z;
            sMinion.CurrentRegion = player.CurrentRegion;
            sMinion.Heading = player.Heading;
            sMinion.AddToWorld();
            sMinion.StartAttack(beliathan.TargetObject);
        }
    }
}

namespace DOL.AI.Brain
{
    public class BeliathanBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool RemoveAdds = false;
        public override void Think()
        {
            if (!CheckProximityAggro())
            {
                //set state to RETURN TO SPAWN
                FSM.SetCurrentState(EFsmStateType.RETURN_TO_SPAWN);
                if (!RemoveAdds)
                {
                    foreach (GameNPC npc in Body.GetNPCsInRadius(4000))
                    {
                        if (npc.Brain is BeliathanMinionBrain)
                        {
                            npc.RemoveFromWorld();
                        }
                    }
                    RemoveAdds = true;
                }
            }
            if (HasAggro && Body.TargetObject != null)
                RemoveAdds = false;
            base.Think();
        }
    }
}

namespace DOL.GS
{
    public class BeliathanMinion : GameNPC
    {
        public override int MaxHealth
        {
            get { return 550; }
        }

        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60158351);
            LoadTemplate(npcTemplate);
            Level = 50;
            Strength = 300;
            Size = 50;
            Name += "'s Minion";
            RoamingRange = 350;
            RespawnInterval = -1;
            MaxDistance = 1500;
            TetherRange = 2000;
            IsWorthReward = false; // worth no reward
            Realm = ERealm.None;
            BeliathanMinionBrain adds = new BeliathanMinionBrain();
            LoadedFromScript = true;
            SetOwnBrain(adds);

            // demon
            BodyType = 2;

            Faction = FactionMgr.GetFactionByID(191);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(191));

            base.AddToWorld();
            return true;
        }
        public override void DropLoot(GameObject killer) //no loot
        {
        }
        public override long ExperienceValue => 0;
        public override void Die(GameObject killer)
        {
            base.Die(null); // null to not gain experience
        }
    }
}

namespace DOL.AI.Brain
{
    public class BeliathanMinionBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BeliathanMinionBrain()
        {
            AggroLevel = 100;
            AggroRange = 450;
        }
    }
}