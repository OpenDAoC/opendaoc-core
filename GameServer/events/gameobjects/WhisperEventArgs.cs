using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the Whisper event of GameLivings
	/// </summary>
	public class WhisperEventArgs : SayEventArgs
	{
		private GameObject target;
		/// <summary>
		/// Constructs a new WhisperEventArgs
		/// </summary>
		/// <param name="target">the target of the whisper</param>
		/// <param name="text">the text being whispered</param>
		public WhisperEventArgs(GameObject target, string text) : base(text)
		{
			this.target = target;
		}

		/// <summary>
		/// Gets the target of the whisper
		/// </summary>
		public GameObject Target
		{
			get { return target; }
		}
	}
}
