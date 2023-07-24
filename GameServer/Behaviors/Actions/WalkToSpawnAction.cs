using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.WalkToSpawn,DefaultValueP=EDefaultValueConstants.NPC)]
    public class WalkToSpawnAction : AbstractAction<GameNpc,Unused>
    {               

        public WalkToSpawnAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.WalkToSpawn, p, q)
        {                
        }


        public WalkToSpawnAction(GameNpc defaultNPC, GameNpc npc)
            : this(defaultNPC,  (object)npc, (object)null) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            P.ReturnToSpawnPoint();
        }
    }
}
