using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.GiveXP)]
    public class GiveXPAction : AbstractAction<long,Unused>
    {               

        public GiveXPAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.GiveXP, p, q) {                
        }


        public GiveXPAction(GameNPC defaultNPC, long p)
            : this(defaultNPC, (object)p,(object) null) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            player.GainExperience(eXPSource.NPC, P);
        }
    }
}
