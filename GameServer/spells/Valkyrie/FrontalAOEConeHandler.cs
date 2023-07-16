using System;
using System.Collections;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Handler to make the frontal pulsing cone show the effect animation on every pulse
	/// </summary>
	[SpellHandlerAttribute("FrontalPulseConeDD")]
	public class FrontalAOEConeHandler : DirectDamageSpellHandler
	{
		public FrontalAOEConeHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

		public override void OnSpellPulse(DOL.GS.Effects.PulsingSpellEffect effect)
		{
			SendCastAnimation();
			base.OnSpellPulse(effect);
		}
	}
}
