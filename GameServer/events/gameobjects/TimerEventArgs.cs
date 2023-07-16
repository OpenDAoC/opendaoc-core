using System;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the Timer event of GameObjects
	/// </summary>
	public class TimerEventArgs : SourceEventArgs
	{		
        private string timerId;

		/// <summary>
		/// Constructs a new TimerEventArgs
		/// </summary>
		public TimerEventArgs(GameLiving source, string timerId) : base (source)
		{
			this.timerId = timerId;
		}

		/// <summary>
		/// Gets the id of timer
		/// </summary>
		public string TimerID
		{
			get { return timerId; }
		}
	}
}
