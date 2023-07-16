
using System;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("CeremonialBracerMezz")]
	public class CeremonialBracerMezzSpellHandler : SpellHandler
	{
		public CeremonialBracerMezzSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	[SpellHandlerAttribute("CeremonialBracerStun")]
	public class CeremonialBracerStunSpellHandler : SpellHandler
	{
		public CeremonialBracerStunSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}
