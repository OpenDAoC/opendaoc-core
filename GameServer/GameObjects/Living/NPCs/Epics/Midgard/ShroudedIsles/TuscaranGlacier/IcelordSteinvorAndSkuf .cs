﻿using System;
using System.Collections;
using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS
{
    public class Steinvor : GameEpicBoss
    {
        public Steinvor() : base()
        {
        }
        public override int GetResist(EDamageType damageType)
        {
            switch (damageType)
            {
                case EDamageType.Slash: return 40;// dmg reduction for melee dmg
                case EDamageType.Crush: return 40;// dmg reduction for melee dmg
                case EDamageType.Thrust: return 40;// dmg reduction for melee dmg
                default: return 70;// dmg reduction for rest resists
            }
        }
        public override double AttackDamage(DbInventoryItem weapon)
        {
            return base.AttackDamage(weapon) * Strength / 100 * ServerProperties.Properties.EPICS_DMG_MULTIPLIER;
        }

        public override int AttackRange
        {
            get { return 350; }
            set { }
        }

        public override bool HasAbility(string keyName)
        {
            if (IsAlive && keyName == GS.Abilities.CCImmunity)
                return true;

            return base.HasAbility(keyName);
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
        public override void Die(GameObject killer) //on kill generate orbs
        {
            SpawnSeers();
            base.Die(killer);
        }
        public void SpawnSeers()
        {
            for (int i = 0; i < 2; i++)
            {
                HrimthursaSeer Add1 = new HrimthursaSeer();
                Add1.X = 29996 + Util.Random(-100, 100);
                Add1.Y = 52911 + Util.Random(-100, 100);
                Add1.Z = 11890;
                Add1.CurrentRegion = this.CurrentRegion;
                Add1.Heading = 2032;
                Add1.PackageID = "SteinvorDeathAdds";
                Add1.RespawnInterval = -1;
                Add1.AddToWorld();
            }
        }
        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60162350);
            LoadTemplate(npcTemplate);
            Strength = npcTemplate.Strength;
            Dexterity = npcTemplate.Dexterity;
            Constitution = npcTemplate.Constitution;
            Quickness = npcTemplate.Quickness;
            Piety = npcTemplate.Piety;
            Intelligence = npcTemplate.Intelligence;
            Empathy = npcTemplate.Empathy;
            Faction = FactionMgr.GetFactionByID(140);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
            RespawnInterval = ServerProperties.Properties.SET_SI_EPIC_ENCOUNTER_RESPAWNINTERVAL * 60000; //1min is 60000 miliseconds
            BodyType = (ushort)EBodyType.Giant;
            SteinvorBrain.PlayerX = 0;
            SteinvorBrain.PlayerY = 0;
            SteinvorBrain.PlayerZ = 0;
            SteinvorBrain.RandomTarget = null;
            SteinvorBrain.PickedTarget = false;

            SteinvorBrain sbrain = new SteinvorBrain();
            SetOwnBrain(sbrain);
            LoadedFromScript = false; //load from database
            SaveIntoDatabase();
            base.AddToWorld();
            return true;
        }

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            GameNpc[] npcs;
            npcs = WorldMgr.GetNPCsByNameFromRegion("Icelord Steinvor", 160, (ERealm)0);
            if (npcs.Length == 0)
            {
                log.Warn("Icelord Steinvor not found, creating it...");

                log.Warn("Initializing Icelord Steinvor ...");
                Steinvor TG = new Steinvor();
                TG.Name = "Icelord Steinvor";
                TG.Model = 918;
                TG.Realm = 0;
                TG.Level = 80;
                TG.Size = 70;
                TG.CurrentRegionID = 160; //tuscaran glacier
                TG.MeleeDamageType = EDamageType.Crush;
                TG.RespawnInterval =
                    ServerProperties.Properties.SET_SI_EPIC_ENCOUNTER_RESPAWNINTERVAL *
                    60000; //1min is 60000 miliseconds
                TG.Faction = FactionMgr.GetFactionByID(140);
                TG.Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
                TG.BodyType = (ushort)EBodyType.Giant;

                TG.X = 25405;
                TG.Y = 57241;
                TG.Z = 11359;
                TG.Heading = 1939;
                SteinvorBrain ubrain = new SteinvorBrain();
                TG.SetOwnBrain(ubrain);
                TG.AddToWorld();
                TG.SaveIntoDatabase();
                TG.Brain.Start();
            }
            else
                log.Warn("Icelord Steinvor exist ingame, remove it and restart server if you want to add by script code.");
        }
    }
}

