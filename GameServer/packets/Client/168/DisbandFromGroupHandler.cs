namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// Handles the disband group packet
	/// </summary>
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.DisbandFromGroup, "Disband From Group Request Handler", eClientStatus.PlayerInGame)]
	public class DisbandFromGroupHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			new PlayerDisbandAction(client.Player).Start(1);
		}

		/// <summary>
		/// Handles players disband actions
		/// </summary>
		protected class PlayerDisbandAction : RegionAction
		{
			/// <summary>
			/// Constructs a new PlayerDisbandAction
			/// </summary>
			/// <param name="actionSource">The disbanding player</param>
			public PlayerDisbandAction(GamePlayer actionSource) : base(actionSource)
			{
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				if (player.Group == null)
					return 0;

				GameLiving disbandMember = player;

				if (player.TargetObject != null &&
				    player.TargetObject is GameLiving &&
				    (player.TargetObject as GameLiving).Group != null &&
				    (player.TargetObject as GameLiving).Group == player.Group)
					disbandMember = player.TargetObject as GameLiving;

				if (disbandMember != player && player != player.Group.Leader)
					return 0;

				player.Group.RemoveMember(disbandMember);
				return 0;
			}
		}
	}
}