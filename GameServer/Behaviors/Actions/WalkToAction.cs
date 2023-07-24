using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.WalkTo,DefaultValueQ=EDefaultValueConstants.NPC)]
    public class WalkToAction : AbstractAction<IPoint3D,GameNpc>
    {

        public WalkToAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.WalkTo, p, q)
        {                
            }


        public WalkToAction(GameNpc defaultNPC,  IPoint3D destination, GameNpc npc)
            : this(defaultNPC, (object) destination,(object) npc) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            IPoint3D location = (P is IPoint3D) ? (IPoint3D)P : player;            

            Q.WalkTo(location, Q.CurrentSpeed);
            
        }
    }
}
