using DOL.GS.Effects;

namespace DOL.GS.PacketHandler.Client.v168
{
	/// <summary>
	/// Called when player removes concentration spell in conc window
	/// </summary>
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.RemoveConcentrationEffect, "Handles Concentration Effect Remove Request", eClientStatus.PlayerInGame)]
	public class RemoveConcentrationEffectHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			int index = packet.ReadByte();

			new CancelEffectHandler(client.Player, index).Start(1);
		}

		/// <summary>
		/// Handles player cancel effect requests
		/// </summary>
		protected class CancelEffectHandler : RegionAction
		{
			/// <summary>
			/// The effect index
			/// </summary>
			protected readonly int m_index;

			/// <summary>
			/// Constructs a new CancelEffectHandler
			/// </summary>
			/// <param name="actionSource">The action source</param>
			/// <param name="index">The effect index</param>
			public CancelEffectHandler(GamePlayer actionSource, int index) : base(actionSource)
			{
				m_index = index;
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				var player = (GamePlayer) m_actionSource;

				IConcentrationEffect effect = null;
                lock (player.effectListComponent.EffectsLock)
                {
                    if (m_index < player.effectListComponent.ConcentrationEffects.Count)
					{
						effect = player.effectListComponent.ConcentrationEffects[m_index];
					}
				}

				EffectService.RequestImmediateCancelConcEffect(effect, true);
				return 0;
			}
		}
	}
}