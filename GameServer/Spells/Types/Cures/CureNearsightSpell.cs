using System.Collections.Generic;

namespace DOL.GS.Spells
{
	/// <summary>
	/// 
	/// </summary>
	[SpellHandler("CureNearsight")]
	public class CureNearsightSpell : RemoveSpellEffectHandler
	{
		// constructor
		public CureNearsightSpell(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{
			// RR4: now it's a list
			m_spellTypesToRemove = new List<string>();
			m_spellTypesToRemove.Add("Nearsight");
            m_spellTypesToRemove.Add("Silence");
		} 
	}
}