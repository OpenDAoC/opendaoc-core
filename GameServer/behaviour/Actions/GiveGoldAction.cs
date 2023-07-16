using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.GiveGold)]
    public class GiveGoldAction : AbstractAction<long,Unused>
    {               

        public GiveGoldAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.GiveGold, p, q)
        {                
        }


        public GiveGoldAction(GameNPC defaultNPC, long p)
            : this(defaultNPC,  (object)p,(object) null) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            player.AddMoney(P);
            InventoryLogging.LogInventoryAction(NPC, player, eInventoryActionType.Quest, P);
        }
    }
}
