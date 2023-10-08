using System;
using System.Reflection;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using log4net;

namespace DOL.GS.Behaviour.Actions
{
    [Action(ActionType = EActionType.Attack,IsNullableP=true)]
    public class AttackAction : AAction<int?,GameNPC>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AttackAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, EActionType.Attack, p, q)
        {
        }


        public AttackAction(GameNPC defaultNPC, Nullable<Int32> aggroAmount, GameNPC attacker)
            : this(defaultNPC, (object)aggroAmount, (object)attacker) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtil.GuessGamePlayerFromNotify(e, sender, args);

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