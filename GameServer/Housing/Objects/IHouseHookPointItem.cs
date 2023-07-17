using System;
using DOL.Database;

namespace DOL.GS.Housing
{
	/// <summary>
	/// House item interface.
	/// </summary>
	public interface IHouseHookPointItem
	{
		bool Attach(House house, uint hookpointID, ushort heading);
		bool Attach(House house, DbHouseHookPointItems hookedItem);
		bool Detach(GamePlayer player);
		int Index { get; }
		String TemplateID { get; }
	}
}
