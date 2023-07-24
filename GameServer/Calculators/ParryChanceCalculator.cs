using System;
using DOL.AI.Brain;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// The parry chance calculator. Returns 0 .. 1000 chance.
	/// 
	/// BuffBonusCategory1 unused
	/// BuffBonusCategory2 unused
	/// BuffBonusCategory3 unused
	/// BuffBonusCategory4 unused
	/// BuffBonusMultCategory1 unused
	/// </summary>
	[PropertyCalculator(EProperty.ParryChance)]
	public class ParryChanceCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, EProperty property)
		{
			
			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				int buff = player.BaseBuffBonusCategory[(int)property] * 10
				+ player.SpecBuffBonusCategory[(int)property] * 10
				- player.DebuffCategory[(int)property] * 10
				+ player.BuffBonusCategory4[(int)property] * 10
				+ player.AbilityBonus[(int)property] * 10;
				int parrySpec = 0;
				if (player.HasSpecialization(Specs.Parry))
				{					
					parrySpec = (player.Dexterity * 2 - 100) / 4 + (player.GetModifiedSpecLevel(Specs.Parry) - 1) * (10 / 2) + 50;
				}
                if (parrySpec > 500)
                {
                    parrySpec = 500;
                }
				return parrySpec + buff;
			}
			NecromancerPet pet = living as NecromancerPet;
			if (pet != null)
			{
				IControlledBrain brain = pet.Brain as IControlledBrain;
				if (brain != null)
				{
					int buff = pet.BaseBuffBonusCategory[(int)property] * 10
					+ pet.SpecBuffBonusCategory[(int)property] * 10
					- pet.DebuffCategory[(int)property] * 10
					+ pet.BuffBonusCategory4[(int)property] * 10
					+ pet.AbilityBonus[(int)property] * 10
					+ (pet.GetModified(EProperty.Dexterity) * 2 - 100) / 4
					+ pet.ParryChance * 10;
					return buff;
				}
			}			
			GameNpc npc = living as GameNpc;
			if (npc != null)
			{
				return npc.ParryChance * 10;
			}

			return 0;
		}
	}
}
