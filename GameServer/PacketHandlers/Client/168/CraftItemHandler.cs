using System;
using DOL.Database;

namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// makeproducthandler handle the crafted product start
	/// </summary>
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.CraftRequest, "Handles the crafted product answer", eClientStatus.PlayerInGame)]
	public class CraftItemHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			ushort ItemID = packet.ReadShort();
			client.Player.CraftItem(ItemID);
		}
	}
}
