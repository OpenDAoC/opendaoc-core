using System;
using System.Linq;

using DOL.GS;
using DOL.Database;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Fury shout
	/// </summary>
	[SkillHandlerAttribute(Abilities.ClimbSpikes)]
	public class ClimbingAbilityHandler : SpellCastingHandler
	{
		private static int spellid = -1;
		
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
				return spellid;
			}
		}

		public ClimbingAbilityHandler()
		{
			// Graveen: crappy, but not hardcoded. if we except by the ability name ofc...
			// problems are: 
			// 		- matching vs ability name / spell name needed
			//		- spell name is not indexed
			// perhaps a basis to think about, but definitively not the design we want.
			if (spellid == -1)
			{
				spellid=0;
				DBSpell climbSpell = CoreDb<DBSpell>.SelectObject(DB.Column("Name").IsEqualTo(Abilities.ClimbSpikes));
				if (climbSpell != null)
					spellid = climbSpell.SpellID;
			}
		}
	}
}
