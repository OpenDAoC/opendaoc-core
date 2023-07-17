using System;
using System.Collections;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.WarmapBonusRequest, "Show warmap bonuses", eClientStatus.PlayerInGame)]
	public class WarMapBonusesRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			client.Out.SendWarmapBonuses();
		}
	}
}