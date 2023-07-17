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
    [ActionAttribute(ActionType = EActionType.DropItem)]
    public class DropItemAction : AbstractAction<DbItemTemplates,Unused>
    {               

        public DropItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.DropItem, p, q)
        {                
        }


        public DropItemAction(GameNPC defaultNPC, DbItemTemplates itemTemplate)
            : this(defaultNPC, (object) itemTemplate,(object) null) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
			InventoryItem inventoryItem = GameInventoryItem.Create(P as DbItemTemplates);

            player.CreateItemOnTheGround(inventoryItem);
            player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.DropItemAction.DropsFrontYou", inventoryItem.Name), EChatType.CT_Loot, EChatLoc.CL_SystemWindow);
        }
    }
}
