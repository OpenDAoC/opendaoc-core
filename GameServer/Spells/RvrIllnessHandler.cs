using System;

namespace DOL.GS.Spells
{
    /// <summary>
    /// RvR Resurrection Illness Handler
    /// </summary>
    [SpellHandlerAttribute("RvrResurrectionIllness")]
    public class RvrIllnessHandler : PveIllnessHandler
	{
		// constructor
		public RvrIllnessHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
		}
	}
}
