using System;

namespace DOL.GS.Spells
{
    /// <summary>
    /// RvR Resurrection Illness Handler
    /// </summary>
    [SpellHandlerAttribute("RvrResurrectionIllness")]
    public class RvrResurrectionIllness : PveResurrectionIllness
	{
		// constructor
		public RvrResurrectionIllness(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
		}
	}
}
