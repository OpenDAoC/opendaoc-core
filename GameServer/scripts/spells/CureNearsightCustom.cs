
using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;

namespace DOL.GS.Scripts
{
    /// <summary>
    /// 
    /// </summary>
    [SpellHandlerAttribute("CureNearsightCustom")]
    public class CureNearsightCustomSpellHandler : RemoveSpellEffectHandler
    {
        private Spell spell;
        // constructor
        public CureNearsightCustomSpellHandler(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
            // RR4: now it's a list
            m_spellTypesToRemove = new List<string>();
            m_spellTypesToRemove.Add("Nearsight");
            m_spellTypesToRemove.Add("Silence");

            this.spell = spell;
        }

        public override int CalculateCastingTime()
        {
            return spell.CastTime;
        }
    }
}