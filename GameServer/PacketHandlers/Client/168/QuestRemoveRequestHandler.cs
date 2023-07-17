using System;
using System.Collections;

using DOL.GS.Quests;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.RemoveQuestRequest, "Quest Remove request Handler.", eClientStatus.PlayerInGame)]
	public class QuestRemoveRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			ushort unk1 = packet.ReadShort();
			ushort questIndex = packet.ReadShort();
			ushort unk2 = packet.ReadShort();
			ushort unk3 = packet.ReadShort();

			AbstractQuest quest = null;

			int index = 0;
			lock (client.Player.QuestList)
			{
				foreach (AbstractQuest q in client.Player.QuestList)
				{
					// ignore completed quests
					if (q.Step == -1)
						continue;

					if (index == questIndex)
					{
						quest = q;
						break;
					}

				index++;
				}
			}

			quest?.AbortQuest();
		}
	}
}
