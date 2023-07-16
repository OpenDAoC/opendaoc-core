using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)eCharacterClass.Theurgist, "Theurgist", "Elementalist")]
	public class ClassTheurgist : ClassElementalist
	{
		public ClassTheurgist()
			: base()
		{
			m_profession = "PlayerClass.Profession.DefendersofAlbion";
			m_specializationMultiplier = 10;
			m_primaryStat = eStat.INT;
			m_secondaryStat = eStat.DEX;
			m_tertiaryStat = eStat.QUI;
			m_manaStat = eStat.INT;
		}

		public override bool HasAdvancedFromBaseClass()
		{
			return true;
		}

		public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
		{
			// PlayerRace.Avalonian, PlayerRace.Briton, PlayerRace.HalfOgre,
			PlayerRace.Avalonian, PlayerRace.Briton,
		};

		/// <summary>
		/// Releases controlled object
		/// </summary>
		public override void CommandNpcRelease()
		{
			TheurgistPet tPet = Player.TargetObject as TheurgistPet;
			if (tPet != null && tPet.Brain is TheurgistPetBrain && Player.IsControlledNPC(tPet))
			{
				Player.Notify(GameLivingEvent.PetReleased, tPet);
				return;
			}

			base.CommandNpcRelease();
		}
	}
}
