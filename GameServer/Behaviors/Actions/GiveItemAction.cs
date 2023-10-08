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
using DOL.Database;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions
{
    [Action(ActionType = eActionType.GiveItem,IsNullableQ=true)]
    public class GiveItemAction : AbstractAction<DbItemTemplate,GameNPC>
    {               

        public GiveItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.GiveItem, p, q)
        {                
        }


        public GiveItemAction(GameNPC defaultNPC,  DbItemTemplate itemTemplate, GameNPC itemGiver)
            : this(defaultNPC, (object) itemTemplate, (object)itemGiver) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
			DbInventoryItem inventoryItem = GameInventoryItem.Create(P as DbItemTemplate);

            if (Q == null)
            {

                if (!player.Inventory.AddItem(EInventorySlot.FirstEmptyBackpack, inventoryItem))
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.GiveButInventFull", inventoryItem.GetName(0, false)), EChatType.CT_System, EChatLoc.CL_SystemWindow);                    
                }
                else
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.YouReceiveItem", inventoryItem.GetName(0, false)), EChatType.CT_Loot, EChatLoc.CL_SystemWindow);
                    InventoryLogging.LogInventoryAction(Q, player, EInventoryActionType.Quest, inventoryItem.Template, inventoryItem.Count);
                }
            }
            else
            {                
                player.ReceiveItem(Q, inventoryItem);
            }            
        }
    }
}
