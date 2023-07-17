using System;
using DOL.Database;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the ReceiveItem event of GameObjects
	/// </summary>
	public class ReceiveItemEventArgs : SourceEventArgs
	{
		private GameObject target;
		private InventoryItem item;

		/// <summary>
		/// Constructs new ReceiveItemEventArgs
		/// </summary>
		/// <param name="source">the source of the item</param>
		/// <param name="target">the target of the item</param>
		/// <param name="item">the item to transfer</param>
		public ReceiveItemEventArgs(GameLiving source, GameObject target, InventoryItem item)
			: base(source)
		{
			this.target = target;
			this.item = item;
		}

		/// <summary>
		/// Gets the GameObject who receives the item
		/// </summary>
		public GameObject Target
		{
			get { return target; }
		}

		/// <summary>
		/// Gets the item to transfer
		/// </summary>
		public InventoryItem Item
		{
			get { return item; }
		}
	}
}
