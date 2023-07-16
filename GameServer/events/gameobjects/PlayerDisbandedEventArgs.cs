using System;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Holds the arguments for the PlayerDisbanded event of PlayerGroup.
	/// </summary>
	public class MemberDisbandedEventArgs : EventArgs
	{
		private GameLiving m_member;

		/// <summary>
		/// Constructs new PlayerDisbandedEventArgs
		/// </summary>
		/// <param name="living">The disbanded living</param>
		public MemberDisbandedEventArgs(GameLiving living)
		{
			m_member = living;
		}

		/// <summary>
		/// The disbanded player
		/// </summary>
		public GameLiving Member
		{
			get { return m_member; }
		}
	}
}
