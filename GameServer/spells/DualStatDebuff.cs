using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using System.Linq;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Debuffs two stats at once, goes into specline bonus category
	/// </summary>	
	public abstract class DualStatDebuff : SingleStatDebuff
	{
		public override eBuffBonusCategory BonusCategory1 { get { return eBuffBonusCategory.Debuff; } }
		public override eBuffBonusCategory BonusCategory2 { get { return eBuffBonusCategory.Debuff; } }

        // public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        // {
		// 	var debuffs = target.effectListComponent.GetSpellEffects()
		// 						.Where(x => x.SpellHandler is DualStatDebuff);

		// 	foreach (var debuff in debuffs)
		// 	{
		// 		var debuffSpell = debuff.SpellHandler as DualStatDebuff;

		// 		if (debuffSpell.Property1 == this.Property1 && debuffSpell.Property2 == this.Property2 && debuffSpell.Spell.Value >= Spell.Value)
		// 		{
		// 			// Old Spell is Better than new one
		// 			SendSpellResistAnimation(target);
		// 			this.MessageToCaster(eChatType.CT_SpellResisted, "{0} already has that effect.", target.GetName(0, true));
		// 			MessageToCaster("Wait until it expires. Spell Failed.", eChatType.CT_SpellResisted);
		// 			// Prevent Adding.
		// 			return;
		// 		}
		// 	}

		// 	base.ApplyEffectOnTarget(target, effectiveness);
		// }

        // constructor
        public DualStatDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Str/Con stat specline debuff
	/// </summary>
	[SpellHandlerAttribute("StrengthConstitutionDebuff")]
	public class StrengthConDebuff : DualStatDebuff
	{
		public override eProperty Property1 { get { return eProperty.Strength; } }
		public override eProperty Property2 { get { return eProperty.Constitution; } }

		// constructor
		public StrengthConDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Dex/Qui stat specline debuff
	/// </summary>
	[SpellHandlerAttribute("DexterityQuicknessDebuff")]
	public class DexterityQuiDebuff : DualStatDebuff
	{
		public override eProperty Property1 { get { return eProperty.Dexterity; } }
		public override eProperty Property2 { get { return eProperty.Quickness; } }

		// constructor
		public DexterityQuiDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
	/// Dex/Con Debuff for assassin poisons
	/// <summary>
	/// Dex/Con stat specline debuff
	/// </summary>
	[SpellHandlerAttribute("DexterityConstitutionDebuff")]
	public class DexterityConDebuff : DualStatDebuff
	{
		public override eProperty Property1 { get { return eProperty.Dexterity; } }
		public override eProperty Property2 { get { return eProperty.Constitution; } }

		// constructor
		public DexterityConDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandlerAttribute("WeaponSkillConstitutionDebuff")]
	public class WeaponskillConDebuff : DualStatDebuff
	{
		public override eProperty Property1 { get { return eProperty.WeaponSkill; } }
		public override eProperty Property2 { get { return eProperty.Constitution; } }
		public WeaponskillConDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}
