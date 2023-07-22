using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.Effects;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;
using DOL.GS.ServerProperties;
using DOL.GS.SkillHandler;
using DOL.GS.Spells;
using DOL.GS.Styles;
using DOL.Language;

namespace DOL.GS
{
    public class AttackComponent : IManagedEntity
    {
        public GameLiving owner;
        public WeaponAction weaponAction;
        public AttackAction attackAction;
        public EntityManagerId EntityManagerId { get; set; } = new();

        /// <summary>
        /// The objects currently attacking this living
        /// To be more exact, the objects that are in combat
        /// and have this living as target.
        /// </summary>
        protected List<GameObject> m_attackers = new();

        /// <summary>
        /// Returns the list of attackers
        /// </summary>
        public List<GameObject> Attackers => m_attackers;

        /// <summary>
        /// Adds an attacker to the attackerlist
        /// </summary>
        /// <param name="attacker">the attacker to add</param>
        public void AddAttacker(GameObject attacker)
        {
            lock (Attackers)
            {
                if (attacker == owner)
                    return;

                if (m_attackers.Contains(attacker))
                    return;

                m_attackers.Add(attacker);
            }
        }

        /// <summary>
        /// Removes an attacker from the list
        /// </summary>
        /// <param name="attacker">the attacker to remove</param>
        public void RemoveAttacker(GameObject attacker)
        {
            //			log.Warn(Name + ": RemoveAttacker "+attacker.Name);
            //			log.Error(Environment.StackTrace);
            lock (Attackers)
            {
                m_attackers.Remove(attacker);

                //if (m_attackers.Count() == 0)
                //    EntityManager.RemoveComponent(typeof(AttackComponent), owner);
            }
        }

        /// <summary>
        /// The target that was passed when 'StartAttackReqest' was called and the request accepted.
        /// </summary>
        private GameObject m_startAttackTarget;

        /// <summary>
        /// Actually a boolean. Use 'StartAttackRequested' to preserve thread safety.
        /// </summary>
        private long m_startAttackRequested;

        public bool StartAttackRequested
        {
            get => Interlocked.Read(ref m_startAttackRequested) == 1;
            set => Interlocked.Exchange(ref m_startAttackRequested, Convert.ToInt64(value));
        }

        public AttackComponent(GameLiving owner)
        {
            this.owner = owner;
        }

        public void Tick(long time)
        {
            if (StartAttackRequested)
            {
                StartAttackRequested = false;
                StartAttack();
            }

            attackAction?.Tick(time);

            if (weaponAction?.AttackFinished == true)
                weaponAction = null;

            if (weaponAction is null && attackAction is null && !owner.InCombat)
                EntityMgr.Remove(EntityMgr.EntityType.AttackComponent, this);
        }

        /// <summary>
        /// The chance for a critical hit
        /// </summary>
        /// <param name="weapon">attack weapon</param>
        public int AttackCriticalChance(WeaponAction action, InventoryItem weapon)
        {
            if (owner is GamePlayer playerOwner)
            {
                if (weapon != null)
                {
                    if (weapon.Item_Type != Slot.RANGED)
                        return playerOwner.GetModified(EProperty.CriticalMeleeHitChance);
                    else
                    {
                        if (action.RangedAttackType == ERangedAttackType.Critical)
                            return 0;
                        else
                            return playerOwner.GetModified(EProperty.CriticalArcheryHitChance);
                    }
                }

                // Base of 10% critical chance.
                return 10;
            }

            /// [Atlas - Takii] Wild Minion Implementation. We don't want any non-pet NPCs to crit.
            /// We cannot reliably check melee vs ranged here since archer pets don't necessarily have a proper weapon with the correct slot type assigned.
            /// Since Wild Minion is the only way for pets to crit and we (currently) want it to affect melee/ranged/spells, we can just rely on the Melee crit chance even for archery attacks
            /// and as a result we don't actually need to detect melee vs ranged to end up with the correct behavior since all attack types will have the same % chance to crit in the end.
            if (owner is GameNPC npc)
            {
                // Player-Summoned pet.
                if (npc is GameSummonedPet summonedPet && summonedPet.Owner is GamePlayer)
                    return npc.GetModified(EProperty.CriticalMeleeHitChance);

                // Charmed Pet.
                if (npc.Brain is IControlledBrain charmedPetBrain && charmedPetBrain.GetPlayerOwner() != null)
                    return npc.GetModified(EProperty.CriticalMeleeHitChance);
            }

            return 0;
        }

        /// <summary>
        /// Returns the damage type of the current attack
        /// </summary>
        /// <param name="weapon">attack weapon</param>
        public EDamageType AttackDamageType(InventoryItem weapon)
        {
            if (owner is GamePlayer || owner is CommanderPet)
            {
                var p = owner as GamePlayer;

                if (weapon == null)
                    return EDamageType.Natural;

                switch ((EObjectType) weapon.Object_Type)
                {
                    case EObjectType.Crossbow:
                    case EObjectType.Longbow:
                    case EObjectType.CompositeBow:
                    case EObjectType.RecurvedBow:
                    case EObjectType.Fired:
                        InventoryItem ammo = p.rangeAttackComponent.Ammo;

                        if (ammo == null)
                            return (EDamageType) weapon.Type_Damage;

                        return (EDamageType) ammo.Type_Damage;
                    case EObjectType.Shield:
                        return EDamageType.Crush; // TODO: shields do crush damage (!) best is if Type_Damage is used properly
                    default:
                        return (EDamageType) weapon.Type_Damage;
                }
            }
            else if (owner is GameNPC)
                return (owner as GameNPC).MeleeDamageType;
            else
                return EDamageType.Natural;
        }

        /// <summary>
        /// Gets the attack-state of this living
        /// </summary>
        public virtual bool AttackState { get; set; }

        /// <summary>
        /// Gets which weapon was used for the last dual wield attack
        /// 0: right (or non dual wield user), 1: left, 2: both
        /// </summary>
        public int UsedHandOnLastDualWieldAttack { get; set; }

