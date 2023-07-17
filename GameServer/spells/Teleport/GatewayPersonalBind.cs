using System.Collections.Generic;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Spells
{
    /// <summary>
    /// The spell used for the Personal Bind Recall Stone.
    /// </summary>
    /// <author>Aredhel</author>
    [SpellHandlerAttribute("GatewayPersonalBind")]
	public class GatewayPersonalBind : SpellHandler
	{
		public GatewayPersonalBind(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine) { }

		/// <summary>
		/// Can this spell be queued with other spells?
		/// </summary>
		public override bool CanQueue => false;

		/// <summary>
		/// Whether this spell can be cast on the selected target at all.
		/// </summary>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			if (Caster is not GamePlayer player)
				return false;

			if (((player.CurrentZone != null && player.CurrentZone.IsRvR) || (player.CurrentRegion != null && player.CurrentRegion.IsInstance)) && GameServer.Instance.Configuration.ServerType != EGameServerType.GST_PvE)
			{
				// Actual live message is: You can't use that item!
				player.Out.SendMessage("You can't use that here!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if (player.IsMoving)
			{
				player.Out.SendMessage("You must be standing still to use this item!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if (player.InCombat || GameStaticRelic.IsPlayerCarryingRelic(player))
			{
				player.Out.SendMessage("You have been in combat recently and cannot use this item!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if(player.CurrentRegion.ID == 497 && player.Client.Account.PrivLevel == 1)
			{
				player.Out.SendMessage("You can't use Bind Stone in Jail!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Always a constant casting time
		/// </summary>
		public override int CalculateCastingTime()
		{
			return m_spell.CastTime;
		}

		/// <summary>
		/// Apply the effect.
		/// </summary>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			if (Caster is not GamePlayer player)
				return;

			if (player.InCombat || GameStaticRelic.IsPlayerCarryingRelic(player) || player.IsMoving)
				return;

			SendEffectAnimation(player, 0, false, 1);
			UniPortalEffect effect = new(this, 1000);
			effect.Start(player);
			player.MoveTo((ushort)player.BindRegion, player.BindXpos, player.BindYpos, player.BindZpos, (ushort)player.BindHeading);
		}

		public override void CasterMoves()
		{
			InterruptCasting();

			if (Caster is GamePlayer)
				(Caster as GamePlayer).Out.SendMessage(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SpellHandler.CasterMove"), eChatType.CT_Important, eChatLoc.CL_SystemWindow);
		}

		public override IList<string> DelveInfo
		{
			get
			{
				List<string> list = new();
				list.Add(string.Format("  {0}", Spell.Description));
				return list;
			}
		}
	}
}
