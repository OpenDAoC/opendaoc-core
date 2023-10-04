﻿using DOL.GS;

namespace DOL.AI;

public abstract class FsmState
{
    public eFSMStateType StateType { get; protected set; }

    public FsmState() { }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Think();
}