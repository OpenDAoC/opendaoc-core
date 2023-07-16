
using System;
using System.Collections;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Database;
using DOL.GS.Spells;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("Facilis")]
	public class FacilisSpellHandler : SpellHandler
	{
		public override bool IsOverwritable(GameSpellEffect compare)
		{
			return true;
		}
		public FacilisSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}
