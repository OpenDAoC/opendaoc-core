using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.CustomDialog)]
    public class CustomDialogAction : AbstractAction<string, CustomDialogResponse>
    {               

        public CustomDialogAction(GameNpc defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.CustomDialog, p, q)
        {                
        }


        public CustomDialogAction(GameNpc defaultNPC, string message, CustomDialogResponse customDialogResponse)
            : this(defaultNPC,  (object)message, (object)customDialogResponse) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);

            string message = BehaviorUtils.GetPersonalizedMessage(P, player);            
            player.Out.SendCustomDialog(message, Q);
        }
    }
}
