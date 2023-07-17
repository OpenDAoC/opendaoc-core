
namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(EPacketHandlerType.TCP, EClientPackets.PlayerDismountRequest, "Handles Player Dismount Request.", eClientStatus.PlayerInGame)]
	public class PlayerDismountRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GsPacketIn packet)
		{
			new DismountRequestHandler(client.Player).Start(1);
		}

		/// <summary>
		/// Handles player dismount requests
		/// </summary>
		protected class DismountRequestHandler : RegionAction
		{
			/// <summary>
			/// Constructs a new DismountRequestHandler
			/// </summary>
			/// <param name="actionSource"></param>
			public DismountRequestHandler(GamePlayer actionSource) : base(actionSource)
			{
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				if (!player.IsRiding)
				{
					ChatUtil.SendSystemMessage(player, "You are not riding any steed!");
					return 0;
				}

				player.DismountSteed(false);
				return 0;
			}
		}
	}
}