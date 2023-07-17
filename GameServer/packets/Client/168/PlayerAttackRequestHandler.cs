
namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.PlayerAttackRequest, "Handles Player Attack Request", eClientStatus.PlayerInGame)]
	public class PlayerAttackRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			var mode = (byte) packet.ReadByte();
			bool userAction = packet.ReadByte() == 0;
				// set to 0 if user pressed the button, set to 1 if client decided to stop attack

			new AttackRequestHandler(client.Player, mode != 0, userAction).Start(1);
		}

		/// <summary>
		/// Handles change attack mode requests
		/// </summary>
		protected class AttackRequestHandler : RegionAction
		{
			/// <summary>
			/// True if attack should be started
			/// </summary>
			protected readonly bool m_start;

			/// <summary>
			/// True if user initiated the action else was done by the client
			/// </summary>
			protected readonly bool m_userAction;

			/// <summary>
			/// Constructs a new AttackRequestHandler
			/// </summary>
			/// <param name="actionSource">The action source</param>
			/// <param name="start">True if attack should be started</param>
			/// <param name="userAction">True if user initiated the action else was done by the client</param>
			public AttackRequestHandler(GamePlayer actionSource, bool start, bool userAction) : base(actionSource)
			{
				m_start = start;
				m_userAction = userAction;
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				if (player.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
				{
					if (m_userAction)
						player.Out.SendMessage("You can't enter melee combat mode with a fired weapon!", eChatType.CT_YouHit,
						                       eChatLoc.CL_SystemWindow);
					return 0;
				}

				if (m_start)
				{
					player.attackComponent.RequestStartAttack(player.TargetObject);
					// unstealth right after entering combat mode if anything is targeted
					if (player.attackComponent.AttackState && player.TargetObject != null)
						player.Stealth(false);
					return 0;
				}
				else
				{
					player.attackComponent.StopAttack();
					return 0;
				}
			}
		}
	}
}