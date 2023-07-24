using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[PlayerClass((int)ECharacterClass.Bard, "Bard", "Naturalist")]
	public class ClassBard : ClassNaturalist
	{
		public ClassBard()
			: base()
		{
			m_profession = "PlayerClass.Profession.PathofEssence";
			m_specializationMultiplier = 15;
			m_primaryStat = EStat.CHR;
			m_secondaryStat = EStat.EMP;
			m_tertiaryStat = EStat.CON;
			m_manaStat = EStat.CHR;
			m_wsbase = 360;
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override ushort MaxPulsingSpells
		{
			get { return 1; } //atlas down from 2
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Celt, PlayerRace.Firbolg,
		};
	}
}
