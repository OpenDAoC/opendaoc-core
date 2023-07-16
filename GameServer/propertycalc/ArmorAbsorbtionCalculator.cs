using System;
using System.Linq;

namespace DOL.GS.PropertyCalc
{
	[PropertyCalculator(eProperty.ArmorAbsorption)]
	public class ArmorAbsorptionCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, eProperty property)
		{
			int buffBonus = living.BaseBuffBonusCategory[property];
			int debuffMalus = Math.Abs(living.DebuffCategory[property]);
			int itemBonus = living.ItemBonus[property];
			int abilityBonus = living.AbilityBonus[property];
			int hardCap = 50;
			if (living is GameSummonedPet)
			{
				buffBonus += living.effectListComponent.GetAllEffects().Count(e => e is ECSGameSpellEffect spellEffect && spellEffect.SpellHandler.Spell.IsBuff) * 4;
			}
			return Math.Min(hardCap, (buffBonus - debuffMalus + itemBonus + abilityBonus));
		}
	}
}
