//Create by phoenix

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
	[SpellHandlerAttribute("CureDisease")]
	public class CureDiseaseHandler : RemoveSpellEffectHandler
	{
		// constructor
		public CureDiseaseHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
			// RR4: now it's a list
			m_spellTypesToRemove = new List<string>();
			m_spellTypesToRemove.Add("Disease");
		}
	}
}