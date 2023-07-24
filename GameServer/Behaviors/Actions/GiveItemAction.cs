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
    [ActionAttribute(ActionType = EActionType.GiveItem,IsNullableQ=true)]
    public class GiveItemAction : AbstractAction<DbItemTemplates,GameNpc>
    {               

        public GiveItemAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.GiveItem, p, q)
        {                
        }


        public GiveItemAction(GameNpc defaultNPC,  DbItemTemplates itemTemplate, GameNpc itemGiver)
            : this(defaultNPC, (object) itemTemplate, (object)itemGiver) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
			InventoryItem inventoryItem = GameInventoryItem.Create(P as DbItemTemplates);

            if (Q == null)
            {

                if (!player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, inventoryItem))
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.GiveButInventFull", inventoryItem.GetName(0, false)), EChatType.CT_System, EChatLoc.CL_SystemWindow);                    
                }
                else
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.GiveItemAction.YouReceiveItem", inventoryItem.GetName(0, false)), EChatType.CT_Loot, EChatLoc.CL_SystemWindow);
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
