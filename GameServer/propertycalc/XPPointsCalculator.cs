

using System;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// Calculator for XP % bonus
	/// </summary>
	[PropertyCalculator(eProperty.XpPoints)]
	public class XpPointsCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, eProperty property)
		{
			if (living is GamePlayer)
			{
				return Math.Min(10, living.ItemBonus[(int)property]);
			}

			return 0;
		}
	}
}
