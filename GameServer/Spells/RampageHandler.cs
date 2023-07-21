using System;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
    [SpellHandlerAttribute("Rampage")]
    public class RampageHandler : SpellHandler
    {
        public RampageHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}
