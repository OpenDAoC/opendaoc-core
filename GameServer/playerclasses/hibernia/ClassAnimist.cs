using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)eCharacterClass.Animist, "Animist", "Forester")]
	public class ClassAnimist : CharacterClassAnimist
	{
		public ClassAnimist()
			: base()
		{
			m_specializationMultiplier = 10;
			m_wsbase = 280;
			m_baseHP = 560;
			m_manaStat = eStat.INT;

			m_profession = "PlayerClass.Profession.PathofAffinity";
			m_primaryStat = eStat.INT;
			m_secondaryStat = eStat.CON;
			m_tertiaryStat = eStat.DEX;
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
			 PlayerRace.Celt, PlayerRace.Firbolg, PlayerRace.Sylvan,
		};
	}
}
