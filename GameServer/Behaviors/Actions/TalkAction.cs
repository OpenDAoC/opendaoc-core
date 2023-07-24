using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.Talk,DefaultValueQ=EDefaultValueConstants.NPC)]
    public class TalkAction : AbstractAction<String,GameNpc>
    {

        public TalkAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.Talk, p, q)
        {
        }


        public TalkAction(GameNpc defaultNPC, String message, GameNpc npc)
            : this(defaultNPC, (object)message, (object)npc) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            String message = BehaviorUtils.GetPersonalizedMessage(P, player);
            Q.TurnTo(player);
            Q.SayTo(player, message);
        }
    }
}
