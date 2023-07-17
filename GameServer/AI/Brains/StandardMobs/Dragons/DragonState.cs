using System;
using DOL.AI.Brain;
using DOL.GS;
using FiniteStateMachine;

public class DragonState : StandardMobState
{
    protected new DragonBrain _brain = null;
    public DragonState(Fsm fsm, DragonBrain brain) : base(fsm, brain)
    {
        _brain = brain;
    }
}

public class DragonState_WakingUp : DragonState {
    public DragonState_WakingUp(Fsm fsm, DragonBrain brain) : base(fsm, brain) {
        _id = eFSMStateType.WAKING_UP;
    }

    public override void Think() {
        _brain.FSM.SetCurrentState(eFSMStateType.IDLE);
        base.Think();
    }
}

public class DragonState_Idle : DragonState
{
    public DragonState_Idle(Fsm fsm, DragonBrain brain) : base(fsm, brain)
    {
        _id = eFSMStateType.IDLE;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Dragon {_brain.Body} has entered IDLE");
        }
        base.Enter();
    }

    public override void Think()
    {
        //if we're walking home, do nothing else
        if (_brain.Body.IsReturningToSpawnPoint) return;

        //if dragon is full health, reset the encounter stages
        if (_brain.Body.HealthPercent == 100 && _brain.Stage < 10)
            _brain.Stage = 10;

        // If we aren't already aggroing something, look out for
        // someone we can aggro on and attack right away.
        _brain.CheckProximityAggro();

        if (_brain.HasAggro)
        {
            //Set state to AGGRO
            _brain.AttackMostWanted();
            _brain.FSM.SetCurrentState(eFSMStateType.AGGRO);
            return;
        }
        else
        {
            if (_brain.Body.attackComponent.AttackState)
                _brain.Body.StopAttack();

            _brain.Body.TargetObject = null;
        }

        // If dragon has run out of tether range, clear aggro list and let it 
        // return to its spawn point.
        if (_brain.CheckTether())
        {
            //set state to RETURN TO SPAWN
            _brain.FSM.SetCurrentState(eFSMStateType.RETURN_TO_SPAWN);
        }
    }
}

public class DragonState_Aggro : DragonState
{
    public DragonState_Aggro(Fsm fsm, DragonBrain brain) : base(fsm, brain)
    {
        _id = eFSMStateType.AGGRO;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Dragon {_brain.Body} has entered AGGRO on target {_brain.Body.TargetObject}");
        }
        base.Enter();
    }

    public override void Think()
    {

        if (_brain.CheckHealth()) return;
        if (_brain.PickGlareTarget()) return;
        _brain.PickThrowTarget();

        // If dragon has run out of tether range, or has clear aggro list, 
        // let it return to its spawn point.
        if (_brain.CheckTether() || !_brain.CheckProximityAggro())
        {
            //set state to RETURN TO SPAWN
            _brain.FSM.SetCurrentState(eFSMStateType.RETURN_TO_SPAWN);
        }
    }
}

public class DragonState_ReturnToSpawn : DragonState
{
    public DragonState_ReturnToSpawn(Fsm fsm, DragonBrain brain) : base(fsm, brain)
    {
        _id = eFSMStateType.RETURN_TO_SPAWN;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Dragon {_brain.Body} is returning to spawn");
        }
        _brain.Body.StopFollowing();
        GameDragon dragon = _brain.Body as GameDragon;
        if (dragon != null)
        {
            dragon.PrepareToStun();
        }

        _brain.ClearAggroList();
        _brain.Body.ReturnToSpawnPoint();
    }

    public override void Think()
    {
        if (_brain.Body.IsNearSpawn)
        {
            _brain.Body.CancelReturnToSpawnPoint();
            _brain.FSM.SetCurrentState(eFSMStateType.IDLE);
        }
    }
}