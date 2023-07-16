using System;
using DOL.GS.PacketHandler;
using DOL.GS;

namespace DOL.GS.SkillHandler
{
    /// <summary>
    /// Handler for Sprint Ability clicks
    /// </summary>
    [SkillHandlerAttribute(Abilities.MetalGuard)]
    public class MetalGuardAbilityHandler : SpellCastingAbilityHandler
    {
		public override long Preconditions
		{
			get
			{
				return DEAD | SITTING | MEZZED | STUNNED | NOTINGROUP;
			}
		}
 		public override int SpellID
		{
			get
			{
				return 14375;
			}
		}     
    }
}
