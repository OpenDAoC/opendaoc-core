using System.Reflection;
using DOL;
using Core.Base.Network;
using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.ClientCrash, "Handles client crash packets", eClientStatus.None)]
	public class ClientCrashPacketHandler : IPacketHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			string dllName = packet.ReadString(16);
			packet.Position = 0x50;
			uint upTime = packet.ReadInt();
			string text = $"Client crash ({client.ToString()}) dll:{dllName} clientUptime:{upTime}sec";
			log.Info(text);

			if (log.IsDebugEnabled)
			{
				log.Debug("Last client sent/received packets (from older to newer):");
				foreach (IPacket prevPak in client.PacketProcessor.GetLastPackets())
					log.Info(prevPak.ToHumanReadable());
			}

			//Eden
			client.Out.SendPlayerQuit(true);
			if (client.Player != null)
			{
				client.Player.SaveIntoDatabase();
				client.Player.Quit(true);
			}
			client.Disconnect();
		}
	}
}
