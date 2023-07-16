using System;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the TurnToHeading event of GameNPC
	/// </summary>
	public class TurnToHeadingEventArgs : EventArgs
	{
		private ushort heading;

		/// <summary>
		/// Constructs a new TurnToHeadingEventArgs
		/// </summary>
		/// <param name="heading">the target heading</param>
		public TurnToHeadingEventArgs(ushort heading)
		{
			this.heading = heading;
		}

		/// <summary>
		/// Gets the target heading
		/// </summary>
		public uint Heading
		{
			get { return heading; }
		}		
	}
}
