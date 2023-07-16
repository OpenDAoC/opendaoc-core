using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)eCharacterClass.Bonedancer, "Bonedancer", "Mystic")]
	public class ClassBonedancer : CharacterClassBoneDancer
	{
		public ClassBonedancer()
			: base()
		{
			m_specializationMultiplier = 10;
			m_wsbase = 280;
			m_baseHP = 560;
			m_manaStat = eStat.PIE;

			m_profession = "PlayerClass.Profession.HouseofBodgar";
			m_primaryStat = eStat.PIE;
			m_secondaryStat = eStat.DEX;
			m_tertiaryStat = eStat.QUI;
		}

		public override eClassType ClassType
		{
			get { return eClassType.ListCaster; }
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Kobold, PlayerRace.Troll, PlayerRace.Valkyn,
		};
	}
}
