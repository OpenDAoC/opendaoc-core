using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Valkyrie Trainer
	/// </summary>
	[NPCGuildScript("Valkyrie Trainer", ERealm.Midgard)]		// this attribute instructs DOL to use this script for all "Valkyrie Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class ValkyrieTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Valkyrie; }
		}

		public const string WEAPON_ID1 = "valkyrie_item_sword";
		public const string WEAPON_ID2 = "valkyrie_item_spear";


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
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.Interact.Text2", this.Name), eChatType.CT_System, eChatLoc.CL_ChatWindow);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.Interact.Text1", this.Name), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
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

			String lowerCase = text.ToLower();

			if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceiveCase.Text1"))
			{
				// promote player to other class
				if (CanPromotePlayer(player))
				{
					PromotePlayer(player, (int)ECharacterClass.Valkyrie, LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceive.Text1"), null);
				}
			}
			else if ((player.Inventory.GetFirstItemByID(WEAPON_ID1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null) &&
			         (player.Inventory.GetFirstItemByID(WEAPON_ID2, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null))
			{
				if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceiveCase.Text2"))
				{
					player.ReceiveItem(this, WEAPON_ID1);
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceive.Text2"), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
				}
				else if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceiveCase.Text3"))
				{
					player.ReceiveItem(this, WEAPON_ID2);
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "ValkyrieTrainer.WhisperReceive.Text2"), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
				}
			}
			return true;
		}
	}
}
