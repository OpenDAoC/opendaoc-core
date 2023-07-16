using System;
using System.Collections;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.WarmapBonusRequest, "Show warmap bonuses", eClientStatus.PlayerInGame)]
	public class WarmapBonusesRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			client.Out.SendWarmapBonuses();
		}
	}
}