namespace DOL.AI.Brain
{
    public class SteinvorBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SteinvorBrain()
            : base()
        {
            AggroLevel = 100;
            AggroRange = 600;
            ThinkInterval = 1500;
        }

        public static bool IsPulled = false;
        public override void OnAttackedByEnemy(AttackData ad)
        {
            if (IsPulled == false)
            {
                foreach (GameNpc npc in WorldMgr.GetNPCsFromRegion(Body.CurrentRegionID))
                {
                    if (npc != null)
                    {
                        if (npc.IsAlive && npc.Brain is SkufBrain)
                        {
                            AddAggroListTo(npc.Brain as SkufBrain);
                            IsPulled = true;
                        }
                    }
                }
            }
            base.OnAttackedByEnemy(ad);
        }
        private bool RemoveAdds = false;
        public override void Think()
        {
            if (!CheckProximityAggro())
            {
                //set state to RETURN TO SPAWN
                FiniteStateMachine.SetCurrentState(EFSMStateType.RETURN_TO_SPAWN);
                Body.Health = Body.MaxHealth;
                IsPulled = false;
                PickedTarget = false;
            }

            if (Body.IsOutOfTetherRange)
            {
                Body.Health = Body.MaxHealth;
                ClearAggroList();
            }
            else if (Body.InCombatInLast(30 * 1000) == false && this.Body.InCombatInLast(35 * 1000))
            {
                Body.Health = Body.MaxHealth;
                if (!RemoveAdds)
                {
                    foreach (GameNpc mob in Body.GetNPCsInRadius(5000))
                    {
                        if (mob != null)
                        {
                            if (mob.IsAlive)
                            {
                                if (mob.Brain is EffectMobBrain)
                                    mob.RemoveFromWorld();
                            }
                        }
                    }
                    RemoveAdds = true;
                }
            }

            if (Body.TargetObject != null && HasAggro)
            {
                RemoveAdds = false;
                if (PickedTarget == false)
                {
                    new EcsGameTimer(Body, new EcsGameTimer.EcsTimerCallback(PickPlayer), 1000);
                    PickedTarget = true;
                }
            }
            base.Think();
        }

        public static GamePlayer randomtarget = null;
        public static GamePlayer RandomTarget
        {
            get { return randomtarget; }
            set { randomtarget = value; }
        }
        public static bool PickedTarget = false;
        public static int PlayerX = 0;
        public static int PlayerY = 0;
        public static int PlayerZ = 0;

        public int PickPlayer(EcsGameTimer timer)
        {
            if (Body.IsAlive)
            {
                IList enemies = new ArrayList(AggroTable.Keys);
                foreach (GamePlayer player in Body.GetPlayersInRadius(1100))
                {
                    if (player != null)
                    {
                        if (player.IsAlive && player.Client.Account.PrivLevel == 1)
                        {
                            if (!AggroTable.ContainsKey(player))
                                AggroTable.Add(player, 1);
                        }
                    }
                }
                if (enemies.Count == 0)
                {}
                else
                {
                    List<GameLiving> damage_enemies = new List<GameLiving>();
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (enemies[i] == null)
                            continue;
                        if (!(enemies[i] is GameLiving))
                            continue;
                        if (!(enemies[i] as GameLiving).IsAlive)
                            continue;
                        GameLiving living = null;
                        living = enemies[i] as GameLiving;
                        if (living.IsVisibleTo(Body) && Body.TargetInView && living is GamePlayer)
                        {
                            damage_enemies.Add(enemies[i] as GameLiving);
                        }
                    }
                    if (damage_enemies.Count > 0)
                    {
                        GamePlayer PortTarget = (GamePlayer)damage_enemies[Util.Random(0, damage_enemies.Count - 1)];
                        RandomTarget = PortTarget;
                        PlayerX = RandomTarget.X;
                        PlayerY = RandomTarget.Y;
                        PlayerZ = RandomTarget.Z;
                        SpawnEffectMob();
                        BroadcastMessage(String.Format(Body.Name + " says, '" + RandomTarget.Name +" you are not going anywhere'"));
                        new EcsGameTimer(Body, new EcsGameTimer.EcsTimerCallback(EffectTimer), 8000);
                    }
                }
            }
            return 0;
        }

        public void BroadcastMessage(String message)
        {
            foreach (GamePlayer player in Body.GetPlayersInRadius(WorldMgr.OBJ_UPDATE_DISTANCE))
            {
                player.Out.SendMessage(message, EChatType.CT_Broadcast, EChatLoc.CL_SystemWindow);
            }
        }
        public int EffectTimer(EcsGameTimer timer) //pick and remove effect mob
        {
            if (Body.IsAlive)
            {
                foreach (GameNpc npc in Body.GetNPCsInRadius(5000))
                {
                    if (npc != null)
                    {
                        if (npc.IsAlive && npc.Brain is EffectMobBrain)
                            npc.RemoveFromWorld();
                    }
                }
                new EcsGameTimer(Body, new EcsGameTimer.EcsTimerCallback(RestartEffectTimer), Util.Random(10000, 15000));
            }
            return 0;
        }

        public int RestartEffectTimer(EcsGameTimer timer) //reset timer so boss can repeat it again
        {
            if (Body.IsAlive)
            {
                PlayerX = 0;
                PlayerY = 0;
                PlayerZ = 0;
                RandomTarget = null;
                PickedTarget = false;
            }
            return 0;
        }
        public void SpawnEffectMob() //spawn mob to show effect on ground
        {
            EffectMob npc = new EffectMob();
            npc.X = PlayerX;
            npc.Y = PlayerY;
            npc.Z = PlayerZ;
            npc.RespawnInterval = -1;
            npc.Heading = Body.Heading;
            npc.CurrentRegion = Body.CurrentRegion;
            npc.AddToWorld();
        }
    }
}

