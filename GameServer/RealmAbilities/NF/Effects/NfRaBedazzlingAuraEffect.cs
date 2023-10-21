using System;
using System.Collections.Generic;
using Core.Events;
using Core.GS.Effects;
using Core.GS.Enums;

namespace Core.GS.RealmAbilities
{
	public class NfRaBedazzlingAuraEffect : TimedEffect, IGameEffect
	{
		private int m_value;

		/// <summary>
		/// Default constructor for AmelioratingMelodiesEffect
		/// </summary>
		public NfRaBedazzlingAuraEffect()
			: base(30000)
		{

		}

		/// <summary>
        /// Called when effect is to be started
		/// </summary>
		/// <param name="living"></param>
		/// <param name="value"></param>
		public void Start(GameLiving living, int value)
		{
			m_value = value;

			if (living.TempProperties.GetProperty(RealmAbilities.NfRaBarrierOfFortitudeAbility.BofBaSb, false))
				return;

			base.Start(living);

			living.AbilityBonus[(int)EProperty.Resist_Body] += m_value;
			living.AbilityBonus[(int)EProperty.Resist_Cold] += m_value;
			living.AbilityBonus[(int)EProperty.Resist_Energy] += m_value;
			living.AbilityBonus[(int)EProperty.Resist_Heat] += m_value;
			living.AbilityBonus[(int)EProperty.Resist_Matter] += m_value;
			living.AbilityBonus[(int)EProperty.Resist_Spirit] += m_value;

			if (living is GamePlayer)
			{
				GameEventMgr.AddHandler(living, GamePlayerEvent.Quit, new CoreEventHandler(PlayerLeftWorld));
				(living as GamePlayer).Out.SendCharResistsUpdate();
			}
			living.TempProperties.SetProperty(RealmAbilities.NfRaBarrierOfFortitudeAbility.BofBaSb, true);
		}

		/// <summary>
		/// Called when a player leaves the game
		/// </summary>
		/// <param name="e">The event which was raised</param>
		/// <param name="sender">Sender of the event</param>
		/// <param name="args">EventArgs associated with the event</param>
		private static void PlayerLeftWorld(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = (GamePlayer)sender;

			NfRaBedazzlingAuraEffect BoFEffect = player.EffectList.GetOfType<NfRaBedazzlingAuraEffect>();
			if (BoFEffect != null)
			{
				BoFEffect.Cancel(false);
			}
		}

		public override void Stop()
		{
			base.Stop();

			m_owner.AbilityBonus[(int)EProperty.Resist_Body] -= m_value;
			m_owner.AbilityBonus[(int)EProperty.Resist_Cold] -= m_value;
			m_owner.AbilityBonus[(int)EProperty.Resist_Energy] -= m_value;
			m_owner.AbilityBonus[(int)EProperty.Resist_Heat] -= m_value;
			m_owner.AbilityBonus[(int)EProperty.Resist_Matter] -= m_value;
			m_owner.AbilityBonus[(int)EProperty.Resist_Spirit] -= m_value;
			if (m_owner is GamePlayer)
			{
				(m_owner as GamePlayer).Out.SendCharResistsUpdate();
				GameEventMgr.RemoveHandler(m_owner, GamePlayerEvent.Quit, new CoreEventHandler(PlayerLeftWorld));
			}
			m_owner.TempProperties.RemoveProperty(RealmAbilities.NfRaBarrierOfFortitudeAbility.BofBaSb);
		}


		/// <summary>
		/// Name of the effect
		/// </summary>
		public override string Name
		{
			get
			{
				return "Bedazzling Aura";
			}
		}

		/// <summary>
		/// Icon ID
		/// </summary>
		public override UInt16 Icon
		{
			get
			{
				return 3029;
			}
		}

		/// <summary>
		/// Delve information
		/// </summary>
		public override IList<string> DelveInfo
		{
			get
			{
				var delveInfoList = new List<string>(8);
				delveInfoList.Add("Grants the group increased resistance to magical damage (Does not stack with Soldier's Barricade or Barrier of Fortitude).");
				delveInfoList.Add(" ");
				delveInfoList.Add("Value: " + m_value + "%");

				int seconds = (int)(RemainingTime / 1000);
				if (seconds > 0)
				{
					delveInfoList.Add(" ");
					delveInfoList.Add("- " + seconds + " seconds remaining.");
				}

				return delveInfoList;
			}
		}
	}
}
