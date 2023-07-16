using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.TakeGold)]
    public class TakeGoldAction : AbstractAction<long,Unused>
    {               

        public TakeGoldAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.TakeGold, p, q)
        {                
        }


        public TakeGoldAction(GameNPC defaultNPC, long p)
            : this(defaultNPC, (object)p, (object)null) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            player.RemoveMoney(P);
            InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, P);
        }
    }
}
