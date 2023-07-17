using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.GiveXP)]
    public class GiveXpAction : AbstractAction<long,Unused>
    {               

        public GiveXpAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.GiveXP, p, q) {                
        }


        public GiveXpAction(GameNPC defaultNPC, long p)
            : this(defaultNPC, (object)p,(object) null) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            player.GainExperience(EXpSource.NPC, P);
        }
    }
}
