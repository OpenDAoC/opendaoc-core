using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = eActionType.FinishQuest)]
    public class FinishQuestAction: AbstractAction<Type,Unused>
    {

        public FinishQuestAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, eActionType.FinishQuest, p, q) 
        { }

        public FinishQuestAction(GameNPC defaultNPC, Type questType)
            : this(defaultNPC, (object) questType,(object) null)
        { }


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            AbstractQuest playerQuest = player.IsDoingQuest(P);
            if (playerQuest != null)
                playerQuest.FinishQuest();
        }
    }
}
