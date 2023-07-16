using System;
using DOL.GS;

namespace DOL.Events
{
	/// <summary>
	/// Description r�sum�e de InteractWithEventArgs.
	/// </summary>
	public class InteractWithEventArgs : EventArgs
	{
		private GameObject m_target;

		public InteractWithEventArgs(GameObject target)
		{
			this.m_target = target;
		}

		/// <summary>
		/// Gets the GameObject target whose player interact with
		/// </summary>
		public virtual GameObject Target
		{
			get { return m_target; }
		}

	}
}
