using System;
using System.Reflection;
using System.Linq;

using DOL.Database;
using DOL.Events;

using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// No longer used after version 1.104
	/// </summary>
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.CharacterDeleteRequest, "Handles character delete requests", eClientStatus.LoggedIn)]
	public class CharacterDeleteRequestHandler : IPacketHandler
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			string charName = packet.ReadString(30);
			DbCoreCharacters[] chars = client.Account.Characters;

			var foundChar = chars?.FirstOrDefault(ch => ch.Name.Equals(charName, StringComparison.OrdinalIgnoreCase));
			if (foundChar != null)
			{
				var slot = foundChar.AccountSlot;
				CharacterCreateRequestHandler.CheckForDeletedCharacter(foundChar.AccountName, client, slot);
			}
		}
	}
}
