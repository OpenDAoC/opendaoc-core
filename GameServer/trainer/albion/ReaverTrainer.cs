using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Reaver Trainer
	/// </summary>
	[NpcGuild("Reaver Trainer", ERealm.Albion)]		// this attribute instructs DOL to use this script for all "Reaver Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class ReaverTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Reaver; }
		}

		public ReaverTrainer() : base()
		{
		}

		/// <summary>
		/// Interact with trainer
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;
			
			// check if class matches.
			if (player.CharacterClass.ID == (int)TrainedClass)
			{
				OfferTraining(player);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(this.Name + " says, \"You have come to seek admittance into the [Temple of Arawn] to worship the old god that your ancestors worshipped?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					if (!player.IsLevelRespecUsed)
					{
						OfferRespecialize(player);
					}
				}
				else
				{
					CheckChampionTraining(player);
				}
			}
			return true;
		}

		/// <summary>
		/// Talk to trainer
		/// </summary>
		/// <param name="source"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public override bool WhisperReceive(GameLiving source, string text)
		{
			if (!base.WhisperReceive(source, text)) return false;
			GamePlayer player = source as GamePlayer;
			
			switch (text) {
				case "Temple of Arawn":
					// promote player to other class
					if (CanPromotePlayer(player))
					{
						//TODO:
						//##Melarlian says, "You have come to seek admittance into the [Temple of Arawn] to worship the old god that your ancestors worshipped?"
						//##Melarlian says, "Very well then. Choose your weapon, and it shall be done. Know that once this choice is made, there is no return. You may choose a flexible [slashing] or a flexible [crushing] weapon?"
						//##Melarlian says, "Here is your Whip of the Initiate. Welcome to the Temple of Arawn, Calaoron."
						PromotePlayer(player, (int)ECharacterClass.Reaver, "Welcome to the Temple of Arawn, " + player.Name + ".", null);	// TODO: gifts
					}
					break;
			}
			return true;
		}
	}
}

