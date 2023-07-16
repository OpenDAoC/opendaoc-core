using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the YellReceive event of GameLivings
	/// </summary>
	public class YellReceiveEventArgs : SayReceiveEventArgs
	{
		/// <summary>
		/// Constructs a new YellReceivedEventArgs
		/// </summary>
		/// <param name="source">the source of the yell</param>
		/// <param name="target">the target who listened to the yell</param>
		/// <param name="text">the text being yelled</param>
		public YellReceiveEventArgs(GameLiving source, GameLiving target, string text) : base(source, target, text)
		{
		}
	}
}
