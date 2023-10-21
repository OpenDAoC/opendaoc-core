using System;
using System.Collections.Generic;
using Core.Events;
using Core.GS.Effects;
using Core.GS.Enums;

namespace Core.GS.RealmAbilities
{
	public class NfRaAdrenalineRushEffect : TimedEffect, IGameEffect
	{
		private int m_value;

		/// <summary>
		/// Default constructor for AdrenalineRushEffect
		/// </summary>
		public NfRaAdrenalineRushEffect(int duration, int value)
			: base(duration)
		{
			m_value = value;
		}

		/// <summary>
		/// Called when effect is to be started
		/// </summary>
		/// <param name="living">The living to start the effect for</param>
		public override void Start(GameLiving living)
		{
			base.Start(living);

			if (living is GamePlayer)
				GameEventMgr.AddHandler(living, GamePlayerEvent.Quit, new CoreEventHandler(PlayerLeftWorld));
			living.AbilityBonus[(int)EProperty.MeleeDamage] += m_value;
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

			NfRaAdrenalineRushEffect SPEffect = player.EffectList.GetOfType<NfRaAdrenalineRushEffect>();
			if (SPEffect != null)
			{
				SPEffect.Cancel(false);
			}
		}

		public override void Stop()
		{
			base.Stop();
			m_owner.AbilityBonus[(int)EProperty.MeleeDamage] -= m_value;
			if (m_owner is GamePlayer)
				GameEventMgr.RemoveHandler(m_owner, GamePlayerEvent.Quit, new CoreEventHandler(PlayerLeftWorld));
		}

		/// <summary>
		/// Name of the effect
		/// </summary>
		public override string Name
		{
			get { return "Adrenaline Rush"; }
		}

		/// <summary>
		/// Icon ID
		/// </summary>
		public override UInt16 Icon
		{
			get { return 3001; }
		}

		/// <summary>
		/// Delve information
		/// </summary>
		public override IList<string> DelveInfo
		{
			get
			{
				var delveInfoList = new List<string>(8);
				delveInfoList.Add("Doubles the base melee damage for 20 seconds.");
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