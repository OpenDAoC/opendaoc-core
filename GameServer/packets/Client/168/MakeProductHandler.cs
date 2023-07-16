using System;
using DOL.Database;

namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// makeproducthandler handle the crafted product start
	/// </summary>
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.CraftRequest, "Handles the crafted product answer", eClientStatus.PlayerInGame)]
	public class MakeProductHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			ushort ItemID = packet.ReadShort();
			client.Player.CraftItem(ItemID);
		}
	}
}
