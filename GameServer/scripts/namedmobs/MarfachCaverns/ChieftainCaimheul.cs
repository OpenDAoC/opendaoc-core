﻿using System;
using DOL.AI.Brain;
using DOL.Events;
using DOL.Database;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Styles;
namespace DOL.GS
{
    public class ChieftainCaimheul : GameEpicBoss
    {
        public ChieftainCaimheul() : base()
        {
        }
        public static int TauntID = 247;
        public static int TauntClassID = 44;
        public static Style Taunt = SkillBase.GetStyleByID(TauntID, TauntClassID);

        public static int Taunt2hID = 309;
        public static int Taunt2hClassID = 44; 
        public static Style taunt2h = SkillBase.GetStyleByID(Taunt2hID, Taunt2hClassID);

        public static int SlamID = 228;
        public static int SlamClassID = 44;
        public static Style slam = SkillBase.GetStyleByID(SlamID, SlamClassID);

        public static int SideStyleID = 318;
        public static int SideStyleClassID = 44;
        public static Style SideStyle = SkillBase.GetStyleByID(SideStyleID, SideStyleClassID);

        public static int SideFollowUpID = 319;
        public static int SideFollowUpClassID = 44;
        public static Style SideFollowUp = SkillBase.GetStyleByID(SideFollowUpID, SideFollowUpClassID);

        public static int AfterParryID = 313;
        public static int AfterParryClassID = 44;
        public static Style AfterParry = SkillBase.GetStyleByID(AfterParryID, AfterParryClassID);
        public override void OnAttackedByEnemy(AttackData ad) // on Boss actions
        {
             base.OnAttackedByEnemy(ad);
        }
        public override void OnAttackEnemy(AttackData ad) //on enemy actions
        {
            base.OnAttackEnemy(ad);
        }
        public override int GetResist(EDamageType damageType)
        {
            switch (damageType)
            {
                case EDamageType.Slash: return 20; // dmg reduction for melee dmg
                case EDamageType.Crush: return 20; // dmg reduction for melee dmg
                case EDamageType.Thrust: return 20; // dmg reduction for melee dmg
                default: return 30; // dmg reduction for rest resists
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
            get { return 30000; }
        }
        public override void TakeDamage(GameObject source, EDamageType damageType, int damageAmount, int criticalAmount)
        {
            if (source is GamePlayer || source is GameSummonedPet)
            {
                if (IsOutOfTetherRange)
                {
                    if (damageType == EDamageType.Body || damageType == EDamageType.Cold ||
                        damageType == EDamageType.Energy || damageType == EDamageType.Heat
                        || damageType == EDamageType.Matter || damageType == EDamageType.Spirit ||
                        damageType == EDamageType.Crush || damageType == EDamageType.Thrust
                        || damageType == EDamageType.Slash)
                    {
                        GamePlayer truc;
                        if (source is GamePlayer)
                            truc = (source as GamePlayer);
                        else
                            truc = ((source as GameSummonedPet).Owner as GamePlayer);
                        if (truc != null)
                            truc.Out.SendMessage(this.Name + " is immune to any damage!", eChatType.CT_System,
                                eChatLoc.CL_ChatWindow);
                        base.TakeDamage(source, damageType, 0, 0);
                        return;
                    }
                }
                else
                {
                    base.TakeDamage(source, damageType, damageAmount, criticalAmount);
                }
            }
        }
        public override double AttackDamage(InventoryItem weapon)
        {
            return base.AttackDamage(weapon) * Strength / 100;
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
        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(8821);
            LoadTemplate(npcTemplate);
            Strength = npcTemplate.Strength;
            Dexterity = npcTemplate.Dexterity;
            Constitution = npcTemplate.Constitution;
            Quickness = npcTemplate.Quickness;
            Piety = npcTemplate.Piety;
            Intelligence = npcTemplate.Intelligence;
            Empathy = npcTemplate.Empathy;
            Faction = FactionMgr.GetFactionByID(187);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(187));
            RespawnInterval = ServerProperties.Properties.SET_EPIC_GAME_ENCOUNTER_RESPAWNINTERVAL * 60000; //1min is 60000 miliseconds
            BodyType = (ushort)NpcTemplateMgr.eBodyType.Humanoid;

            GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
            template.AddNPCEquipment(eInventorySlot.TorsoArmor, 667, 0, 0, 6);//modelID,color,effect,extension
            template.AddNPCEquipment(eInventorySlot.ArmsArmor, 410, 0);
            template.AddNPCEquipment(eInventorySlot.LegsArmor, 409, 0);
            template.AddNPCEquipment(eInventorySlot.HandsArmor, 411, 0, 0, 4);
            template.AddNPCEquipment(eInventorySlot.FeetArmor, 412, 0, 0, 5);
            template.AddNPCEquipment(eInventorySlot.HeadArmor, 1200, 0, 0, 0);
            template.AddNPCEquipment(eInventorySlot.Cloak, 678, 0, 0, 0);
            template.AddNPCEquipment(eInventorySlot.RightHandWeapon, 446, 0, 0);
            template.AddNPCEquipment(eInventorySlot.LeftHandWeapon, 1147, 0, 0);
            template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 475, 0, 0);
            Inventory = template.CloseTemplate();
            SwitchWeapon(EActiveWeaponSlot.Standard);
            if (!Styles.Contains(Taunt))
                Styles.Add(Taunt);
            if (!Styles.Contains(taunt2h))
                Styles.Add(taunt2h);
            if (!Styles.Contains(SideStyle))
                Styles.Add(SideStyle);
            if (!Styles.Contains(SideFollowUp))
                Styles.Add(SideFollowUp);
            if (!Styles.Contains(slam))
                Styles.Add(slam);
            if (!Styles.Contains(AfterParry))
                Styles.Add(AfterParry);

