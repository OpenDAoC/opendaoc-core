using DOL.AI.Brain;
using FiniteStateMachine;

public class NecroPetState_WakingUp : ControlledNpcState_WakingUp
{
    public NecroPetState_WakingUp(Fsm fsm, NecroPetBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.WAKING_UP;
    }

    public override void Think()
    {
        base.Think();
    }
}

public class NecroPetState_Defensive : ControlledNpcState_Defensive
{
    public NecroPetState_Defensive(Fsm fsm, ControlledNpcBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.IDLE;
    }

    public override void Think()
    {
        NecroPetBrain brain = _brain as NecroPetBrain;

        // If spells are queued then handle them first.
        if (brain.HasSpellsQueued())
            brain.CheckSpellQueue();

        base.Think();
    }
}

public class NecroPetState_Aggro : ControlledNpcState_Aggro
{
    public NecroPetState_Aggro(Fsm fsm, ControlledNpcBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.AGGRO;
    }

    public override void Think()
    {
        NecroPetBrain brain = _brain as NecroPetBrain;

        // If spells are queued then handle them first.
        if (brain.HasSpellsQueued())
            brain.CheckSpellQueue();

        base.Think();
    }
}

public class NecroPetState_Passive : ControlledNpcState_Passive
{
    public NecroPetState_Passive(Fsm fsm, ControlledNpcBrain brain) : base(fsm, brain)
    {
        _id = EFsmStateType.PASSIVE;
    }

    public override void Think()
    {
        NecroPetBrain brain = _brain as NecroPetBrain;

        // If spells are queued then handle them first.
        if (brain.HasSpellsQueued())
            brain.CheckSpellQueue();

        base.Think();
    }
}
