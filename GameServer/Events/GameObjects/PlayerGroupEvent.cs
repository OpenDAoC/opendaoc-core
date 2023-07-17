using System;

namespace DOL.Events
{
	/// <summary>
	/// This class holds all possible player group events.
	/// Only constants defined here!
	/// </summary>
	public class PlayerGroupEvent : CoreEvent
	{
		/// <summary>
		/// Constructs a new PlayerGroup event
		/// </summary>
		/// <param name="name">the event name</param>
		public PlayerGroupEvent(string name) : base (name)
		{
		}

		/// <summary>
		/// The PlayerJoined event is fired whenever player joins the group
		/// </summary>
		public static readonly PlayerGroupEvent MemberJoined = new PlayerGroupEvent("Group.MemberJoined");
		/// <summary>
		/// The PlayerDisbanded event is fired whenever player disbands
		/// </summary>
		public static readonly PlayerGroupEvent MemberDisbanded = new PlayerGroupEvent("Group.MemberDisbanded");
	}
}
