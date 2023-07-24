using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = EActionType.OfferQuestAbort)]
    public class OfferQuestAbortAction : AbstractAction<Type,String>
    {               

        public OfferQuestAbortAction(GameNpc defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.OfferQuestAbort, p, q)
        {
        }


        public OfferQuestAbortAction(GameNpc defaultNPC, Type questType, String offerAbortMessage)
            : this(defaultNPC, (object)questType, (object)offerAbortMessage) { }
        

        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            string message = BehaviorUtils.GetPersonalizedMessage(Q, player);
            QuestMgr.AbortQuestToPlayer(P, message, player, NPC);
        }
    }
}
