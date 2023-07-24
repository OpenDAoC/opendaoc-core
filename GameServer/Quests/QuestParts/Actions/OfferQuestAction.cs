using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = EActionType.OfferQuest)]
    public class OfferQuestAction : AbstractAction<Type,String>
    {               

        public OfferQuestAction(GameNpc defaultNPC, EActionType actionType, Object p, Object q)
            : base(defaultNPC, EActionType.OfferQuest, p, q)
        {                
        }

        public OfferQuestAction(GameNpc defaultNPC, Type questType, String offerMessage)
            : this(defaultNPC, EActionType.OfferQuest, (object)questType, (object)offerMessage) { }
        

        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            string message = BehaviorUtils.GetPersonalizedMessage(Q, player);
            QuestMgr.ProposeQuestToPlayer(P, message, player, NPC);
        }
    }
}
