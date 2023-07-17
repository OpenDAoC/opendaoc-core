using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Elementalist Trainer
	/// </summary>
	[NPCGuildScript("Elementalist Trainer", ERealm.Albion)]		// this attribute instructs DOL to use this script for all "Elementalist Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class ElementalistTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Elementalist; }
		}

		public const string PRACTICE_WEAPON_ID = "trimmed_branch";
		

		public ElementalistTrainer() : base(eChampionTrainerType.Elementalist)
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
			if (player.CharacterClass.ID == (int)TrainedClass)
			{
				// player can be promoted
				if (player.Level>=5)
				{
					player.Out.SendMessage(this.Name + " says, \"You must now seek your training elsewhere. Which path would you like to follow? [Theurgist] or [Wizard]?\"", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
				}
				else
				{
					OfferTraining(player);
				}

				// ask for basic equipment if player doesnt own it
				if (player.Inventory.GetFirstItemByID(PRACTICE_WEAPON_ID, eInventorySlot.MinEquipable, eInventorySlot.LastBackpack) == null)
				{
					player.Out.SendMessage(this.Name + " says, \"Do you require a [practice weapon]?\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
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
				case "Theurgist":
					if(player.Race == (int) ERace.Briton || player.Race == (int) ERace.Avalonian || player.Race == (int) ERace.HalfOgre){
						player.Out.SendMessage(this.Name + " says, \"You wish to study the art of magical enchantments do you? The Defenders of Albion rely immensely on this ability and their art of building and animating creatures that can fight and protect the army while in battle.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Theurgist is not available to your race. Please choose another.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					return true;
				case "Wizard":
					if(player.Race == (int) ERace.Briton || player.Race == (int) ERace.Avalonian || player.Race == (int) ERace.HalfOgre){
						player.Out.SendMessage(this.Name + " says, \"I see you wish to specialize in molding the four elements of fire, ice, earth, and air to create magical spells of immense power. Even now many of The Academy well-trained Wizards rain destruction upon our enemies.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Wizard is not available to your race. Please choose another.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
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
