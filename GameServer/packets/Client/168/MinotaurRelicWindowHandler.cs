using System;
using System.Reflection;
using DOL.GS;
using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.MinotaurRelicWindow, "Handles Relic window commands", eClientStatus.PlayerInGame)]
    public class MinotaurRelicWindowHandler : IPacketHandler
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void HandlePacket(GameClient client, GSPacketIn packet)
        {
            // todo
        }
    }
}