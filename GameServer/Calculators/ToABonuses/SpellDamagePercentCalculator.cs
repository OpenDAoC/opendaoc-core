using System;
using Core.GS.Enums;

namespace Core.GS.Calculators;

[PropertyCalculator(EProperty.SpellDamage)]
public class SpellDamagePercentCalculator : PropertyCalculator
{
    public override int CalcValue(GameLiving living, EProperty property)
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