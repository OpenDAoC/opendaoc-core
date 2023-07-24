using System.Collections.Generic;
using Core.GS.Players.Classes.Animist;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[PlayerClass((int)ECharacterClass.Animist, "Animist", "Forester")]
	public class ClassAnimist : ClassAnimistOwner
	{
		public ClassAnimist()
			: base()
		{
			m_specializationMultiplier = 10;
			m_wsbase = 280;
			m_baseHP = 560;
			m_manaStat = EStat.INT;

			m_profession = "PlayerClass.Profession.PathofAffinity";
			m_primaryStat = EStat.INT;
			m_secondaryStat = EStat.CON;
			m_tertiaryStat = EStat.DEX;
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
