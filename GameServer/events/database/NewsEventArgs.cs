using System;
using DOL.Database;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the news event
	/// </summary>
	public class NewsEventArgs : EventArgs
	{
		/// <summary>
		/// Holds the target news for this event
		/// </summary>
		private DBNews m_news;
		
		/// <summary>
		/// Constructs a new event argument class for the
		/// news events 
		/// </summary>
		/// <param name="account"></param>
		public NewsEventArgs(DBNews news)
		{
			m_news = news;
		}

		/// <summary>
		/// Gets the target news for this event
		/// </summary>
		public DBNews News
		{
			get
			{
				return m_news;
			}
		}
	}
}
