using System;
using System.Collections.Generic;

namespace DOL.GS.Spells
{
	/// <summary>
	/// 
	/// </summary>
	[SpellHandlerAttribute("CurePoison")]
	public class CurePoisonHandler : RemoveSpellEffectHandler
	{
		// constructor
		public CurePoisonHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{
			// RR4: now it's a list
			m_spellTypesToRemove = new List<string>();
			m_spellTypesToRemove.Add("DamageOverTime");
            m_spellTypesToRemove.Add("StyleBleeding");
		} 
	}
}
