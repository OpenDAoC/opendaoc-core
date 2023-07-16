using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Style combat speed debuff effect spell handler
	/// </summary>
	[SpellHandler("StylePowerDrain")]
	public class StylePowerDrain : DamageToPowerSpellHandler
	{
		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
		
		public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            base.OnDirectEffect(target, effectiveness);
			SendEffectAnimation(target, 0, false, 1);
        }

		// constructor
		public StylePowerDrain(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}
