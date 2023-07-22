﻿using System;
using DOL.AI.Brain;
using FiniteStateMachine;

public class ArosState : StandardMobState
{
    protected new ArosBrain _brain = null;
    public ArosState(Fsm fsm, ArosBrain brain) : base(fsm, brain)
    {
        _brain = brain;
    }
}

public class ArosState_IDLE : ArosState
{
    public ArosState_IDLE(Fsm fsm, ArosBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.IDLE;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Aros the Spiritmaster {_brain.Body} has entered IDLE");
        }
        base.Enter();
    }

    public override void Think()
    {
        //if we're walking home, do nothing else
        if (_brain.Body.IsReturningToSpawnPoint) return;

        //if Aros is full health, reset the encounter stages
        if (_brain.Body.HealthPercent == 100 && _brain.Stage < 10)
            _brain.Stage = 10;

        // If we aren't already aggroing something, look out for
        // someone we can aggro on and attack right away.
        if (!_brain.HasAggro && _brain.AggroLevel > 0)
        {
            _brain.CheckProximityAggro();

            if (_brain.HasAggro)
            {
                //Set state to AGGRO
                _brain.AttackMostWanted();
                _brain.FSM.SetCurrentState(EFsmStateType.AGGRO);
                return;
            }
            else
            {
                if (_brain.Body.attackComponent.AttackState)
                    _brain.Body.StopAttack();

                _brain.Body.TargetObject = null;
            }
        }

        // If Aros the Spiritmaster has run out of tether range, clear aggro list and let it 
        // return to its spawn point.
        if (_brain.CheckTether())
        {
            //set state to RETURN TO SPAWN
            _brain.FSM.SetCurrentState(EFsmStateType.RETURN_TO_SPAWN);
        }
    }
}

public class ArosState_AGGRO : ArosState
{
    public ArosState_AGGRO(Fsm fsm, ArosBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.AGGRO;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Aros the Spiritmaster {_brain.Body} has entered AGGRO on target {_brain.Body.TargetObject}");
        }
        base.Enter();
    }

    public override void Think()
    {
        if (_brain.CheckHealth()) return;
        if (_brain.PickDebuffTarget()) return;

        // If Aros the Spiritmaster has run out of tether range, or has clear aggro list, 
        // let it return to its spawn point.
        if (_brain.CheckTether() || !_brain.CheckProximityAggro())
        {
            //set state to RETURN TO SPAWN
            _brain.FSM.SetCurrentState(EFsmStateType.RETURN_TO_SPAWN);
        }
    }

}

public class ArosState_RETURN_TO_SPAWN : ArosState
{
    public ArosState_RETURN_TO_SPAWN(Fsm fsm, ArosBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.RETURN_TO_SPAWN;
    }

    public override void Enter()
    {
        if (ECS.Debug.Diagnostics.StateMachineDebugEnabled)
        {
            Console.WriteLine($"Aros the Spiritmaster {_brain.Body} is returning to spawn");
        }
        _brain.Body.StopFollowing();
        _brain.ClearAggroList();
        _brain.Body.ReturnToSpawnPoint();
    }

    public override void Think()
    {
        if (_brain.Body.IsNearSpawn)
        {
            _brain.Body.CancelReturnToSpawnPoint();
            _brain.FSM.SetCurrentState(EFsmStateType.IDLE);
        }
    }
}