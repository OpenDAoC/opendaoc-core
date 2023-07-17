using System;
using DOL.AI.Brain;
using DOL.GS;
using FiniteStateMachine;

public class AlluvianGlobuleState_Idle : StandardMobState_Idle
{
    public AlluvianGlobuleState_Idle(Fsm fsm, AlluvianGlobuleBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.IDLE;
    }

    public override void Enter()
    {
        Console.WriteLine($"{_brain.Body} is entering ALLUVIAN IDLE");
        base.Enter();
    }

    public override void Think()
    {
        if ((_brain as AlluvianGlobuleBrain).CheckStorm())
        {
            if (!(_brain as AlluvianGlobuleBrain).hasGrown)
            {
                (_brain as AlluvianGlobuleBrain).Grow(); //idle
            }
        }
        base.Think();
    }
}

public class AlluvianGlobuleState_Roaming : StandardMobState_Roaming
{
    public AlluvianGlobuleState_Roaming(Fsm fsm, AlluvianGlobuleBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.ROAMING;
    }

    public override void Enter()
    {
        Console.WriteLine($"{_brain.Body} is entering ALLUVIAN ROAM");
        base.Enter();
    }

    public override void Think()
    {
        if (!_brain.Body.attackComponent.AttackState && !_brain.Body.IsMoving && !_brain.Body.InCombat)
        {
            // loc range around the lake that Alluvian spanws.
            _brain.Body.WalkTo(544196 + UtilCollection.Random(1, 3919), 514980 + UtilCollection.Random(1, 3200), 3140 + UtilCollection.Random(1, 540), 80);
        }
        base.Think();
    }
}


