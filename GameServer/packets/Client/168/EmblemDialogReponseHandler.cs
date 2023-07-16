using System;
using DOL.Database;

namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// EmblemDialogReponseHandler is the response of client wend when we close the emblem selection dialogue.
	/// </summary>
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.EmblemDialogResponse, "Handles when a player chooses a guild emblem", eClientStatus.PlayerInGame)]
	public class EmblemDialogReponseHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			if(client.Player.Guild == null)
				return;
			if(!client.Player.Guild.HasRank(client.Player, Guild.eRank.Leader))
				return;
			int primarycolor = packet.ReadByte() & 0x0F; //4bits
			int secondarycolor = packet.ReadByte() & 0x07; //3bits
			int pattern = packet.ReadByte() & 0x03; //2bits
			int logo = packet.ReadByte(); //8bits
			int oldemblem = client.Player.Guild.Emblem;
			int newemblem = ((logo << 9) | (pattern << 7) | (primarycolor << 3) | secondarycolor);
			if (GuildMgr.IsEmblemUsed(newemblem))
			{
				client.Player.Out.SendMessage("This emblem is already in use by another guild, please choose another!", eChatType.CT_System, eChatLoc.CL_SystemWindow );
				return;
			}
			GuildMgr.ChangeEmblem(client.Player, oldemblem, newemblem);
		}
	}
}
