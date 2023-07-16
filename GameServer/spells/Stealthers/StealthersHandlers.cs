using System;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("BloodRage")]
	public class BloodRage : SpellHandler
	{
		public BloodRage(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandlerAttribute("HeightenedAwareness")]
	public class HeightenedAwareness : SpellHandler
	{
		public HeightenedAwareness(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandlerAttribute("SubtleKills")]
	public class SubtleKills : SpellHandler
	{
		public SubtleKills(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}