        /// <summary>
        /// Returns this attack's range
        /// </summary>
        public int AttackRange
        {
            /* tested with:
            staff					= 125-130
            sword			   		= 126-128.06
            shield (Numb style)		= 127-129
            polearm	(Impale style)	= 127-130
            mace (Daze style)		= 127.5-128.7
            Think it's safe to say that it never changes; different with mobs. */

            get
            {
                if (owner is GamePlayer)
                {
                    InventoryItem weapon = owner.ActiveWeapon;

                    if (weapon == null)
                        return 0;

                    var player = owner as GamePlayer;
                    GameLiving target = player.TargetObject as GameLiving;

                    // TODO: Change to real distance of bows.
                    if (weapon.SlotPosition == (int)eInventorySlot.DistanceWeapon)
                    {
                        double range;

                        switch ((EObjectType) weapon.Object_Type)
                        {
                            case EObjectType.Longbow:
                                range = 1760;
                                break;
                            case EObjectType.RecurvedBow:
                                range = 1680;
                                break;
                            case EObjectType.CompositeBow:
                                range = 1600;
                                break;
                            case EObjectType.Thrown:
                                range = 1160;
                                if (weapon.Name.ToLower().Contains("weighted"))
                                    range = 1450;
                                break;
                            default:
                                range = 1200;
                                break; // Shortbow, crossbow, throwing.
                        }

                        range = Math.Max(32, range * player.GetModified(EProperty.ArcheryRange) * 0.01);
                        InventoryItem ammo = player.rangeAttackComponent.Ammo;

                        if (ammo != null)
                            switch ((ammo.SPD_ABS >> 2) & 0x3)
                            {
                                case 0:
                                    range *= 0.85;
                                    break; // Clout -15%
                                //case 1:
                                //  break; // (none) 0%
                                case 2:
                                    range *= 1.15;
                                    break; // Doesn't exist on live
                                case 3:
                                    range *= 1.25;
                                    break; // Flight +25%
                            }

                        if (target != null)
                            range += Math.Min((player.Z - target.Z) / 2.0, 500);
                        if (range < 32)
                            range = 32;

                        return (int)range;
                    }


                    // int meleeRange = 128;
                    int meleeRange = 150; // Increase default melee range to 150 to help with higher latency players.

                    if (target is GameKeepComponent)
                        meleeRange += 150;
                    else
                    {
                        if (target != null && target.IsMoving)
                            meleeRange += 32;
                        if (player.IsMoving)
                            meleeRange += 32;
                    }

                    return meleeRange;
                }
                else
                {
                    if (owner.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                        return Math.Max(32, (int) (2000.0 * owner.GetModified(EProperty.ArcheryRange) * 0.01));

                    return 200;
                }
            }
        }

        /// <summary>
        /// Gets the current attackspeed of this living in milliseconds
        /// </summary>
        /// <returns>effective speed of the attack. average if more than one weapon.</returns>
        public int AttackSpeed(InventoryItem mainWeapon, InventoryItem leftWeapon = null)
        {
            if (owner is GamePlayer player)
            {
                if (mainWeapon == null)
                    return 0;

                double speed = 0;
                bool bowWeapon = false;

                // If leftWeapon is null even on a dual wield attack, use the mainWeapon instead
                switch (UsedHandOnLastDualWieldAttack)
                {
                    case 2:
                        speed = mainWeapon.SPD_ABS;
                        if (leftWeapon != null)
                        {
                            speed += leftWeapon.SPD_ABS;
                            speed /= 2;
                        }
                        break;
                    case 1:
                        speed = leftWeapon != null ? leftWeapon.SPD_ABS : mainWeapon.SPD_ABS;
                        break;
                    case 0:
                        speed = mainWeapon.SPD_ABS;
                        break;
                }

                if (speed == 0)
                    return 0;

                switch (mainWeapon.Object_Type)
                {
                    case (int) EObjectType.Fired:
                    case (int) EObjectType.Longbow:
                    case (int) EObjectType.Crossbow:
                    case (int) EObjectType.RecurvedBow:
                    case (int) EObjectType.CompositeBow:
                        bowWeapon = true;
                        break;
                }

                int qui = Math.Min(250, player.Quickness); //250 soft cap on quickness

                if (bowWeapon)
                {
                    if (ServerProperties.ServerProperties.ALLOW_OLD_ARCHERY)
                    {
                        //Draw Time formulas, there are very many ...
                        //Formula 2: y = iBowDelay * ((100 - ((iQuickness - 50) / 5 + iMasteryofArcheryLevel * 3)) / 100)
                        //Formula 1: x = (1 - ((iQuickness - 60) / 500 + (iMasteryofArcheryLevel * 3) / 100)) * iBowDelay
                        //Table a: Formula used: drawspeed = bowspeed * (1-(quickness - 50)*0.002) * ((1-MoA*0.03) - (archeryspeedbonus/100))
                        //Table b: Formula used: drawspeed = bowspeed * (1-(quickness - 50)*0.002) * (1-MoA*0.03) - ((archeryspeedbonus/100 * basebowspeed))

                        //For now use the standard weapon formula, later add ranger haste etc.
                        speed *= (1.0 - (qui - 60) * 0.002);
                        double percent = 0;
                        // Calcul ArcherySpeed bonus to substract
                        percent = speed * 0.01 * player.GetModified(EProperty.ArcherySpeed);
                        // Apply RA difference
                        speed -= percent;
                        //log.Debug("speed = " + speed + " percent = " + percent + " eProperty.archeryspeed = " + GetModified(eProperty.ArcherySpeed));

                        if (owner.rangeAttackComponent.RangedAttackType == ERangedAttackType.Critical) 
                            speed = speed * 2 - (player.GetAbilityLevel(Abilities.Critical_Shot) - 1) * speed / 10;
                    }
                    else
                    {
                        // no archery bonus
                        speed *= (1.0 - (qui - 60) * 0.002);
                    }
                }
                else
                {
                    // TODO use haste
                    //Weapon Speed*(1-(Quickness-60)/500]*(1-Haste)
                    speed *= ((1.0 - (qui - 60) * 0.002) * 0.01 * player.GetModified(EProperty.MeleeSpeed));
                    //Console.WriteLine($"Speed after {speed} quiMod {(1.0 - (qui - 60) * 0.002)} melee speed {0.01 * p.GetModified(eProperty.MeleeSpeed)} together {(1.0 - (qui - 60) * 0.002) * 0.01 * p.GetModified(eProperty.MeleeSpeed)}");
                }

                // apply speed cap
                if (speed < 15)
                {
                    speed = 15;
                }

                return (int) (speed * 100);
            }
            else
            {
                double speed = NpcWeaponSpeed() * 100 * (1.0 - (owner.GetModified(EProperty.Quickness) - 60) / 500.0);
                if (owner is GameSummonedPet pet)
                {
                    if (pet != null)
                    {
                        switch(pet.Name)
                        {
                            case "amber simulacrum": speed *= (owner.GetModified(EProperty.MeleeSpeed) * 0.01) * 1.45; break;
                            case "emerald simulacrum": speed *= (owner.GetModified(EProperty.MeleeSpeed) * 0.01) * 1.45; break;
                            case "ruby simulacrum": speed *= (owner.GetModified(EProperty.MeleeSpeed) * 0.01) * 0.95; break;
                            case "sapphire simulacrum": speed *= (owner.GetModified(EProperty.MeleeSpeed) * 0.01) * 0.95; break;
                            case "jade simulacrum": speed *= (owner.GetModified(EProperty.MeleeSpeed) * 0.01) * 0.95; break;
                            default: speed *= owner.GetModified(EProperty.MeleeSpeed) * 0.01; break;
                        }
                        //return (int)speed;
                    }
                }
                else
                {
                    if (owner.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                    {
                        // Old archery uses archery speed, but new archery uses casting speed
                        if (ServerProperties.ServerProperties.ALLOW_OLD_ARCHERY)
                            speed *= 1.0 - owner.GetModified(EProperty.ArcherySpeed) * 0.01;
                        else
                            speed *= 1.0 - owner.GetModified(EProperty.CastingSpeed) * 0.01;
                    }
                    else
                    {
                        speed *= owner.GetModified(EProperty.MeleeSpeed) * 0.01;
                    }
                }

                return (int) Math.Max(500.0, speed);
            }
        }

        /// <summary>
        /// Gets the speed of a NPC's weapon, based on its ActiveWeaponSlot.
        /// InventoryItem.SPD_ABS isn't set for NPCs, so this method must be used instead.
        /// </summary>
        public int NpcWeaponSpeed()
        {
            switch (owner.ActiveWeaponSlot)
            {
                default:
                case EActiveWeaponSlot.Standard:
                    return 30;
                case EActiveWeaponSlot.TwoHanded:
                    return 40;
                case EActiveWeaponSlot.Distance:
                    return 45;
            }
        }

        /// <summary>
        /// Gets the attack damage
        /// </summary>
        /// <param name="weapon">the weapon used for attack</param>
        /// <returns>the weapon damage</returns>
        public double AttackDamage(InventoryItem weapon)
        {
            if (owner is GamePlayer p)
            {
                if (weapon == null)
                    return 0;

                double effectiveness = 1.00;
                double damage = p.WeaponDamage(weapon) * weapon.SPD_ABS * 0.1;
                
                //slow weapon bonus as found here: https://www2.uthgard.net/tracker/issue/2753/@/Bow_damage_variance_issue_(taking_item_/_spec_???)
                //EDPS * (your WS/target AF) * (1-absorb) * slow weap bonus * SPD * 2h weapon bonus * Arrow Bonus 
                damage *= 1 + (weapon.SPD_ABS - 20) * 0.03 * 0.1;

                if (weapon.Hand == 1) // two-hand
                {
                    // twohanded used weapons get 2H-Bonus = 10% + (Skill / 2)%
                    int spec = p.WeaponSpecLevel(weapon) - 1;
                    damage *= 1.1 + spec * 0.005;
                }

                if (weapon.Item_Type == Slot.RANGED)
                {
                    //ammo damage bonus
                    InventoryItem ammo = p.rangeAttackComponent.Ammo;

                    if (ammo != null)
                    {
                        switch ((ammo.SPD_ABS) & 0x3)
                        {
                            case 0:
                                damage *= 0.85;
                                break; //Blunt       (light) -15%
                            //case 1: damage *= 1;	break; //Bodkin     (medium)   0%
                            case 2:
                                damage *= 1.15;
                                break; //doesn't exist on live
                            case 3:
                                damage *= 1.25;
                                break; //Broadhead (X-heavy) +25%
                        }
                    }

                    //Ranged damage buff,debuff,Relic,RA
                    effectiveness += p.GetModified(EProperty.RangedDamage) * 0.01;
                }
                else if (weapon.Item_Type == Slot.RIGHTHAND || weapon.Item_Type == Slot.LEFTHAND ||
                         weapon.Item_Type == Slot.TWOHAND)
                {
                    //Melee damage buff,debuff,Relic,RA
                    effectiveness += p.GetModified(EProperty.MeleeDamage) * 0.01;

                    if (weapon.Item_Type != Slot.TWOHAND)
                    {
                        if (p.Inventory?.GetItem(eInventorySlot.LeftHandWeapon) != null)
                        {
                            var leftWep = p.Inventory.GetItem(eInventorySlot.LeftHandWeapon);
                            if (p.GetModifiedSpecLevel(Specs.Left_Axe) > 0)
                            {
                                int LASpec = owner.GetModifiedSpecLevel(Specs.Left_Axe);
                                if (LASpec > 0)
                                {
                                    var leftAxeEffectiveness = 0.625 + 0.0034 * LASpec;
                                
                                    if (p.GetModified(EProperty.OffhandDamageAndChance) > 0)
                                        leftAxeEffectiveness += 0.01 * p.GetModified(EProperty.OffhandDamageAndChance);

                                    damage *= leftAxeEffectiveness;
                                }
                            }
                        }
                    }
                }

                damage *= effectiveness;
                return damage;
            }
            else
            {
                double effectiveness = 1.00;
                double damage = (1.0 + owner.Level / ServerProperties.ServerProperties.PVE_MOB_DAMAGE_F1 + owner.Level * owner.Level / ServerProperties.ServerProperties.PVE_MOB_DAMAGE_F2) * NpcWeaponSpeed() * 0.1;

                if (weapon == null
                    || weapon.SlotPosition == Slot.RIGHTHAND
                    || weapon.SlotPosition == Slot.LEFTHAND
                    || weapon.SlotPosition == Slot.TWOHAND)
                    //Melee damage buff,debuff,RA
                    effectiveness += owner.GetModified(EProperty.MeleeDamage) * 0.01;
                else if (weapon.SlotPosition == Slot.RANGED)
                {
                    if (weapon.Object_Type == (int)EObjectType.Longbow
                        || weapon.Object_Type == (int)EObjectType.RecurvedBow
                        || weapon.Object_Type == (int)EObjectType.CompositeBow)
                    {
                        if (ServerProperties.ServerProperties.ALLOW_OLD_ARCHERY)
                            effectiveness += owner.GetModified(EProperty.RangedDamage) * 0.01;
                        else
                            effectiveness += owner.GetModified(EProperty.SpellDamage) * 0.01;
                    }
                    else
                        effectiveness += owner.GetModified(EProperty.RangedDamage) * 0.01;
                }

                damage *= effectiveness;
                return damage;
            }
        }

        public void RequestStartAttack(GameObject attackTarget)
        {
            if (!StartAttackRequested)
            {
                m_startAttackTarget = attackTarget;
                StartAttackRequested = true;
                EntityMgr.Add(EntityMgr.EntityType.AttackComponent, this);
            }
        }

        private void StartAttack()
        {
            if (owner is GamePlayer player)
            {
                if (player.CharacterClass.StartAttack(m_startAttackTarget) == false)
                {
                    return;
                }

                if (!player.IsAlive)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.YouCantCombat"),
                        EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    return;
                }

                // Necromancer with summoned pet cannot attack
                if (player.ControlledBrain?.Body is NecromancerPet)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language,
                            "GamePlayer.StartAttack.CantInShadeMode"), EChatType.CT_YouHit,
                        EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.IsStunned)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language,
                            "GamePlayer.StartAttack.CantAttackStunned"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.IsMezzed)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language,
                            "GamePlayer.StartAttack.CantAttackmesmerized"), EChatType.CT_YouHit,
                        EChatLoc.CL_SystemWindow);
                    return;
                }

                long vanishTimeout = player.TempProperties.getProperty<long>(NfRaVanishEffect.VANISH_BLOCK_ATTACK_TIME_KEY);
                if (vanishTimeout > 0 && vanishTimeout > GameLoop.GameLoopTime)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.YouMustWaitAgain",
                            (vanishTimeout - GameLoop.GameLoopTime + 1000) / 1000), EChatType.CT_YouHit,
                        EChatLoc.CL_SystemWindow);
                    return;
                }

                long VanishTick = player.TempProperties.getProperty<long>(NfRaVanishEffect.VANISH_BLOCK_ATTACK_TIME_KEY);
                long changeTime = GameLoop.GameLoopTime - VanishTick;
                if (changeTime < 30000 && VanishTick > 0)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.YouMustWait",
                            ((30000 - changeTime) / 1000).ToString()), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.IsOnHorse)
                    player.IsOnHorse = false;

                if (player.Steed != null && player.Steed is GameSiegeRam)
                {
                    player.Out.SendMessage("You can't enter combat mode while riding a siegeram!.", EChatType.CT_YouHit,EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.IsDisarmed)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.CantDisarmed"),
                        EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.IsSitting)
                {
                    player.Sit(false);
                }

                InventoryItem attackWeapon = owner.ActiveWeapon;

                if (attackWeapon == null)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language,
                            "GamePlayer.StartAttack.CannotWithoutWeapon"), EChatType.CT_YouHit,
                        EChatLoc.CL_SystemWindow);
                    return;
                }

                if (attackWeapon.Object_Type == (int) EObjectType.Instrument)
                {
                    player.Out.SendMessage(
                        LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.CannotMelee"),
                        EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    return;
                }

                if (player.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                {
                    if (ServerProperties.ServerProperties.ALLOW_OLD_ARCHERY == false)
                    {
                        if ((ECharacterClass) player.CharacterClass.ID == ECharacterClass.Scout ||
                            (ECharacterClass) player.CharacterClass.ID == ECharacterClass.Hunter ||
                            (ECharacterClass) player.CharacterClass.ID == ECharacterClass.Ranger)
                        {
                            // There is no feedback on live when attempting to fire a bow with arrows
                            return;
                        }
                    }

                    // Check arrows for ranged attack
                    if (player.rangeAttackComponent.UpdateAmmo(attackWeapon) == null)
                    {
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language,
                                "GamePlayer.StartAttack.SelectQuiver"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        return;
                    }

                    // Check if selected ammo is compatible for ranged attack
                    if (!player.rangeAttackComponent.IsAmmoCompatible)
                    {
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language,
                                "GamePlayer.StartAttack.CantUseQuiver"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        return;
                    }

                    if (EffectListService.GetAbilityEffectOnTarget(player, EEffect.SureShot) != null)
                        player.rangeAttackComponent.RangedAttackType = ERangedAttackType.SureShot;
                    if (EffectListService.GetAbilityEffectOnTarget(player, EEffect.RapidFire) != null)
                        player.rangeAttackComponent.RangedAttackType = ERangedAttackType.RapidFire;
                    if (EffectListService.GetAbilityEffectOnTarget(player, EEffect.TrueShot) != null)
                        player.rangeAttackComponent.RangedAttackType = ERangedAttackType.Long;


                    if (player.rangeAttackComponent?.RangedAttackType == ERangedAttackType.Critical &&
                        player.Endurance < RangeAttackComponent.CRITICAL_SHOT_ENDURANCE_COST)
                    {
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.TiredShot"),
                            EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        return;
                    }

                    if (player.Endurance < RangeAttackComponent.DEFAULT_ENDURANCE_COST)
                    {
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.TiredUse",
                                attackWeapon.Name), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        return;
                    }

                    if (player.IsStealthed)
                    {
                        // -Chance to unstealth while nocking an arrow = stealth spec / level
                        // -Chance to unstealth nocking a crit = stealth / level  0.20
                        int stealthSpec = player.GetModifiedSpecLevel(Specs.Stealth);
                        int stayStealthed = stealthSpec * 100 / player.Level;
                        if (player.rangeAttackComponent?.RangedAttackType == ERangedAttackType.Critical)
                            stayStealthed -= 20;

                        if (!UtilCollection.Chance(stayStealthed))
                            player.Stealth(false);
                    }
                }
                else
                {
                    if (m_startAttackTarget == null)
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language,
                                "GamePlayer.StartAttack.CombatNoTarget"), EChatType.CT_YouHit,
                            EChatLoc.CL_SystemWindow);
                    else if (m_startAttackTarget is GameNPC)
                    {
                        player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language,
                                "GamePlayer.StartAttack.CombatTarget",
                                m_startAttackTarget.GetName(0, false, player.Client.Account.Language, (m_startAttackTarget as GameNPC))),
                            EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    }
                    else
                    {
                        player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.CombatTarget",
                                m_startAttackTarget.GetName(0, false)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    }
                }

                /*
                if (p.CharacterClass is PlayerClass.ClassVampiir)
                {
                    GameSpellEffect removeEffect = SpellHandler.FindEffectOnTarget(p, "VampiirSpeedEnhancement");
                    if (removeEffect != null)
                        removeEffect.Cancel(false);
                }
                else
                {
                    // Bard RR5 ability must drop when the player starts a melee attack
                    IGameEffect DreamweaverRR5 = p.EffectList.GetOfType<DreamweaverEffect>();
                    if (DreamweaverRR5 != null)
                        DreamweaverRR5.Cancel(false);
                }*/
                if (LivingStartAttack())
                {
                    if (player.castingComponent.SpellHandler?.Spell.Uninterruptible == false)
                    {
                        player.StopCurrentSpellcast();
                        player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.SpellCancelled"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                    }

                    if (player.ActiveWeaponSlot != EActiveWeaponSlot.Distance)
                        player.Out.SendAttackMode(AttackState);
                    else
                    {
                        player.TempProperties.setProperty(RangeAttackComponent.RANGED_ATTACK_START, GameLoop.GameLoopTime);

                        string typeMsg = "shot";
                        if (attackWeapon.Object_Type == (int) EObjectType.Thrown)
                            typeMsg = "throw";

                        string targetMsg = "";
                        if (m_startAttackTarget != null)
                        {
                            if (player.IsWithinRadius(m_startAttackTarget, AttackRange))
                                targetMsg = LanguageMgr.GetTranslation(player.Client.Account.Language,
                                    "GamePlayer.StartAttack.TargetInRange");
                            else
                                targetMsg = LanguageMgr.GetTranslation(player.Client.Account.Language,
                                    "GamePlayer.StartAttack.TargetOutOfRange");
                        }

                        int speed = AttackSpeed(attackWeapon) / 100;
                        if (player.rangeAttackComponent.RangedAttackType == ERangedAttackType.RapidFire)
                            speed = Math.Max(15, speed / 2);

                        if (!player.effectListComponent.ContainsEffectForEffectType(EEffect.Volley))//volley check
                            player.Out.SendMessage(
                            LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.StartAttack.YouPrepare",
                                typeMsg, speed / 10, speed % 10, targetMsg), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    }
                }
            }
            else if (owner is GameNPC && m_startAttackTarget != null)
                NpcStartAttack(m_startAttackTarget);
            else
                LivingStartAttack();
        }

        private bool LivingStartAttack()
        {
            if (owner.IsIncapacitated)
                return false;

            if (owner.IsEngaging)
                owner.CancelEngageEffect();

            AttackState = true;
            InventoryItem attackWeapon = owner.ActiveWeapon;

            int speed = AttackSpeed(attackWeapon);

            if (speed <= 0)
                return false;

            // NPCs aren't allowed to prepare their ranged attack while moving or out of range.
            if (owner is not GamePlayer && owner.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
            {
                if (owner.IsMoving || !owner.IsWithinRadius(owner.TargetObject, owner.attackComponent.AttackRange))
                    return false;
            }

            attackAction = owner.CreateAttackAction();

            if (owner.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
            {
                // Only start another attack action if we aren't already aiming to shoot.
                if (owner.rangeAttackComponent.RangedAttackState != ERangedAttackState.Aim)
                {
                    if (attackAction.CheckInterruptTimer())
                        return false;

                    owner.rangeAttackComponent.RangedAttackState = ERangedAttackState.Aim;

                    if (owner is not GamePlayer || !owner.effectListComponent.ContainsEffectForEffectType(EEffect.Volley))
                    {
                        // The 'stance' parameter appears to be used to tell whether or not the animation should be held, and doesn't seem to be related to the weapon speed.
                        foreach (GamePlayer player in owner.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                            player.Out.SendCombatAnimation(owner, null, (ushort)(attackWeapon != null ? attackWeapon.Model : 0), 0, player.Out.BowPrepare, 0x1A, 0x00, 0x00);
                    }

                    attackAction.StartTime = owner.rangeAttackComponent?.RangedAttackType == ERangedAttackType.RapidFire ? Math.Max(1500, speed / 2) : speed;
                }
            }

            return true;
        }

        private void NpcStartAttack(GameObject attackTarget)
        {
            GameNPC npc = owner as GameNPC;
            npc.TargetObject = attackTarget;
            npc.StopMovingOnPath();

            if (npc.Brain != null && npc.Brain is IControlledBrain)
            {
                if ((npc.Brain as IControlledBrain).AggressionState == EAggressionState.Passive)
                    return;
            }

            LivingStartAttack();

            if (AttackState)
            {
                // Archer mobs sometimes bug and keep trying to fire at max range unsuccessfully so force them to get just a tad closer.
                if (npc.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                    npc.Follow(attackTarget, AttackRange - 30, GameNPC.STICK_MAXIMUM_RANGE);
                else
                    npc.Follow(attackTarget, GameNPC.STICK_MINIMUM_RANGE, GameNPC.STICK_MAXIMUM_RANGE);
            }
        }

        public void StopAttack()
        {
            if (owner.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
            {
                // Only cancel the animation if the ranged ammo isn't released already.
                if (AttackState && weaponAction?.AttackFinished != true)
                {
                    foreach (GamePlayer player in owner.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                        player.Out.SendInterruptAnimation(owner);
                }

                owner.rangeAttackComponent.RangedAttackState = ERangedAttackState.None;
                owner.rangeAttackComponent.RangedAttackType = ERangedAttackType.Normal;
            }

            AttackState = false;
            owner.CancelEngageEffect();
            owner.styleComponent.NextCombatStyle = null;
            owner.styleComponent.NextCombatBackupStyle = null;

            if (owner is GamePlayer playerOwner && playerOwner.IsAlive)
                playerOwner.Out.SendAttackMode(AttackState);
            else if (owner is GameNPC npcOwner && npcOwner.Inventory?.GetItem(eInventorySlot.DistanceWeapon) != null && npcOwner.ActiveWeaponSlot != EActiveWeaponSlot.Distance)
                npcOwner.SwitchWeapon(EActiveWeaponSlot.Distance);
        }

        /// <summary>
        /// Called whenever a single attack strike is made
        /// </summary>
        public AttackData MakeAttack(WeaponAction action, GameObject target, InventoryItem weapon, Style style, double effectiveness, int interruptDuration, bool dualWield)
        {
            if (owner is GamePlayer playerOwner)
            {
                if (playerOwner.IsCrafting)
                {
                    playerOwner.Out.SendMessage(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GamePlayer.Attack.InterruptedCrafting"), EChatType.CT_System, EChatLoc.CL_SystemWindow);
                    playerOwner.craftComponent.StopCraft();
                    playerOwner.CraftTimer = null;
                    playerOwner.Out.SendCloseTimerWindow();
                }

                if (playerOwner.IsSalvagingOrRepairing)
                {
                    playerOwner.Out.SendMessage(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GamePlayer.Attack.InterruptedCrafting"), EChatType.CT_System, EChatLoc.CL_SystemWindow);
                    playerOwner.CraftTimer.Stop();
                    playerOwner.CraftTimer = null;
                    playerOwner.Out.SendCloseTimerWindow();
                }

                AttackData ad = LivingMakeAttack(action, target, weapon, style, effectiveness * playerOwner.Effectiveness, interruptDuration, dualWield);

                switch (ad.AttackResult)
                {
                    case EAttackResult.HitStyle:
                    case EAttackResult.HitUnstyled:
                    {
                        // Keep component.
                        if ((ad.Target is GameKeepComponent || ad.Target is GameKeepDoor || ad.Target is GameSiegeWeapon) &&
                            ad.Attacker is GamePlayer && ad.Attacker.GetModified(EProperty.KeepDamage) > 0)
                        {
                            int keepdamage = (int) Math.Floor(ad.Damage * ((double) ad.Attacker.GetModified(EProperty.KeepDamage) / 100));
                            int keepstyle = (int) Math.Floor(ad.StyleDamage * ((double) ad.Attacker.GetModified(EProperty.KeepDamage) / 100));
                            ad.Damage += keepdamage;
                            ad.StyleDamage += keepstyle;
                        }

                        // Vampiir.
                        if (playerOwner.CharacterClass is PlayerClass.ClassVampiir &&
                            target is not GameKeepComponent and not GameKeepDoor and not GameSiegeWeapon)
                        {
                            int perc = Convert.ToInt32((double) (ad.Damage + ad.CriticalDamage) / 100 * (55 - playerOwner.Level));
                            perc = (perc < 1) ? 1 : ((perc > 15) ? 15 : perc);
                            playerOwner.Mana += Convert.ToInt32(Math.Ceiling((decimal) (perc * playerOwner.MaxMana) / 100));
                        }

                        break;
                    }
                }

                switch (ad.AttackResult)
                {
                    case EAttackResult.Blocked:
                    case EAttackResult.Fumbled:
                    case EAttackResult.HitStyle:
                    case EAttackResult.HitUnstyled:
                    case EAttackResult.Missed:
                    case EAttackResult.Parried:
                    {
                        // Condition percent can reach 70%.
                        // Durability percent can reach 0%.

                        if (weapon is GameInventoryItem weaponItem)
                            weaponItem.OnStrikeTarget(playerOwner, target);

                        // Camouflage will be disabled only when attacking a GamePlayer or ControlledNPC of a GamePlayer.
                        if ((target is GamePlayer && playerOwner.HasAbility(Abilities.Camouflage)) ||
                            (target is GameNPC targetNpc && targetNpc.Brain is IControlledBrain targetNpcBrain && targetNpcBrain.GetPlayerOwner() != null))
                        {
                            CamouflageEcsEffect camouflage = (CamouflageEcsEffect) EffectListService.GetAbilityEffectOnTarget(playerOwner, EEffect.Camouflage);

                            if (camouflage != null)
                                EffectService.RequestImmediateCancelEffect(camouflage, false);

                            playerOwner.DisableSkill(SkillBase.GetAbility(Abilities.Camouflage), CamouflageHandler.DISABLE_DURATION);
                        }

                        // Multiple Hit check.
                        if (ad.AttackResult == EAttackResult.HitStyle)
                        {
                            int numTargetsCanHit;
                            int index;
                            List<GameObject> extraTargets = new();
                            List<GameObject> listAvailableTargets = new();
                            InventoryItem attackWeapon = owner.ActiveWeapon;
                            InventoryItem leftWeapon = playerOwner.Inventory?.GetItem(eInventorySlot.LeftHandWeapon);

                            switch (style.ID)
                            {
                                case 374:
                                    numTargetsCanHit = 1;
                                    break; // Tribal Assault: Hits 2 targets.
                                case 377:
                                    numTargetsCanHit = 1;
                                    break; // Clan's Might: Hits 2 targets.
                                case 379:
                                    numTargetsCanHit = 2;
                                    break; // Totemic Wrath: Hits 3 targets.
                                case 384:
                                    numTargetsCanHit = 3;
                                    break; // Totemic Sacrifice: Hits 4 targets.
                                case 600:
                                    numTargetsCanHit = 255;
                                    break; // Shield Swipe: No cap.
                                default:
                                    numTargetsCanHit = 0;
                                    break;
                            }

                            if (numTargetsCanHit > 0)
                            {
                                if (style.ID != 600) // Not Shield Swipe.
                                {
                                    foreach (GamePlayer pl in playerOwner.GetPlayersInRadius((ushort) AttackRange))
                                    {
                                        if (GameServer.ServerRules.IsAllowedToAttack(playerOwner, pl, true))
                                            listAvailableTargets.Add(pl);
                                    }

                                    foreach (GameNPC npc in playerOwner.GetNPCsInRadius((ushort) AttackRange))
                                    {
                                        if (GameServer.ServerRules.IsAllowedToAttack(playerOwner, npc, true))
                                            listAvailableTargets.Add(npc);
                                    }

                                    // Remove primary target.
                                    numTargetsCanHit = Math.Min(numTargetsCanHit, listAvailableTargets.Count);

                                    if (listAvailableTargets.Count > 0)
                                    {
                                        while (extraTargets.Count < numTargetsCanHit)
                                        {
                                            index = UtilCollection.Random(listAvailableTargets.Count - 1);
                                            GameObject availableTarget = listAvailableTargets[index];

                                            if (target != availableTarget && !extraTargets.Contains(availableTarget))
                                                extraTargets.Add(availableTarget);

                                            listAvailableTargets.RemoveAt(index);
                                        }

                                        foreach (GameObject obj in extraTargets)
                                        {
                                            if (obj is GamePlayer player && player.IsSitting)
                                                effectiveness *= 2;

                                            weaponAction = new WeaponAction(playerOwner, obj, attackWeapon, leftWeapon, effectiveness, AttackSpeed(attackWeapon), null);
                                            weaponAction.Execute();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (GameNPC npc in playerOwner.GetNPCsInRadius((ushort) AttackRange))
                                    {
                                        if (GameServer.ServerRules.IsAllowedToAttack(playerOwner, npc, true))
                                            listAvailableTargets.Add(npc);
                                    }

                                    numTargetsCanHit = Math.Min(numTargetsCanHit, listAvailableTargets.Count);

                                    if (listAvailableTargets.Count > 1)
                                    {
                                        while (extraTargets.Count < numTargetsCanHit)
                                        {
                                            index = UtilCollection.Random(listAvailableTargets.Count - 1);
                                            GameObject availableTarget = listAvailableTargets[index];

                                            if (target != availableTarget && !extraTargets.Contains(availableTarget))
                                                extraTargets.Add(availableTarget);

                                            listAvailableTargets.RemoveAt(index);
                                        }

                                        foreach (GameNPC obj in extraTargets)
                                        {
                                            if (obj != ad.Target)
                                                LivingMakeAttack(action, obj, attackWeapon, null, 1, ServerProperties.ServerProperties.SPELL_INTERRUPT_DURATION, false);
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                }

                return ad;
            }
            else
            {
                if (owner is NecromancerPet necromancerPet)
                    ((NecroPetBrain)necromancerPet.Brain).CheckAttackSpellQueue();
                else
                    effectiveness = 1;

                return LivingMakeAttack(action, target, weapon, style, effectiveness, interruptDuration, dualWield);
            }
        }

        /// <summary>
        /// This method is called to make an attack, it is called from the
        /// attacktimer and should not be called manually
        /// </summary>
        /// <returns>the object where we collect and modifiy all parameters about the attack</returns>
        public AttackData LivingMakeAttack(WeaponAction action, GameObject target, InventoryItem weapon, Style style, double effectiveness,
            int interruptDuration, bool dualWield, bool ignoreLOS = false)
        {
            AttackData ad = new AttackData();
            ad.Attacker = owner;
            ad.Target = target as GameLiving;
            ad.Damage = 0;
            ad.CriticalDamage = 0;
            ad.Style = style;
            ad.WeaponSpeed = AttackSpeed(weapon) / 100;
            ad.DamageType = AttackDamageType(weapon);
            ad.ArmorHitLocation = EArmorSlot.NOTSET;
            ad.Weapon = weapon;
            ad.IsOffHand = weapon != null && weapon.SlotPosition == Slot.LEFTHAND;

            // Asp style range add.
            IEnumerable<(Spell, int, int)> rangeProc = style?.Procs.Where(x => x.Item1.SpellType == ESpellType.StyleRange);
            int addRange = rangeProc?.Any() == true ? (int) (rangeProc.First().Item1.Value - AttackRange) : 0;

            if (dualWield && (ad.Attacker is GamePlayer gPlayer) && gPlayer.CharacterClass.ID != (int) ECharacterClass.Savage)
                ad.AttackType = AttackData.EAttackType.MeleeDualWield;
            else if (weapon == null)
                ad.AttackType = AttackData.EAttackType.MeleeOneHand;
            else
            {
                ad.AttackType = weapon.SlotPosition switch
                {
                    Slot.TWOHAND => AttackData.EAttackType.MeleeTwoHand,
                    Slot.RANGED => AttackData.EAttackType.Ranged,
                    _ => AttackData.EAttackType.MeleeOneHand,
                };
            }

            // No target.
            if (ad.Target == null)
            {
                ad.AttackResult = (target == null) ? EAttackResult.NoTarget : EAttackResult.NoValidTarget;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            // Region / state check.
            if (ad.Target.CurrentRegionID != owner.CurrentRegionID || ad.Target.ObjectState != GameObject.eObjectState.Active)
            {
                ad.AttackResult = EAttackResult.NoValidTarget;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            // LoS / in front check.
            if (!ignoreLOS && ad.AttackType != AttackData.EAttackType.Ranged && owner is GamePlayer &&
                !(ad.Target is GameKeepComponent) &&
                !(owner.IsObjectInFront(ad.Target, 120) && owner.TargetInView))
            {
                ad.AttackResult = EAttackResult.TargetNotVisible;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            // Target is already dead.
            if (!ad.Target.IsAlive)
            {
                ad.AttackResult = EAttackResult.TargetDead;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            // Melee range check (ranged is already done at this point).
            if (ad.AttackType != AttackData.EAttackType.Ranged)
            {
                if (!owner.IsWithinRadius(ad.Target, AttackRange + addRange))
                {
                    ad.AttackResult = EAttackResult.OutOfRange;
                    SendAttackingCombatMessages(action, ad);
                    return ad;
                }
            }

            if (!GameServer.ServerRules.IsAllowedToAttack(ad.Attacker, ad.Target, attackAction != null && GameLoop.GameLoopTime - attackAction.RoundWithNoAttackTime <= 1500))
            {
                ad.AttackResult = EAttackResult.NotAllowed_ServerRules;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            if (ad.Target.IsSitting)
                effectiveness *= 2;

            // Apply Mentalist RA5L.
            NfRaSelectiveBlindnessEffect selectiveBlindness = owner.EffectList.GetOfType<NfRaSelectiveBlindnessEffect>();
            if (selectiveBlindness != null)
            {
                GameLiving EffectOwner = selectiveBlindness.EffectSource;
                if (EffectOwner == ad.Target)
                {
                    if (owner is GamePlayer)
                        ((GamePlayer) owner).Out.SendMessage(
                            string.Format(
                                LanguageMgr.GetTranslation(((GamePlayer) owner).Client.Account.Language,
                                    "GameLiving.AttackData.InvisibleToYou"), ad.Target.GetName(0, true)),
                            EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                    ad.AttackResult = EAttackResult.NoValidTarget;
                    SendAttackingCombatMessages(action, ad);
                    return ad;
                }
            }

            // DamageImmunity Ability.
            if ((GameLiving) target != null && ((GameLiving) target).HasAbility(Abilities.DamageImmunity))
            {
                //if (ad.Attacker is GamePlayer) ((GamePlayer)ad.Attacker).Out.SendMessage(string.Format("{0} can't be attacked!", ad.Target.GetName(0, true)), eChatType.CT_Missed, eChatLoc.CL_SystemWindow);
                ad.AttackResult = EAttackResult.NoValidTarget;
                SendAttackingCombatMessages(action, ad);
                return ad;
            }

            // Add ourself to the target's attackers list. Should be done before any enemy reaction for accurate calculation.
            ad.Target.attackComponent.AddAttacker(owner);

            // Calculate our attack result and attack damage.
            ad.AttackResult = ad.Target.attackComponent.CalculateEnemyAttackResult(action, ad, weapon);

            // Strafing miss.
            if (owner is GamePlayer playerOwner && playerOwner.IsStrafing && ad.Target is GamePlayer && UtilCollection.Chance(30))
            {
                // Used to tell the difference between a normal miss and a strafing miss.
                // Ugly, but we shouldn't add a new field to 'AttackData' just for that purpose.
                ad.MissRate = 0;
                ad.AttackResult = EAttackResult.Missed;
            }

            // Calculate damage only if we hit the target.
            if (ad.AttackResult is EAttackResult.HitUnstyled or EAttackResult.HitStyle)
            {
                double damage = AttackDamage(weapon) * effectiveness;
                InventoryItem armor = null;

                if (ad.Target.Inventory != null)
                    armor = ad.Target.Inventory.GetItem((eInventorySlot) ad.ArmorHitLocation);

                InventoryItem weaponForSpecModifier = null;

                if (weapon != null)
                {
                    weaponForSpecModifier = new InventoryItem();
                    weaponForSpecModifier.Object_Type = weapon.Object_Type;
                    weaponForSpecModifier.SlotPosition = weapon.SlotPosition;

                    if (owner is GamePlayer && owner.Realm == ERealm.Albion && ServerProperties.ServerProperties.ENABLE_ALBION_ADVANCED_WEAPON_SPEC &&
                        (GameServer.ServerRules.IsObjectTypesEqual((EObjectType) weapon.Object_Type, EObjectType.TwoHandedWeapon) ||
                        GameServer.ServerRules.IsObjectTypesEqual((EObjectType) weapon.Object_Type, EObjectType.PolearmWeapon)))
                    {
                        // Albion dual spec penalty, which sets minimum damage to the base damage spec.
                        if (weapon.Type_Damage == (int) EDamageType.Crush)
                            weaponForSpecModifier.Object_Type = (int) EObjectType.CrushingWeapon;
                        else if (weapon.Type_Damage == (int) EDamageType.Slash)
                            weaponForSpecModifier.Object_Type = (int) EObjectType.SlashingWeapon;
                        else
                            weaponForSpecModifier.Object_Type = (int) EObjectType.ThrustWeapon;
                    }
                }

                double specModifier = CalculateSpecModifier(ad.Target, weaponForSpecModifier);
                double modifiedWeaponSkill = CalculateModifiedWeaponSkill(ad.Target, weapon, specModifier);
                double armorMod = CalculateTargetArmor(ad.Target, ad.ArmorHitLocation);
                double damageMod = Math.Min(3.0, modifiedWeaponSkill / armorMod);

                if (owner is GamePlayer playerOwner2)
                {
                    damage *= damageMod;

                    if (playerOwner2.UseDetailedCombatLog)
                    {
                        playerOwner2.Out.SendMessage($"Damage Modifier: {(int) (damageMod * 1000)}",
                            EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                    }

                    if (ad.Target is GamePlayer attackee && attackee.UseDetailedCombatLog)
                    {
                        attackee.Out.SendMessage($"Damage Modifier: {(int) (damageMod * 1000)}", EChatType.CT_DamageAdd,
                            EChatLoc.CL_SystemWindow);
                    }

                    // Badge Of Valor Calculation 1+ absorb or 1- absorb
                    // if (ad.Attacker.EffectList.GetOfType<BadgeOfValorEffect>() != null)
                    //     damage *= 1.0 + Math.Min(0.85, ad.Target.GetArmorAbsorb(ad.ArmorHitLocation));
                    // else
                    //     damage *= 1.0 - Math.Min(0.85, ad.Target.GetArmorAbsorb(ad.ArmorHitLocation));
                }
                else
                {
                    if (owner is GameEpicBoss boss)
                        damage *= damageMod + boss.Strength / 200;
                    else
                        damage *= damageMod;

                    if (ad.Target is GamePlayer attackee && attackee.UseDetailedCombatLog)
                        attackee.Out.SendMessage($"NPC Damage Modifier: {(int) (damageMod * 1000)}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                    // Badge Of Valor Calculation 1+ absorb or 1- absorb
                    // if (ad.Attacker.EffectList.GetOfType<BadgeOfValorEffect>() != null)
                    //     damage *= 1.0 + Math.Min(0.85, ad.Target.GetArmorAbsorb(ad.ArmorHitLocation));
                    // else
                    //     damage *= 1.0 - Math.Min(0.85, ad.Target.GetArmorAbsorb(ad.ArmorHitLocation));
                }

                if (ad.IsOffHand)
                    damage *= 1 + owner.GetModified(EProperty.OffhandDamage) * 0.01;

                // Against NPC targets this just doubles the resists. Applying only to player targets as a fix.
                if (ad.Target is GamePlayer)
                    ad.Modifier = (int) (damage * (ad.Target.GetResist(ad.DamageType) + SkillBase.GetArmorResist(armor, ad.DamageType)) * -0.01);

                // RA resist check.
                int resist = (int) (damage * ad.Target.GetDamageResist(owner.GetResistTypeForDamage(ad.DamageType)) * -0.01);
                EProperty property = ad.Target.GetResistTypeForDamage(ad.DamageType);
                int secondaryResistModifier = ad.Target.SpecBuffBonusCategory[(int) property];
                int resistModifier = 0;
                resistModifier += (int) ((ad.Damage + (double) resist) * secondaryResistModifier * -0.01);
                damage += resist;
                damage += resistModifier;
                ad.Modifier += resist;
                damage += ad.Modifier;
                ad.Damage = (int) damage;

                if (action.RangedAttackType == ERangedAttackType.Critical)
                    ad.Damage = Math.Min(ad.Damage, (int) (UnstyledDamageCap(weapon) * 2));
                else
                    ad.Damage = Math.Min(ad.Damage, (int) (UnstyledDamageCap(weapon) /* * effectiveness*/));

                // If the target is another player's pet, shouldn't 'PVP_MELEE_DAMAGE' be used?
                if (owner is GamePlayer || (owner is GameNPC npcOwner && npcOwner.Brain is IControlledBrain && owner.Realm != 0))
                {
                    if (target is GamePlayer)
                        ad.Damage = (int) (ad.Damage * ServerProperties.ServerProperties.PVP_MELEE_DAMAGE);
                    else if (target is GameNPC)
                        ad.Damage = (int) (ad.Damage * ServerProperties.ServerProperties.PVE_MELEE_DAMAGE);
                }

                // Conversion.
                if (ad.Target is GamePlayer playerTarget && ad.Target.GetModified(EProperty.Conversion) > 0)
                {
                    int manaconversion = (int) Math.Round((ad.Damage + ad.CriticalDamage) * ad.Target.GetModified(EProperty.Conversion) / 100.0);
                    int enduconversion = (int) Math.Round((ad.Damage + ad.CriticalDamage) * ad.Target.GetModified(EProperty.Conversion) / 100.0);

                    if (ad.Target.Mana + manaconversion > ad.Target.MaxMana)
                        manaconversion = ad.Target.MaxMana - ad.Target.Mana;

                    if (ad.Target.Endurance + enduconversion > ad.Target.MaxEndurance)
                        enduconversion = ad.Target.MaxEndurance - ad.Target.Endurance;

                    if (manaconversion < 1)
                        manaconversion = 0;

                    if (enduconversion < 1)
                        enduconversion = 0;

                    if (manaconversion >= 1)
                        playerTarget.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(playerTarget.Client.Account.Language, "GameLiving.AttackData.GainPowerPoints"), manaconversion), EChatType.CT_Spell, EChatLoc.CL_SystemWindow);

                    if (enduconversion >= 1)
                        playerTarget.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(playerTarget.Client.Account.Language, "GameLiving.AttackData.GainEndurancePoints"), enduconversion), EChatType.CT_Spell, EChatLoc.CL_SystemWindow);

                    ad.Target.Endurance += enduconversion;

                    if (ad.Target.Endurance > ad.Target.MaxEndurance)
                        ad.Target.Endurance = ad.Target.MaxEndurance;

                    ad.Target.Mana += manaconversion;

                    if (ad.Target.Mana > ad.Target.MaxMana)
                        ad.Target.Mana = ad.Target.MaxMana;
                }

                if (ad.Damage == 0)
                    ad.Damage = 1;
            }

            // Add styled damage if style hits and remove endurance if missed.
            if (StyleProcessor.ExecuteStyle(owner, ad, weapon))
                ad.AttackResult = EAttackResult.HitStyle;

            if (ad.AttackResult is EAttackResult.HitUnstyled or EAttackResult.HitStyle)
                ad.CriticalDamage = GetMeleeCriticalDamage(ad, action, weapon);

            // Attacked living may modify the attack data. Primarily used for keep doors and components.
            ad.Target.ModifyAttack(ad);

            string message = "";
            bool broadcast = true;

            ArrayList excludes = new()
            {
                ad.Attacker,
                ad.Target
            };

            switch (ad.AttackResult)
            {
                case EAttackResult.Parried:
                    message = string.Format("{0} attacks {1} and is parried!", ad.Attacker.GetName(0, true), ad.Target.GetName(0, false));
                    break;
                case EAttackResult.Evaded:
                    message = string.Format("{0} attacks {1} and is evaded!", ad.Attacker.GetName(0, true), ad.Target.GetName(0, false));
                    break;
                case EAttackResult.Fumbled:
                    message = string.Format("{0} fumbled!", ad.Attacker.GetName(0, true), ad.Target.GetName(0, false));
                    break;
                case EAttackResult.Missed:
                    message = string.Format("{0} attacks {1} and misses!", ad.Attacker.GetName(0, true), ad.Target.GetName(0, false));
                    break;
                case EAttackResult.Blocked:
                {
                    message = string.Format("{0} attacks {1} and is blocked!", ad.Attacker.GetName(0, true),
                        ad.Target.GetName(0, false));
                    // guard messages
                    if (target != null && target != ad.Target)
                    {
                        excludes.Add(target);

                        // another player blocked for real target
                        if (target is GamePlayer)
                            ((GamePlayer) target).Out.SendMessage(
                                string.Format(
                                    LanguageMgr.GetTranslation(((GamePlayer) target).Client.Account.Language,
                                        "GameLiving.AttackData.BlocksYou"), ad.Target.GetName(0, true),
                                    ad.Attacker.GetName(0, false)), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);

                        // blocked for another player
                        if (ad.Target is GamePlayer)
                        {
                            ((GamePlayer) ad.Target).Out.SendMessage(
                                string.Format(
                                    LanguageMgr.GetTranslation(((GamePlayer) ad.Target).Client.Account.Language,
                                        "GameLiving.AttackData.YouBlock") +
                                        $" ({ad.BlockChance:0.0}%)", ad.Attacker.GetName(0, false),
                                    target.GetName(0, false)), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                            ((GamePlayer) ad.Target).Stealth(false);
                        }
                    }
                    else if (ad.Target is GamePlayer)
                    {
                        ((GamePlayer) ad.Target).Out.SendMessage(
                            string.Format(
                                LanguageMgr.GetTranslation(((GamePlayer) ad.Target).Client.Account.Language,
                                    "GameLiving.AttackData.AttacksYou") +
                                    $" ({ad.BlockChance:0.0}%)", ad.Attacker.GetName(0, true)),
                            EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                    }

                    break;
                }
                case EAttackResult.HitUnstyled:
                case EAttackResult.HitStyle:
                {
                    if (ad.AttackResult == EAttackResult.HitStyle)
                    {
                        if (owner is GamePlayer)
                        {
                            GamePlayer player = owner as GamePlayer;

                            string damageAmount = (ad.StyleDamage > 0)
                                ? " (+" + ad.StyleDamage + ", GR: " + ad.Style.GrowthRate + ")"
                                : "";
                            player.Out.SendMessage(
                                LanguageMgr.GetTranslation(player.Client.Account.Language,
                                    "StyleProcessor.ExecuteStyle.PerformPerfectly", ad.Style.Name, damageAmount),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        }
                        else if (owner is GameNPC)
                        {
                            ControlledNpcBrain brain = ((GameNPC) owner).Brain as ControlledNpcBrain;

                            if (brain != null)
                            {
                                GamePlayer player = brain.GetPlayerOwner();
                                if (player != null)
                                {
                                    string damageAmount = (ad.StyleDamage > 0)
                                        ? " (+" + ad.StyleDamage + ", GR: " + ad.Style.GrowthRate + ")"
                                        : "";
                                    player.Out.SendMessage(
                                        LanguageMgr.GetTranslation(player.Client.Account.Language,
                                            "StyleProcessor.ExecuteStyle.PerformsPerfectly", owner.Name, ad.Style.Name,
                                            damageAmount), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                                }
                            }
                        }
                    }
                    
                    if (target != null && target != ad.Target)
                    {
                        message = string.Format("{0} attacks {1} but hits {2}!", ad.Attacker.GetName(0, true),
                            target.GetName(0, false), ad.Target.GetName(0, false));
                        excludes.Add(target);

                        // intercept for another player
                        if (target is GamePlayer)
                            ((GamePlayer) target).Out.SendMessage(
                                string.Format(
                                    LanguageMgr.GetTranslation(((GamePlayer) target).Client.Account.Language,
                                        "GameLiving.AttackData.StepsInFront"), ad.Target.GetName(0, true)),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);

                        // intercept by player
                        if (ad.Target is GamePlayer)
                            ((GamePlayer) ad.Target).Out.SendMessage(
                                string.Format(
                                    LanguageMgr.GetTranslation(((GamePlayer) ad.Target).Client.Account.Language,
                                        "GameLiving.AttackData.YouStepInFront"), target.GetName(0, false)),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                    }
                    else
                    {
                        if (ad.Attacker is GamePlayer)
                        {
                            string hitWeapon = "weapon";
                            if (weapon != null)
                                hitWeapon = GlobalConstants.NameToShortName(weapon.Name);
                            message = string.Format("{0} attacks {1} with {2} {3}!", ad.Attacker.GetName(0, true),
                                ad.Target.GetName(0, false), ad.Attacker.GetPronoun(1, false), hitWeapon);
                        }
                        else
                        {
                            message = string.Format("{0} attacks {1} and hits!", ad.Attacker.GetName(0, true),
                                ad.Target.GetName(0, false));
                        }
                    }

                    break;
                }
                default:
                    broadcast = false;
                    break;
            }

            SendAttackingCombatMessages(action, ad);

            #region Prevent Flight

            if (ad.Attacker is GamePlayer)
            {
                GamePlayer attacker = ad.Attacker as GamePlayer;
                if (attacker.HasAbilityType(typeof(OfRaPreventFlightHandler)) && UtilCollection.Chance(35))
                {
                    if (owner.IsObjectInFront(ad.Target, 120) && ad.Target.IsMoving)
                    {
                        bool preCheck = false;
                        float angle = ad.Target.GetAngle(ad.Attacker);
                        if (angle >= 150 && angle < 210) preCheck = true;

                        if (preCheck)
                        {
                            Spell spell = SkillBase.GetSpellByID(7083);
                            if (spell != null)
                            {
                                ISpellHandler spellHandler = ScriptMgr.CreateSpellHandler(owner, spell,
                                    SkillBase.GetSpellLine(GlobalSpellsLines.Reserved_Spells));
                                if (spellHandler != null)
                                {
                                    spellHandler.StartSpell(ad.Target);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region controlled messages

            if (ad.Attacker is GameNPC)
            {
                IControlledBrain brain = ((GameNPC) ad.Attacker).Brain as IControlledBrain;

                if (brain != null)
                {
                    GamePlayer owner = brain.GetPlayerOwner();

                    if (owner != null)
                    {
                        excludes.Add(owner);

                        switch (ad.AttackResult)
                        {
                            case EAttackResult.HitStyle:
                            case EAttackResult.HitUnstyled:
                            {
                                string modmessage = "";

                                if (ad.Modifier > 0)
                                    modmessage = $" (+{ad.Modifier})";
                                else if (ad.Modifier < 0)
                                    modmessage = $" ({ad.Modifier})";

                                string attackTypeMsg;

                                if (action.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                                    attackTypeMsg = "shoots";
                                else
                                    attackTypeMsg = "attacks";

                                owner.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(owner.Client.Account.Language, "GameLiving.AttackData.YourHits"),
                                    ad.Attacker.Name, attackTypeMsg, ad.Target.GetName(0, false), ad.Damage, modmessage),
                                    EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);

                                if (ad.CriticalDamage > 0)
                                {
                                    owner.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(owner.Client.Account.Language, "GameLiving.AttackData.YourCriticallyHits"),
                                        ad.Attacker.Name, ad.Target.GetName(0, false), ad.CriticalDamage) + $" ({AttackCriticalChance(action, ad.Weapon)}%)",
                                        EChatType.CT_YouHit,EChatLoc.CL_SystemWindow);
                                }

                                break;
                            }
                            case EAttackResult.Missed:
                            {
                                owner.Out.SendMessage(message + $" ({ad.MissRate}%)", EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                                break;
                            }
                            default:
                                owner.Out.SendMessage(message, EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                                break;
                        }
                    }
                }
            }

            if (ad.Target is GameNPC)
            {
                IControlledBrain brain = ((GameNPC) ad.Target).Brain as IControlledBrain;
                if (brain != null)
                {
                    GameLiving owner_living = brain.GetLivingOwner();
                    excludes.Add(owner_living);
                    if (owner_living != null && owner_living is GamePlayer && owner_living.ControlledBrain != null &&
                        ad.Target == owner_living.ControlledBrain.Body)
                    {
                        GamePlayer owner = owner_living as GamePlayer;
                        switch (ad.AttackResult)
                        {
                            case EAttackResult.Blocked:
                                owner.Out.SendMessage(
                                    string.Format(
                                        LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                            "GameLiving.AttackData.Blocked"), ad.Attacker.GetName(0, true),
                                        ad.Target.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                                break;
                            case EAttackResult.Parried:
                                owner.Out.SendMessage(
                                    string.Format(
                                        LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                            "GameLiving.AttackData.Parried"), ad.Attacker.GetName(0, true),
                                        ad.Target.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                                break;
                            case EAttackResult.Evaded:
                                owner.Out.SendMessage(
                                    string.Format(
                                        LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                            "GameLiving.AttackData.Evaded"), ad.Attacker.GetName(0, true),
                                        ad.Target.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                                break;
                            case EAttackResult.Fumbled:
                                owner.Out.SendMessage(
                                    string.Format(
                                        LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                            "GameLiving.AttackData.Fumbled"), ad.Attacker.GetName(0, true)),
                                    EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                                break;
                            case EAttackResult.Missed:
                                if (ad.AttackType != AttackData.EAttackType.Spell)
                                    owner.Out.SendMessage(
                                        string.Format(
                                            LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                                "GameLiving.AttackData.Misses"), ad.Attacker.GetName(0, true),
                                            ad.Target.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                                break;
                            case EAttackResult.HitStyle:
                            case EAttackResult.HitUnstyled:
                            {
                                string modmessage = "";
                                if (ad.Modifier > 0) modmessage = " (+" + ad.Modifier + ")";
                                if (ad.Modifier < 0) modmessage = " (" + ad.Modifier + ")";
                                owner.Out.SendMessage(
                                    string.Format(
                                        LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                            "GameLiving.AttackData.HitsForDamage"), ad.Attacker.GetName(0, true),
                                        ad.Target.Name, ad.Damage, modmessage), EChatType.CT_Damaged,
                                    EChatLoc.CL_SystemWindow);
                                if (ad.CriticalDamage > 0)
                                {
                                    owner.Out.SendMessage(
                                        string.Format(
                                            LanguageMgr.GetTranslation(owner.Client.Account.Language,
                                                "GameLiving.AttackData.CriticallyHitsForDamage"),
                                            ad.Attacker.GetName(0, true), ad.Target.Name, ad.CriticalDamage),
                                        EChatType.CT_Damaged, EChatLoc.CL_SystemWindow);
                                }

                                break;
                            }
                            default: break;
                        }
                    }
                }
            }

            #endregion

            // broadcast messages
            if (broadcast)
            {
                Message.SystemToArea(ad.Attacker, message, EChatType.CT_OthersCombat,
                    (GameObject[]) excludes.ToArray(typeof(GameObject)));
            }

            // Interrupt the target of the attack
            ad.Target.StartInterruptTimer(interruptDuration, ad.AttackType, ad.Attacker);

            // If we're attacking via melee, start an interrupt timer on ourselves so we cannot swing + immediately cast.
            if (ad.AttackType != AttackData.EAttackType.Spell && ad.AttackType != AttackData.EAttackType.Ranged && owner.StartInterruptTimerOnItselfOnMeleeAttack())
                owner.StartInterruptTimer(owner.SpellInterruptDuration, ad.AttackType, ad.Attacker);

            owner.OnAttackEnemy(ad);

            //Return the result
            return ad;
        }

        public double CalculateModifiedWeaponSkill(GameLiving target, InventoryItem weapon, double specModifier)
        {
            return CalculateModifiedWeaponSkill(target, 1 + owner.GetWeaponSkill(weapon), 1 + RelicMgr.GetRelicBonusModifier(owner.Realm, eRelicType.Strength), specModifier);
        }

        public double CalculateModifiedWeaponSkill(GameLiving target, double weaponSkill, double relicBonus, double specModifier)
        {
            double modifiedWeaponSkill;

            if (owner is GamePlayer)
            {
                modifiedWeaponSkill = weaponSkill * relicBonus * specModifier;

                if (owner is GamePlayer weaponskiller && weaponskiller.UseDetailedCombatLog)
                {
                    weaponskiller.Out.SendMessage(
                        $"Base WS: {weaponSkill:0.00} | Calc WS: {modifiedWeaponSkill:0.00} | SpecMod: {specModifier:0.00}",
                        EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }

                if (target is GamePlayer attackee && attackee.UseDetailedCombatLog)
                {
                    attackee.Out.SendMessage(
                        $"Base WS: {weaponSkill:0.00} | Calc WS: {modifiedWeaponSkill:0.00} | SpecMod: {specModifier:0.00}",
                        EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }
            }
            else
            {
                modifiedWeaponSkill = weaponSkill + target.Level * 65 / 50.0;

                if (owner.Level < 10)
                    modifiedWeaponSkill *= 1 - 0.05 * (10 - owner.Level);
            }

            return modifiedWeaponSkill;
        }

        public double CalculateSpecModifier(GameLiving target, InventoryItem weapon)
        {
            double specModifier;

            if (owner is GamePlayer playerOwner)
            {
                int spec = owner.WeaponSpecLevel(weapon);

                if (owner.Level < 5 && spec < 2)
                    spec = 2;

                double lowerLimit = Math.Min(0.75 * (spec - 1) / (target.EffectiveLevel + 1) + 0.25, 1.0);

                if (lowerLimit < 0.01)
                    lowerLimit = 0.01;

                double upperLimit = Math.Min(Math.Max(1.25 + (3.0 * (spec - 1) / (target.EffectiveLevel + 1) - 2) * 0.25, 1.25), 1.50);
                int varianceRange = (int) (upperLimit * 100 - lowerLimit * 100);
                specModifier = playerOwner.SpecLock > 0 ? playerOwner.SpecLock : lowerLimit + UtilCollection.Random(varianceRange) * 0.01;
            }
            else
            {
                int minimun;
                int maximum;

                if (owner is GameEpicBoss)
                {
                    minimun = 95;
                    maximum = 105;
                }
                else
                {
                    minimun = 75;
                    maximum = 125;
                }

                specModifier = (UtilCollection.Random(maximum - minimun) + minimun) * 0.01;
            }

            return specModifier;
        }

        public double CalculateTargetArmor(GameLiving target, EArmorSlot armorSlot)
        {
            double armorMod;
            int AFLevelScalar = 25;

            if (owner is GamePlayer)
            {
                double baseAF = target is GamePlayer ? target.Level * AFLevelScalar / 50.0 : 2;

                if (baseAF < 1)
                    baseAF = 1;

                armorMod = (baseAF + target.GetArmorAF(armorSlot)) / (1 - target.GetArmorAbsorb(armorSlot));

                if (owner is GamePlayer weaponskiller && weaponskiller.UseDetailedCombatLog)
                {
                    weaponskiller.Out.SendMessage(
                        $"Base AF: {target.GetArmorAF(armorSlot) + baseAF:0.00} | ABS: {target.GetArmorAbsorb(armorSlot) * 100:0.00} | AF/ABS: {armorMod:0.00}",
                        EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }

                if (target is GamePlayer attackee && attackee.UseDetailedCombatLog)
                {
                    attackee.Out.SendMessage(
                        $"Base AF: {target.GetArmorAF(armorSlot) + baseAF:0.00} | ABS: {target.GetArmorAbsorb(armorSlot) * 100:0.00} | AF/ABS: {armorMod:0.00}",
                        EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }

                return armorMod;
            }
            else
            {
                if (target.Level < 21)
                    AFLevelScalar += 20 - target.Level;

                double baseAF = target.Level * AFLevelScalar / 50.0;
                armorMod = (baseAF + target.GetArmorAF(armorSlot)) / (1 - target.GetArmorAbsorb(armorSlot));
            }

            if (armorMod <= 0)
                armorMod = 0.1;

            return armorMod;
        }

        public virtual bool CheckBlock(AttackData ad, double attackerConLevel)
        {
            double blockChance = owner.TryBlock(ad, attackerConLevel, m_attackers.Count);
            ad.BlockChance = blockChance;
            double ranBlockNum = UtilCollection.CryptoNextDouble() * 10000;
            ranBlockNum = Math.Floor(ranBlockNum);
            ranBlockNum /= 100;
            blockChance *= 100;

            if (blockChance > 0)
            {
                double? blockDouble = (owner as GamePlayer)?.RandomNumberDeck.GetPseudoDouble();
                double? blockOutput = (blockDouble != null) ? Math.Round((double) (blockDouble * 100), 2) : ranBlockNum;

                if (ad.Attacker is GamePlayer blockAttk && blockAttk.UseDetailedCombatLog)
                    blockAttk.Out.SendMessage($"target block%: {Math.Round(blockChance, 2)} rand: {blockOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                if (ad.Target is GamePlayer blockTarg && blockTarg.UseDetailedCombatLog)
                    blockTarg.Out.SendMessage($"your block%: {Math.Round(blockChance, 2)} rand: {blockOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                if (blockDouble == null || ServerProperties.ServerProperties.OVERRIDE_DECK_RNG)
                {
                    if (blockChance > ranBlockNum)
                        return true;
                }
                else
                {
                    blockDouble *= 100;

                    if (blockChance > blockDouble)
                        return true;
                }
            }

            if (ad.AttackType is AttackData.EAttackType.Ranged or AttackData.EAttackType.Spell)
            {
                // Nature's shield, 100% block chance, 120° frontal angle.
                if (owner.IsObjectInFront(ad.Attacker, 120) && (owner.styleComponent.NextCombatStyle?.ID == 394 || owner.styleComponent.NextCombatBackupStyle?.ID == 394))
                {
                    ad.BlockChance = 1;
                    return true;
                }
            }

            return false;
        }

        public bool CheckGuard(AttackData ad, bool stealthStyle, double attackerConLevel)
        {
            GuardEcsEffect guard = EffectListService.GetAbilityEffectOnTarget(owner, EEffect.Guard) as GuardEcsEffect;

            if (guard?.GuardTarget != owner)
                return false;

            GameLiving guardSource = guard.GuardSource;

            if (guardSource == null ||
                guardSource.ObjectState != GameObject.eObjectState.Active ||
                guardSource.IsStunned != false ||
                guardSource.IsMezzed != false ||
                guardSource.ActiveWeaponSlot == EActiveWeaponSlot.Distance ||
                !guardSource.IsAlive ||
                guardSource.IsSitting ||
                stealthStyle ||
                !guard.GuardSource.IsWithinRadius(guard.GuardTarget, GuardHandler.GUARD_DISTANCE))
                return false;

            InventoryItem leftHand = guard.GuardSource.Inventory.GetItem(eInventorySlot.LeftHandWeapon);
            InventoryItem rightHand = guard.GuardSource.ActiveWeapon;

            if (((rightHand != null && rightHand.Hand == 1) || leftHand == null || leftHand.Object_Type != (int) EObjectType.Shield) && guard.GuardSource is not GameNPC)
                return false;

            // TODO: Insert actual formula for guarding here, this is just a guessed one based on block.
            int guardLevel = guard.GuardSource.GetAbilityLevel(Abilities.Guard);
            double guardchance;

            if (guard.GuardSource is GameNPC)
                guardchance = guard.GuardSource.GetModified(EProperty.BlockChance) * 0.001;
            else
                guardchance = guard.GuardSource.GetModified(EProperty.BlockChance) * 0.001 * (leftHand.Quality * 0.01);

            guardchance += guardLevel * 5 * 0.01; // 5% additional chance to guard with each Guard level.
            guardchance += attackerConLevel * 0.05;
            int shieldSize = 1;

            if (leftHand != null)
            {
                shieldSize = Math.Max(leftHand.Type_Damage, 1);

                if (guardSource is GamePlayer)
                    guardchance += (double) (leftHand.Level - 1) / 50 * 0.15; // Up to 15% extra block chance based on shield level.
            }

            if (m_attackers.Count > shieldSize)
                guardchance *= shieldSize / (double) m_attackers.Count;

            if (guardchance < 0.01)
                guardchance = 0.01;
            //else if (ad.Attacker is GamePlayer && guardchance > 0.6)
            // guardchance = 0.6;
            else if (shieldSize == 1 && guardchance > 0.8)
                guardchance = 0.8;
            else if (shieldSize == 2 && guardchance > 0.9)
                guardchance = 0.9;
            else if (shieldSize == 3 && guardchance > 0.99)
                guardchance = 0.99;

            if (ad.AttackType == AttackData.EAttackType.MeleeDualWield)
                guardchance /= 2;

            double ranBlockNum = UtilCollection.CryptoNextDouble() * 10000;
            ranBlockNum = Math.Floor(ranBlockNum);
            ranBlockNum /= 100;
            guardchance *= 100;

            double? blockDouble = (owner as GamePlayer)?.RandomNumberDeck.GetPseudoDouble();
            double? blockOutput = (blockDouble != null) ? blockDouble * 100 : ranBlockNum;

            if (guard.GuardSource is GamePlayer blockAttk && blockAttk.UseDetailedCombatLog)
                blockAttk.Out.SendMessage($"Chance to guard: {guardchance} rand: {blockOutput} GuardSuccess? {guardchance > blockOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

            if (guard.GuardTarget is GamePlayer blockTarg && blockTarg.UseDetailedCombatLog)
                blockTarg.Out.SendMessage($"Chance to be guarded: {guardchance} rand: {blockOutput} GuardSuccess? {guardchance > blockOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

            if (blockDouble == null || ServerProperties.ServerProperties.OVERRIDE_DECK_RNG)
            {
                if (guardchance > ranBlockNum)
                {
                    ad.Target = guard.GuardSource;
                    return true;
                }
            }
            else
            {
                if (guardchance > blockOutput)
                {
                    ad.Target = guard.GuardSource;
                    return true;
                }
            }

            return false;
        }

        public bool CheckDashingDefense(AttackData ad, bool stealthStyle, double attackerConLevel, out EAttackResult result)
        {
            // Not implemented.
            result = EAttackResult.Any;
            return false;
            NfRaDashingDefenseEffect dashing = null;

            if (dashing == null ||
                dashing.GuardSource.ObjectState != GameObject.eObjectState.Active ||
                dashing.GuardSource.IsStunned != false ||
                dashing.GuardSource.IsMezzed != false ||
                dashing.GuardSource.ActiveWeaponSlot == EActiveWeaponSlot.Distance ||
                !dashing.GuardSource.IsAlive ||
                stealthStyle)
                return false;

            if (!dashing.GuardSource.IsWithinRadius(dashing.GuardTarget, NfRaDashingDefenseEffect.GUARD_DISTANCE))
                return false;

            InventoryItem leftHand = dashing.GuardSource.Inventory.GetItem(eInventorySlot.LeftHandWeapon);
            InventoryItem rightHand = dashing.GuardSource.ActiveWeapon;

            if ((rightHand == null || rightHand.Hand != 1) && leftHand != null && leftHand.Object_Type == (int) EObjectType.Shield)
            {
                int guardLevel = dashing.GuardSource.GetAbilityLevel(Abilities.Guard);
                double guardchance = dashing.GuardSource.GetModified(EProperty.BlockChance) * leftHand.Quality * 0.00001;
                guardchance *= guardLevel * 0.25 + 0.05;
                guardchance += attackerConLevel * 0.05;

                if (guardchance > 0.99)
                    guardchance = 0.99;
                else if (guardchance < 0.01)
                    guardchance = 0.01;

                int shieldSize = 0;

                if (leftHand != null)
                    shieldSize = leftHand.Type_Damage;

                if (m_attackers.Count > shieldSize)
                    guardchance *= shieldSize / (double) m_attackers.Count;

                if (ad.AttackType == AttackData.EAttackType.MeleeDualWield)
                    guardchance /= 2;

                double parrychance = dashing.GuardSource.GetModified(EProperty.ParryChance);

                if (parrychance != double.MinValue)
                {
                    parrychance *= 0.001;
                    parrychance += 0.05 * attackerConLevel;

                    if (parrychance > 0.99)
                        parrychance = 0.99;
                    else if (parrychance < 0.01)
                        parrychance = 0.01;

                    if (m_attackers.Count > 1)
                        parrychance /= m_attackers.Count / 2;
                }

                if (UtilCollection.ChanceDouble(guardchance))
                {
                    ad.Target = dashing.GuardSource;
                    result = EAttackResult.Blocked;
                    return true;
                }
                else if (UtilCollection.ChanceDouble(parrychance))
                {
                    ad.Target = dashing.GuardSource;
                    result = EAttackResult.Parried;
                    return true;
                }
            }
            else
            {
                double parrychance = dashing.GuardSource.GetModified(EProperty.ParryChance);

                if (parrychance != double.MinValue)
                {
                    parrychance *= 0.001;
                    parrychance += 0.05 * attackerConLevel;

                    if (parrychance > 0.99)
                        parrychance = 0.99;
                    else if (parrychance < 0.01)
                        parrychance = 0.01;

                    if (m_attackers.Count > 1)
                        parrychance /= m_attackers.Count / 2;
                }

                if (UtilCollection.ChanceDouble(parrychance))
                {
                    ad.Target = dashing.GuardSource;
                    result = EAttackResult.Parried;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the result of an enemy attack
        /// </summary>
        public virtual EAttackResult CalculateEnemyAttackResult(WeaponAction action, AttackData ad, InventoryItem attackerWeapon)
        {
            if (owner.EffectList.CountOfType<NecromancerShadeEffect>() > 0)
                return EAttackResult.NoValidTarget;

            //1.To-Hit modifiers on styles do not any effect on whether your opponent successfully Evades, Blocks, or Parries.  Grab Bag 2/27/03
            //2.The correct Order of Resolution in combat is Intercept, Evade, Parry, Block (Shield), Guard, Hit/Miss, and then Bladeturn.  Grab Bag 2/27/03, Grab Bag 4/4/03
            //3.For every person attacking a monster, a small bonus is applied to each player's chance to hit the enemy. Allowances are made for those who don't technically hit things when they are participating in the raid  for example, a healer gets credit for attacking a monster when he heals someone who is attacking the monster, because that's what he does in a battle.  Grab Bag 6/6/03
            //4.Block, parry, and bolt attacks are affected by this code, as you know. We made a fix to how the code counts people as "in combat." Before this patch, everyone grouped and on the raid was counted as "in combat." The guy AFK getting Mountain Dew was in combat, the level five guy hovering in the back and hoovering up some exp was in combat  if they were grouped with SOMEONE fighting, they were in combat. This was a bad thing for block, parry, and bolt users, and so we fixed it.  Grab Bag 6/6/03
            //5.Positional degrees - Side Positional combat styles now will work an extra 15 degrees towards the rear of an opponent, and rear position styles work in a 60 degree arc rather than the original 90 degree standard. This change should even out the difficulty between side and rear positional combat styles, which have the same damage bonus. Please note that front positional styles are not affected by this change. 1.62
            //http://daoc.catacombs.com/forum.cfm?ThreadKey=511&DefMessage=681444&forum=DAOCMainForum#Defense

            InterceptEcsEffect intercept = null;
            EcsGameSpellEffect bladeturn = null;
            // ML effects
            GameSpellEffect phaseshift = null;
            GameSpellEffect grapple = null;
            GameSpellEffect brittleguard = null;

            AttackData lastAD = owner.TempProperties.getProperty<AttackData>(GameLiving.LAST_ATTACK_DATA, null);
            bool defenseDisabled = ad.Target.IsMezzed | ad.Target.IsStunned | ad.Target.IsSitting;

            GamePlayer playerOwner = owner as GamePlayer;
            GamePlayer playerAttacker = ad.Attacker as GamePlayer;

            // If berserk is on, no defensive skills may be used: evade, parry, ...
            // unfortunately this as to be check for every action itself to kepp oder of actions the same.
            // Intercept and guard can still be used on berserked
            // BerserkEffect berserk = null;

            if (EffectListService.GetAbilityEffectOnTarget(owner, EEffect.Berserk) != null)
                defenseDisabled = true;

            if (EffectListService.GetSpellEffectOnTarget(owner, EEffect.Bladeturn) is EcsGameSpellEffect bladeturnEffect)
            {
                if (bladeturn == null)
                    bladeturn = bladeturnEffect;
            }

            // We check if interceptor can intercept.
            if (EffectListService.GetAbilityEffectOnTarget(owner, EEffect.Intercept) is InterceptEcsEffect inter)
            {
                if (intercept == null && inter != null && inter.InterceptTarget == owner && !inter.InterceptSource.IsStunned && !inter.InterceptSource.IsMezzed
                    && !inter.InterceptSource.IsSitting && inter.InterceptSource.ObjectState == GameObject.eObjectState.Active && inter.InterceptSource.IsAlive
                    && owner.IsWithinRadius(inter.InterceptSource, InterceptHandler.INTERCEPT_DISTANCE)) // && Util.Chance(inter.InterceptChance))
                {
                    int chance = (owner is GamePlayer own) ? own.RandomNumberDeck.GetInt() : UtilCollection.Random(100);

                    if (chance < inter.InterceptChance)
                        intercept = inter;
                }
            }

            bool stealthStyle = false;

            if (ad.Style != null && ad.Style.StealthRequirement && ad.Attacker is GamePlayer && StyleProcessor.CanUseStyle((GamePlayer) ad.Attacker, ad.Style, attackerWeapon))
            {
                stealthStyle = true;
                defenseDisabled = true;
                intercept = null;
                brittleguard = null;
            }

            if (playerOwner != null)
            {
                GameLiving attacker = ad.Attacker;
                GamePlayer tempPlayerAttacker = playerAttacker ?? ((attacker as GameNPC)?.Brain as IControlledBrain)?.GetPlayerOwner();

                if (tempPlayerAttacker != null && action.ActiveWeaponSlot != EActiveWeaponSlot.Distance)
                {
                    GamePlayer bodyguard = playerOwner.Bodyguard;

                    if (bodyguard != null)
                    {
                        playerOwner.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.YouWereProtected"), bodyguard.Name, attacker.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);
                        bodyguard.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(bodyguard.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.YouHaveProtected"), playerOwner.Name, attacker.Name), EChatType.CT_Missed, EChatLoc.CL_SystemWindow);

                        if (attacker == tempPlayerAttacker)
                            tempPlayerAttacker.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(tempPlayerAttacker.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.YouAttempt"), playerOwner.Name, playerOwner.Name, bodyguard.Name), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                        else
                            tempPlayerAttacker.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(tempPlayerAttacker.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.YourPetAttempts"), playerOwner.Name, playerOwner.Name, bodyguard.Name), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);

                        return EAttackResult.Bodyguarded;
                    }
                }
            }

            if (phaseshift != null)
                return EAttackResult.Missed;

            if (grapple != null)
                return EAttackResult.Grappled;

            if (brittleguard != null)
            {
                playerOwner?.Out.SendMessage(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.BlowIntercepted"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                playerAttacker?.Out.SendMessage(LanguageMgr.GetTranslation(playerAttacker.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.StrikeIntercepted"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                brittleguard.Cancel(false);
                return EAttackResult.Missed;
            }

            if (intercept != null && !stealthStyle)
            {
                ad.Target = intercept.InterceptSource;

                if (intercept.InterceptSource is GamePlayer)
                    intercept.Cancel(false);

                return EAttackResult.HitUnstyled;
            }

            double attackerConLevel = -owner.GetConLevel(ad.Attacker);

            if (!defenseDisabled)
            {
                if (lastAD != null && lastAD.AttackResult != EAttackResult.HitStyle)
                    lastAD = null;

                double evadeChance = owner.TryEvade(ad, lastAD, attackerConLevel, m_attackers.Count);
                ad.EvadeChance = evadeChance;
                double randomEvadeNum = UtilCollection.CryptoNextDouble() * 10000;
                randomEvadeNum = Math.Floor(randomEvadeNum);
                randomEvadeNum /= 100;
                evadeChance *= 100;

                if (evadeChance > 0)
                {
                    double? evadeDouble = (owner as GamePlayer)?.RandomNumberDeck.GetPseudoDouble();
                    double? evadeOutput = (evadeDouble != null) ? Math.Round((double) (evadeDouble * 100),2 ) : randomEvadeNum;

                    if (ad.Attacker is GamePlayer evadeAtk && evadeAtk.UseDetailedCombatLog)
                        evadeAtk.Out.SendMessage($"target evade%: {Math.Round(evadeChance, 2)} rand: {evadeOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                    if (ad.Target is GamePlayer evadeTarg && evadeTarg.UseDetailedCombatLog)
                        evadeTarg.Out.SendMessage($"your evade%: {Math.Round(evadeChance, 2)} rand: {evadeOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                    if (evadeDouble == null || ServerProperties.ServerProperties.OVERRIDE_DECK_RNG)
                    {
                        if (evadeChance > randomEvadeNum)
                            return EAttackResult.Evaded;
                    }
                    else
                    {
                        evadeDouble *= 100;

                        if (evadeChance > evadeDouble)
                            return EAttackResult.Evaded;
                    }
                }

                if (ad.IsMeleeAttack)
                {
                    double parryChance = owner.TryParry(ad, lastAD, attackerConLevel, m_attackers.Count);
                    ad.ParryChance = parryChance;
                    double ranParryNum = UtilCollection.CryptoNextDouble() * 10000;
                    ranParryNum = Math.Floor(ranParryNum);
                    ranParryNum /= 100;
                    parryChance *= 100;

                    if (parryChance > 0)
                    {
                        double? parryDouble = (owner as GamePlayer)?.RandomNumberDeck.GetPseudoDouble();
                        double? parryOutput = (parryDouble != null) ? Math.Round((double) (parryDouble * 100.0), 2) : ranParryNum;

                        if (ad.Attacker is GamePlayer parryAtk && parryAtk.UseDetailedCombatLog)
                            parryAtk.Out.SendMessage($"target parry%: {Math.Round(parryChance, 2)} rand: {parryOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                        if (ad.Target is GamePlayer parryTarg && parryTarg.UseDetailedCombatLog)
                            parryTarg.Out.SendMessage($"your parry%: {Math.Round(parryChance, 2)} rand: {parryOutput}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                        if (parryDouble == null || ServerProperties.ServerProperties.OVERRIDE_DECK_RNG)
                        {
                            if (parryChance > ranParryNum)
                                return EAttackResult.Parried;
                        }
                        else
                        {
                            parryDouble *= 100;

                            if (parryChance > parryDouble)
                                return EAttackResult.Parried;
                        }
                    }
                }

                if (CheckBlock(ad, attackerConLevel))
                    return EAttackResult.Blocked;
            }

            if (CheckGuard(ad, stealthStyle, attackerConLevel))
                return EAttackResult.Blocked;

            // Not implemented.
            // if (CheckDashingDefense(ad, stealthStyle, attackerConLevel, out eAttackResult result)
            //     return result;

            // Miss chance.
            int missChance = GetMissChance(action, ad, lastAD, attackerWeapon);

            // Check for dirty trick fumbles before misses.
            DirtyTricksDetrimentalECSGameEffect dt = (DirtyTricksDetrimentalECSGameEffect)EffectListService.GetAbilityEffectOnTarget(ad.Attacker, EEffect.DirtyTricksDetrimental);

            if (dt != null && ad.IsRandomFumble)
                return EAttackResult.Fumbled;

            ad.MissRate = missChance;

            if (missChance > 0)
            {
                double rand = !ServerProperties.ServerProperties.OVERRIDE_DECK_RNG && playerAttacker != null ? playerAttacker.RandomNumberDeck.GetPseudoDouble() : UtilCollection.CryptoNextDouble();

                if (ad.Attacker is GamePlayer misser && misser.UseDetailedCombatLog)
                {
                    misser.Out.SendMessage($"miss rate on target: {missChance}% rand: {rand * 100:0.##}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                    misser.Out.SendMessage($"Your chance to fumble: {100 * ad.Attacker.ChanceToFumble:0.##}% rand: {100 * rand:0.##}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }

                if (ad.Target is GamePlayer missee && missee.UseDetailedCombatLog)
                    missee.Out.SendMessage($"chance to be missed: {missChance}% rand: {rand * 100:0.##}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

                // Check for normal fumbles.
                // NOTE: fumbles are a subset of misses, and a player can only fumble if the attack would have been a miss anyways.
                if (missChance > rand * 100)
                {
                    if (ad.Attacker.ChanceToFumble > rand)
                        return EAttackResult.Fumbled;

                    return EAttackResult.Missed;
                }
            }

            // Bladeturn
            // TODO: high level mob attackers penetrate bt, players are tested and do not penetrate (lv30 vs lv20)
            /*
             * http://www.camelotherald.com/more/31.shtml
             * - Bladeturns can now be penetrated by attacks from higher level monsters and
             * players. The chance of the bladeturn deflecting a higher level attack is
             * approximately the caster's level / the attacker's level.
             * Please be aware that everything in the game is
             * level/chance based - nothing works 100% of the time in all cases.
             * It was a bug that caused it to work 100% of the time - now it takes the
             * levels of the players involved into account.
             */
            if (bladeturn != null)
            {
                bool penetrate = false;

                if (stealthStyle)
                    return EAttackResult.HitUnstyled; // Exit early for stealth to prevent breaking bubble but still register a hit.

                if (action.RangedAttackType == ERangedAttackType.Long ||
                    (ad.AttackType == AttackData.EAttackType.Ranged && ad.Target != bladeturn.SpellHandler.Caster && playerAttacker?.HasAbility(Abilities.PenetratingArrow) == true))
                    penetrate = true;

                if (ad.IsMeleeAttack && !UtilCollection.ChanceDouble(bladeturn.SpellHandler.Caster.Level / ad.Attacker.Level))
                    penetrate = true;

                if (penetrate)
                {
                    if (playerOwner != null)
                    {
                        playerOwner.Out.SendMessage(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.BlowPenetrated"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                        EffectService.RequestImmediateCancelEffect(bladeturn);
                    }
                }
                else
                {
                    if (playerOwner != null)
                    {
                        playerOwner.Out.SendMessage(LanguageMgr.GetTranslation(playerOwner.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.BlowAbsorbed"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                        playerOwner.Stealth(false);
                    }

                    playerAttacker?.Out.SendMessage(LanguageMgr.GetTranslation(playerAttacker.Client.Account.Language, "GameLiving.CalculateEnemyAttackResult.StrikeAbsorbed"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
                    EffectService.RequestImmediateCancelEffect(bladeturn);
                    return EAttackResult.Missed;
                }
            }

            if (playerOwner?.IsOnHorse == true)
                playerOwner.IsOnHorse = false;

            return EAttackResult.HitUnstyled;
        }

        private int GetBonusCapForLevel(int level)
        {
            int bonusCap = 0;
            if (level < 15) bonusCap = 0;
            else if (level < 20) bonusCap = 5;
            else if (level < 25) bonusCap = 10;
            else if (level < 30) bonusCap = 15;
            else if (level < 35) bonusCap = 20;
            else if (level < 40) bonusCap = 25;
            else if (level < 45) bonusCap = 30;
            else bonusCap = 35;

            return bonusCap;
        }

        /// <summary>
        /// Send the messages to the GamePlayer
        /// </summary>
        public void SendAttackingCombatMessages(WeaponAction action, AttackData ad)
        {
            // Used to prevent combat log spam when the target is out of range, dead, not visible, etc.
            // A null attackAction means it was cleared up before we had a chance to send combat messages.
            // This typically happens when a ranged weapon is shot once without auto reloading.
            // In this case, we simply assume the last round should show a combat message.
            if (attackAction != null)
            {
                if (ad.AttackResult is not EAttackResult.Missed
                    and not EAttackResult.HitUnstyled
                    and not EAttackResult.HitStyle
                    and not EAttackResult.Evaded
                    and not EAttackResult.Blocked
                    and not EAttackResult.Parried)
                {
                    if (GameLoop.GameLoopTime - attackAction.RoundWithNoAttackTime <= 1500)
                        return;

                    attackAction.RoundWithNoAttackTime = 0;
                }
            }

            if (owner is GamePlayer)
            {
                var p = owner as GamePlayer;

                GameObject target = ad.Target;
                InventoryItem weapon = ad.Weapon;
                if (ad.Target is GameNPC)
                {
                    switch (ad.AttackResult)
                    {
                        case EAttackResult.TargetNotVisible:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.NotInView",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.OutOfRange:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.TooFarAway",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.TargetDead:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.AlreadyDead",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Blocked:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.Blocked",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Parried:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.Parried",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Evaded:
                            p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.Evaded",
                                    ad.Target.GetName(0, true, p.Client.Account.Language, (ad.Target as GameNPC))),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.NoTarget:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.NeedTarget"),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.NoValidTarget:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.CantBeAttacked"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Missed:
                            string message;
                            if (ad.MissRate > 0)
                                message = LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Miss") + $" ({ad.MissRate}%)";
                            else
                                message = LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.StrafMiss");
                            p.Out.SendMessage(message, EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Fumbled:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Fumble"),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.HitStyle:
                        case EAttackResult.HitUnstyled:
                            string modmessage = "";
                            if (ad.Modifier > 0) modmessage = " (+" + ad.Modifier + ")";
                            if (ad.Modifier < 0) modmessage = " (" + ad.Modifier + ")";

                            string hitWeapon = "";

                            switch (p.Client.Account.Language)
                            {
                                case "DE":
                                    if (weapon != null)
                                        hitWeapon = weapon.Name;
                                    break;
                                default:
                                    if (weapon != null)
                                        hitWeapon = GlobalConstants.NameToShortName(weapon.Name);
                                    break;
                            }

                            if (hitWeapon.Length > 0)
                                hitWeapon = " " +
                                            LanguageMgr.GetTranslation(p.Client.Account.Language,
                                                "GamePlayer.Attack.WithYour") + " " + hitWeapon;

                            string attackTypeMsg = LanguageMgr.GetTranslation(p.Client.Account.Language,
                                "GamePlayer.Attack.YouAttack");
                            if (action.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                                attackTypeMsg = LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.YouShot");

                            // intercept messages
                            if (target != null && target != ad.Target)
                            {
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.Intercepted", ad.Target.GetName(0, true),
                                        target.GetName(0, false)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.InterceptedHit", attackTypeMsg, target.GetName(0, false),
                                        hitWeapon, ad.Target.GetName(0, false), ad.Damage, modmessage),
                                    EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            }
                            else
                                p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.InterceptHit", attackTypeMsg,
                                    ad.Target.GetName(0, false, p.Client.Account.Language, (ad.Target as GameNPC)),
                                    hitWeapon, ad.Damage, modmessage), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);

                            // critical hit
                            if (ad.CriticalDamage > 0)
                                p.Out.SendMessage(LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.Critical",
                                        ad.Target.GetName(0, false, p.Client.Account.Language, (ad.Target as GameNPC)),
                                        ad.CriticalDamage) + $" ({AttackCriticalChance(action, ad.Weapon)}%)",
                                    EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                    }
                }
                else
                {
                    switch (ad.AttackResult)
                    {
                        case EAttackResult.TargetNotVisible:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.NotInView",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.OutOfRange:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.TooFarAway",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.TargetDead:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.AlreadyDead",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Blocked:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Blocked",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Parried:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Parried",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Evaded:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Evaded",
                                    ad.Target.GetName(0, true)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.NoTarget:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.NeedTarget"),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.NoValidTarget:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.CantBeAttacked"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Missed:
                            string message;
                            if (ad.MissRate > 0)
                                message = LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Miss") + $" ({ad.MissRate}%)";
                            else
                                message = LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.StrafMiss");
                            p.Out.SendMessage(message, EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.Fumbled:
                            p.Out.SendMessage(
                                LanguageMgr.GetTranslation(p.Client.Account.Language, "GamePlayer.Attack.Fumble"),
                                EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            break;
                        case EAttackResult.HitStyle:
                        case EAttackResult.HitUnstyled:
                            string modmessage = "";
                            if (ad.Modifier > 0) modmessage = " (+" + ad.Modifier + ")";
                            if (ad.Modifier < 0) modmessage = " (" + ad.Modifier + ")";

                            string hitWeapon = "";

                            switch (p.Client.Account.Language)
                            {
                                case "DE":
                                    if (weapon != null)
                                        hitWeapon = weapon.Name;
                                    break;
                                default:
                                    if (weapon != null)
                                        hitWeapon = GlobalConstants.NameToShortName(weapon.Name);
                                    break;
                            }

                            if (hitWeapon.Length > 0)
                                hitWeapon = " " +
                                            LanguageMgr.GetTranslation(p.Client.Account.Language,
                                                "GamePlayer.Attack.WithYour") + " " + hitWeapon;

                            string attackTypeMsg = LanguageMgr.GetTranslation(p.Client.Account.Language,
                                "GamePlayer.Attack.YouAttack");
                            if (action.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
                                attackTypeMsg = LanguageMgr.GetTranslation(p.Client.Account.Language,
                                    "GamePlayer.Attack.YouShot");

                            // intercept messages
                            if (target != null && target != ad.Target)
                            {
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.Intercepted", ad.Target.GetName(0, true),
                                        target.GetName(0, false)), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.InterceptedHit", attackTypeMsg, target.GetName(0, false),
                                        hitWeapon, ad.Target.GetName(0, false), ad.Damage, modmessage),
                                    EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                            }
                            else
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.InterceptHit", attackTypeMsg, ad.Target.GetName(0, false),
                                        hitWeapon, ad.Damage, modmessage), EChatType.CT_YouHit,
                                    EChatLoc.CL_SystemWindow);

                            // critical hit
                            if (ad.CriticalDamage > 0)
                                p.Out.SendMessage(
                                    LanguageMgr.GetTranslation(p.Client.Account.Language,
                                        "GamePlayer.Attack.Critical", ad.Target.GetName(0, false),
                                        ad.CriticalDamage) + $" ({AttackCriticalChance(action, ad.Weapon)}%)", EChatType.CT_YouHit,
                                    EChatLoc.CL_SystemWindow);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates melee critical damage
        /// </summary>
        /// <param name="ad">The attack data</param>
        /// <param name="weapon">The weapon used</param>
        /// <returns>The amount of critical damage</returns>
        public int GetMeleeCriticalDamage(AttackData ad, WeaponAction action, InventoryItem weapon)
        {
            if (!UtilCollection.Chance(AttackCriticalChance(action, weapon)))
                return 0;

            if (owner is GamePlayer)
            {
                // triple wield prevents critical hits
                if (EffectListService.GetAbilityEffectOnTarget(ad.Target, EEffect.TripleWield) != null)
                    return 0;

                int critMin;
                int critMax;
                EcsGameEffect berserk = EffectListService.GetEffectOnTarget(owner, EEffect.Berserk);

                if (berserk != null)
                {
                    int level = owner.GetAbilityLevel(Abilities.Berserk);
                    // According to : http://daoc.catacombs.com/forum.cfm?ThreadKey=10833&DefMessage=922046&forum=37
                    // Zerk 1 = 1-25%
                    // Zerk 2 = 1-50%
                    // Zerk 3 = 1-75%
                    // Zerk 4 = 1-99%
                    critMin = (int) (0.01 * ad.Damage);
                    critMax = (int) (Math.Min(0.99, (level * 0.25)) * ad.Damage);
                }
                else
                {
                    //think min crit dmage is 10% of damage
                    critMin = ad.Damage / 10;
                    // Critical damage to players is 50%, low limit should be around 20% but not sure
                    // zerkers in Berserk do up to 99%
                    if (ad.Target is GamePlayer)
                        critMax = ad.Damage >> 1;
                    else
                        critMax = ad.Damage;
                }

                critMin = Math.Max(critMin, 0);
                critMax = Math.Max(critMin, critMax);
                return UtilCollection.Random(critMin, critMax);
            }
            else
            {
                int maxCriticalDamage = (ad.Target is GamePlayer) ? ad.Damage / 2 : ad.Damage;
                int minCriticalDamage = (int) (ad.Damage * MinMeleeCriticalDamage);

                if (minCriticalDamage > maxCriticalDamage)
                    minCriticalDamage = maxCriticalDamage;

                return UtilCollection.Random(minCriticalDamage, maxCriticalDamage);
            }
        }

        public int GetMissChance(WeaponAction action, AttackData ad, AttackData lastAD, InventoryItem weapon)
        {
            // No miss if the target is sitting or for Volley attacks.
             if ((owner is GamePlayer player && player.IsSitting) || action.RangedAttackType == ERangedAttackType.Volley)
                return 0;

            int missChance = ad.Attacker is GamePlayer or GameSummonedPet ? 18 : 25;
            missChance -= ad.Attacker.GetModified(EProperty.ToHitBonus);

            // PVE group miss rate.
            if (owner is GameNPC && ad.Attacker is GamePlayer playerAttacker && playerAttacker.Group != null && (int) (0.90 * playerAttacker.Group.Leader.Level) >= ad.Attacker.Level && ad.Attacker.IsWithinRadius(playerAttacker.Group.Leader, 3000))
                missChance -= (int) (5 * playerAttacker.Group.Leader.GetConLevel(owner));
            else if (owner is GameNPC || ad.Attacker is GameNPC)
            {
                GameLiving misscheck = ad.Attacker;

                if (ad.Attacker is GameSummonedPet petAttacker && petAttacker.Level < petAttacker.Owner.Level)
                    misscheck = petAttacker.Owner;

                missChance += (int) (5 * misscheck.GetConLevel(owner));
            }

            // Experimental miss rate adjustment for number of attackers.
            if ((owner is GamePlayer && ad.Attacker is GamePlayer) == false)
                missChance -= Math.Max(0, Attackers.Count - 1) * ServerProperties.ServerProperties.MISSRATE_REDUCTION_PER_ATTACKERS;

            // Weapon and armor bonuses.
            int armorBonus = 0;

            if (ad.Target is GamePlayer p)
            {
                ad.ArmorHitLocation = ((GamePlayer) ad.Target).CalculateArmorHitLocation(ad);

                if (ad.Target.Inventory != null)
                {
                    InventoryItem armor = ad.Target.Inventory.GetItem((eInventorySlot) ad.ArmorHitLocation);

                    if (armor != null)
                        armorBonus = armor.Bonus;
                }

                int bonusCap = GetBonusCapForLevel(p.Level);

                if (armorBonus > bonusCap)
                    armorBonus = bonusCap;
            }

            if (weapon != null)
            {
                int bonusCap = GetBonusCapForLevel(ad.Attacker.Level);
                int weaponBonus = weapon.Bonus;

                if (weaponBonus > bonusCap)
                    weaponBonus = bonusCap;

                armorBonus -= weaponBonus;
            }

            if (ad.Target is GamePlayer && ad.Attacker is GamePlayer)
                missChance += armorBonus;
            else
                missChance += missChance * armorBonus / 100;

            // Style bonuses.
            if (ad.Style != null)
                missChance -= ad.Style.BonusToHit;

            if (lastAD != null && lastAD.AttackResult == EAttackResult.HitStyle && lastAD.Style != null)
                missChance += lastAD.Style.BonusToDefense;

            if (owner is GamePlayer && ad.Attacker is GamePlayer && weapon != null)
                missChance -= (int) ((ad.Attacker.WeaponSpecLevel(weapon) - 1) * 0.1);

            if (action.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
            {
                InventoryItem ammo = ad.Attacker.rangeAttackComponent.Ammo;

                if (ammo != null)
                {
                    switch ((ammo.SPD_ABS >> 4) & 0x3)
                    {
                        // http://rothwellhome.org/guides/archery.htm
                        case 0:
                            missChance += (int) Math.Round(missChance * 0.15);
                            break; // Rough
                        //case 1:
                        //  missrate -= 0;
                        //  break;
                        case 2:
                            missChance -= (int) Math.Round(missChance * 0.15);
                            break; // doesn't exist (?)
                        case 3:
                            missChance -= (int) Math.Round(missChance * 0.25);
                            break; // Footed
                    }
                }
            }

            return missChance;
        }

        /// <summary>
        /// Minimum melee critical damage as a percentage of the
        /// raw damage.
        /// </summary>
        protected float MinMeleeCriticalDamage => 0.1f;

        /// <summary>
        /// Max. Damage possible without style
        /// </summary>
        /// <param name="weapon">attack weapon</param>
        public double UnstyledDamageCap(InventoryItem weapon)
        {
            if (owner is GameEpicBoss) //damage cap for epic encounters if they use melee weapons,if errors appear remove from here
            {
                var p = owner as GameEpicBoss;
                return AttackDamage(weapon) * ((double) p.Empathy / 100) *
                       ServerProperties.ServerProperties.SET_EPIC_ENCOUNTER_WEAPON_DAMAGE_CAP;
            } ///////////////////////////remove until here if errors appear
              
            if (owner is GameDragon) //damage cap for dragon encounter
            {
                var p = owner as GameDragon;
                return AttackDamage(weapon) * ((double) p.Empathy / 100) *
                       ServerProperties.ServerProperties.SET_EPIC_ENCOUNTER_WEAPON_DAMAGE_CAP;
            } 

            if (owner is GamePlayer)
            {
                var p = owner as GamePlayer;

                if (weapon != null)
                {
                    int DPS = weapon.DPS_AF;
                    int cap = 12 + 3 * p.Level;
                    if (p.RealmLevel > 39)
                        cap += 3;
                    if (DPS > cap)
                        DPS = cap;

                    double result = DPS * weapon.SPD_ABS * 0.03 * (0.94 + 0.003 * weapon.SPD_ABS);

                    if (weapon.Hand == 1) //2h
                    {
                        result *= 1.1 + (owner.WeaponSpecLevel(weapon) - 1) * 0.005;
                        if (weapon.Item_Type == Slot.RANGED)
                        {
                            // http://home.comcast.net/~shadowspawn3/bowdmg.html
                            //ammo damage bonus
                            double ammoDamageBonus = 1;
                            InventoryItem ammo = p.rangeAttackComponent.Ammo;

                            if (ammo != null)
                            {
                                switch ((ammo.SPD_ABS) & 0x3)
                                {
                                    case 0:
                                        ammoDamageBonus = 0.85;
                                        break; //Blunt (light) -15%
                                    case 1:
                                        ammoDamageBonus = 1;
                                        break; //Bodkin (medium) 0%
                                    case 2:
                                        ammoDamageBonus = 1.15;
                                        break; //doesn't exist on live
                                    case 3:
                                        ammoDamageBonus = 1.25;
                                        break; //Broadhead (X-heavy) +25%
                                }
                            }

                            result *= ammoDamageBonus;
                        }
                    }

                    if (weapon.Item_Type == Slot.RANGED && (weapon.Object_Type == (int) EObjectType.Longbow ||
                                                            weapon.Object_Type == (int) EObjectType.RecurvedBow ||
                                                            weapon.Object_Type == (int) EObjectType.CompositeBow))
                    {
                        if (ServerProperties.ServerProperties.ALLOW_OLD_ARCHERY)
                        {
                            result += p.GetModified(EProperty.RangedDamage) * 0.01;
                        }
                        else
                        {
                            result += p.GetModified(EProperty.SpellDamage) * 0.01;
                            result += p.GetModified(EProperty.RangedDamage) * 0.01;
                        }
                    }
                    else if (weapon.Item_Type == Slot.RANGED)
                    {
                        //Ranged damage buff,debuff,Relic,RA
                        result += p.GetModified(EProperty.RangedDamage) * 0.01;
                    }
                    else if (weapon.Item_Type == Slot.RIGHTHAND || weapon.Item_Type == Slot.LEFTHAND ||
                             weapon.Item_Type == Slot.TWOHAND)
                    {
                        result *= 1 + p.GetModified(EProperty.MeleeDamage) * 0.01;
                    }

                    if (p.Inventory?.GetItem(eInventorySlot.LeftHandWeapon) != null && weapon.Item_Type != Slot.TWOHAND)
                    {
                        if (p.GetModifiedSpecLevel(Specs.Left_Axe) > 0)
                        {
                            int LASpec = owner.GetModifiedSpecLevel(Specs.Left_Axe);
                            if (LASpec > 0)
                            {
                                var leftAxeEffectiveness = 0.625 + 0.0034 * LASpec;
                                
                                if (p.GetModified(EProperty.OffhandDamageAndChance) > 0)
                                {
                                    leftAxeEffectiveness += 0.01 * p.GetModified(EProperty.OffhandDamageAndChance);
                                }

                                result *= leftAxeEffectiveness;
                            }
                        }
                    }

                    if (result <= 0) //Checking if 0 or negative
                        result = 1;
                    return result;
                }
                else
                {
                    // TODO: whats the damage cap without weapon?
                    return AttackDamage(weapon) * 3 * (1 + (AttackSpeed(weapon) * 0.001 - 2) * 0.03);
                }
            }
            else
            {
                return AttackDamage(weapon) * (2.82 + 0.00009 * AttackSpeed(weapon));
            }
        }

        /// <summary>
        /// Whether the living is actually attacking something.
        /// </summary>
        public virtual bool IsAttacking => AttackState && (attackAction != null);

        /// <summary>
        /// Checks whether Living has ability to use lefthanded weapons
        /// </summary>
        public bool CanUseLefthandedWeapon
        {
            get
            {
                if (owner is GamePlayer)
                    return (owner as GamePlayer).CharacterClass.CanUseLefthandedWeapon;
                else
                    return false;
            }
        }

        /// <summary>
        /// Calculates how many times left hand swings
        /// </summary>
        public int CalculateLeftHandSwingCount()
        {
            if (owner is GamePlayer)
            {
                if (CanUseLefthandedWeapon == false)
                    return 0;

                if (owner.GetBaseSpecLevel(Specs.Left_Axe) > 0)
                {
                    if (owner is GamePlayer ptemp && ptemp.UseDetailedCombatLog)
                    {
                        int LASpec = owner.GetModifiedSpecLevel(Specs.Left_Axe);
                        double effectiveness = 0;
                        if (LASpec > 0)
                        {
                            effectiveness = 0.625 + 0.0034 * LASpec;
                        }

                        ptemp.Out.SendMessage(
                            $"{Math.Round(effectiveness * 100, 2)}% dmg (after LA penalty) \n",
                            EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                    }

                    return 1; // always use left axe
                }
                

                int specLevel = Math.Max(owner.GetModifiedSpecLevel(Specs.Celtic_Dual),
                    owner.GetModifiedSpecLevel(Specs.Dual_Wield));
                specLevel = Math.Max(specLevel, owner.GetModifiedSpecLevel(Specs.Fist_Wraps));

                decimal tmpOffhandChance = (25 + (specLevel - 1) * 68 / 100);
                tmpOffhandChance += owner.GetModified(EProperty.OffhandChance) +
                                    owner.GetModified(EProperty.OffhandDamageAndChance);
                
                if (owner is GamePlayer p && p.UseDetailedCombatLog && owner.GetModifiedSpecLevel(Specs.HandToHand) <= 0)
                {
                    p.Out.SendMessage(
                        $"OH swing%: {Math.Round(tmpOffhandChance, 2)} ({owner.GetModified(EProperty.OffhandChance) + owner.GetModified(EProperty.OffhandDamageAndChance)}% from RAs) \n",
                        EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                }
                
                if (specLevel > 0)
                {
                    return UtilCollection.Chance((int) tmpOffhandChance) ? 1 : 0;
                }

                // HtH chance
                specLevel = owner.GetModifiedSpecLevel(Specs.HandToHand);
                InventoryItem attackWeapon = owner.ActiveWeapon;
                InventoryItem leftWeapon = (owner.Inventory == null)
                    ? null
                    : owner.Inventory.GetItem(eInventorySlot.LeftHandWeapon);
                if (specLevel > 0 && attackWeapon != null && //attackWeapon.Object_Type == (int) eObjectType.HandToHand &&
                    leftWeapon != null && leftWeapon.Object_Type == (int) EObjectType.HandToHand)
                {
                    specLevel--;
                    int randomChance = UtilCollection.Random(99);
                    int doubleHitChance = (specLevel >> 1) + owner.GetModified(EProperty.OffhandChance) + owner.GetModified(EProperty.OffhandDamageAndChance);
                    int tripleHitChance = doubleHitChance + (specLevel >> 2) + ((owner.GetModified(EProperty.OffhandChance) + owner.GetModified(EProperty.OffhandDamageAndChance)) >> 1);
                    int quadHitChance = tripleHitChance + (specLevel >> 4) + ((owner.GetModified(EProperty.OffhandChance) + owner.GetModified(EProperty.OffhandDamageAndChance)) >> 2);
                    
                    if (owner is GamePlayer pl && pl.UseDetailedCombatLog)
                    {
                        pl.Out.SendMessage(
                            $"Chance for 2 hits: {doubleHitChance}% | 3 hits: { (specLevel > 25 ? tripleHitChance-doubleHitChance : 0)}% | 4 hits: {(specLevel > 40 ? quadHitChance-tripleHitChance : 0)}% \n",
                            EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                    }
                    
                    if (randomChance < doubleHitChance)
                        return 1; // 1 hit = spec/2
                    
                    //doubleHitChance += specLevel >> 2;
                    if (randomChance < tripleHitChance && specLevel > 25)
                        return 2; // 2 hits = spec/4
                    
                    //doubleHitChance += specLevel >> 4;
                    if (randomChance < quadHitChance && specLevel > 40)
                        return 3; // 3 hits = spec/16

                    return 0;
                }
                
                
            }

            return 0;
        }

        /// <summary>
        /// Returns a multiplier to adjust left hand damage
        /// </summary>
        /// <returns></returns>
        public double CalculateLeftHandEffectiveness(InventoryItem mainWeapon, InventoryItem leftWeapon)
        {
            double effectiveness = 1.0;

            if (owner is GamePlayer)
            {
                if (CanUseLefthandedWeapon && leftWeapon != null &&
                    leftWeapon.Object_Type == (int) EObjectType.LeftAxe && mainWeapon != null &&
                    (mainWeapon.Item_Type == Slot.RIGHTHAND || mainWeapon.Item_Type == Slot.LEFTHAND))
                {
                    int LASpec = owner.GetModifiedSpecLevel(Specs.Left_Axe);
                    if (LASpec > 0)
                    {
                        effectiveness = 0.625 + 0.0034 * LASpec;
                    }
                }
            }

            return effectiveness;
        }

        /// <summary>
        /// Returns a multiplier to adjust right hand damage
        /// </summary>
        /// <param name="leftWeapon"></param>
        /// <returns></returns>
        public double CalculateMainHandEffectiveness(InventoryItem mainWeapon, InventoryItem leftWeapon)
        {
            double effectiveness = 1.0;

            if (owner is GamePlayer)
            {
                if (CanUseLefthandedWeapon && leftWeapon != null &&
                    leftWeapon.Object_Type == (int) EObjectType.LeftAxe && mainWeapon != null &&
                    (mainWeapon.Item_Type == Slot.RIGHTHAND || mainWeapon.Item_Type == Slot.LEFTHAND))
                {
                    int LASpec = owner.GetModifiedSpecLevel(Specs.Left_Axe);
                    if (LASpec > 0)
                    {
                        effectiveness = 0.625 + 0.0034 * LASpec;
                    }
                }
            }

            return effectiveness;
        }
    }
}