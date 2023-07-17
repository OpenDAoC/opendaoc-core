using System;
using DOL.GS;

namespace DOL.Events
{
    /// <summary>
    /// This class holds the old and the new target for
    /// GameLiving.SwitchedTargetEvent.
    /// </summary>
    public class SwitchedTargetEventArgs : EventArgs
    {
        public SwitchedTargetEventArgs(GameObject previousTarget, GameObject newTarget)
        {
            PreviousTarget = previousTarget;
            NewTarget = newTarget;
        }

        public GameObject PreviousTarget { get; private set; }
        public GameObject NewTarget { get; private set; }
    }
}
