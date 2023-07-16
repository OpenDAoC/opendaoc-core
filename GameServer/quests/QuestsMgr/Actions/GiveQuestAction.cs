using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = eActionType.GiveQuest)]
    public class GiveQuestAction : AbstractAction<Type,GameNPC>
    {

        public GiveQuestAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, eActionType.GiveQuest, p, q) { }


        public GiveQuestAction(GameNPC defaultNPC, Type questType, GameNPC questGiver)
            : this(defaultNPC, (object)questType, (object)questGiver) { }
        

        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            QuestMgr.GiveQuestToPlayer(P, player, Q);            
        }
    }
}
