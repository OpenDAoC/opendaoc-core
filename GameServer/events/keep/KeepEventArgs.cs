using System;

using DOL.GS;
using DOL.GS.Keeps;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the Keep event
	/// </summary>
	public class KeepEventArgs : EventArgs
	{

		/// <summary>
		/// The keep
		/// </summary>
		private AbstractGameKeep m_keep;

		/// <summary>
		/// The realm
		/// </summary>
		private eRealm m_realm;

		/// <summary>
		/// Constructs a new KeepEventArgs
		/// </summary>
		public KeepEventArgs(AbstractGameKeep keep)
		{
			this.m_keep = keep;
		}

		public KeepEventArgs(AbstractGameKeep keep, eRealm realm)
		{
			this.m_keep = keep;
			this.m_realm = realm;
		}

		/// <summary>
		/// Gets the Keep
		/// </summary>
		public AbstractGameKeep Keep
		{
			get { return m_keep; }
		}

		/// <summary>
		/// Gets the Realm
		/// </summary>
		public eRealm Realm
		{
			get { return m_realm; }
		}
	}
}