using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.Effects;
using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// The spell used by classic teleporters.
	/// </summary>
	/// <author>Aredhel</author>
	[SpellHandlerAttribute("UniPortal")]
	public class UniPortal : SpellHandler
	{
		private DbTeleports m_destination;

		public UniPortal(GameLiving caster, Spell spell, SpellLine spellLine, DbTeleports destination)
			: base(caster, spell, spellLine) 
		{
			m_destination = destination;
		}

		/// <summary>
		/// Whether this spell can be cast on the selected target at all.
		/// </summary>
		/// <param name="selectedTarget"></param>
		/// <returns></returns>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			if (!base.CheckBeginCast(selectedTarget))
				return false;
			return (selectedTarget is GamePlayer);
		}

		/// <summary>
		/// Apply the effect.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="effectiveness"></param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			GamePlayer player = target as GamePlayer;
			if (player == null)
				return;
			
			if (player.InCombat || GameStaticRelic.IsPlayerCarryingRelic(player))
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "GamePlayer.UseSlot.CantUseInCombat"), EChatType.CT_System, EChatLoc.CL_SystemWindow);
				return;
			}
			
			SendEffectAnimation(player, 0, false, 1);

			UniPortalEffect effect = new UniPortalEffect(this, 1000);
			effect.Start(player);

			player.LeaveHouse();
			player.MoveTo((ushort)m_destination.RegionID, m_destination.X, m_destination.Y, m_destination.Z, (ushort)m_destination.Heading);
		}
	}
	
	[SpellHandlerAttribute("UniPortalKeep")]
	public class UniPortalKeep : SpellHandler
	{
		private DbKeepDoorTeleports m_destination;

		public UniPortalKeep(GameLiving caster, Spell spell, SpellLine spellLine, DbKeepDoorTeleports destination)
			: base(caster, spell, spellLine)
		{
			m_destination = destination;
		}

		/// <summary>
		/// Whether this spell can be cast on the selected target at all.
		/// </summary>
		/// <param name="selectedTarget"></param>
		/// <returns></returns>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			if (!base.CheckBeginCast(selectedTarget))
				return false;
			return (selectedTarget is GamePlayer);
		}

		/// <summary>
		/// Apply the effect.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="effectiveness"></param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			GamePlayer player = target as GamePlayer;
			if (player == null)
				return;

			/*
			if (player.IsAlive && !player.IsStunned && !player.IsMezzed)
			{
			    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client, "GamePlayer.UseSlot.CantUseInCombat"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
			    return;
			}
			*/
			SendEffectAnimation(player, 0, false, 1);

			UniPortalEffect effect = new UniPortalEffect(this, 1500);
			effect.Start(player);

			player.LeaveHouse();
			player.MoveTo((ushort)m_destination.Region, m_destination.X, m_destination.Y, m_destination.Z, (ushort)m_destination.Heading);
		}
	}
}
