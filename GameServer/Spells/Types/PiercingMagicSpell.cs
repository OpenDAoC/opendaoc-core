﻿namespace DOL.GS.Spells
{
    [SpellHandler("PiercingMagic")]
    public class PiercingMagicSpell : SpellHandler
    {
        public override void CreateECSEffect(EcsGameEffectInitParams initParams)
        {
            new PiercingMagicEcsSpellEffect(initParams);
        }
        // constructor
        public PiercingMagicSpell(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
        {
        }
    }   
}
