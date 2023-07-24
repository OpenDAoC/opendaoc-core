using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.CustomTimer)]
    public class CustomTimerAction : AbstractAction<ECSGameTimer,int>
    {

        public CustomTimerAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.CustomTimer, p, q)
        { 
            
        }


        public CustomTimerAction(GameNpc defaultNPC, ECSGameTimer gameTimer, int delay)
            : this(defaultNPC, (object) gameTimer,(object) delay) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            var timer = (ECSGameTimer)P;
            timer.Start(Q);
        }
    }
}
