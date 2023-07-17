using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Nightshade, "Nightshade", "Stalker")]
	public class ClassNightshade : ClassStalker
	{
		private static readonly string[] AutotrainableSkills = new[] { Specs.Stealth };

		public ClassNightshade()
			: base()
		{
			m_profession = "PlayerClass.Profession.PathofEssence";
			m_specializationMultiplier = 22;
			m_primaryStat = EStat.DEX;
			m_secondaryStat = EStat.QUI;
			m_tertiaryStat = EStat.STR;
			m_manaStat = EStat.DEX;
		}

		public override bool CanUseLefthandedWeapon
		{
			get { return true; }
		}

		public override eClassType ClassType
		{
			get { return eClassType.Hybrid; }
		}

		public override IList<string> GetAutotrainableSkills()
		{
			return AutotrainableSkills;
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Elf, PlayerRace.Lurikeen,
		};
	}
}
