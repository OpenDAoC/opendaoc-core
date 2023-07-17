using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
	[CharacterClass((int)ECharacterClass.Theurgist, "Theurgist", "Elementalist")]
	public class ClassTheurgist : ClassElementalist
	{
		public ClassTheurgist()
			: base()
		{
			m_profession = "PlayerClass.Profession.DefendersofAlbion";
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
