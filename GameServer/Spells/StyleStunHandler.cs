using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Style stun effect spell handler
	/// </summary>
	[SpellHandler("StyleStun")]
	public class StyleStunHandler : StunSpellHandler
	{
		public override void CreateECSEffect(ECSGameEffectInitParams initParams)
		{
			new StunECSGameEffect(initParams);
		}
		
		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}

		/// <summary>
		/// Calculates the effect duration in milliseconds
		/// </summary>
		/// <param name="target">The effect target</param>
		/// <param name="effectiveness">The effect effectiveness</param>
		/// <returns>The effect duration in milliseconds</returns>
		protected override int CalculateEffectDuration(GameLiving target, double effectiveness)
		{
			NPCECSStunImmunityEffect npcImmune = (NPCECSStunImmunityEffect)EffectListService.GetEffectOnTarget(target, EEffect.NPCStunImmunity);
			if (npcImmune != null)
			{
				int duration = (int)npcImmune.CalclulateStunDuration(Spell.Duration);
				return  duration > 1 ? duration : 1;
			}
			else
				return Spell.Duration;
		}

		/// <summary>
		/// When an applied effect expires.
		/// Duration spells only.
		/// </summary>
		/// <param name="effect">The expired effect</param>
		/// <param name="noMessages">true, when no messages should be sent to player and surrounding</param>
		/// <returns>immunity duration in milliseconds</returns>
		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			// http://www.camelotherald.com/more/1749.shtml
			// immunity timer will now be exactly five times the length of the stun
			base.OnEffectExpires(effect, noMessages);
			return Spell.Duration * 5;
		}

		/// <summary>
		/// Determines wether this spell is compatible with given spell
		/// and therefore overwritable by better versions
		/// spells that are overwritable cannot stack
		/// </summary>
		/// <param name="compare"></param>
		/// <returns></returns>
		public override bool IsOverwritable(GameSpellEffect compare)
		{
			if (Spell.EffectGroup != 0 || compare.Spell.EffectGroup != 0)
				return Spell.EffectGroup == compare.Spell.EffectGroup;
			if (compare.Spell.SpellType == ESpellType.Stun) return true;
			return base.IsOverwritable(compare);
		}

		// constructor
		public StyleStunHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}