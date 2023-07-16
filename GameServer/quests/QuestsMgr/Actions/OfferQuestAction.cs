using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = eActionType.OfferQuest)]
    public class OfferQuestAction : AbstractAction<Type,String>
    {               

        public OfferQuestAction(GameNPC defaultNPC, eActionType actionType, Object p, Object q)
            : base(defaultNPC, eActionType.OfferQuest, p, q)
        {                
        }

        public OfferQuestAction(GameNPC defaultNPC, Type questType, String offerMessage)
            : this(defaultNPC, eActionType.OfferQuest, (object)questType, (object)offerMessage) { }
        

        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            string message = BehaviourUtils.GetPersonalizedMessage(Q, player);
            QuestMgr.ProposeQuestToPlayer(P, message, player, NPC);
        }
    }
}
