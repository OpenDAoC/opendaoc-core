using System;
using DOL.Database;
using DOL.GS;

namespace DOL.GS.ServerRules
{
	/// <summary>
	/// Denotes a class as a jump point handler
	/// </summary>
	public interface IJumpPointHandler
	{
		/// <summary>
		/// Decides whether player can jump to the target point.
		/// All messages with reasons must be sent here.
		/// Can change destination too.
		/// </summary>
		/// <param name="targetPoint">The jump destination</param>
		/// <param name="player">The jumping player</param>
		/// <returns>True if allowed</returns>
		bool IsAllowedToJump(ZonePoint targetPoint, GamePlayer player);
	}
}
