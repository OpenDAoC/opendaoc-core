using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS;

namespace DOL.Events
{
    /// <summary>
    /// Holds parameters for InterruptedEvent.
    /// </summary>
    public class InterruptedEventArgs : EventArgs
    {
        public InterruptedEventArgs(GameLiving attacker)
        {
            Attacker = attacker;
        }

        public GameLiving Attacker { get; private set; }
    }
}
