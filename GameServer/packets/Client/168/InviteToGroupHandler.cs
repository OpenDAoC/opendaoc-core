namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.InviteToGroup, "Handle Invite to Group Request.", eClientStatus.PlayerInGame)]
	public class InviteToGroupHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			new HandleGroupInviteAction(client.Player).Start(1);
		}

		/// <summary>
		/// Handles group invlite actions
		/// </summary>
		protected class HandleGroupInviteAction : RegionECSAction
		{
			/// <summary>
			/// constructs a new HandleGroupInviteAction
			/// </summary>
			/// <param name="actionSource">The action source</param>
			public HandleGroupInviteAction(GamePlayer actionSource) : base(actionSource)
			{
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				if (player.TargetObject == null || player.TargetObject == player)
				{
					ChatUtil.SendSystemMessage(player, "You have not selected a valid player as your target.");
					return 0;
				}

				if (!(player.TargetObject is GamePlayer))
				{
					ChatUtil.SendSystemMessage(player, "You have not selected a valid player as your target.");
					return 0;
				}

				var target = (GamePlayer) player.TargetObject;

				if (player.Group != null && player.Group.Leader != player)
				{
					ChatUtil.SendSystemMessage(player, "You are not the leader of your group.");
					return 0;
				}
                
                if (player.Group != null && player.Group.MemberCount >= ServerProperties.Properties.GROUP_MAX_MEMBER)
				{
					ChatUtil.SendSystemMessage(player, "The group is full.");
					return 0;
				}

				if (!GameServer.ServerRules.IsAllowedToGroup(player, target, false))
					return 0;

				if (target.Group != null)
				{
					ChatUtil.SendSystemMessage(player, "The player is still in a group.");
					return 0;
				}

				ChatUtil.SendSystemMessage(player, "You have invited " + target.Name + " to join your group.");
				target.Out.SendGroupInviteCommand(player,
				                                  player.Name + " has invited you to join\n" + player.GetPronoun(1, false) +
				                                  " group. Do you wish to join?");
				ChatUtil.SendSystemMessage(target,
				                           player.Name + " has invited you to join " + player.GetPronoun(1, false) + " group.");

				return 0;
			}
		}
	}
}