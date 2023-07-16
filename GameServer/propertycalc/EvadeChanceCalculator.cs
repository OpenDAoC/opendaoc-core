using System;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// The evade chance calculator. Returns 0 .. 1000 chance.
	/// 
	/// BuffBonusCategory1 unused
	/// BuffBonusCategory2 unused
	/// BuffBonusCategory3 unused
	/// BuffBonusCategory4 unused
	/// BuffBonusMultCategory1 unused
	/// </summary>
	[PropertyCalculator(eProperty.EvadeChance)]
	public class EvadeChanceCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, eProperty property)
		{
			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				int evadechance = 0;
				if (player.HasAbility(Abilities.Evade))
					evadechance += (int)(((((player.Dexterity + player.Quickness) / 2 - 50) * 0.05) + player.GetAbilityLevel(Abilities.Evade) * 5) * 10);
				
					
				evadechance += player.BaseBuffBonusCategory[(int)property] * 10
								+ player.SpecBuffBonusCategory[(int)property] * 10
								- player.DebuffCategory[(int)property] * 10
								+ player.BuffBonusCategory4[(int)property] * 10
								+ player.AbilityBonus[(int)property] * 10;
				return evadechance;
			}

			GameNPC npc = living as GameNPC;
			if (npc != null)
			{
				return living.AbilityBonus[(int)property] * 10 + npc.EvadeChance * 10;
			}

			return 0;
		}
	}
}
