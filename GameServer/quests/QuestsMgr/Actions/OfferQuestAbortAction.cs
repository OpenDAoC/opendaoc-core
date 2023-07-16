using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = eActionType.OfferQuestAbort)]
    public class OfferQuestAbortAction : AbstractAction<Type,String>
    {               

        public OfferQuestAbortAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, eActionType.OfferQuestAbort, p, q)
        {
        }


        public OfferQuestAbortAction(GameNPC defaultNPC, Type questType, String offerAbortMessage)
            : this(defaultNPC, (object)questType, (object)offerAbortMessage) { }
        

        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            string message = BehaviourUtils.GetPersonalizedMessage(Q, player);
            QuestMgr.AbortQuestToPlayer(P, message, player, NPC);
        }
    }
}
