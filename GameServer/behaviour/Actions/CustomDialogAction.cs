using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.CustomDialog)]
    public class CustomDialogAction : AbstractAction<string, CustomDialogResponse>
    {               

        public CustomDialogAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, eActionType.CustomDialog, p, q)
        {                
        }


        public CustomDialogAction(GameNPC defaultNPC, string message, CustomDialogResponse customDialogResponse)
            : this(defaultNPC,  (object)message, (object)customDialogResponse) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);

            string message = BehaviourUtils.GetPersonalizedMessage(P, player);            
            player.Out.SendCustomDialog(message, Q);
        }
    }
}