            ChieftainCaimheulBrain.Phase2 = false;
            ChieftainCaimheulBrain.CanWalk = false;
            ChieftainCaimheulBrain.IsPulled = false;
            VisibleActiveWeaponSlots = 16;
            MeleeDamageType = EDamageType.Slash;
            ChieftainCaimheulBrain sbrain = new ChieftainCaimheulBrain();
            SetOwnBrain(sbrain);
            LoadedFromScript = false; //load from database
            SaveIntoDatabase();
            base.AddToWorld();
            return true;
        }

        [ScriptLoadedEvent]
        public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
        {
            GameNPC[] npcs;
            npcs = WorldMgr.GetNPCsByNameFromRegion("Chieftain Caimheul", 276, (ERealm)0);
            if (npcs.Length == 0)
            {
                log.Warn("Chieftain Caimheul not found, creating it...");

                log.Warn("Initializing Chieftain Caimheul...");
                ChieftainCaimheul HOC = new ChieftainCaimheul();
                HOC.Name = "Chieftain Caimheul";
                HOC.Model = 354;
                HOC.Realm = 0;
                HOC.Level = 65;
                HOC.Size = 50;
                HOC.CurrentRegionID = 276; //marfach caverns
                HOC.RespawnInterval = ServerProperties.Properties.SET_SI_EPIC_ENCOUNTER_RESPAWNINTERVAL * 60000; //1min is 60000 miliseconds
                HOC.Faction = FactionMgr.GetFactionByID(187);
                HOC.Faction.AddFriendFaction(FactionMgr.GetFactionByID(187));

                HOC.X = 32597;
                HOC.Y = 38903;
                HOC.Z = 15061;
                HOC.Heading = 694;
                ChieftainCaimheulBrain ubrain = new ChieftainCaimheulBrain();
                HOC.SetOwnBrain(ubrain);
                HOC.AddToWorld();
                HOC.SaveIntoDatabase();
                HOC.Brain.Start();
            }
            else
                log.Warn("Chieftain Caimheul exist ingame, remove it and restart server if you want to add by script code.");
        }
    }
}

