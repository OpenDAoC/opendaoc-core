namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the Yell event of GameLivings
	/// </summary>
	public class YellEventArgs : SayEventArgs
	{
		/// <summary>
		/// Constructs a new YellEventArgs
		/// </summary>
		/// <param name="text">the text being yelled</param>
		public YellEventArgs(string text) : base(text)
		{
		}
	}
}
