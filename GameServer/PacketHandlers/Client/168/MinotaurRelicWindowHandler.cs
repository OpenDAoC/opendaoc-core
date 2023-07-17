using System;
using System.Reflection;
using DOL.GS;
using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.MinotaurRelicWindow, "Handles Relic window commands", eClientStatus.PlayerInGame)]
    public class MinotaurRelicWindowHandler : IPacketHandler
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void HandlePacket(GameClient client, GsPacketIn packet)
        {
            // todo
        }
    }
}