using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Actions
{
    [ActionAttribute(ActionType = EActionType.AbortQuest)]
    public class AbortQuestAction: AbstractAction<Type,Unused>
    {

        public AbortQuestAction(GameNpc defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.AbortQuest, p, q) 
        { }

        public AbortQuestAction(GameNpc defaultNPC, Type questType)
            : this(defaultNPC, (object)questType, (object)null)
        { }


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            AbstractQuest playerQuest = player.IsDoingQuest(P);
            if (playerQuest != null)
            {
                playerQuest.AbortQuest();
            }
        }
    }
}
