using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;
using DOL.AI.Brain;
using log4net;
using System.Reflection;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.Attack,IsNullableP=true)]
    public class AttackAction : AbstractAction<Nullable<Int32>,GameNPC>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AttackAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.Attack, p, q)
        {
        }


        public AttackAction(GameNPC defaultNPC, Nullable<Int32> aggroAmount, GameNPC attacker)
            : this(defaultNPC, (object)aggroAmount, (object)attacker) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);

            int aggroAmount = P.HasValue ? P.Value : player.Level << 1;
            GameNPC attacker = Q;

            if (attacker.Brain is IOldAggressiveBrain)
            {
                IOldAggressiveBrain brain = (IOldAggressiveBrain)attacker.Brain;
                brain.AddToAggroList(player, aggroAmount);                
            }
            else
            {
                if (log.IsWarnEnabled)
                log.Warn("Non agressive mob " + attacker.Name + " was order to attack player. This goes against the first directive and will not happen");                
            }
        }
    }
}
