using System;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
    [SpellHandlerAttribute("Rampage")]
    public class RampageBuffHandler : SpellHandler
    {
        public RampageBuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}