////////////////////////////////////////////////////////////////////Icelord Skuf////////////////////////////////////////////
namespace DOL.GS
{
    public class Skuf : GameEpicBoss
    {
        public Skuf() : base()
        {
        }
        public override int GetResist(EDamageType damageType)
        {
            switch (damageType)
            {
                case EDamageType.Slash: return 40;// dmg reduction for melee dmg
                case EDamageType.Crush: return 40;// dmg reduction for melee dmg
                case EDamageType.Thrust: return 40;// dmg reduction for melee dmg
                default: return 70;// dmg reduction for rest resists
            }
        }

        public override double AttackDamage(DbInventoryItem weapon)
        {
            return base.AttackDamage(weapon) * Strength / 100 * ServerProperties.Properties.EPICS_DMG_MULTIPLIER;
        }

        public override int AttackRange
        {
            get { return 350; }
            set { }
        }

        public override bool HasAbility(string keyName)
        {
            if (IsAlive && keyName == GS.Abilities.CCImmunity)
                return true;

            return base.HasAbility(keyName);
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
        public override void Die(GameObject killer) //on kill generate orbs
        {
            base.Die(killer);
        }
        public static bool Spawn_Snakes = false;
        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60162349);
            LoadTemplate(npcTemplate);
            Strength = npcTemplate.Strength;
            Dexterity = npcTemplate.Dexterity;
            Constitution = npcTemplate.Constitution;
            Quickness = npcTemplate.Quickness;
            Piety = npcTemplate.Piety;
            Intelligence = npcTemplate.Intelligence;
            Empathy = npcTemplate.Empathy;
            Faction = FactionMgr.GetFactionByID(140);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
            RespawnInterval =ServerProperties.Properties.SET_SI_EPIC_ENCOUNTER_RESPAWNINTERVAL * 60000; //1min is 60000 miliseconds
            BodyType = (ushort)EBodyType.Giant;

