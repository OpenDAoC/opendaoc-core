using System;
using System.Collections.Generic;
using System.Text;

namespace DOL.Events
{
	/// <summary>
	/// Holds arguments for an item bonus change (for example when an
	/// artifact gains a new bonus).
	/// </summary>
	/// <author>Aredhel</author>
	class ItemBonusChangedEventArgs : EventArgs
	{
		private int m_bonusType, m_bonusAmount;

		public ItemBonusChangedEventArgs(int bonusType, int bonusAmount)
		{
			m_bonusType = bonusType;
			m_bonusAmount = bonusAmount;
		}

		/// <summary>
		/// The bonus type.
		/// </summary>
		public int BonusType
		{
			get { return m_bonusType; }
		}

		/// <summary>
		/// The bonus amount.
		/// </summary>
		public int BonusAmount
		{
			get { return m_bonusAmount; }
		}
	}
}
