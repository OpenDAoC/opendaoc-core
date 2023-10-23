using System;
using System.Reflection;
using Core.GS.AI;
using Core.GS.Enums;
using Core.GS.Events;
using Core.GS.World;
using log4net;

namespace Core.GS.Behaviors;

[Action(ActionType = EActionType.SetMonsterPath,DefaultValueP=EDefaultValueConstants.NPC)]
public class SetMonsterPathAction : AAction<PathPoint,GameNpc>
{

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public SetMonsterPathAction(GameNpc defaultNPC,  Object p, Object q)
        : base(defaultNPC, EActionType.SetMonsterPath, p, q)
    {                
    }


    public SetMonsterPathAction(GameNpc defaultNPC,  PathPoint firstPathPoint, GameNpc npc)
        : this(defaultNPC,  (object)firstPathPoint, (object)npc) { }
    


    public override void Perform(CoreEvent e, object sender, EventArgs args)
    {
        GameNpc npc = Q;

        if (npc.Brain is MobRoundsBrain)
        {
            MobRoundsBrain brain = (MobRoundsBrain)npc.Brain;
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