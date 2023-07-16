using System;
using DOL.Database;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the ItemEquipped event of PlayerInventory
	/// </summary>
	public class ItemEquippedArgs : EventArgs
	{
		private InventoryItem m_item;
		private eInventorySlot m_previousSlotPosition;

		/// <summary>
		/// Constructs a new ItemEquippedArgs
		/// </summary>
		/// <param name="item">The equipped item</param>
		/// <param name="previousSlotPosition">The slot position item had before it was equipped</param>
		public ItemEquippedArgs(InventoryItem item, eInventorySlot previousSlotPosition)
		{
			m_item = item;
			m_previousSlotPosition = previousSlotPosition;
		}

		/// <summary>
		/// Constructs a new ItemEquippedArgs
		/// </summary>
		/// <param name="item">The equipped item</param>
		/// <param name="previousSlotPosition">The slot position item had before it was equipped</param>
		public ItemEquippedArgs(InventoryItem item, int previousSlotPosition)
		{
			m_item = item;
			m_previousSlotPosition = (eInventorySlot)previousSlotPosition;
		}

		/// <summary>
		/// Gets the equipped item
		/// </summary>
		public InventoryItem Item
		{
			get { return m_item; }
		}

		/// <summary>
		/// Gets the previous slot position
		/// </summary>
		public eInventorySlot PreviousSlotPosition
		{
			get { return m_previousSlotPosition; }
		}
	}
}
