using System;
using DOL.GS.PacketHandler;
using DOL.GS;

namespace DOL.GS.SkillHandler
{
    /// <summary>
    /// Handler for Rampage Shout
    /// </summary>
    [SkillHandlerAttribute(Abilities.Rampage)]
    public class RampageHandler : SpellCastingHandler
    {
		public override long Preconditions
		{
			get
			{
				return DEAD | SITTING | MEZZED | STUNNED;
			}
		}
 		public override int SpellID
		{
			get
			{
				return 14373;
			}
		}      
    }
}
