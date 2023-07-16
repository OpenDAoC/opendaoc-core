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
    [ActionAttribute(ActionType = eActionType.DropItem)]
    public class DropItemAction : AbstractAction<ItemTemplate,Unused>
    {               

        public DropItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.DropItem, p, q)
        {                
        }


        public DropItemAction(GameNPC defaultNPC, ItemTemplate itemTemplate)
            : this(defaultNPC, (object) itemTemplate,(object) null) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
			InventoryItem inventoryItem = GameInventoryItem.Create(P as ItemTemplate);

            player.CreateItemOnTheGround(inventoryItem);
            player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.DropItemAction.DropsFrontYou", inventoryItem.Name), eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
        }
    }
}
