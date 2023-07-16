using System;
using DOL.AI.Brain;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// The Character Stat calculator
	/// 
	/// BuffBonusCategory1 is used for all single stat buffs
	/// BuffBonusCategory2 is used for all dual stat buffs
	/// BuffBonusCategory3 is used for all debuffs (positive values expected here)
	/// BuffBonusCategory4 is used for all other uncapped modifications
	///                    category 4 kicks in at last
	/// BuffBonusMultCategory1 used after all buffs/debuffs
	/// </summary>
	[PropertyCalculator(eProperty.DPS)]
	public class DPSCalculator : PropertyCalculator
	{
		public DPSCalculator() {}

		public override int CalcValue(GameLiving living, eProperty property)
		{
			return (int)(
				+living.BaseBuffBonusCategory[(int)property]
				+living.SpecBuffBonusCategory[(int)property]
				-living.DebuffCategory[(int)property]
				+living.BuffBonusCategory4[(int)property]);
		}
	}
}
