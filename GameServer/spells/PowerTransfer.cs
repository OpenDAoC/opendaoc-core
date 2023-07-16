using System;
using System.Collections.Generic;
using System.Text;
using DOL.AI.Brain;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Spell handler for power trasnfer.
	/// </summary>
	/// <author>Aredhel</author>
	[SpellHandlerAttribute("PowerTransfer")]
	class PowerTransfer : SpellHandler
	{
		/// <summary>
		/// Check if player tries to transfer power to himself.
		/// </summary>
		/// <param name="selectedTarget"></param>
		/// <returns></returns>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			GamePlayer owner = Owner();
			if (owner == null || selectedTarget == null)
				return false;

			if (selectedTarget == Caster || selectedTarget == owner)
			{
				owner.Out.SendMessage("You cannot transfer power to yourself!",
					eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				return false;
			}

			return base.CheckBeginCast(selectedTarget);
		}

		/// <summary>
		/// Execute direct effect.
		/// </summary>
		/// <param name="target">Target power is transferred to.</param>
		/// <param name="effectiveness">Effectiveness of the spell (0..1, equalling 0-100%).</param>
		public override void OnDirectEffect(GameLiving target, double effectiveness)
		{
			if (target == null) return;
			if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;

			// Calculate the amount of power to transfer from the owner.
			// TODO: Effectiveness plays a part here.

			GamePlayer owner = Owner();
			if (owner == null)
				return;

			int powerTransfer = (int)Math.Min(Spell.Value, owner.Mana);
			int powerDrained = -owner.ChangeMana(owner, eManaChangeType.Spell, -powerTransfer);

			if (powerDrained <= 0)
				return;

			int powerHealed = target.ChangeMana(owner, eManaChangeType.Spell, powerDrained);

			if (powerHealed <= 0)
			{
				SendEffectAnimation(target, 0, false, 0);
				owner.Out.SendMessage(String.Format("{0} is at full power already!",
					target.Name), eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
			}
			else
			{
				SendEffectAnimation(target, 0, false, 1);
				owner.Out.SendMessage(String.Format("You transfer {0} power to {1}!",
					powerHealed, target.Name), eChatType.CT_Spell, eChatLoc.CL_SystemWindow);

				if (target is GamePlayer)
					(target as GamePlayer).Out.SendMessage(String.Format("{0} transfers {1} power to you!",
						owner.Name, powerHealed), eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
			}
		}

		/// <summary>
		/// Returns a reference to the shade.
		/// </summary>
		/// <returns></returns>
		protected virtual GamePlayer Owner()
		{
			if (Caster is GamePlayer)
				return Caster as GamePlayer;
			
			return null;
		}

		/// <summary>
        /// Create a new handler for the power transfer spell.
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="spell"></param>
		/// <param name="line"></param>
		public PowerTransfer(GameLiving caster, Spell spell, SpellLine line) 
            : base(caster, spell, line) { }
	}
}
