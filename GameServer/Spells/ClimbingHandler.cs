
using System;
using System.Collections;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("Climbing")]
	public class ClimbingHandler : SpellHandler
	{
		private GamePlayer gp;
		
		public override void OnEffectStart(GameSpellEffect effect)
		{
			gp = effect.Owner as GamePlayer;
			if (gp != null)
			{
				gp.AddAbility(SkillBase.GetAbility(Abilities.Climbing));
				gp.Out.SendUpdatePlayerSkills();
			}
		}

		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			gp = effect.Owner as GamePlayer;
			if (gp != null)
			{
				gp.RemoveAbility(Abilities.Climbing);
				gp.Out.SendUpdatePlayerSkills();
			}
			return 0;
		}

		public ClimbingHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}