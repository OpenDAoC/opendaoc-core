using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    public class Fsm
    {
        protected Dictionary<int, BaseState> m_states = new Dictionary<int, BaseState>();
        protected BaseState m_currentState;

        public Fsm()
        {

        }

        public void Add(int key, BaseState state)
        {
            m_states.Add(key, state);
        }

        public void ClearStates()
        {
            m_states.Clear();
        }

        public BaseState GetState(int key)
        {
            if (m_states.ContainsKey(key)){ return m_states[key];} 
            else { return null; }
            
        }

        public void SetCurrentState(BaseState state)
        {
            if(m_currentState != null)
            {
                m_currentState.Exit();
            }

            m_currentState = state;
            if(m_currentState != null)
            {
                m_currentState.Enter();
            }
        }

        public BaseState GetCurrentState()
        {
            return m_currentState;
        }

        public void Think()
        {
            if (m_currentState != null)
            {
                m_currentState.Think();
            }
        }

        public virtual void KillFSM()
        {
          
        }
    }
}
