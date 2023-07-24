using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.MonsterUnspawn,DefaultValueP=EDefaultValueConstants.NPC)]
    public class MonsterDespawnAction : AbstractAction<GameLiving,Unused>
    {               

        public MonsterDespawnAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.MonsterUnspawn, p, q)
        {                
        }


        public MonsterDespawnAction(GameNpc defaultNPC,  GameLiving monsterToUnspawn)
            : this(defaultNPC, (object)monsterToUnspawn, (object)null) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            P.RemoveFromWorld();
        }
    }
}
