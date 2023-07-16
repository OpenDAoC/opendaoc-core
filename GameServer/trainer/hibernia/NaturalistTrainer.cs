using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Naturalist Trainer
	/// </summary>
	[NPCGuildScript("Naturalist Trainer", eRealm.Hibernia)]		// this attribute instructs DOL to use this script for all "Naturalist Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class NaturalistTrainer : GameTrainer
	{
		public override eCharacterClass TrainedClass
		{
			get { return eCharacterClass.Naturalist; }
		}

		public const string PRACTICE_WEAPON_ID = "training_club";
		public const string PRACTICE_SHIELD_ID = "training_shield";

		public NaturalistTrainer() : base(eChampionTrainerType.Naturalist)
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
					player.Out.SendMessage(this.Name + " says, \"You must now seek your training elsewhere. Which path would you like to follow? [Bard], [Druid] or [Warden]?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
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
				if (player.Inventory.GetFirstItemByID(PRACTICE_SHIELD_ID, eInventorySlot.MinEquipable, eInventorySlot.LastBackpack) == null)
				{
					player.Out.SendMessage(this.Name + " says, \"Do you require a [training shield]?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
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
				case "Bard":
					if(player.Race == (int) eRace.Celt || player.Race == (int) eRace.Firbolg){
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Bard is not available to your race. Please choose another.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					return true;
				case "Druid":
					if(player.Race == (int)eRace.Celt || player.Race == (int)eRace.Firbolg || player.Race == (int)eRace.Sylvan || player.Race == (int)eRace.HiberniaMinotaur)
					{
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					else{
						player.Out.SendMessage(this.Name + " says, \"The path of a Druid is not available to your race. Please choose another.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					return true;
				case "Warden":
					if(player.Race == (int) eRace.Celt || player.Race == (int) eRace.Firbolg || player.Race == (int) eRace.Sylvan)
					{
						player.Out.SendMessage(this.Name + " says, \"I can't tell you something about this class.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					else
					{
						player.Out.SendMessage(this.Name + " says, \"The path of a Warden is not available to your race. Please choose another.\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					}
					return true;
				case "practice weapon":
					if (player.Inventory.GetFirstItemByID(PRACTICE_WEAPON_ID, eInventorySlot.Min_Inv, eInventorySlot.Max_Inv) == null)
					{
						player.ReceiveItem(this,PRACTICE_WEAPON_ID);
					}
					return true;
				case "training shield":
					if (player.Inventory.GetFirstItemByID(PRACTICE_SHIELD_ID, eInventorySlot.Min_Inv, eInventorySlot.Max_Inv) == null)
					{
						player.ReceiveItem(this,PRACTICE_SHIELD_ID);
					}
					return true;
			}
			return true;
		}
	}
}
