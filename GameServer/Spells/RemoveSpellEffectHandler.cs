using System;
using System.Collections.Generic;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Base class for all spells that remove effects
	/// </summary>
	public abstract class RemoveSpellEffectHandler : SpellHandler
	{
		/// <summary>
		/// Stores spell effect type that will be removed
		/// RR4: now its a list of effects to remove
		/// </summary>
		protected List<string> m_spellTypesToRemove = null;

		/// <summary>
		/// Spell effect type that will be removed
		/// RR4: now its a list of effects to remove
		/// </summary>
		public virtual List<string> SpellTypesToRemove
		{
			get { return m_spellTypesToRemove; }
		}

		/// <summary>
		/// called when spell effect has to be started and applied to targets
		/// </summary>
		/// <param name="target"></param>
		public override void FinishSpellCast(GameLiving target)
		{
			m_caster.Mana -= PowerCost(target);
			base.FinishSpellCast(target);
		}

		public override void OnEffectPulse(GameSpellEffect effect)
		{
			base.OnEffectPulse(effect);
			OnDirectEffect(effect.Owner, effect.Effectiveness);
		}
		/// <summary>
		/// execute non duration spell effect on target
		/// </summary>
		/// <param name="target"></param>
		/// <param name="effectiveness"></param>
		public override void OnDirectEffect(GameLiving target, double effectiveness)
		{
			base.OnDirectEffect(target, effectiveness);
			if (target == null || !target.IsAlive)
				return;

			// RR4: we remove all the effects
			foreach (string toRemove in SpellTypesToRemove)
			{
				//GameSpellEffect effect = target.FindEffectOnTarget(toRemove);
				EEffect.TryParse(toRemove, out EEffect effectType);
				ECSGameEffect effect = EffectListService.GetEffectOnTarget(target, effectType);
				if (toRemove == "Mesmerize")
					effect = EffectListService.GetEffectOnTarget(target, EEffect.Mez);
				if (effect != null)
					EffectService.RequestCancelEffect(effect);
					//effect.Cancel(false);
			}
			SendEffectAnimation(target, 0, false, 1);
		}

		// constructor
		protected RemoveSpellEffectHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}