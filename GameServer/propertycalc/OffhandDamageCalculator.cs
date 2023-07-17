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
	[PropertyCalculator(EProperty.OffhandDamage)]
	public class OffhandDamageCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			return Math.Max(0, living.AbilityBonus[(int) property]);
		}
	}
	
	/// <summary>
	/// The Spell Range bonus percent calculator
	///
	/// BuffBonusCategory1 unused
	/// BuffBonusCategory2 unused
	/// BuffBonusCategory3 is used for debuff
	/// BuffBonusCategory4 unused
	/// BuffBonusMultCategory1 unused
	/// </summary>
	[PropertyCalculator(EProperty.OffhandChance)]
	public class OffhandChanceCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			return Math.Max(0, living.AbilityBonus[(int) property]);
		}
	}
	
	/// <summary>
	/// The Spell Range bonus percent calculator
	///
	/// BuffBonusCategory1 unused
	/// BuffBonusCategory2 unused
	/// BuffBonusCategory3 is used for debuff
	/// BuffBonusCategory4 unused
	/// BuffBonusMultCategory1 unused
	/// </summary>
	[PropertyCalculator(EProperty.OffhandDamageAndChance)]
	public class OffhandDamageAndChanceCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			return Math.Max(0, living.AbilityBonus[(int) property]);
		}
	}
}
