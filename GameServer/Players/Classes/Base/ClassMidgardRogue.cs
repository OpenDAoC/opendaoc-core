using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[PlayerClass((int)ECharacterClass.MidgardRogue, "Rogue", "Rogue")]
	public class ClassMidgardRogue : PlayerClassBase
	{
		public ClassMidgardRogue()
			: base()
		{
			m_specializationMultiplier = 10;
			m_wsbase = 360; 
			m_baseHP = 720;
		}

		public override string GetTitle(GamePlayer player, int level)
		{
			return HasAdvancedFromBaseClass() ? base.GetTitle(player, level) : base.GetTitle(player, 0);
		}

		public override eClassType ClassType
		{
			get { return eClassType.PureTank; }
		}

		public override GameTrainer.eChampionTrainerType ChampionTrainerType()
		{
			return GameTrainer.eChampionTrainerType.MidgardRogue;
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return false;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			 PlayerRace.Dwarf, PlayerRace.Frostalf, PlayerRace.Kobold, PlayerRace.Norseman, PlayerRace.Valkyn,
		};
	}
}
