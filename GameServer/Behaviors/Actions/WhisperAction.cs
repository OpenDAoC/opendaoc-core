using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.Whisper,DefaultValueQ=EDefaultValueConstants.NPC)]
    public class WhisperAction : AbstractAction<String,GameNPC>
    {

        public WhisperAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.Whisper, p, q)
        {       
        }


        public WhisperAction(GameNPC defaultNPC, String message, GameNPC npc)
            : this(defaultNPC, (object)message, (object)npc) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            String message = BehaviorUtils.GetPersonalizedMessage(P, player);
            Q.TurnTo(player);
            Q.Whisper(player, message);            
        }
    }
}
