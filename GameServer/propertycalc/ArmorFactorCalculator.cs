using System;
using System.Linq;
using DOL.GS.Keeps;
using log4net.Core;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// The Armor Factor calculator
	///
	/// BuffBonusCategory1 is used for base buffs directly in player.GetArmorAF because it must be capped by item AF cap
	/// BuffBonusCategory2 is used for spec buffs, level*1.875 cap for players
	/// BuffBonusCategory3 is used for debuff, uncapped
	/// BuffBonusCategory4 is used for buffs, uncapped
	/// BuffBonusMultCategory1 unused
	/// ItemBonus is used for players TOA bonuse, living.Level cap
	/// </summary>
	[PropertyCalculator(eProperty.ArmorFactor)]
	public class ArmorFactorCalculator : PropertyCalculator
	{
		public override int CalcValue(GameLiving living, eProperty property)
		{
			if (living is GamePlayer || living is GameTrainingDummy)
			{
				int af;

				//base AF buffs are calculated in the item's armor calc since they have the same cap
				//af = living.BaseBuffBonusCategory[(int) property];
				// 1.5*1.25 spec line buff cap
				af = Math.Min((int)(living.Level * 1.875), living.SpecBuffBonusCategory[(int)property]);
				// debuff
				af -= Math.Abs(living.DebuffCategory[(int)property]);
				// TrialsOfAtlantis af bonus
				af += Math.Min(living.Level, living.ItemBonus[(int)property]);
				// uncapped category
				af += living.BuffBonusCategory4[(int)property];
				
				// buffs should be spread across each armor piece since the damage calculation is based on piece hit
				if(living is GamePlayer)
					af /= 6;

				return af;
			}
			else if (living is GameKeepDoor || living is GameKeepComponent)
			{
				GameKeepComponent component = null;
				if (living is GameKeepDoor)
					component = (living as GameKeepDoor).Component;
				if (living is GameKeepComponent)
					component = living as GameKeepComponent;

				if (component == null) return 1;
				
				double keepLevelMod = 1 + component.Keep.Level * .1;

				int amount = (int)(component.Keep.BaseLevel * 4 * keepLevelMod);
				if (component.Keep is GameKeep)
					return amount;
				else return amount / 2;
			}
			else if (living is GameEpicNPC epic || living is GameEpicBoss)
			{
				GameLiving bossnpc = living;
				double epicScaleFactor = 8;
				int petCap = 16;
				int petCount = 0;

				if(bossnpc is GameEpicBoss)
                {
					epicScaleFactor *= 2;
					petCap = 24;
                }

				var attackerList = bossnpc.attackComponent.Attackers.ToList();

                foreach (var attacker in attackerList)
                {
					if(attacker is GamePlayer)
						epicScaleFactor -= 0.4;
					if (attacker is GameSummonedPet && petCount <= petCap)
                    {
						epicScaleFactor -= 0.1;
						petCount++;
					}
						
				}

				if (epicScaleFactor < 4)
					epicScaleFactor = 4;

				epicScaleFactor *= .1;

				return (int)((1 + (living.Level / 50.0)) * (living.Level << 1) * epicScaleFactor) 
				+ living.SpecBuffBonusCategory[(int)property]
				- Math.Abs(living.DebuffCategory[(int)property])/6
				+ living.BuffBonusCategory4[(int)property];
			}
			else if (living is GameSummonedPet)
			{
				int baseVal = (int)((1 + (living.Level / 175.0)) * (living.Level << 1))
				              + (living.BaseBuffBonusCategory[(int)property] / 6)
				              + (living.SpecBuffBonusCategory[(int)property] / 6)
				              - Math.Abs(living.DebuffCategory[(int)property])/6
				              + living.BuffBonusCategory4[(int)property] / 6;

				if (living is NecromancerPet)
					baseVal += 10;

				return baseVal;
			}
			else
			{
				int baseVal = (int)((1 + (living.Level / 200.0)) * (living.Level << 1))
				+ (living.SpecBuffBonusCategory[(int)property] / 6)
				- Math.Abs(living.DebuffCategory[(int)property])/6
				+ living.BuffBonusCategory4[(int)property] / 6;

				if (living is GuardLord)
					baseVal += 20 * living.Level / 50;
				
				return baseVal;
			}
		}
	}
}
