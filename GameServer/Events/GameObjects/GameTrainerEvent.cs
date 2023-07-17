using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// This class holds all possible trainer events.
	/// Only constants defined here!
	/// </summary>
	public class GameTrainerEvent : GameNpcEvent
	{
		/// <summary>
		/// Constructs a new GameTrainer event
		/// </summary>
		/// <param name="name">the event name</param>
		protected GameTrainerEvent(string name) : base(name)
		{
		}
		
		/// <summary>
		/// Tests if this event is valid for the specified object
		/// </summary>
		/// <param name="o">The object for which the event wants to be registered</param>
		/// <returns>true if valid, false if not</returns>
		public override bool IsValidFor(object o)
		{
			return o is GameTrainer;
		}

		/// <summary>
		/// The PlayerPromoted event is fired whenever trainer
		/// promotes the player to a new class
		/// </summary>
		public static readonly GameTrainerEvent PlayerPromoted = new GameTrainerEvent("GameTrainer.PlayerPromoted");
	}
}
