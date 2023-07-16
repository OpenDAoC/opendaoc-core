using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.CustomTimer)]
    public class CustomTimerAction : AbstractAction<ECSGameTimer,int>
    {

        public CustomTimerAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.CustomTimer, p, q)
        { 
            
        }


        public CustomTimerAction(GameNPC defaultNPC, ECSGameTimer gameTimer, int delay)
            : this(defaultNPC, (object) gameTimer,(object) delay) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            var timer = (ECSGameTimer)P;
            timer.Start(Q);
        }
    }
}
