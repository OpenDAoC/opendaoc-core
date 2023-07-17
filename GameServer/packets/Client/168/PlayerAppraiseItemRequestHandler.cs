using DOL.Database;
using DOL.GS.Housing;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.PlayerAppraiseItemRequest, "Player Appraise Item Request handler.", eClientStatus.PlayerInGame)]
	public class PlayerAppraiseItemRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			uint X = packet.ReadInt();
			uint Y = packet.ReadInt();
			ushort id = packet.ReadShort();
			ushort item_slot = packet.ReadShort();

			new AppraiseActionHandler(client.Player, item_slot).Start(1);
		}

		/// <summary>
		/// Handles item apprise actions
		/// </summary>
		protected class AppraiseActionHandler : RegionAction
		{
			/// <summary>
			/// The item slot
			/// </summary>
			protected readonly int m_slot;

			/// <summary>
			/// Constructs a new AppraiseAction
			/// </summary>
			/// <param name="actionSource">The action source</param>
			/// <param name="slot">The item slot</param>
			public AppraiseActionHandler(GamePlayer actionSource, int slot) : base(actionSource)
			{
				m_slot = slot;
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				if (player.TargetObject == null)
					return 0;

				InventoryItem item = player.Inventory.GetItem((eInventorySlot) m_slot);

				if (player.TargetObject is GameMerchant merchant)
					merchant.OnPlayerAppraise(player, item, false);
				else if (player.TargetObject is GameLotMarker lot)
					lot.OnPlayerAppraise(player, item, false);

				return 0;
			}
		}
	}
}