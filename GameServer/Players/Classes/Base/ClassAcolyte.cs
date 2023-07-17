using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Acolyte, "Acolyte", "Acolyte")]
	public class ClassAcolyte : CharacterClassBase
	{
		public ClassAcolyte()
			: base()
		{
			m_specializationMultiplier = 10;
			m_wsbase = 320;
			m_baseHP = 720;
			m_manaStat = EStat.PIE;
		}

		public override string GetTitle(GamePlayer player, int level)
		{
			return HasAdvancedFromBaseClass() ? base.GetTitle(player, level) : base.GetTitle(player, 0);
		}

		public override eClassType ClassType
		{
			get { return eClassType.Hybrid; }
		}

        public override GameTrainer.eChampionTrainerType ChampionTrainerType()
		{
			return GameTrainer.eChampionTrainerType.Acolyte;
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return false;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			PlayerRace.Korazh, PlayerRace.Avalonian, PlayerRace.Briton, PlayerRace.Highlander, PlayerRace.Inconnu,
		};
	}
}
