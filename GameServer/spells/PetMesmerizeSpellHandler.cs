using System;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Events;

namespace DOL.GS.Spells
{
    /// <summary>
    /// PetMezz 
    /// </summary>
    [SpellHandlerAttribute("PetMesmerize")]
    public class PetMesmerizeSpellHandler : MesmerizeSpellHandler
    {
        public PetMesmerizeSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine) { }
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (!(target is IControlledBrain))
                return;
            base.ApplyEffectOnTarget(target, effectiveness);
        }
    }
}