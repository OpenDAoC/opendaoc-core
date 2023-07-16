using System;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the Walk event of GameNPC
	/// </summary>
	public class WalkEventArgs : EventArgs
	{
		private int speed;

		/// <summary>
		/// Constructs a new WalkEventArgs
		/// </summary>
		/// <param name="speed">the walk speed</param>
		public WalkEventArgs(int speed)
		{
			this.speed=speed;
		}
		
		/// <summary>
		/// Gets the walk speed
		/// </summary>
		public int Speed
		{
			get { return speed; }
		}
	}
}
