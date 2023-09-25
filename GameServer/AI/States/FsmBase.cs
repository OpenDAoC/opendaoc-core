using System.Collections.Generic;
using DOL.GS;

namespace DOL.AI
{
    /// <summary>
    /// Base class for the FiniteStateManager.
    /// </summary>
    public class FsmBase
    {
        protected Dictionary<eFSMStateType, FsmAttribute> _states = new();
        protected FsmAttribute _state;

        public FsmBase() { }

        public virtual void Add(FsmAttribute state)
        {
            _states.Add(state.StateType, state);
        }

        public virtual void ClearStates()
        {
            _states.Clear();
        }

        public virtual FsmAttribute GetState(eFSMStateType stateType)
        {
            _states.TryGetValue(stateType, out FsmAttribute state);
            return state;
        }

        public virtual void SetCurrentState(eFSMStateType stateType)
        {
            if (_state != null)
                _state.Exit();

            _states.TryGetValue(stateType, out _state);

            if (_state != null)
                _state.Enter();
        }

        public virtual FsmAttribute GetCurrentState()
        {
            return _state;
        }

        public virtual void Think()
        {
            if (_state != null)
                _state.Think();
        }

        public virtual void KillFSM() { }
    }
}