            SkufBrain sbrain = new SkufBrain();
            SetOwnBrain(sbrain);
            LoadedFromScript = false; //load from database
            SaveIntoDatabase();
            base.AddToWorld();
            return true;
        }

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            GameNpc[] npcs;
            npcs = WorldMgr.GetNPCsByNameFromRegion("Icelord Skuf", 160, (ERealm)0);
            if (npcs.Length == 0)
            {
                log.Warn("Icelord Skuf not found, creating it...");

                log.Warn("Initializing Icelord Skuf ...");
                Skuf TG = new Skuf();
                TG.Name = "Icelord Skuf";
                TG.Model = 918;
                TG.Realm = 0;
                TG.Level = 80;
                TG.Size = 70;
                TG.CurrentRegionID = 160; //tuscaran glacier
                TG.MeleeDamageType = EDamageType.Crush;
                TG.RespawnInterval =
                    ServerProperties.Properties.SET_SI_EPIC_ENCOUNTER_RESPAWNINTERVAL *
                    60000; //1min is 60000 miliseconds
                TG.Faction = FactionMgr.GetFactionByID(140);
                TG.Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
                TG.BodyType = (ushort)EBodyType.Giant;

                TG.X = 25405;
                TG.Y = 57241;
                TG.Z = 11359;
                TG.Heading = 1939;
                SkufBrain ubrain = new SkufBrain();
                TG.SetOwnBrain(ubrain);
                TG.AddToWorld();
                TG.SaveIntoDatabase();
                TG.Brain.Start();
            }
            else
                log.Warn("Icelord Skuf exist ingame, remove it and restart server if you want to add by script code.");
        }
    }
}

namespace DOL.AI.Brain
{
    public class SkufBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SkufBrain()
            : base()
        {
            AggroLevel = 100;
            AggroRange = 600;
            ThinkInterval = 1500;
        }
        public static bool IsPulled2 = false;
        public override void OnAttackedByEnemy(AttackData ad)
        {
            if (IsPulled2 == false)
            {
                foreach (GameNpc npc in WorldMgr.GetNPCsFromRegion(Body.CurrentRegionID))
                {
                    if (npc != null)
                    {
                        if (npc.IsAlive && npc.Brain is SteinvorBrain)
                        {
                            AddAggroListTo(npc.Brain as SteinvorBrain);
                            IsPulled2 = true;
                        }
                    }
                }
            }
            base.OnAttackedByEnemy(ad);
        }

        public override void Think()
        {
            if (!CheckProximityAggro())
            {
                //set state to RETURN TO SPAWN
                FiniteStateMachine.SetCurrentState(EFSMStateType.RETURN_TO_SPAWN);
                Body.Health = Body.MaxHealth;
                IsPulled2 = false;
            }

            if (Body.IsOutOfTetherRange)
            {
                Body.Health = Body.MaxHealth;
                ClearAggroList();
            }
            else if (Body.InCombatInLast(30 * 1000) == false && this.Body.InCombatInLast(35 * 1000))
            {
                Body.Health = Body.MaxHealth;
            }
            base.Think();
        }
    }
}

///////////////////////////////////////////////////////Spawn Adds////////////////////////////////////////////
namespace DOL.GS
{
    public class HrimthursaSeer : GameEpicNPC
    {
        public HrimthursaSeer() : base()
        {
        }
        public override int GetResist(EDamageType damageType)
        {
            switch (damageType)
            {
                case EDamageType.Slash: return 35; // dmg reduction for melee dmg
                case EDamageType.Crush: return 35; // dmg reduction for melee dmg
                case EDamageType.Thrust: return 35; // dmg reduction for melee dmg
                default: return 35; // dmg reduction for rest resists
            }
        }

        public override void ReturnToSpawnPoint(short speed)
        {
            return;
        }

        public override double AttackDamage(DbInventoryItem weapon)
        {
            return base.AttackDamage(weapon) * Strength / 100 * ServerProperties.Properties.EPICS_DMG_MULTIPLIER;
        }
        public override int AttackRange
        {
            get { return 350; }
            set { }
        }
        public override double GetArmorAF(EArmorSlot slot)
        {
            return 200;
        }
        public override double GetArmorAbsorb(EArmorSlot slot)
        {
            // 85% ABS is cap.
            return 0.15;
        }
        public override int MaxHealth
        {
            get { return 20000; }
        }
        public override void Die(GameObject killer)
        {
            base.Die(killer);
        }
        public override short Quickness { get => base.Quickness; set => base.Quickness = 80; }
        public override short Strength { get => base.Strength; set => base.Strength = 150; }
        public override bool AddToWorld()
        {
            Model = 918;
            MeleeDamageType = EDamageType.Crush;
            Name = "hrimthursa seer";
            MaxDistance = 3500;
            TetherRange = 3800;
            Size = 60;
            Level = (byte)Util.Random(73, 75);
            MaxSpeedBase = 270;

            Faction = FactionMgr.GetFactionByID(140);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
            BodyType = (ushort)EBodyType.Giant;
            Realm = ERealm.None;
            RespawnInterval = -1;

            HrimthursaSeerBrain.walkto_point = false;
            HrimthursaSeerBrain adds = new HrimthursaSeerBrain();
            SetOwnBrain(adds);
            LoadedFromScript = false;
            base.AddToWorld();
            return true;
        }
    }
}

