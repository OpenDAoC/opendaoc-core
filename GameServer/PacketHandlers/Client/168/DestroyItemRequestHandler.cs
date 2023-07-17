using System;
using System.Collections;
using DOL.Database;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.DestroyItemRequest, "Handles destroy item requests from client", eClientStatus.PlayerInGame)]
	public class DestroyItemRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			packet.Skip(4);
			int slot = packet.ReadShort();
			InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
			if (item != null)
			{
				if (item.IsIndestructible)
				{
					client.Out.SendMessage($"You can't destroy {item.GetName(0, false)}!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}

				if (item.Id_nb == "ARelic")
				{
					client.Out.SendMessage("You cannot destroy a relic!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}

				if (client.Player.Inventory.EquippedItems.Contains(item))
				{
					client.Out.SendMessage("You cannot destroy an equipped item!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return;
				}

				if (client.Player.Inventory.RemoveItem(item))
				{
					client.Out.SendMessage($"You destroy the {item.Name}.", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					InventoryLogging.LogInventoryAction(client.Player, "(destroy)", eInventoryActionType.Other, item.Template, item.Count);
				}
			}
		}
	}
}
