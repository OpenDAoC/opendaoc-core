using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Buffs two stats at once, goes into specline bonus category
	/// </summary>	
	public abstract class DualStatBuffHandler : SingleStatBuffHandler
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.SpecBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.SpecBuff; } }

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected DualStatBuffHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{
		}
	}

	/// <summary>
	/// Str/Con stat specline buff
	/// </summary>
	[SpellHandlerAttribute("StrengthConstitutionBuff")]
	public class StrengthConBuff : DualStatBuffHandler
	{
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirStrength)
        	   || target.HasAbility(Abilities.VampiirConstitution))
            {
                MessageToCaster("Your target already has an effect of that type!", EChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
		public override EProperty Property1 { get { return EProperty.Strength; } }	
		public override EProperty Property2 { get { return EProperty.Constitution; } }	

		// constructor
		public StrengthConBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Dex/Qui stat specline buff
	/// </summary>
	[SpellHandlerAttribute("DexterityQuicknessBuff")]
	public class DexterityQuiBuff : DualStatBuffHandler
	{
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            if (target.HasAbility(Abilities.VampiirDexterity)
        	   || target.HasAbility(Abilities.VampiirQuickness))
            {
                MessageToCaster("Your target already has an effect of that type!", EChatType.CT_Spell);
                return;
            }
            base.ApplyEffectOnTarget(target, effectiveness);
        }
		public override EProperty Property1 { get { return EProperty.Dexterity; } }	
		public override EProperty Property2 { get { return EProperty.Quickness; } }	

		// constructor
		public DexterityQuiBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
}
