using System;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.GameOpenRequest, "Checks if UDP is working for the client", eClientStatus.None)]
	public class GameOpenRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			int flag = packet.ReadByte();
			client.UdpPingTime = DateTime.Now.Ticks;
			client.UdpConfirm = flag == 1;
			client.Out.SendGameOpenReply();
			client.Out.SendStatusUpdate(); // based on 1.74 logs
			client.Out.SendUpdatePoints(); // based on 1.74 logs
			client.Player?.UpdateDisabledSkills(); // based on 1.74 logs
		}
	}
}
