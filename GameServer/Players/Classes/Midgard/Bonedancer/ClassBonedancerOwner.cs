using DOL.AI;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS;
using DOL.GS.PlayerClass;

namespace Core.GS.Players.Classes.Bonedancer
{
	/// <summary>
	/// The Bonedancer character class.
	/// </summary>
	public class ClassBonedancerOwner : ClassMystic
	{
		/// <summary>
		/// Releases controlled object
		/// </summary>
		public override void CommandNpcRelease()
		{
			BdPet subpet = Player.TargetObject as BdPet;
			if (subpet != null && subpet.Brain is BdPetBrain && Player.ControlledBrain is BdCommanderBrain && (Player.ControlledBrain as BdCommanderBrain).FindPet(subpet.Brain as IControlledBrain))
			{
				Player.Notify(GameLivingEvent.PetReleased, subpet);
				BdCommanderPet commander = (subpet.Brain as IControlledBrain).Owner as BdCommanderPet;
				commander.RemoveControlledNpc(subpet.Brain as IControlledBrain);
				return;
			}

			base.CommandNpcRelease();
		}

		/// <summary>
		/// Add all spell-lines and other things that are new when this skill is trained
		/// </summary>
		/// <param name="player">player to modify</param>
		/// <param name="skill">The skill that is trained</param>
		public override void OnSkillTrained(GamePlayer player, Specialization skill)
		{
			base.OnSkillTrained(player, skill);

			// BD subpet spells can be scaled with the BD's spec as a cap, so when a BD
			//	trains, we have to re-scale spells for subpets from that spec.
			if (DOL.GS.ServerProperties.ServerProperties.PET_SCALE_SPELL_MAX_LEVEL > 0
				&& DOL.GS.ServerProperties.ServerProperties.PET_CAP_BD_MINION_SPELL_SCALING_BY_SPEC
				&& player.ControlledBrain != null && player.ControlledBrain.Body is GameSummonedPet pet
				&& pet.ControlledNpcList != null)
					foreach (ABrain subBrain in pet.ControlledNpcList)
						if (subBrain != null && subBrain.Body is BdSubPet subPet && subPet.PetSpecLine == skill.KeyName)
							subPet.SortSpells();
		}


	}
}
