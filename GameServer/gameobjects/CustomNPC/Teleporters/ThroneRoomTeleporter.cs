using System;
using System.Linq;

using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// Teleporter for entering and leaving the throne room.
	/// </summary>
	/// <author>Aredhel</author>
	public class ThroneRoomTeleporter : GameNPC
	{
		private static new readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Interact with the NPC.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player) || player == null)
				return false;

			if (GlobalConstants.IsExpansionEnabled((int)EClientExpansion.DarknessRising))
			{
				if (player.CurrentRegion.Expansion == (int)EClientExpansion.DarknessRising)
				{
					SayTo(player, "Do you wish to [exit]?");
				}
				else
				{
					SayTo(player, "Do you require an audience with the [King]?");
				}
				return true;
			}
			else
			{
				String reply = "I am afraid, but the King is busy right now.";

				if (player.Inventory.CountItemTemplate("Personal_Bind_Recall_Stone", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv) == 0)
					reply += " If you're only here to get your Personal Bind Recall Stone then I'll see what I can [do].";

				SayTo(player, reply);
				return false;
			}
		}

		/// <summary>
		/// Talk to the NPC.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="str"></param>
		/// <returns></returns>
		public override bool WhisperReceive(GameLiving source, string text)
		{
			if (!base.WhisperReceive(source, text) || !(source is GamePlayer))
				return false;

			GamePlayer player = source as GamePlayer;

			if ((text.ToLower() == "king" || text.ToLower() == "exit") && GlobalConstants.IsExpansionEnabled((int)EClientExpansion.DarknessRising))
			{
				uint throneRegionID = 0;
				string teleportThroneID = "error";
				string teleportExitID = "error";

				switch (Realm)
				{
					case ERealm.Albion:
						throneRegionID = 394;
						teleportThroneID = "AlbThroneRoom";
						teleportExitID = "AlbThroneExit";
						break;
					case ERealm.Midgard:
						throneRegionID = 360;
						teleportThroneID = "MidThroneRoom";
						teleportExitID = "MidThroneExit";
						break;
					case ERealm.Hibernia:
						throneRegionID = 395;
						teleportThroneID = "HibThroneRoom";
						teleportExitID = "HibThroneExit";
						break;
				}

				if (throneRegionID == 0)
				{
					log.ErrorFormat("Can't find King for player {0} speaking to {1} of realm {2}!", player.Name, Name, Realm);
					player.Out.SendMessage("Server error, can't find throne room.", DOL.GS.PacketHandler.eChatType.CT_Staff, DOL.GS.PacketHandler.eChatLoc.CL_SystemWindow);
					return false;
				}

				DbTeleports teleport = null;

				if (player.CurrentRegionID == throneRegionID)
				{
					teleport = CoreDb<DbTeleports>.SelectObject(DB.Column("TeleportID").IsEqualTo(teleportExitID));
					if (teleport == null)
					{
						log.ErrorFormat("Can't find throne room exit TeleportID {0}!", teleportExitID);
						player.Out.SendMessage("Server error, can't find exit to this throne room.  Moving you to your last bind point.", DOL.GS.PacketHandler.eChatType.CT_Staff, DOL.GS.PacketHandler.eChatLoc.CL_SystemWindow);
						player.MoveToBind();
					}
				}
				else
				{
					teleport = CoreDb<DbTeleports>.SelectObject(DB.Column("TeleportID").IsEqualTo(teleportThroneID));
					if (teleport == null)
					{
						log.ErrorFormat("Can't find throne room TeleportID {0}!", teleportThroneID);
						player.Out.SendMessage("Server error, can't find throne room teleport location.", DOL.GS.PacketHandler.eChatType.CT_Staff, DOL.GS.PacketHandler.eChatLoc.CL_SystemWindow);
					}
				}

				if (teleport != null)
				{
					SayTo(player, "Very well ...");
					player.MoveTo((ushort)teleport.RegionID, teleport.X, teleport.Y, teleport.Z, (ushort)teleport.Heading);
				}

				return true;
			}


			if (text.ToLower() == "do")
			{
				if (player.Inventory.CountItemTemplate("Personal_Bind_Recall_Stone", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv) == 0)
				{
					SayTo(player, "Very well then. Here's your Personal Bind Recall Stone, may it serve you well.");
					player.ReceiveItem(this, "Personal_Bind_Recall_Stone");
				}
				return false;
			}

			return true;
		}
	}
}
