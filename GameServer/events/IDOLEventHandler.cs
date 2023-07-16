using System;

namespace DOL.Events
{
	/// <summary>
	/// Objects Able to handle Notifications from DOLEvents.
	/// </summary>
	public interface IDOLEventHandler
	{
		void Notify(DOL.Events.DOLEvent e, object sender, EventArgs args);
	}
}
