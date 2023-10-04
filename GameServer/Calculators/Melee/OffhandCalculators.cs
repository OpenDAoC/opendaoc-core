using System;

namespace DOL.GS.PropertyCalc;

[APropertyCalculator(eProperty.OffhandDamage)]
public class OffhandDamageCalculator : PropertyCalculator
{
	public override int CalcValue(GameLiving living, eProperty property)
	{
		return Math.Max(0, living.AbilityBonus[(int) property]);
	}
}

[APropertyCalculator(eProperty.OffhandChance)]
public class OffhandChanceCalculator : PropertyCalculator
{
	public override int CalcValue(GameLiving living, eProperty property)
	{
		return Math.Max(0, living.AbilityBonus[(int) property]);
	}
}

[APropertyCalculator(eProperty.OffhandDamageAndChance)]
public class OffhandDamageAndChanceCalculator : PropertyCalculator
{
	public override int CalcValue(GameLiving living, eProperty property)
	{
		return Math.Max(0, living.AbilityBonus[(int) property]);
	}
}