using System;

namespace DOL.Events
{
	/// <summary>
	/// Objects Able to handle Notifications from DOLEvents.
	/// </summary>
	public interface ICoreEventHandler
	{
		void Notify(DOL.Events.CoreEvent e, object sender, EventArgs args);
	}
}
