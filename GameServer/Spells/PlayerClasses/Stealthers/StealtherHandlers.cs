namespace DOL.GS.Spells
{
	[SpellHandler("BloodRage")]
	public class BloodRageHandler : SpellHandler
	{
		public BloodRageHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandler("HeightenedAwareness")]
	public class HeightenedAwarenessHandler : SpellHandler
	{
		public HeightenedAwarenessHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandler("SubtleKills")]
	public class SubtleKillsHandler : SpellHandler
	{
		public SubtleKillsHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}