using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Forester Trainer
	/// </summary>
	[NpcGuild("Forester Trainer", ERealm.Hibernia)]		// this attribute instructs DOL to use this script for all "Forester Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class ForesterTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Forester; }
		}

		public const string PRACTICE_WEAPON_ID = "training_staff";
		
		public ForesterTrainer() : base(eChampionTrainerType.Forester)
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

			// check if class matches
			if (player.CharacterClass.ID == (int) TrainedClass)
			{
				// player can be promoted
				if (player.Level>=5)
				{
					player.Out.SendMessage(this.Name + " says, \"You must now seek your training elsewhere. Which path would you like to follow? [Animist] or [Valewalker]?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
				}
				else
				{
					OfferTraining(player);
				}

				// ask for basic equipment if player doesnt own it
				if (player.Inventory.GetFirstItemByID(PRACTICE_WEAPON_ID, eInventorySlot.MinEquipable, eInventorySlot.LastBackpack) == null)
				{
					player.Out.SendMessage(this.Name + " says, \"Do you require a [practice weapon]?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
				}
				
			}
			else
			{
				CheckChampionTraining(player);
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
				case "Animist":
					if(player.Race == (int) ERace.Celt || player.Race == (int) ERace.Firbolg || player.Race == (int) ERace.Sylvan){
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Animist is not available to your race. Please choose another.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					return true;
				case "Valewalker":
					if(player.Race == (int) ERace.Celt || player.Race == (int) ERace.Firbolg || player.Race == (int) ERace.Sylvan){
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Valewalker is not available to your race. Please choose another.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					return true;
				case "practice weapon":
					if (player.Inventory.GetFirstItemByID(PRACTICE_WEAPON_ID, eInventorySlot.Min_Inv, eInventorySlot.Max_Inv) == null)
					{
						player.ReceiveItem(this,PRACTICE_WEAPON_ID);
					}
					return true;
					
			}
			return true;
		}
	}
}
