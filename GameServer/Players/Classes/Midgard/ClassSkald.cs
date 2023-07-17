using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Skald, "Skald", "Viking")]
	public class ClassSkald : ClassViking
	{
		public ClassSkald()
			: base()
		{
			m_profession = "PlayerClass.Profession.HouseofBragi";
			m_specializationMultiplier = 15;
			m_primaryStat = EStat.CHR;
			m_secondaryStat = EStat.STR;
			m_tertiaryStat = EStat.CON;
			m_manaStat = EStat.CHR;
			m_wsbase = 380;
			m_baseHP = 760;
		}

		public override eClassType ClassType
		{
			get { return eClassType.Hybrid; }
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override ushort MaxPulsingSpells
		{
			get { return 1; } //atlas down from 1
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Dwarf, PlayerRace.Kobold, PlayerRace.Norseman, PlayerRace.Troll,
		};
	}
}
