using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// This class holds all possible GameNPC events.
	/// Only constants defined here!
	/// </summary>
	public class GameNpcEvent : GameLivingEvent
	{
		/// <summary>
		/// Constructs a new GameNPCEvent
		/// </summary>
		/// <param name="name">the event name</param>
		protected GameNpcEvent(string name) : base(name)
		{
		}

		/// <summary>
		/// Tests if this event is valid for the specified object
		/// </summary>
		/// <param name="o">The object for which the event wants to be registered</param>
		/// <returns>true if valid, false if not</returns>
		public override bool IsValidFor(object o)
		{
			return o is GameNpc;
		}

		/// <summary>
		/// The TurnTo event is fired whenever the npc turns towards some coordinates
		/// <seealso cref="TurnToEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent TurnTo = new GameNpcEvent("GameNPC.TurnTo");
		/// <summary>
		/// The TurnToHeading event is fired whenever the npc turns towards a specific heading
		/// <seealso cref="TurnToHeadingEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent TurnToHeading = new GameNpcEvent("GameNPC.TurnToHeading");
		/// <summary>
		/// The ArriveAtTarget event is fired whenever the npc arrives at its WalkTo target
		/// <see cref="GameNpc.WalkTo(int, int, int, int)"/>
		/// </summary>
		public static readonly GameNpcEvent ArriveAtTarget = new GameNpcEvent("GameNPC.ArriveAtTarget");
        /// <summary>
        /// The ArriveAtSpawnPoint event is fired whenever the npc arrives at its spawn point
        /// <see cref="GameNpc.WalkTo(int, int, int, int)"/>
        /// </summary>
        public static readonly GameNpcEvent ArriveAtSpawnPoint = new GameNpcEvent("GameNPC.ArriveAtSpawnPoint");
		/// <summary>
		/// The CloseToTarget event is fired whenever the npc is close to its WalkTo target
		/// <see cref="GameNpc.WalkTo(int, int, int, int)"/>
		/// </summary>
		public static readonly GameNpcEvent CloseToTarget = new GameNpcEvent("GameNPC.CloseToTarget");
		/// <summary>
		/// The WalkTo event is fired whenever the npc is commanded to walk to a specific target
		/// <seealso cref="WalkToEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent WalkTo = new GameNpcEvent("GameNPC.WalkTo");
		/// <summary>
		/// The Walk event is fired whenever the npc is commanded to walk
		/// <seealso cref="WalkEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent Walk = new GameNpcEvent("GameNPC.Walk");
		/// <summary>
		/// The RiderMount event is fired whenever the npc is mounted by a ride
		/// <seealso cref="RiderMountEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent RiderMount = new GameNpcEvent("GameNPC.RiderMount");
		/// <summary>
		/// The RiderDismount event is fired whenever the rider dismounts from the npc
		/// <seealso cref="RiderDismountEventArgs"/>
		/// </summary>
		public static readonly GameNpcEvent RiderDismount = new GameNpcEvent("GameNPC.RiderDismount");
		/// <summary>
		/// Fired when pathing starts
		/// </summary>
		public static readonly GameNpcEvent PathMoveStarts = new GameNpcEvent("GameNPC.PathMoveStarts");
		/// <summary>
		/// Fired when npc is on end of path
		/// </summary>
		public static readonly GameNpcEvent PathMoveEnds = new GameNpcEvent("GameNPC.PathMoveEnds");
		/// <summary>
		/// Fired on every AI callback
		/// </summary>
		public static readonly GameNpcEvent OnAICallback = new GameNpcEvent("GameNPC.OnAICallback");
		/// <summary>
		/// Fired whenever following NPC lost its target
		/// </summary>
		public static readonly GameNpcEvent FollowLostTarget = new GameNpcEvent("GameNPC.FollowLostTarget");
		/// <summary>
		/// Fired whenever pet is supposed to cast a spell.
		/// </summary>
		public static readonly GameNpcEvent PetSpell = new GameNpcEvent("GameNPC.PetSpell");
		/// <summary>
		/// Fired whenever pet is out of tether range (necromancer).
		/// </summary>
		public static readonly GameNpcEvent OutOfTetherRange = new GameNpcEvent("GameNPC.OutOfTetherRange");
		/// <summary>
		/// Fired when pet is lost (necromancer).
		/// </summary>
		public static readonly GameNpcEvent PetLost = new GameNpcEvent("GameNPC.PetLost");
        /// <summary>
        /// The SwitchedTarget event is fired when an NPC changes its target.
        /// </summary>
        public static readonly GameLivingEvent SwitchedTarget = new GameNpcEvent("GameNPC.SwitchedTarget");
	}
}
