using DOL.GS;

namespace DOL.AI.Brain;

public class CaptainBardalphBrain : StandardMobBrain
{
    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public CaptainBardalphBrain()
        : base()
    {
        AggroLevel = 100;
        AggroRange = 500;
        ThinkInterval = 1500;
    }
    public static bool reset_darra = false;
    public override void Think()
    {
        if (!CheckProximityAggro())
        {
            //set state to RETURN TO SPAWN
            FiniteStateMachine.SetCurrentState(EFSMStateType.RETURN_TO_SPAWN);
            Body.Health = Body.MaxHealth;
        }
        if (Body.IsOutOfTetherRange)
        {
            Body.Health = Body.MaxHealth;
        }
        else if (Body.InCombatInLast(30 * 1000) == false && this.Body.InCombatInLast(35 * 1000))
        {
            Body.Health = Body.MaxHealth;
        }
        if (Body.InCombat && HasAggro)
        {
            if (Body.TargetObject != null)
            {
                Body.styleComponent.NextCombatBackupStyle = CaptainBardalph.Taunt;
                Body.styleComponent.NextCombatStyle = CaptainBardalph.AfterEvade;
            }
        }
        base.Think();
    }
}