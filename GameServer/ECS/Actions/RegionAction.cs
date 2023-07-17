using System;
using System.Text;

namespace DOL.GS
{
	public abstract class RegionAction : ECSGameTimer
	{
		/// <summary>
		/// The source of the action
		/// </summary>
		protected readonly GameObject m_actionSource;

		/// <summary>
		/// Constructs a new region action
		/// </summary>
		/// <param name="actionSource">The action source</param>
		public RegionAction(GameObject actionSource) : base(actionSource)
		{
			if (actionSource == null)
				throw new ArgumentNullException("actionSource");
			m_actionSource = actionSource;

			TimerOwner = actionSource;
			Callback = new ECSTimerCallback(OnTick);
		}

		protected abstract int OnTick(ECSGameTimer timer);

		/// <summary>
		/// Returns short information about the timer
		/// </summary>
		/// <returns>Short info about the timer</returns>
		public override string ToString()
		{
			return new StringBuilder(base.ToString(), 128)
				.Append(" actionSource: (").Append(m_actionSource.ToString())
				.Append(')')
				.ToString();
		}
	}

	public abstract class AuxRegionAction : AuxECSGameTimer
	{
		/// <summary>
		/// The source of the action
		/// </summary>
		protected readonly GameObject m_actionSource;

		/// <summary>
		/// Constructs a new region action
		/// </summary>
		/// <param name="actionSource">The action source</param>
		public AuxRegionAction(GameObject actionSource) : base(actionSource)
		{
			if (actionSource == null)
				throw new ArgumentNullException("actionSource");
			m_actionSource = actionSource;

			TimerOwner = actionSource;
			Callback = new AuxECSTimerCallback(OnTick);
		}

		protected abstract int OnTick(AuxECSGameTimer timer);

		/// <summary>
		/// Returns short information about the timer
		/// </summary>
		/// <returns>Short info about the timer</returns>
		public override string ToString()
		{
			return new StringBuilder(base.ToString(), 128)
				.Append(" actionSource: (").Append(m_actionSource.ToString())
				.Append(')')
				.ToString();
		}
	}
}
