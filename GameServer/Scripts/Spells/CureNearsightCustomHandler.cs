using System.Collections.Generic;
using DOL.GS.Spells;

namespace DOL.GS.Scripts
{
    /// <summary>
    /// 
    /// </summary>
    [SpellHandlerAttribute("CureNearsightCustom")]
    public class CureNearsightCustomHandler : RemoveSpellEffectHandler
    {
        private Spell spell;
        // constructor
        public CureNearsightCustomHandler(GameLiving caster, Spell spell, SpellLine line)
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