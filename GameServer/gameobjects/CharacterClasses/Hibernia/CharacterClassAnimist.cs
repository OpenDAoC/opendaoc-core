using System;

using DOL.GS;
using DOL.Events;
using DOL.AI.Brain;
using DOL.GS.PlayerClass;

namespace DOL.GS
{
	/// <summary>
	/// The Animist character class.
	/// </summary>
	public class CharacterClassAnimist : ClassForester
	{
		/// <summary>
		/// Releases controlled object
		/// </summary>
		public override void CommandNpcRelease()
		{
			TurretPet turretFnF = Player.TargetObject as TurretPet;
			if (turretFnF != null && turretFnF.Brain is TurretFNFBrain && Player.IsControlledNPC(turretFnF))
			{
				Player.Notify(GameLivingEvent.PetReleased, turretFnF);
				return;
			}

			base.CommandNpcRelease();
		}
	}
}
