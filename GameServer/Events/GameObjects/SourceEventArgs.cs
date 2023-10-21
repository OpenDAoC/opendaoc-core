using System;
using DOL.GS;

namespace DOL.Events;

public class SourceEventArgs : EventArgs
{
	private GameLiving source;

	public SourceEventArgs(GameLiving source)
	{
		this.source = source;
	}

	/// <summary>
	/// The GameLiving who caused this event
	/// </summary>
	public virtual GameLiving Source
	{
		get { return source; }
	}
}