namespace DOL.AI.Brain
{
    public class HrimthursaSeerBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HrimthursaSeerBrain()
            : base()
        {
            AggroLevel = 100;
            AggroRange = 1500;
            ThinkInterval = 1000;
        }

        public static bool walkto_point = false;
        public void Walk_To_Room()
        {
            Point3D point1 = new Point3D();
            point1.X = 29986;
            point1.Y = 50345;
            point1.Z = 11377;

            if (!Body.InCombat && !HasAggro)
            {
                if (Body.CurrentRegionID == 160) //TG
                {
                    if (!Body.IsWithinRadius(point1, 30) && walkto_point == false)
                        Body.WalkTo(point1, 100);
                    else
                        walkto_point = true;
                }
            }
        }
        public override void Think()
        {
            Walk_To_Room();
            base.Think();
        }
    }
}

/////////////////////////////////////////////////////////effect mob/////////////////////
namespace DOL.GS
{
    public class EffectMob : GameEpicNPC
    {
        public EffectMob() : base()
        {
        }
        public override void StartAttack(GameObject target)
        {
        }
        public int Show_Effect(EcsGameTimer timer)
        {
            if (IsAlive)
            {
                foreach (GamePlayer player in this.GetPlayersInRadius(8000))
                {
                    if (player != null)
                        player.Out.SendSpellEffectAnimation(this, this, 177, 0, false, 0x01);
                }
                new EcsGameTimer(this, new EcsGameTimer.EcsTimerCallback(DoCast), 500);
            }
            return 0;
        }

        public int DoCast(EcsGameTimer timer)
        {
            if (IsAlive)
            {
                GroundTarget.X = X;
                GroundTarget.Y = Y;
                GroundTarget.Z = Z;
                CastSpell(Icelord_Gtaoe, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
            }
            return 0;
        }

        public override bool AddToWorld()
        {
            Model = 665;
            Size = 70;
            MaxSpeedBase = 0;
            Name = "Pillar of Ice";
            Level = 80;
            Flags = ENpcFlags.DONTSHOWNAME;
            Flags = ENpcFlags.CANTTARGET;
            Flags = ENpcFlags.STATUE;

            Faction = FactionMgr.GetFactionByID(140);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(140));
            Realm = ERealm.None;
            RespawnInterval = -1;

            EffectMobBrain adds = new EffectMobBrain();
            SetOwnBrain(adds);
            bool success = base.AddToWorld();
            if (success)
            {
                new EcsGameTimer(this, new EcsGameTimer.EcsTimerCallback(Show_Effect), 3000);
            }
            return success;
        }

        private Spell m_Icelord_Gtaoe;
        private Spell Icelord_Gtaoe
        {
            get
            {
                if (m_Icelord_Gtaoe == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.RecastDelay = 10;
                    spell.ClientEffect = 208;
                    spell.Icon = 208;
                    spell.TooltipId = 234;
                    spell.Damage = 750;
                    spell.Name = "Pillar of Frost";
                    spell.Radius = 350;
                    spell.Range = 1800;
                    spell.SpellID = 11747;
                    spell.Target = "Area";
                    spell.Type = ESpellType.DirectDamageNoVariance.ToString();
                    spell.Uninterruptible = true;
                    spell.MoveCast = true;
                    spell.DamageType = (int)EDamageType.Cold;
                    m_Icelord_Gtaoe = new Spell(spell, 70);
                    SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_Icelord_Gtaoe);
                }
                return m_Icelord_Gtaoe;
            }
        }
    }
}

namespace DOL.AI.Brain
{
    public class EffectMobBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EffectMobBrain()
            : base()
        {
            AggroLevel = 0;
            AggroRange = 0;
            ThinkInterval = 1500;
        }
        public override void Think()
        {
            if (Body.IsAlive)
            {
                foreach (GamePlayer player in Body.GetPlayersInRadius(2500))
                {
                    if (player != null)
                    {
                        if (player.IsAlive && player.Client.Account.PrivLevel == 1)
                        {
                            if (!AggroTable.ContainsKey(player))
                                AggroTable.Add(player, 100);
                        }
                    }
                }
            }
            base.Think();
        }
    }
}
