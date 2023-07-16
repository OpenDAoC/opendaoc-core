using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Reduce range needed to cast the sepll
	/// </summary>
	[SpellHandler("Silence")]
	public class SilenceSpellHandler : SpellHandler
	{
		/// <summary>
		/// Apply effect on target or do spell action if non duration spell
		/// </summary>
		/// <param name="target">target that gets the effect</param>
		/// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
	        /*
            GameSpellEffect effect;
            effect = SpellHandler.FindEffectOnTarget(target, "Silence");
			if(effect!=null)
            {
				MessageToCaster("Your target already have an effect of that type!", eChatType.CT_SpellResisted);
				return;
            }*/
            base.ApplyEffectOnTarget(target, effectiveness);
        }
		
		public override void OnEffectStart(GameSpellEffect effect)
		{
			base.OnEffectStart(effect);
			if(effect.Owner is GamePlayer)
			{
				effect.Owner.SilencedTime = effect.Owner.CurrentRegion.Time + CalculateEffectDuration(effect.Owner, Caster.Effectiveness);
				effect.Owner.StopCurrentSpellcast();
				effect.Owner.StartInterruptTimer(effect.Owner.SpellInterruptDuration, AttackData.eAttackType.Spell, Caster);
			}
		}

		// constructor
		public SilenceSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine) {}
	}
}
