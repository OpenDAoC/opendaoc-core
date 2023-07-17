namespace DOL.Events
{
	/// <summary>
	/// This class holds all possible player inventory events.
	/// Only constants defined here!
	/// </summary>
	public class PlayerInventoryEvent : CoreEvent
	{
		/// <summary>
		/// Constructs a new PlayerInventory event
		/// </summary>
		/// <param name="name"></param>
		public PlayerInventoryEvent(string name) : base(name)
		{
		}

		/// <summary>
		/// The item was just equipped
		/// </summary>
		public static readonly PlayerInventoryEvent ItemEquipped = new PlayerInventoryEvent("PlayerInventory.ItemEquipped");
		/// <summary>
		/// The item was just unequipped
		/// </summary>
		public static readonly PlayerInventoryEvent ItemUnequipped = new PlayerInventoryEvent("PlayerInventory.ItemUnequipped");
		/// <summary>
		/// The item was just dropped
		/// </summary>
		public static readonly PlayerInventoryEvent ItemDropped = new PlayerInventoryEvent("PlayerInventory.ItemDropped");
		/// <summary>
		/// A bonus on an item changed.
		/// </summary>
		public static readonly PlayerInventoryEvent ItemBonusChanged = new PlayerInventoryEvent("PlayerInventory.ItemBonusChanged");
	}
}
