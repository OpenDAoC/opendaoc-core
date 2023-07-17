using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Eldritch, "Eldritch", "Magician")]
	public class ClassEldritch : ClassMagician
	{
		public ClassEldritch()
			: base()
		{
			m_profession = "PlayerClass.Profession.PathofFocus";
			m_specializationMultiplier = 10;
			m_primaryStat = EStat.INT;
			m_secondaryStat = EStat.DEX;
			m_tertiaryStat = EStat.QUI;
			m_manaStat = EStat.INT;
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
