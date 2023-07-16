using System;
using System.Collections.Generic;
using System.Text;

namespace DOL.Events
{
    /// <summary>
    /// Event to signal time remaining until pet is lost.
    /// </summary>
    class TetherEventArgs : EventArgs
    {
        private int m_seconds;

        public TetherEventArgs(int seconds)
        {
            m_seconds = seconds;
        }

        public int Seconds
        {
            get { return m_seconds; }
        }
    }
}
