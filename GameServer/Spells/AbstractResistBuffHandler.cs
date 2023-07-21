using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Base class for all resist buffs, needed to set effectiveness
	/// </summary>
	public abstract class AbstractResistBuff : PropertyChangingHandler
	{
		/// <summary>
		/// Apply effect on target or do spell action if non duration spell
		/// </summary>
		/// <param name="target">target that gets the effect</param>
		/// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			effectiveness *= (1.0 + m_caster.GetModified(EProperty.BuffEffectiveness) * 0.01);
			base.ApplyEffectOnTarget(target, effectiveness);
		}

        public override void CreateECSEffect(ECSGameEffectInitParams initParams)
        {
			new StatBuffEcsEffect(initParams);
        }

        protected override void SendUpdates(GameLiving target)
		{
			base.SendUpdates(target);
			if (target is GamePlayer)
			{
				GamePlayer player = (GamePlayer)target;
				player.Out.SendCharResistsUpdate();
			}
		}

		public AbstractResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Body resistance buff
	/// </summary>
	[SpellHandlerAttribute("BodyResistBuff")]
	public class BodyResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Body; } }

		// constructor
		public BodyResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Cold resistance buff
	/// </summary>
	[SpellHandlerAttribute("ColdResistBuff")]
	public class ColdResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Cold; } }

		// constructor
		public ColdResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Energy resistance buff
	/// </summary>
	[SpellHandlerAttribute("EnergyResistBuff")]
	public class EnergyResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Energy; } }

		// constructor
		public EnergyResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Heat resistance buff
	/// </summary>
	[SpellHandlerAttribute("HeatResistBuff")]
	public class HeatResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Heat; } }

		// constructor
		public HeatResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Matter resistance buff
	/// </summary>
	[SpellHandlerAttribute("MatterResistBuff")]
	public class MatterResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Matter; } }

		// constructor
		public MatterResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Spirit resistance buff
	/// </summary>
	[SpellHandlerAttribute("SpiritResistBuff")]
	public class SpiritResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Spirit; } }

		// constructor
		public SpiritResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Body/Spirit/Energy resistance buff
	/// </summary>
	[SpellHandlerAttribute("BodySpiritEnergyBuff")]
	public class BodySpiritEnergyBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory3 { get { return EBuffBonusCategory.BaseBuff; } }

		public override EProperty Property1 { get { return EProperty.Resist_Body; } }
		public override EProperty Property2 { get { return EProperty.Resist_Spirit; } }
		public override EProperty Property3 { get { return EProperty.Resist_Energy; } }

		// constructor
		public BodySpiritEnergyBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Heat/Cold/Matter resistance buff
	/// </summary>
	[SpellHandlerAttribute("HeatColdMatterBuff")]
	public class HeatColdMatterBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory3 { get { return EBuffBonusCategory.BaseBuff; } }

		public override EProperty Property1 { get { return EProperty.Resist_Heat; } }
		public override EProperty Property2 { get { return EProperty.Resist_Cold; } }
		public override EProperty Property3 { get { return EProperty.Resist_Matter; } }

		// constructor
		public HeatColdMatterBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Body/Spirit/Energy/Heat/Cold/Matter resistance buff
	/// </summary>
	[SpellHandlerAttribute("AllMagicResistsBuff")]
	public class AllMagicResistsBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory3 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory4 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory5 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory6 { get { return EBuffBonusCategory.BaseBuff; } }
		
		public override EProperty Property1 { get { return EProperty.Resist_Heat; } }
		public override EProperty Property2 { get { return EProperty.Resist_Cold; } }
		public override EProperty Property3 { get { return EProperty.Resist_Matter; } }
		public override EProperty Property4 { get { return EProperty.Resist_Body; } }
		public override EProperty Property5 { get { return EProperty.Resist_Spirit; } }
		public override EProperty Property6 { get { return EProperty.Resist_Energy; } }

		// constructor
		public AllMagicResistsBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Crush/Slash/Thrust resistance buff
	/// </summary>
	[SpellHandlerAttribute("CrushSlashThrustBuff")]
	[SpellHandlerAttribute("AllMeleeResistsBuff")]
	public class CrushSlashThrustBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory3 { get { return EBuffBonusCategory.BaseBuff; } }

		public override EProperty Property1 { get { return EProperty.Resist_Crush; } }
		public override EProperty Property2 { get { return EProperty.Resist_Slash; } }
		public override EProperty Property3 { get { return EProperty.Resist_Thrust; } }

		// constructor
		public CrushSlashThrustBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	[SpellHandlerAttribute("CrushResistBuff")]
	public class CrushResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Crush; } }

		// constructor
		public CrushResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Slash buff
	/// </summary>
	[SpellHandlerAttribute("SlashResistBuff")]
	public class SlashResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Slash; } }

		// constructor
		public SlashResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Thrust buff
	/// </summary>
	[SpellHandlerAttribute("ThrustResistBuff")]
	public class ThrustResistBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EProperty Property1 { get { return EProperty.Resist_Thrust; } }

		// constructor
		public ThrustResistBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

	/// <summary>
	/// Resist all 
	/// </summary>
	[SpellHandlerAttribute("AllResistsBuff")]
	public class AllResistsBuff : AbstractResistBuff
	{
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory2 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory3 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory4 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory5 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory6 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory7 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory8 { get { return EBuffBonusCategory.BaseBuff; } }
		public override EBuffBonusCategory BonusCategory9 { get { return EBuffBonusCategory.BaseBuff; } }

		public override EProperty Property1 { get { return EProperty.Resist_Heat; } }
		public override EProperty Property2 { get { return EProperty.Resist_Cold; } }
		public override EProperty Property3 { get { return EProperty.Resist_Matter; } }
		public override EProperty Property4 { get { return EProperty.Resist_Body; } }
		public override EProperty Property5 { get { return EProperty.Resist_Spirit; } }
		public override EProperty Property6 { get { return EProperty.Resist_Energy; } }
		public override EProperty Property7 { get { return EProperty.Resist_Crush; } }
		public override EProperty Property8 { get { return EProperty.Resist_Slash; } }
		public override EProperty Property9 { get { return EProperty.Resist_Thrust; } }

		// constructor
		public AllResistsBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}

}
