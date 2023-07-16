using System;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS
{
	[NPCGuildScript("Guild Emblemeer")]
	public class EmblemNPC : GameNPC
	{
		public const long EMBLEM_COST = 50000;
		private const string EMBLEMIZE_ITEM_WEAK = "emblemise item";

		/// <summary>
		/// Can accept any item
		/// </summary>
		public override bool CanTradeAnyItem
		{
			get { return true; }
		}

		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			TurnTo(player, 5000);
			
			// Check for ambient trigger messages for the NPC in the 'MobXAmbientBehaviour' table
			var triggers = GameServer.Instance.NpcManager.AmbientBehaviour[base.Name];
			// If the NPC has no ambient trigger message assigned, then return this message
			if (triggers == null || triggers.Length == 0)
				SayTo(player, eChatLoc.CL_ChatWindow, "For 5 gold, I can put the emblem of your guild on the item. Just hand me the item.");

			return true;
		}

		public override bool ReceiveItem(GameLiving source, InventoryItem item)
		{
			GamePlayer t = source as GamePlayer;
			if (t == null || item == null)
				return false;
			
			if (item.Emblem != 0)
			{
				t.Out.SendMessage("This item already has an emblem on it.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if (item.Object_Type == (int) eObjectType.Shield
				|| item.Item_Type == Slot.CLOAK)
			{
				if (t.Guild == null)
				{
					t.Out.SendMessage("You have no guild.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return false;
				}
				if (t.Guild.Emblem == 0)
				{
					t.Out.SendMessage("Your guild has no emblem.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return false;
				}
				if (t.Level < 20) //if level of player < 20 so can not put emblem
				{
					if (t.CraftingPrimarySkill == eCraftingSkill.NoCrafting)
					{
						t.Out.SendMessage("You have to be at least level 20 or have 400 in a tradeskill to be able to wear an emblem.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						return false;
					}
					else
					{
						if (t.GetCraftingSkillValue(t.CraftingPrimarySkill) < 400)
						{
							t.Out.SendMessage("You have to be at least level 20 or have 400 in a tradeskill to be able to wear an emblem.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return false;
						}
					}

				}

				if (!t.Guild.HasRank(t, Guild.eRank.Emblem))
				{
					t.Out.SendMessage("You do not have enough privileges for that.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return false;
				}
				t.TempProperties.setProperty(EMBLEMIZE_ITEM_WEAK, new WeakRef(item));
				t.Out.SendCustomDialog("Do you agree to put an emblem on this object?", new CustomDialogResponse(EmblemerDialogResponse));
			}
			else
				t.Out.SendMessage("I can not put an emblem on this item.", eChatType.CT_System, eChatLoc.CL_SystemWindow);

			return false;
		}

		protected void EmblemerDialogResponse(GamePlayer player, byte response)
		{
			WeakReference itemWeak =
				(WeakReference) player.TempProperties.getProperty<object>(
					EMBLEMIZE_ITEM_WEAK,
					new WeakRef(null)
					);
			player.TempProperties.removeProperty(EMBLEMIZE_ITEM_WEAK);

			if (response != 0x01)
				return; //declined

			InventoryItem item = (InventoryItem) itemWeak.Target;

			if (item == null || item.SlotPosition == (int) eInventorySlot.Ground
				|| item.OwnerID == null || item.OwnerID != player.InternalID)
			{
				player.Out.SendMessage("Invalid item.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (!player.RemoveMoney(EMBLEM_COST))
			{
                InventoryLogging.LogInventoryAction(player, this, eInventoryActionType.Merchant, EMBLEM_COST);
				player.Out.SendMessage("You don't have enough money.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			item.Emblem = player.Guild.Emblem;
			player.Out.SendInventoryItemsUpdate(new InventoryItem[] {item});
			if (item.SlotPosition < (int) eInventorySlot.FirstBackpack)
				player.UpdateEquipmentAppearance();
			SayTo(player, eChatLoc.CL_ChatWindow, "I have put an emblem on your item.");
			return;
		}
	}
}