using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Sorcerer Trainer
	/// </summary>
	[NPCGuildScript("Sorcerer Trainer", ERealm.Albion)]		// this attribute instructs DOL to use this script for all "Sorcerer Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class SorcererTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Sorcerer; }
		}

		public const string WEAPON_ID = "sorcerer_item";

		public SorcererTrainer() : base()
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
					player.Out.SendMessage(this.Name + " says, \"Is it your wish to [join the Academy] and lend us your power as a Sorcerer?\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
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
				case "join the Academy":
					// promote player to other class
					if (CanPromotePlayer(player)) {
						PromotePlayer(player, (int)ECharacterClass.Sorcerer, "You are now part of our shadow! You shall forever have a place among us! Here too is your guild weapon, a Staff of Focus!", null);
						player.ReceiveItem(this,WEAPON_ID);
					}
					break;
			}
			return true;
		}
	}
}
