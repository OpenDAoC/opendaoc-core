using System;
using System.Net;
using System.Reflection;
using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.UDP, EClientPackets.UDPInitRequest, "Handles UDP init", eClientStatus.None)]
	public class UdpInitRequestHandler : IPacketHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			string localIP;
			ushort localPort;
			if (client.Version >= GameClient.EClientVersion.Version1124)
			{
				localIP = packet.ReadString(20);
				localPort = packet.ReadShort();
			}
			else
			{
				localIP = packet.ReadString(22);
				localPort = packet.ReadShort();
			}
			client.LocalIP = localIP;
			// client.UdpEndPoint = new IPEndPoint(IPAddress.Parse(localIP), localPort);
			client.Out.SendUDPInitReply();
		}
	}
}
