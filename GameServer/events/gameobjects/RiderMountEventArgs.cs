using System;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the RiderMount event of GameNPC
	/// </summary>
	public class RiderMountEventArgs : EventArgs
	{
		private GamePlayer rider;
		private GameLiving steed;

		/// <summary>
		/// Constructs a new RiderMountEventArgs
		/// </summary>
		/// <param name="rider">the rider mounting</param>
		/// <param name="steed">the steed being mounted</param>
		public RiderMountEventArgs(GamePlayer rider, GameLiving steed)
		{
			this.rider = rider;
			this.steed = steed;
		}

		/// <summary>
		/// Gets the GamePlayer rider who is mounting the steed
		/// </summary>
		public GamePlayer Rider
		{
			get { return rider; }
		}

		/// <summary>
		/// Gets the GameLiving steed who is being mounted by the rider
		/// </summary>
		public GameLiving Steed
		{
			get { return steed; }
		}

	}
}
