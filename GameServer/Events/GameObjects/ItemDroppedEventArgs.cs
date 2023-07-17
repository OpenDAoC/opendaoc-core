using System;
using DOL.Database;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the ItemEquipped event of PlayerInventory
	/// </summary>
	public class ItemDroppedEventArgs : EventArgs
	{
		private InventoryItem m_sourceItem;
		private WorldInventoryItem m_groundItem;

		public ItemDroppedEventArgs(InventoryItem sourceItem, WorldInventoryItem groundItem)
		{
			m_sourceItem = sourceItem;
			m_groundItem = groundItem;
		}

		/// <summary>
		/// Gets the source item
		/// </summary>
		public InventoryItem SourceItem
		{
			get { return m_sourceItem; }
		}

		/// <summary>
		/// Gets the ground item
		/// </summary>
		public WorldInventoryItem GroundItem
		{
			get { return m_groundItem; }
		}
	}
}
