using System;

namespace DOL.GS.PropertyCalc;

/// <summary>
/// The Fatigue (Endurance) calculator
///
/// BuffBonusCategory1 is used for absolute HP buffs
/// BuffBonusCategory2 unused
/// BuffBonusCategory3 unused
/// BuffBonusCategory4 unused
/// BuffBonusMultCategory1 unused
/// </summary>
[PropertyCalculator(eProperty.Fatigue)]
public class FatigueCalculator : PropertyCalculator
{
    public override int CalcValue(GameLiving living, eProperty property)
    {
        if (living is GamePlayer)
        {
            var player = living as GamePlayer;

            var endurance = player.DBMaxEndurance;
            endurance += (int) (endurance * (Math.Min(15, living.ItemBonus[(int) property]) * .01));
            return endurance;
        }

        return 100;
    }
}