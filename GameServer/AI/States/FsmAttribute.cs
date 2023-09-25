using DOL.GS;

namespace DOL.AI
{
    /// <summary>
    /// Unique attributes for the FiniteStateManager.
    /// </summary>
    public abstract class FsmAttribute
    {
        public eFSMStateType StateType { get; protected set; }

        public FsmAttribute() { }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Think();
    }
}
