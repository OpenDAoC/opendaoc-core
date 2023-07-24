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

        public SetQuestStepAction(GameNpc defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.SetQuestStep, p, q) 
        {
        }

        public SetQuestStepAction(GameNpc defaultNPC, Type questType, int questStep)
            : this(defaultNPC, (object)questType, (object)questStep)
        {
        }


        public override void Perform(CoreEvent e, object sender, EventArgs args)
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
