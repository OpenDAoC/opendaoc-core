using System;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the WalkTo event of GameNPC
	/// </summary>
	public class WalkToEventArgs : EventArgs
	{
        public WalkToEventArgs(IPoint3D target, int speed)
        {
            Target = target;
            Speed = speed;
        }

        /// <summary>
        /// The spot to walk to.
        /// </summary>
        public IPoint3D Target { get; private set; }

		/// <summary>
		/// The speed to walk at.
		/// </summary>
        public int Speed { get; private set; }
	}
}
