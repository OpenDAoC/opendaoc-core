using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS;

namespace DOL.Events
{
	class PetSpellEventArgs : EventArgs
	{
		private Spell m_spell;
		private SpellLine m_spellLine;
		private GameLiving m_target;
        private Spell m_parentSpell;

		public PetSpellEventArgs(Spell spell, SpellLine spellLine, GameLiving target)
		{
			m_spell = spell;
			m_spellLine = spellLine;
			m_target = target;
		}

        public PetSpellEventArgs(Spell spell, SpellLine spellLine, GameLiving target, Spell parentSpell)
        {
            m_spell = spell;
            m_spellLine = spellLine;
            m_target = target;
            m_parentSpell = parentSpell;
        }

        public Spell Spell
		{
			get { return m_spell; }
		}

		public SpellLine SpellLine
		{
			get { return m_spellLine; }
		}

		public GameLiving Target
		{
			get { return m_target; }
		}

        public Spell ParentSpell
        {
            get { return m_parentSpell; }
        }
	}
}
