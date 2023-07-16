using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;
using DOL.Database;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.GiveItem,IsNullableQ=true)]
    public class GiveItemAction : AbstractAction<ItemTemplate,GameNPC>
    {               

        public GiveItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.GiveItem, p, q)
        {                
        }


        public GiveItemAction(GameNPC defaultNPC,  ItemTemplate itemTemplate, GameNPC itemGiver)
            : this(defaultNPC, (object) itemTemplate, (object)itemGiver) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
			InventoryItem inventoryItem = GameInventoryItem.Create(P as ItemTemplate);

            if (Q == null)
            {

                if (!player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, inventoryItem))
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.GiveButInventFull", inventoryItem.GetName(0, false)), eChatType.CT_System, eChatLoc.CL_SystemWindow);                    
                }
                else
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.YouReceiveItem", inventoryItem.GetName(0, false)), eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
                    InventoryLogging.LogInventoryAction(Q, player, eInventoryActionType.Quest, inventoryItem.Template, inventoryItem.Count);
                }
            }
            else
            {                
                player.ReceiveItem(Q, inventoryItem);
            }            
        }
    }
}
