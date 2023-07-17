using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = EActionType.SetQuestStep)]
    public class SetQuestStepAction: AbstractAction<Type,int>
    {

        public SetQuestStepAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.SetQuestStep, p, q) 
        {
        }

        public SetQuestStepAction(GameNPC defaultNPC, Type questType, int questStep)
            : this(defaultNPC, (object)questType, (object)questStep)
        {
        }


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            AbstractQuest playerQuest = player.IsDoingQuest(P) as AbstractQuest;
            if (playerQuest != null)
            {
                playerQuest.Step = Q;                
            }
        }
    }
}
