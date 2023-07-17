using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Mauler Trainer
	/// </summary>
	[NpcGuild("Mauler Trainer", ERealm.Hibernia)]
	public class HiberniaMaulerTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.MaulerHib; }
		}

		public const string WEAPON_ID1 = "maulerhib_item_staff";
		public const string WEAPON_ID2 = "maulerhib_item_fist";

		/// <summary>
		/// Interact with trainer
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;

			// check if class matches.
			if (player.CharacterClass.ID == (int) TrainedClass)
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.Interact.Text2", this.Name), EChatType.CT_System, EChatLoc.CL_ChatWindow);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.Interact.Text1", this.Name), EChatType.CT_Say, EChatLoc.CL_PopupWindow);

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

			if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceiveCase.Text1"))
			{
				// promote player to other class
				if (CanPromotePlayer(player))
				{
					// Mauler_Hib = 62
					PromotePlayer(player, (int)ECharacterClass.MaulerHib, LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceive.Text1"), null);
				}
			}
			else if ((player.Inventory.GetFirstItemByID(WEAPON_ID1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null) &&
			         (player.Inventory.GetFirstItemByID(WEAPON_ID2, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null))
			{
				if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceiveCase.Text2"))
				{
					player.ReceiveItem(this, WEAPON_ID1);
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceive.Text2"), EChatType.CT_Say, EChatLoc.CL_PopupWindow);
				}
				else if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceiveCase.Text3"))
				{
					player.ReceiveItem(this, WEAPON_ID2);
					player.ReceiveItem(this, WEAPON_ID2);
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "MaulerHibTrainer.WhisperReceive.Text2"), EChatType.CT_Say, EChatLoc.CL_PopupWindow);
				}
			}

			return true;
		}
	}
}
