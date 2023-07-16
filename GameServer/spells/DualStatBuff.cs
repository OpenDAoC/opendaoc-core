using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Buffs two stats at once, goes into specline bonus category
	/// </summary>	
	public abstract class DualStatBuff : SingleStatBuff
	{
		public override eBuffBonusCategory BonusCategory1 { get { return eBuffBonusCategory.SpecBuff; } }
		public override eBuffBonusCategory BonusCategory2 { get { return eBuffBonusCategory.SpecBuff; } }

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected DualStatBuff(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{
		}
	}

	/// <summary>
	/// Str/Con stat specline buff
	/// </summary>
	[SpellHandlerAttribute("StrengthConstitutionBuff")]
	public class StrengthConBuff : DualStatBuff
	{
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirStrength)
        	   || target.HasAbility(Abilities.VampiirConstitution))
            {
                MessageToCaster("Your target already has an effect of that type!", eChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
		public override eProperty Property1 { get { return eProperty.Strength; } }	
		public override eProperty Property2 { get { return eProperty.Constitution; } }	

		// constructor
		public StrengthConBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Dex/Qui stat specline buff
	/// </summary>
	[SpellHandlerAttribute("DexterityQuicknessBuff")]
	public class DexterityQuiBuff : DualStatBuff
	{
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirDexterity)
        	   || target.HasAbility(Abilities.VampiirQuickness))
            {
                MessageToCaster("Your target already has an effect of that type!", eChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
		public override eProperty Property1 { get { return eProperty.Dexterity; } }	
		public override eProperty Property2 { get { return eProperty.Quickness; } }	

		// constructor
		public DexterityQuiBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}
