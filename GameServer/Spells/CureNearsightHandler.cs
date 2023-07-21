
using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// 
	/// </summary>
	[SpellHandlerAttribute("CureNearsight")]
	public class CureNearsightHandler : RemoveSpellEffectHandler
	{
		// constructor
		public CureNearsightHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{
			// RR4: now it's a list
			m_spellTypesToRemove = new List<string>();
			m_spellTypesToRemove.Add("Nearsight");
            m_spellTypesToRemove.Add("Silence");
		} 
	}
}