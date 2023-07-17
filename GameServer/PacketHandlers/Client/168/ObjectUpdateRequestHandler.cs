
namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.ObjectUpdateRequest, "Update all GameObjects in Playerrange", eClientStatus.PlayerInGame)]
    public class ObjectUpdateRequestHandler : IPacketHandler
    {
        public void HandlePacket(GameClient client, GsPacketIn packet)
        {
            // Will be picked up by the player service.
            client.Player.LastWorldUpdate = 0;
        }
    }
}
