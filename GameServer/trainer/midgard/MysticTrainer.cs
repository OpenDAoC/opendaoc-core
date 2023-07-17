using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Mystic Trainer
	/// </summary>
	[NPCGuildScript("Mystic Trainer", ERealm.Midgard)]		// this attribute instructs DOL to use this script for all "Mystic Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class MysticTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Mystic; }
		}

		public const string PRACTICE_WEAPON_ID = "trimmed_branch";
		
		public MysticTrainer() : base(eChampionTrainerType.Mystic)
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
					player.Out.SendMessage(this.Name + " says, \"You must now seek your training elsewhere. Which path would you like to follow? [Runemaster] or [Spiritmaster]?\"", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
				}
				else
				{
					OfferTraining(player);
				}

				// ask for basic equipment if player doesnt own it
				if (player.Inventory.GetFirstItemByID(PRACTICE_WEAPON_ID, eInventorySlot.MinEquipable, eInventorySlot.LastBackpack) == null)
				{
					player.Out.SendMessage(this.Name + " says, \"Do you require a [practice branch]?\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
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
				case "Runemaster":
					if(player.Race == (int) ERace.Frostalf || player.Race == (int) ERace.Kobold || player.Race == (int) ERace.Norseman || player.Race == (int) ERace.Dwarf){
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Runemaster is not available to your race. Please choose another.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					return true;
				case "Spiritmaster":
					if(player.Race == (int) ERace.Kobold || player.Race == (int) ERace.Frostalf || player.Race == (int) ERace.Norseman){
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Spiritmaster is not available to your race. Please choose another.\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
					}
					return true;
				case "practice branch":
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
