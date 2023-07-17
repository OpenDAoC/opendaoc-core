using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    // NOTE it is important that we look into the database for the npc because since it's not spawn at the moment the WorldMgr cant find it!!!
    [ActionAttribute(ActionType = EActionType.MonsterSpawn,DefaultValueP=EDefaultValueConstants.NPC)]
    public class MonsterSpawnAction : AbstractAction<GameLiving,Unused>
    {               

        public MonsterSpawnAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.MonsterSpawn, p, q)
        {                
        }


        public MonsterSpawnAction(GameNPC defaultNPC,  GameLiving npcToSpawn)
            : this(defaultNPC,  (object)npcToSpawn, (object)null) { }

        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {            
            if (P.AddToWorld())
            {
                // appear with a big buff of magic
                foreach (GamePlayer visPlayer in P.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                {
                    visPlayer.Out.SendSpellCastAnimation(P, 1, 20);
                }
                
            }
        }
    }
}
