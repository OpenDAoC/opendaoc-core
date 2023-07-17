using System;
using DOL.Database;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the account events
	/// </summary>
	public class AccountEventArgs : EventArgs
	{
		/// <summary>
		/// Holds the target account for this event
		/// </summary>
		private DbAccounts m_account;
		
		/// <summary>
		/// Constructs a new event argument class for the
		/// account events 
		/// </summary>
		/// <param name="account"></param>
		public AccountEventArgs(DbAccounts account)
		{
			m_account = account;
		}

		/// <summary>
		/// Gets the target account for this event
		/// </summary>
		public DbAccounts Account
		{
			get
			{
				return m_account;
			}
		}
	}
}
