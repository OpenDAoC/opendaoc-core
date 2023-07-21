using System;
using System.Linq;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Debuffs a single stat
	/// </summary>
	public abstract class SingleStatDebuffHandler : SingleStatBuffHandler
	{
		// bonus category
		public override EBuffBonusCategory BonusCategory1 { get { return EBuffBonusCategory.Debuff; } }

        public override void CreateECSEffect(ECSGameEffectInitParams initParams)
        {
			new StatDebuffEcsEffect(initParams);
        }

        /// <summary>
        /// Apply effect on target or do spell action if non duration spell
        /// </summary>
        /// <param name="target">target that gets the effect</param>
        /// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			// var debuffs = target.effectListComponent.GetSpellEffects()
			// 					.Where(x => x.SpellHandler is SingleStatDebuff);

			// foreach (var debuff in debuffs)
            // {
			// 	var debuffSpell = debuff.SpellHandler as SingleStatDebuff;

			// 	if (debuffSpell.Property1 == this.Property1 && debuffSpell.Spell.Value >= Spell.Value)
			// 	{
			// 		// Old Spell is Better than new one
			// 		SendSpellResistAnimation(target);
			// 		this.MessageToCaster(eChatType.CT_SpellResisted, "{0} already has that effect.", target.GetName(0, true));
			// 		MessageToCaster("Wait until it expires. Spell Failed.", eChatType.CT_SpellResisted);
			// 		// Prevent Adding.
			// 		return;
			// 	}
            // }


			base.ApplyEffectOnTarget(target, effectiveness);
			
			if (target.Realm == 0 || Caster.Realm == 0)
			{
				target.LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
				Caster.LastAttackTickPvE = GameLoop.GameLoopTime;
			}
			else
			{
				target.LastAttackedByEnemyTickPvP = GameLoop.GameLoopTime;
				Caster.LastAttackTickPvP = GameLoop.GameLoopTime;
			}
			//if(target is GameNPC) 
			//{
			//	IOldAggressiveBrain aggroBrain = ((GameNPC)target).Brain as IOldAggressiveBrain;
			//	if (aggroBrain != null)
			//		aggroBrain.AddToAggroList(Caster, (int)Spell.Value);
			//}
		}

		/// <summary>
		/// Calculates the effect duration in milliseconds
		/// </summary>
		/// <param name="target">The effect target</param>
		/// <param name="effectiveness">The effect effectiveness</param>
		/// <returns>The effect duration in milliseconds</returns>
		protected override int CalculateEffectDuration(GameLiving target, double effectiveness)
		{
			double duration = Spell.Duration;
			duration *= (1.0 + m_caster.GetModified(EProperty.SpellDuration) * 0.01);
			duration -= duration * target.GetResist(Spell.DamageType) * 0.01;

			if (duration < 1)
				duration = 1;
			else if (duration > (Spell.Duration * 4))
				duration = (Spell.Duration * 4);
			return (int)duration;
		}
		
		/// <summary>
		/// Calculates chance of spell getting resisted
		/// </summary>
		/// <param name="target">the target of the spell</param>
		/// <returns>chance that spell will be resisted for specific target</returns>		
        public override int CalculateSpellResistChance(GameLiving target)
        {
        	int basechance =  base.CalculateSpellResistChance(target);      
            /*
 			GameSpellEffect rampage = SpellHandler.FindEffectOnTarget(target, "Rampage");
            if (rampage != null)
            {
            	basechance += (int)rampage.Spell.Value;
            }*/
            return Math.Min(100, basechance);
        }
		// constructor
		public SingleStatDebuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Str stat baseline debuff
	/// </summary>
	[SpellHandlerAttribute("StrengthDebuff")]
	public class StrengthDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.Strength; } }

		// constructor
		public StrengthDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Dex stat baseline debuff
	/// </summary>
	[SpellHandlerAttribute("DexterityDebuff")]
	public class DexterityDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.Dexterity; } }	

		// constructor
		public DexterityDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Con stat baseline debuff
	/// </summary>
	[SpellHandlerAttribute("ConstitutionDebuff")]
	public class ConstitutionDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.Constitution; } }	

		// constructor
		public ConstitutionDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Armor factor debuff
	/// </summary>
	[SpellHandlerAttribute("ArmorFactorDebuff")]
	public class ArmorFactorDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.ArmorFactor; } }	

		// constructor
		public ArmorFactorDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Armor Absorption debuff
	/// </summary>
	[SpellHandlerAttribute("ArmorAbsorptionDebuff")]
	public class ArmorAbsorptionDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.ArmorAbsorption; } }

		/// <summary>
		/// send updates about the changes
		/// </summary>
		/// <param name="target"></param>
		protected override void SendUpdates(GameLiving target)
		{
		}

		// constructor
		public ArmorAbsorptionDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Combat Speed debuff
	/// </summary>
	[SpellHandlerAttribute("CombatSpeedDebuff")]
	public class CombatSpeedDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.MeleeSpeed; } }      
		
		/// <summary>
		/// send updates about the changes
		/// </summary>
		/// <param name="target"></param>
		protected override void SendUpdates(GameLiving target)
		{
		}

		// constructor
		public CombatSpeedDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Melee damage debuff
	/// </summary>
	[SpellHandlerAttribute("MeleeDamageDebuff")]
	public class MeleeDamageDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.MeleeDamage; } }      
		
		/// <summary>
		/// send updates about the changes
		/// </summary>
		/// <param name="target"></param>
		protected override void SendUpdates(GameLiving target)
		{
		}

		// constructor
		public MeleeDamageDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Fatigue reduction debuff
	/// </summary>
	[SpellHandlerAttribute("FatigueConsumptionDebuff")]
	public class FatigueConsumptionDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.FatigueConsumption; } }      
		
		/// <summary>
		/// send updates about the changes
		/// </summary>
		/// <param name="target"></param>
		protected override void SendUpdates(GameLiving target)
		{
		}

		// constructor
		public FatigueConsumptionDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}

	/// <summary>
	/// Fumble chance debuff
	/// </summary>
	[SpellHandlerAttribute("FumbleChanceDebuff")]
	public class FumbleChanceDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.FumbleChance; } }      
		
		/// <summary>
		/// send updates about the changes
		/// </summary>
		/// <param name="target"></param>
		protected override void SendUpdates(GameLiving target)
		{
		}

		// constructor
		public FumbleChanceDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	
	/// <summary>
	/// DPS debuff
	/// </summary>
	[SpellHandlerAttribute("DPSDebuff")]
	public class DPSDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.DPS; } }	

		// constructor
		public DPSDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	/// <summary>
	/// Skills Debuff
	/// </summary>
	[SpellHandlerAttribute("SkillsDebuff")]
	public class SkillsDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.AllSkills; } }	

		// constructor
		public SkillsDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	/// <summary>
	/// Acuity stat baseline debuff
	/// </summary>
	[SpellHandlerAttribute("AcuityDebuff")]
	public class AcuityDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.Acuity; } }	

		// constructor
		public AcuityDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	/// <summary>
	/// Quickness stat baseline debuff
	/// </summary>
	[SpellHandlerAttribute("QuicknessDebuff")]
	public class QuicknessDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.Quickness; } }	

		// constructor
		public QuicknessDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
	/// <summary>
	/// ToHit Skill debuff
	/// </summary>
	[SpellHandlerAttribute("ToHitDebuff")]
	public class ToHitSkillDebuff : SingleStatDebuffHandler
	{
		public override EProperty Property1 { get { return EProperty.ToHitBonus; } }	

		// constructor
		public ToHitSkillDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) {}
	}
 }
