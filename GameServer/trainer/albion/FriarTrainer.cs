using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Friar Trainer
	/// </summary>
	[NPCGuildScript("Friar Trainer", ERealm.Albion)]		// this attribute instructs DOL to use this script for all "Friar Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class FriarTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Friar; }
		}

		/// <summary>
		/// The free starter armor from trainer
		/// </summary>
		public const string ARMOR_ID1 = "friar_item";
		public const string ARMOR_ID2 = "chaplains_robe";
		public const string ARMOR_ID3 = "robes_of_the_neophyte";

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
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.Interact.Text2", this.Name), eChatType.CT_System, eChatLoc.CL_ChatWindow);

				if (player.Level >= 10 && player.Level < 15)
				{
					if (player.Inventory.GetFirstItemByID(ARMOR_ID3, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null)
					{
						player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.Interact.Text4", this.Name), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
						addGift(ARMOR_ID3, player);
					}
					if (player.Inventory.GetFirstItemByID(ARMOR_ID1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null)
					{}
					else
					{
						player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.Interact.Text3", this.Name), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
					}
				}
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.Interact.Text1", this.Name, player.CharacterClass.Name), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
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

			if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.WhisperReceiveCase.Text1"))
			{
				// promote player to other class
				if (CanPromotePlayer(player))
				{
					PromotePlayer(player, (int)ECharacterClass.Friar, LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.WhisperReceive.Text1"), null);
					addGift(ARMOR_ID1, player);
				}
			}
			return true;
		}

		/// <summary>
		/// For Recieving Friar Item.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool ReceiveItem(GameLiving source, InventoryItem item)
		{
			if (source == null || item == null) return false;

			GamePlayer player = source as GamePlayer;

			if (player.Level >= 10 && player.Level < 15 && item.Id_nb == ARMOR_ID1)
			{
				player.Inventory.RemoveCountFromStack(item, 1);
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FriarTrainer.ReceiveItem.Text1", this.Name, player.Name), eChatType.CT_Say, eChatLoc.CL_PopupWindow);
				addGift(ARMOR_ID2, player);
			}
			return base.ReceiveItem(source, item);
		}
	}
}
