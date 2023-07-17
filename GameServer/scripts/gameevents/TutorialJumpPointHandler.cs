using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.ServerRules;

namespace DOL.GS.GameEvents
{
	/// <summary>
	/// Handles Tutorial Jump Point Exits
	/// </summary>
	public class TutorialJumpPointHandler : IJumpPointHandler
	{
		/// <summary>
		/// Decides whether player can jump to the target point.
		/// All messages with reasons must be sent here.
		/// Can change destination too.
		/// </summary>
		/// <param name="targetPoint">The jump destination</param>
		/// <param name="player">The jumping player</param>
		/// <returns>True if allowed</returns>
		public bool IsAllowedToJump(DbZonePoints targetPoint, GamePlayer player)
		{
			StartupLocation loc = StartupLocationsEvent.GetNonTutorialLocation(player);

			if (loc != null)
			{
				targetPoint.TargetX = loc.XPos;
				targetPoint.TargetY = loc.YPos;
				targetPoint.TargetZ = loc.ZPos;
				targetPoint.TargetHeading = (ushort)loc.Heading;
				targetPoint.TargetRegion = (ushort)loc.Region;
				return true;
			}

			return false;
		}
	}
}
