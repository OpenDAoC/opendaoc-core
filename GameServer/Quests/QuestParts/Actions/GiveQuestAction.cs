using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = EActionType.GiveQuest)]
    public class GiveQuestAction : AbstractAction<Type,GameNpc>
    {

        public GiveQuestAction(GameNpc defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.GiveQuest, p, q) { }


        public GiveQuestAction(GameNpc defaultNPC, Type questType, GameNpc questGiver)
            : this(defaultNPC, (object)questType, (object)questGiver) { }
        

        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            QuestMgr.GiveQuestToPlayer(P, player, Q);            
        }
    }
}