namespace DOL.AI.Brain
{
    public class ChieftainCaimheulBrain : StandardMobBrain
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ChieftainCaimheulBrain()
            : base()
        {
            AggroLevel = 100;
            AggroRange = 500;
            ThinkInterval = 1500;
        }
        public static bool Phase2 = false;
        public static bool CanWalk = false;
        public static bool IsPulled = false;
        public override void Think()
        {
            if (!CheckProximityAggro())
            {
                //set state to RETURN TO SPAWN
                INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(8821);
                FSM.SetCurrentState(EFsmStateType.RETURN_TO_SPAWN);
                Body.Health = Body.MaxHealth;
                Phase2 = false;
                CanWalk = false;
                IsPulled = false;
                Body.Strength = npcTemplate.Strength;
                Body.SwitchWeapon(EActiveWeaponSlot.Standard);
                Body.MeleeDamageType = EDamageType.Slash;
                Body.VisibleActiveWeaponSlots = 16;
                if (!Body.Styles.Contains(ChieftainCaimheul.Taunt))
                    Body.Styles.Add(ChieftainCaimheul.Taunt);
                if (!Body.Styles.Contains(ChieftainCaimheul.slam))
                    Body.Styles.Add(ChieftainCaimheul.slam); 
            }
            if (Body.InCombat && HasAggro && Body.TargetObject != null)
            {
                if (IsPulled == false)
                {
                    foreach (GameNPC npc in WorldMgr.GetNPCsFromRegion(Body.CurrentRegionID))
                    {
                        if (npc != null)
                        {
                            if (npc.IsAlive && npc.PackageID == "CaimheulBaf")
                            {
                                AddAggroListTo(npc.Brain as StandardMobBrain);
                            }
                        }
                    }
                    IsPulled = true;
                }
                if (Body.TargetObject != null)
                {
                    INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(8821);
                    GameLiving living = Body.TargetObject as GameLiving;
                    float angle = Body.TargetObject.GetAngle(Body);
                    if (!living.effectListComponent.ContainsEffectForEffectType(EEffect.StunImmunity) && !living.effectListComponent.ContainsEffectForEffectType(EEffect.Stun))
                    {
                        CanWalk = false;//reset flag 
                    }
                    if (Phase2 == false)
                    {
                        if (!living.effectListComponent.ContainsEffectForEffectType(EEffect.Stun) && !living.effectListComponent.ContainsEffectForEffectType(EEffect.StunImmunity))
                        {
                            Body.Strength = npcTemplate.Strength;
                            Body.SwitchWeapon(EActiveWeaponSlot.Standard);
                            Body.VisibleActiveWeaponSlots = 16;
                            Body.styleComponent.NextCombatStyle = ChieftainCaimheul.slam;//check if target has stun or immunity if not slam
                            Body.BlockChance = 50;
                            Body.ParryChance = 0;
                            Body.MeleeDamageType = EDamageType.Crush;
                        }
                        if (living.effectListComponent.ContainsEffectForEffectType(EEffect.Stun))
                        {
                            if (CanWalk == false)
                            {
                                new ECSGameTimer(Body, new ECSGameTimer.ECSTimerCallback(WalkSide), 500);
                                CanWalk = true;
                            }
                        }
                        if ((angle >= 45 && angle < 150) || (angle >= 210 && angle < 315))//side
                        {
                            Body.Strength = 400;
                            Body.BlockChance = 0;
                            Body.ParryChance = 50;
                            Body.SwitchWeapon(EActiveWeaponSlot.TwoHanded);
                            Body.MeleeDamageType = EDamageType.Thrust;
                            Body.VisibleActiveWeaponSlots = (byte)EActiveWeaponSlot.TwoHanded;
                            Body.styleComponent.NextCombatBackupStyle = ChieftainCaimheul.SideStyle;
                            Body.styleComponent.NextCombatStyle = ChieftainCaimheul.SideFollowUp;
                        }
                        else if(!living.effectListComponent.ContainsEffectForEffectType(EEffect.Stun) && living.effectListComponent.ContainsEffectForEffectType(EEffect.StunImmunity))
                        {
                            Body.Strength = npcTemplate.Strength;
                            Body.SwitchWeapon(EActiveWeaponSlot.Standard);
                            Body.VisibleActiveWeaponSlots = 16;
                            Body.styleComponent.NextCombatStyle = ChieftainCaimheul.Taunt;
                            Body.MeleeDamageType = EDamageType.Slash;
                            Body.BlockChance = 50;
                            Body.ParryChance = 0;
                        }
                    }
                    if(Body.HealthPercent <= 50 && Phase2==false)
                    {
                        Phase2 = true;
                    }
                    if(Phase2)
                    {
                        Body.Strength = 400;
                        Body.SwitchWeapon(EActiveWeaponSlot.TwoHanded);
                        Body.VisibleActiveWeaponSlots = (byte)EActiveWeaponSlot.TwoHanded;
                        Body.BlockChance = 0;
                        Body.ParryChance = 50;
                        if(Body.Styles.Contains(ChieftainCaimheul.slam))
                            Body.Styles.Remove(ChieftainCaimheul.slam);
                        if(Body.Styles.Contains(ChieftainCaimheul.Taunt))
                            Body.Styles.Remove(ChieftainCaimheul.Taunt);
                        Body.MeleeDamageType = EDamageType.Thrust;

                        if (living.effectListComponent.ContainsEffectForEffectType(EEffect.Stun))
                        {
                            if (CanWalk == false)
                            {
                                new ECSGameTimer(Body, new ECSGameTimer.ECSTimerCallback(WalkSide), 500);
                                CanWalk = true;
                            }
                        }
                        if ((angle >= 45 && angle < 150) || (angle >= 210 && angle < 315))//side
                        {
                            Body.styleComponent.NextCombatBackupStyle = ChieftainCaimheul.SideStyle;
                            Body.styleComponent.NextCombatStyle = ChieftainCaimheul.SideFollowUp;
                        }
                        else
                        {
                            Body.styleComponent.NextCombatBackupStyle = ChieftainCaimheul.taunt2h;
                            Body.styleComponent.NextCombatStyle = ChieftainCaimheul.AfterParry;
                        }
                    }
                }
            }
            base.Think();
        }
        public int WalkSide(ECSGameTimer timer)
        {
            if (Body.InCombat && HasAggro && Body.TargetObject != null)
            {
                if (Body.TargetObject is GameLiving)
                {
                    GameLiving living = Body.TargetObject as GameLiving;
                    float angle = living.GetAngle(Body);
                    Point2D positionalPoint;
                    positionalPoint = living.GetPointFromHeading((ushort)(living.Heading + (90 * (4096.0 / 360.0))), 65);
                    //Body.WalkTo(positionalPoint.X, positionalPoint.Y, living.Z, 280);
                    Body.X = positionalPoint.X;
                    Body.Y = positionalPoint.Y;
                    Body.Z = living.Z;
                    Body.Heading = 1250;
                }
            }
            return 0;
        }
    }
}



