

using System;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// Calculator for XP % bonus
	/// </summary>
	[PropertyCalculator(EProperty.XpPoints)]
	public class XpPointsCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			if (living is GamePlayer)
			{
				return Math.Min(10, living.ItemBonus[(int)property]);
			}

			return 0;
		}
	}
}
