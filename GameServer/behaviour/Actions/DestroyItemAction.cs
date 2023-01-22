/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;
using DOL.Database;
using System.Collections;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions;

[ActionAttribute(ActionType = eActionType.DestroyItem, DefaultValueQ = 1)]
public class DestroyItemAction : AbstractAction<ItemTemplate, int>
{
    public DestroyItemAction(GameNPC defaultNPC, object p, object q)
        : base(defaultNPC, eActionType.DestroyItem, p, q)
    {
    }


    public DestroyItemAction(GameNPC defaultNPC, ItemTemplate itemTemplate, int quantity)
        : this(defaultNPC, (object) itemTemplate, (object) quantity)
    {
    }


    public override void Perform(DOLEvent e, object sender, EventArgs args)
    {
        var player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
        var count = Q;
        var itemToDestroy = P;

        var dataSlots = new Dictionary<InventoryItem, int?>(10);
        lock (player.Inventory)
        {
            var allBackpackItems =
                player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);

            var result = false;
            foreach (var item in allBackpackItems)
                if (item.Name == itemToDestroy.Name)
                {
                    if (item.IsStackable) // is the item is stackable
                    {
                        if (item.Count >= count)
                        {
                            if (item.Count == count)
                                dataSlots.Add(item, null);
                            else
                                dataSlots.Add(item, count);

                            result = true;
                            break;
                        }
                        else
                        {
                            dataSlots.Add(item, null);
                            count -= item.Count;
                        }
                    }
                    else
                    {
                        dataSlots.Add(item, null);
                        if (count <= 1)
                        {
                            result = true;
                            break;
                        }
                        else
                        {
                            count--;
                        }
                    }
                }

            if (result == false) return;
        }

        var playerInventory = player.Inventory as GamePlayerInventory;
        playerInventory.BeginChanges();
        var enumerator = dataSlots.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var de = enumerator.Current;
            if (!de.Value.HasValue)
            {
                playerInventory.RemoveItem(de.Key);
                InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, de.Key.Template,
                    de.Key.Count);
            }
            else
            {
                playerInventory.RemoveCountFromStack(de.Key, de.Value.Value);
                InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, de.Key.Template,
                    de.Value.Value);
            }
        }

        playerInventory.CommitChanges();


        player.Out.SendMessage(
            LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.DestroyItemAction.Destroyed",
                itemToDestroy.Name), eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
    }
}