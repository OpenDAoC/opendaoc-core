using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the RiderDismount event of GameNPC
	/// </summary>
	public class RiderDismountEventArgs : RiderMountEventArgs
	{
		/// <summary>
		/// Constructs a new RiderDismountEventArgs
		/// </summary>
		/// <param name="rider">the rider dismounting</param>
		/// <param name="steed">the steed the rider is dismounting from</param>
		public RiderDismountEventArgs(GamePlayer rider, GameLiving steed) : base(rider,steed)
		{
		}
	}
}
