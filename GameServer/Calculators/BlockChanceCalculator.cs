using System;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// The block chance calculator. Returns 0 .. 1000 chance.
	/// 
	/// BuffBonusCategory1 unused
	/// BuffBonusCategory2 unused
	/// BuffBonusCategory3 unused
	/// BuffBonusCategory4 unused
	/// BuffBonusMultCategory1 unused
	/// </summary>
	[PropertyCalculator(EProperty.BlockChance)]
	public class BlockChanceCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				int shield = (player.GetModifiedSpecLevel(Specs.Shields) - 1) * (10 / 2);
				int ability = player.AbilityBonus[(int)property] * 10;
				int chance = 50 + shield + ((player.Dexterity * 2 - 100) / 4) + ability;
				
                return chance;
			}

			GameNPC npc = living as GameNPC;
			if (npc != null)
			{
				return npc.BlockChance * 10;
			}

			return 0;
		}
	}
}
