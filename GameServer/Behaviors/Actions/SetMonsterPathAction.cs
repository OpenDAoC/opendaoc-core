
using System;
using System.Reflection;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Movement;
using log4net;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.SetMonsterPath,DefaultValueP=EDefaultValueConstants.NPC)]
    public class SetMonsterPathAction : AbstractAction<PathPointUtil,GameNpc>
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SetMonsterPathAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.SetMonsterPath, p, q)
        {                
        }


        public SetMonsterPathAction(GameNpc defaultNPC,  PathPointUtil firstPathPoint, GameNpc npc)
            : this(defaultNPC,  (object)firstPathPoint, (object)npc) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GameNpc npc = Q;

            if (npc.Brain is RoundsMobBrain)
            {
                RoundsMobBrain brain = (RoundsMobBrain)npc.Brain;
                npc.CurrentWaypoint = P;
                brain.Start();
            }
            else
            {
                if (log.IsWarnEnabled)
                    log.Warn("Mob without RoundsBrain was assigned to walk along Path");                
            }
        }
    }
}
