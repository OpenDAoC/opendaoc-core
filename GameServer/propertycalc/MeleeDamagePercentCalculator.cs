using System;

namespace DOL.GS.PropertyCalc
{
	[PropertyCalculator(eProperty.MeleeDamage)]
	public class MeleeDamagePercentCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, eProperty property)
		{
			if (living is GameNPC)
			{
				int strengthPerMeleeDamagePercent = 8;
				var strengthBuffBonus = living.BaseBuffBonusCategory[eProperty.Strength] + living.SpecBuffBonusCategory[eProperty.Strength];
				var strengthDebuffMalus = living.DebuffCategory[eProperty.Strength] + living.SpecDebuffCategory[eProperty.Strength];
				return ((living as GameNPC).Strength + (strengthBuffBonus - strengthDebuffMalus)) / strengthPerMeleeDamagePercent;
			}

			int hardCap = 10;
			int abilityBonus = living.AbilityBonus[(int)property];
			int itemBonus = Math.Min(hardCap, living.ItemBonus[(int)property]);
			int buffBonus = living.BaseBuffBonusCategory[(int)property] + living.SpecBuffBonusCategory[(int)property];
			int debuffMalus = Math.Min(hardCap, Math.Abs(living.DebuffCategory[(int)property]));
			return abilityBonus + buffBonus + itemBonus - debuffMalus;
		}
	}
}
