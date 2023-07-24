using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[PlayerClass((int)ECharacterClass.Valkyrie, "Valkyrie", "Viking")]
	public class ClassValkyrie : ClassViking
	{
		public ClassValkyrie()
			: base()
		{
			m_profession = "PlayerClass.Profession.HouseofOdin";
			m_specializationMultiplier = 20;
			m_primaryStat = EStat.CON;
			m_secondaryStat = EStat.STR;
			m_tertiaryStat = EStat.DEX;
			m_manaStat = EStat.PIE;
			m_wsbase = 360;
			m_baseHP = 720;
		}

		public override eClassType ClassType
		{
			get { return eClassType.Hybrid; }
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			// PlayerRace.Dwarf, PlayerRace.Frostalf, PlayerRace.Norseman,
		};
	}
}
