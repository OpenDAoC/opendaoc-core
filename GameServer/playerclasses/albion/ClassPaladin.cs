using DOL.GS.Realm;
using System.Collections.Generic;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Paladin, "Paladin", "Fighter")]
	public class ClassPaladin : ClassFighter
	{
		private static readonly string[] AutotrainableSkills = new[] { Specs.Slash, Specs.Chants };

		public ClassPaladin()
			: base()
		{
			m_profession = "PlayerClass.Profession.ChurchofAlbion";
			m_specializationMultiplier = 25; //atlas increased from 20
			m_primaryStat = EStat.CON;
			m_secondaryStat = EStat.PIE;
			m_tertiaryStat = EStat.STR;
			m_manaStat = EStat.PIE;
			m_wsbase = 380;
			m_baseHP = 760;
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

		public override ushort MaxPulsingSpells
		{
			get { return 1; } //atlas reduced from 2
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Avalonian, PlayerRace.Briton, PlayerRace.Highlander, PlayerRace.Saracen,
		};
	}
}
