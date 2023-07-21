using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.PlayerClass;

namespace DOL.GS.Spells
{
    /// <summary>
    /// Buffs a single stat,
    /// considered as a baseline buff (regarding the bonuscategories on statproperties)
    /// </summary>
    public abstract class SingleStatBuffHandler : PropertyChangingHandler
    {
        public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }

        protected override void SendUpdates(GameLiving target)
        {
            target.UpdateHealthManaEndu();
        }

        public override void CreateECSEffect(ECSGameEffectInitParams initParams)
        {
            new StatBuffEcsEffect(initParams);
        }

        /// <summary>
        /// Apply effect on target or do spell action if non duration spell
        /// </summary>
        /// <param name="target">target that gets the effect</param>
        /// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            int specLevel = Caster.GetModifiedSpecLevel(m_spellLine.Spec);

            if (SpellLine.KeyName is GlobalSpellsLines.Potions_Effects or GlobalSpellsLines.Item_Effects)
                effectiveness = 1.0;
            else if (Spell.Level <= 0)
                effectiveness = 1.0;
            else if (Caster is GamePlayer playerCaster)
            {
			    if (playerCaster.CharacterClass.ID != (int)ECharacterClass.Savage && Spell.Target != "Enemy")
                {
                    if (playerCaster.CharacterClass.ClassType != eClassType.ListCaster)
                    {
                        effectiveness = 0.75; // This section is for self bulfs, cleric buffs etc.
                        effectiveness += (specLevel - 1.0) * 0.5 / Spell.Level;
                        effectiveness = Math.Max(0.75, effectiveness);
                        effectiveness = Math.Min(1.25, effectiveness);
                    }
                }
                else if (Spell.Target == "Enemy")
                {
				    effectiveness = 0.75; // This section is for list casters stat debuffs.
				    if (playerCaster.CharacterClass.ClassType == eClassType.ListCaster)
				    {
                        effectiveness += (specLevel - 1.0) * 0.5 / Spell.Level;
                        effectiveness = Math.Max(0.75, effectiveness);
                        effectiveness = Math.Min(1.25, effectiveness);
                        effectiveness *= 1.0 + m_caster.GetModified(EProperty.DebuffEffectivness) * 0.01;

                        if (playerCaster.UseDetailedCombatLog && m_caster.GetModified(EProperty.DebuffEffectivness) > 0)
                            playerCaster.Out.SendMessage($"debuff effectiveness: {m_caster.GetModified(EProperty.DebuffEffectivness)}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
                    }
				    else
				    {
					    effectiveness = 1.0; // Non list casters debuffs. Reaver curses, Champ debuffs etc.
					    effectiveness *= 1.0 + m_caster.GetModified(EProperty.DebuffEffectivness) * 0.01;
                    }
			    }
            }
            else if (Caster is NecromancerPet necroPetCaster && necroPetCaster.Owner is GamePlayer playerOwner && Spell.Target == "Enemy")
            {
                specLevel = playerOwner.GetModifiedSpecLevel(m_spellLine.Spec);

                effectiveness = 0.75; // This section is for list casters stat debuffs.
                effectiveness += (specLevel - 1.0) * 0.5 / Spell.Level;
                effectiveness = Math.Max(0.75, effectiveness);
                effectiveness = Math.Min(1.25, effectiveness);
                effectiveness *= 1.0 + playerOwner.GetModified(EProperty.DebuffEffectivness) * 0.01;                

                if (Spell.SpellType == ESpellType.ArmorFactorDebuff)
                    effectiveness *= 1 + target.GetArmorAbsorb(EArmorSlot.TORSO);

                if (playerOwner.UseDetailedCombatLog && m_caster.GetModified(EProperty.DebuffEffectivness) > 0)
                    playerOwner.Out.SendMessage($"debuff effectiveness: {m_caster.GetModified(EProperty.DebuffEffectivness)}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
            }
            else
                effectiveness = 1.0;

            if (Spell.Target != "Enemy")
            {
                effectiveness *= 1.0 + m_caster.GetModified(EProperty.BuffEffectiveness) * 0.01;

                if (Caster is GamePlayer gamePlayer && gamePlayer.UseDetailedCombatLog && m_caster.GetModified(EProperty.BuffEffectiveness) > 0 )
                    gamePlayer.Out.SendMessage($"buff effectiveness: {m_caster.GetModified(EProperty.BuffEffectiveness)}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);
            }
            else
                effectiveness *= GetCritBonus();

            target.StartHealthRegeneration();
            base.ApplyEffectOnTarget(target, effectiveness);
        }

        /// <summary>
        /// Determines wether this spell is compatible with given spell
        /// and therefore overwritable by better versions
        /// spells that are overwritable cannot stack
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public override bool IsOverwritable(GameSpellEffect compare)
        {
            if (Spell.EffectGroup != 0 || compare.Spell.EffectGroup != 0)
                return Spell.EffectGroup == compare.Spell.EffectGroup;
            if (!base.IsOverwritable(compare))
                return false;
            if (Spell.Duration > 0 && compare.Concentration > 0)
                return compare.Spell.Value >= Spell.Value;
            return compare.SpellHandler.SpellLine.IsBaseLine == SpellLine.IsBaseLine;
        }

        private double GetCritBonus()
        {
            double critMod = 1.0;
            int critChance = Caster.DotCriticalChance;

            if (critChance <= 0)
                return critMod;

            GamePlayer playerCaster = Caster as GamePlayer;

            if (playerCaster?.UseDetailedCombatLog == true && critChance > 0)
                playerCaster.Out.SendMessage($"Debuff crit chance: {Caster.DotCriticalChance}", EChatType.CT_DamageAdd, EChatLoc.CL_SystemWindow);

            if (UtilCollection.Chance(critChance))
            {                    
                critMod *= 1 + UtilCollection.Random(1, 10) * 0.1;
                playerCaster?.Out.SendMessage($"Your {Spell.Name} critically debuffs the enemy for {Math.Round(critMod - 1,3) * 100}% additional effect!", EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
            }

            return critMod;
        }

        // constructor
        protected SingleStatBuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Str stat baseline buff
    /// </summary>
    [SpellHandlerAttribute("StrengthBuff")]
    public class StrengthBuff : SingleStatBuffHandler
    {
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirStrength))
            {
                MessageToCaster("Your target already has an effect of that type!", EChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
        public override EProperty Property1 { get { return EProperty.Strength; } }

        // constructor
        public StrengthBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Dex stat baseline buff
    /// </summary>
    [SpellHandlerAttribute("DexterityBuff")]
    public class DexterityBuff : SingleStatBuffHandler
    {
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirDexterity))
            {
                MessageToCaster("Your target already has an effect of that type!", EChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
        public override EProperty Property1 { get { return EProperty.Dexterity; } }

        // constructor
        public DexterityBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Con stat baseline buff
    /// </summary>
    [SpellHandlerAttribute("ConstitutionBuff")]
    public class ConstitutionBuff : SingleStatBuffHandler
    {
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirConstitution))
            {
                MessageToCaster("Your target already has an effect of that type!", EChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
        public override EProperty Property1 { get { return EProperty.Constitution; } }

        // constructor
        public ConstitutionBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Armor factor buff
    /// </summary>
    [SpellHandlerAttribute("ArmorFactorBuff")]
    public class ArmorFactorBuff : SingleStatBuffHandler
    {
        public override EBuffBonusCategory BonusCategory1
        {
            get
            {
                if (Caster is GamePlayer c && (c.CharacterClass is ClassRanger || c.CharacterClass is ClassHunter) && (SpellLine.KeyName.ToLower().Equals("beastcraft") || SpellLine.KeyName.ToLower().Equals("pathfinding"))) return EBuffBonusCategory.BaseBuff;
            	if (Spell.Target.Equals("Self", StringComparison.OrdinalIgnoreCase)) return EBuffBonusCategory.Other; // no caps for self buffs
                if (m_spellLine.IsBaseLine) return EBuffBonusCategory.BaseBuff; // baseline cap
                return EBuffBonusCategory.Other; // no caps for spec line buffs
            }
        }
        public override EProperty Property1 { get { return EProperty.ArmorFactor; } }

        // constructor
        public ArmorFactorBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Armor Absorption buff
    /// </summary>
    [SpellHandlerAttribute("ArmorAbsorptionBuff")]
    public class ArmorAbsorptionBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.ArmorAbsorption; } }

        /// <summary>
        /// send updates about the changes
        /// </summary>
        /// <param name="target"></param>
        protected override void SendUpdates(GameLiving target)
        {
        }

        // constructor
        public ArmorAbsorptionBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Combat speed buff
    /// </summary>
    [SpellHandlerAttribute("CombatSpeedBuff")]
    public class CombatSpeedBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.MeleeSpeed; } }

        /// <summary>
        /// send updates about the changes
        /// </summary>
        /// <param name="target"></param>
        protected override void SendUpdates(GameLiving target)
        {
        }

        // constructor
        public CombatSpeedBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    
    /// <summary>
    /// Haste Buff stacking with other Combat Speed Buff
    /// </summary>
    [SpellHandlerAttribute("HasteBuff")]
    public class HasteBuff : CombatSpeedBuff
    {
        // constructor
        public HasteBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Celerity Buff stacking with other Combat Speed Buff
    /// </summary>
    [SpellHandlerAttribute("CelerityBuff")]
    public class CelerityBuff : CombatSpeedBuff
    {
        // constructor
        public CelerityBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Fatigue reduction buff
    /// </summary>
    [SpellHandlerAttribute("FatigueConsumptionBuff")]
    public class FatigueConsumptionBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.FatigueConsumption; } }

        /// <summary>
        /// send updates about the changes
        /// </summary>
        /// <param name="target"></param>
        protected override void SendUpdates(GameLiving target)
        {
        }

        // constructor
        public FatigueConsumptionBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Melee damage buff
    /// </summary>
    [SpellHandlerAttribute("MeleeDamageBuff")]
    public class MeleeDamageBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.MeleeDamage; } }

        /// <summary>
        /// send updates about the changes
        /// </summary>
        /// <param name="target"></param>
        protected override void SendUpdates(GameLiving target)
        {
        }

        // constructor
        public MeleeDamageBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Mesmerize duration buff
    /// </summary>
    [SpellHandlerAttribute("MesmerizeDurationBuff")]
    public class MesmerizeDurationBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.MesmerizeDurationReduction; } }

        /// <summary>
        /// send updates about the changes
        /// </summary>
        /// <param name="target"></param>
        protected override void SendUpdates(GameLiving target)
        {
        }

        // constructor
        public MesmerizeDurationBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }


    /// <summary>
    /// Acuity buff
    /// </summary>
    [SpellHandlerAttribute("AcuityBuff")]
    public class AcuityBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.Acuity; } }

        // constructor
        public AcuityBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Quickness buff
    /// </summary>
    [SpellHandlerAttribute("QuicknessBuff")]
    public class QuicknessBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.Quickness; } }

        // constructor
        public QuicknessBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// DPS buff
    /// </summary>
    [SpellHandlerAttribute("DPSBuff")]
    public class DPSBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.DPS; } }

        // constructor
        public DPSBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Evade chance buff
    /// </summary>
    [SpellHandlerAttribute("EvadeBuff")]
    public class EvadeChanceBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.EvadeChance; } }

        // constructor
        public EvadeChanceBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    /// <summary>
    /// Parry chance buff
    /// </summary>
    [SpellHandlerAttribute("ParryBuff")]
    public class ParryChanceBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.ParryChance; } }

        // constructor
        public ParryChanceBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    /// <summary>
    /// WeaponSkill buff
    /// </summary>
    [SpellHandlerAttribute("WeaponSkillBuff")]
    public class WeaponSkillBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.WeaponSkill; } }

        // constructor
        public WeaponSkillBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    /// <summary>
    /// Stealth Skill buff
    /// </summary>
    [SpellHandlerAttribute("StealthSkillBuff")]
    public class StealthSkillBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.Skill_Stealth; } }

        // constructor
        public StealthSkillBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    /// <summary>
    /// To Hit buff
    /// </summary>
    [SpellHandlerAttribute("ToHitBuff")]
    public class ToHitSkillBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.ToHitBonus; } }

        // constructor
        public ToHitSkillBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    /// <summary>
    /// Magic Resists Buff
    /// </summary>
    [SpellHandlerAttribute("MagicResistsBuff")]
    public class MagicResistsBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.MagicAbsorption; } }

        // constructor
        public MagicResistsBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    [SpellHandlerAttribute("StyleAbsorbBuff")]
    public class StyleAbsorbBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.StyleAbsorb; } }
        public StyleAbsorbBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    [SpellHandlerAttribute("ExtraHP")]
    public class ExtraHP : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.ExtraHP; } }
        public ExtraHP(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Paladin Armor factor buff
    /// </summary>
    [SpellHandlerAttribute("PaladinArmorFactorBuff")]
    public class PaladinArmorFactorBuff : SingleStatBuffHandler
    {
        public override EBuffBonusCategory BonusCategory1
        {
            get
            {
                if (Spell.Target == "Self") return EBuffBonusCategory.Other; // no caps for self buffs
                if (m_spellLine.IsBaseLine) return EBuffBonusCategory.BaseBuff; // baseline cap
                return EBuffBonusCategory.Other; // no caps for spec line buffs
            }
        }
        public override EProperty Property1 { get { return EProperty.ArmorFactor; } }

        // constructor
        public PaladinArmorFactorBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    [SpellHandler("FlexibleSkillBuff")]
    public class FlexibleSkillBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.Skill_Flexible_Weapon; } }
        public FlexibleSkillBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    [SpellHandler("ResiPierceBuff")]
    public class ResiPierceBuff : SingleStatBuffHandler
    {
        public override EProperty Property1 { get { return EProperty.ResistPierce; } }
        public ResiPierceBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}