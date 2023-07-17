using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.ReplaceItem)]
    public class ReplaceItemAction : AbstractAction<DbItemTemplates,DbItemTemplates>
    {               

        public ReplaceItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.ReplaceItem, p, q)
        {                
        }


        public ReplaceItemAction(GameNPC defaultNPC,  DbItemTemplates oldItemTemplate, DbItemTemplates newItemTemplate)
            : this(defaultNPC, (object) oldItemTemplate,(object) newItemTemplate) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);

            DbItemTemplates oldItem = P;
            DbItemTemplates newItem = Q;

            //TODO: what about stacked items???
            if (player.Inventory.RemoveTemplate(oldItem.Id_nb, 1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
            {
                InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, oldItem, 1);
				InventoryItem inventoryItem = GameInventoryItem.Create(newItem);
                if (player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, inventoryItem))
                    InventoryLogging.LogInventoryAction(NPC, player, eInventoryActionType.Quest, newItem, 1);
            }
        }
    }
}
