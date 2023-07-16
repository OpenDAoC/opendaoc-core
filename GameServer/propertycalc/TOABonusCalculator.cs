using System;

namespace DOL.GS.PropertyCalc
{
    /// <summary>
    /// The Spell Range bonus percent calculator
    /// 
    /// BuffBonusCategory1 unused
    /// BuffBonusCategory2 unused
    /// BuffBonusCategory3 is used for debuff
    /// BuffBonusCategory4 unused
    /// BuffBonusMultCategory1 unused
    /// </summary>

    //Debuff Effectivness
    [PropertyCalculator(eProperty.DebuffEffectivness)]
    public class DebuffEffectivnessPercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            // Hardcap at 25%
            return Math.Min(25, living.ItemBonus[(int)property] - living.DebuffCategory[(int)property]);
        }
    }

    //Buff Effectivness
    [PropertyCalculator(eProperty.BuffEffectiveness)]
    public class BuffEffectivenessPercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            GameLiving livingToCheck;

            if (living is NecromancerPet necroPet && necroPet.Owner is GamePlayer playerOwner)
                livingToCheck = playerOwner;
            else
                livingToCheck = living;

            // Hardcap at 25%
            // While it doesn't make much sense, ItemBonus is retrieved from the pet too. I don't know if the bonus is supposed to transfer from the owner.
            return Math.Min(25, livingToCheck.ItemBonus[(int)property] + livingToCheck.AbilityBonus[(int)property] - livingToCheck.DebuffCategory[(int)property]);
        }
    }

    // Healing Effectivness
    [PropertyCalculator(eProperty.HealingEffectiveness)]
    public class HealingEffectivenessPercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            // Hardcap at 25%
            int percent = Math.Min(25, living.BaseBuffBonusCategory[(int)property]
                - living.DebuffCategory[(int)property]
                + living.ItemBonus[(int)property]);
            // Add RA bonus
            percent += living.AbilityBonus[(int)property];

            // Relic bonus calculated before RA bonuses
			if (living is GamePlayer or GameSummonedPet)
				percent += (int)(100 * RelicMgr.GetRelicBonusModifier(living.Realm, eRelicType.Magic));

            return percent;
        }
    }

    /// <summary>
    /// The critical heal chance calculator. Returns 0 .. 100 chance.
    /// 
    /// Crit propability is capped to 50%
    /// </summary>
    [PropertyCalculator(eProperty.CriticalHealHitChance)]
    public class CriticalHealHitChanceCalculator : PropertyCalculator
    {
        public CriticalHealHitChanceCalculator() { }

        public override int CalcValue(GameLiving living, eProperty property)
        {
            int percent = living.AbilityBonus[(int)property];

            // Hardcap at 50%
            return Math.Min(50, percent);
        }
    }

    //Cast Speed
    [PropertyCalculator(eProperty.CastingSpeed)]
    public class SpellCastSpeedPercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            GameLiving livingToCheck;

            if (living is NecromancerPet necroPet && necroPet.Owner is GamePlayer playerOwner)
                livingToCheck = playerOwner;
            else
                livingToCheck = living;

            /// [Atlas - Takii] Re-introduce usage of CastingSpeed ability bonus instead of Item bonus since we have Mastery of the Art RA in OF.
            /// Hard cap at 15% since that's what MotA goes up to.
            return Math.Min(15, livingToCheck.AbilityBonus[(int)property] - livingToCheck.DebuffCategory[(int)property]);

            // Hardcap at 10%
            //return Math.Min(10, livingToCheck.ItemBonus[(int)property] - livingToCheck.DebuffCategory[(int)property]);
        }
    }

    //Spell Duration
    [PropertyCalculator(eProperty.SpellDuration)]
    public class SpellDurationPercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            //hardcap at 25%
            return Math.Min(25, living.ItemBonus[(int)property] - living.DebuffCategory[(int)property]);
        }
    }

    //Spell Damage
    [PropertyCalculator(eProperty.SpellDamage)]
    public class SpellDamagePercentCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            // Hardcap at 10%
            int percent = Math.Min(10, living.BaseBuffBonusCategory[(int)property]
                + living.ItemBonus[(int)property]
                - living.DebuffCategory[(int)property]);

            // Add RA bonus
            percent += living.AbilityBonus[(int)property];

            return percent;
        }
    }
}
