using System;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.CreateNPCRequest, "Handles requests for npcs(0x72) in game", eClientStatus.PlayerInGame)]
	public class NpcCreationRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			if (client.Player == null)
				return;
			Region region = client.Player.CurrentRegion;
			if (region == null)
				return;

			ushort id;
			if (client.Version >= GameClient.EClientVersion.Version1126)
				id = packet.ReadShortLowEndian(); // Dre: disassembled game.dll show a write of uint, is it a wip in the game.dll?
			else
				id = packet.ReadShort();
			GameNPC npc = region.GetObject(id) as GameNPC;
			if (npc == null || !client.Player.IsWithinRadius(npc, WorldMgr.OBJ_UPDATE_DISTANCE))
			{
				client.Out.SendObjectDelete(id);
				return;
			}

			if(npc != null)
			{
				Tuple<ushort, ushort> key = new Tuple<ushort, ushort>(npc.CurrentRegionID, (ushort)npc.ObjectID);
				
				long updatetime;
				if (!client.GameObjectUpdateArray.TryGetValue(key, out updatetime))
				{
					updatetime = 0;
				}
				
				client.Out.SendNPCCreate(npc);
				// override update from npc create as this is a client request !
				if (updatetime > 0)
					client.GameObjectUpdateArray[key] = updatetime;
				
				if(npc.Inventory != null)
					client.Out.SendLivingEquipmentUpdate(npc);
				
				//DO NOT SEND A NPC UPDATE, it is done in Create anyway
				//Sending a Update causes a UDP packet to be sent and
				//the client will get the UDP packet before the TCP Create packet
				//Causing the client to issue another NPC CREATION REQUEST!
				//client.Out.SendNPCUpdate(npc); <-- BIG NO NO
			}
		}
	}
}
