
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
    public class SetMonsterPathAction : AbstractAction<PathPoint,GameNPC>
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SetMonsterPathAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.SetMonsterPath, p, q)
        {                
        }


        public SetMonsterPathAction(GameNPC defaultNPC,  PathPoint firstPathPoint, GameNPC npc)
            : this(defaultNPC,  (object)firstPathPoint, (object)npc) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GameNPC npc = Q;

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
