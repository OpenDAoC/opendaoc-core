using FiniteStateMachine;

public class BaseState
{
    protected Fsm m_fsm;
    public BaseState(Fsm fsm)
    {
        m_fsm = fsm;
    }

    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }

    public virtual void Think()
    {

    }
}
