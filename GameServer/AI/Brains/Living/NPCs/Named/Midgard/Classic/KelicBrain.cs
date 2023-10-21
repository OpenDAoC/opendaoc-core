using Core.Database.Tables;
using Core.GS.Enums;

namespace Core.GS.AI.Brains;

public class KelicBrain : StandardMobBrain
{
	private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	public KelicBrain() : base()
	{
		AggroLevel = 100;
		AggroRange = 1000;
		ThinkInterval = 1500;
	}
	public override void Think()
	{
		if (!CheckProximityAggro())
		{
			//set state to RETURN TO SPAWN
			FiniteStateMachine.SetCurrentState(EFsmStateType.RETURN_TO_SPAWN);
			Body.Health = Body.MaxHealth;
		}
		if (Body.TargetObject != null && HasAggro)
		{
			GameLiving target = Body.TargetObject as GameLiving;
			foreach (GameNpc npc in Body.GetNPCsInRadius(2500))
			{
				if (npc != null && npc.IsAlive && npc.Name.ToLower() == "servant of kelic" && npc.Brain is StandardMobBrain brain && npc != Body)
				{
					if (brain != null && !brain.HasAggro && target != null && target.IsAlive)
						brain.AddToAggroList(target, 100);
				}
			}
			if(Util.Chance(25) && target != null && target.IsAlive)
            {
				if(!target.effectListComponent.ContainsEffectForEffectType(EEffect.DamageOverTime))
					Body.CastSpell(KelicDD, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			}
			if (Util.Chance(25) && target != null && target.IsAlive)
			{
				if (!target.effectListComponent.ContainsEffectForEffectType(EEffect.Disease))
					Body.CastSpell(KelicDisease, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			}
			if (Util.Chance(25) && target != null && target.IsAlive)
			{
				if (!target.effectListComponent.ContainsEffectForEffectType(EEffect.SnareImmunity))
					Body.CastSpell(KelicRoot, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			}
		}
		base.Think();
	}
	#region Spells
	private Spell m_KelicDD;
	private Spell KelicDD
	{
		get
		{
			if (m_KelicDD == null)
			{
				DbSpell spell = new DbSpell();
				spell.AllowAdd = false;
				spell.CastTime = 3;
				spell.Power = 0;
				spell.RecastDelay = 0;
				spell.ClientEffect = 562;
				spell.Icon = 562;
				spell.Damage = 89;
				spell.Frequency = 20;
				spell.Duration = 24;
				spell.DamageType = (int)EDamageType.Matter;
				spell.Name = "Delaceration";
				spell.Description = "Inflicts 89 damage to the target every 2 sec for 24 seconds";
				spell.Message1 = "Your body is covered with painful sores!";
				spell.Message2 = "{0}'s skin erupts in open wounds!";
				spell.Message3 = "The destructive energy wounding you fades.";
				spell.Message4 = "The destructive energy around {0} fades.";
				spell.Range = 1500;
				spell.SpellID = 12006;
				spell.Target = "Enemy";
				spell.Type = ESpellType.DamageOverTime.ToString();
				spell.Uninterruptible = true;
				spell.MoveCast = true;
				m_KelicDD = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_KelicDD);
			}
			return m_KelicDD;
		}
	}
	private Spell m_KelicDisease;
	private Spell KelicDisease
	{
		get
		{
			if (m_KelicDisease == null)
			{
				DbSpell spell = new DbSpell();
				spell.AllowAdd = false;
				spell.CastTime = 3;
				spell.RecastDelay = 0;
				spell.ClientEffect = 735;
				spell.Icon = 735;
				spell.Name = "Cursed Essence";
				spell.Description = "Inflicts a wasting disease on the target that slows it, weakens it, and inhibits heal spells.";
				spell.Message1 = "You are diseased!";
				spell.Message2 = "{0} is diseased!";
				spell.Message3 = "You look healthy.";
				spell.Message4 = "{0} looks healthy again.";
				spell.TooltipId = 735;
				spell.Range = 1500;
				spell.Duration = 120;
				spell.SpellID = 12008;
				spell.Target = "Enemy";
				spell.Type = "Disease";
				spell.Uninterruptible = true;
				spell.MoveCast = true;
				spell.DamageType = (int)EDamageType.Energy; //Energy DMG Type
				m_KelicDisease = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_KelicDisease);
			}
			return m_KelicDisease;
		}
	}
	private Spell m_KelicRoot;
	private Spell KelicRoot
	{
		get
		{
			if (m_KelicRoot == null)
			{
				DbSpell spell = new DbSpell();
				spell.AllowAdd = false;
				spell.CastTime = 3;
				spell.RecastDelay = 0;
				spell.ClientEffect = 688;
				spell.Icon = 688;
				spell.Duration = 73;
				spell.Value = 99;
				spell.DamageType = (int)EDamageType.Body;
				spell.Name = "Superior Leg Twisting";
				spell.Message1 = "Your feet are frozen to the ground!";
				spell.Message2 = "{0}'s feet are frozen to the ground!";
				spell.Range = 1500;
				spell.SpellID = 12009;
				spell.Target = "Enemy";
				spell.Type = ESpellType.SpeedDecrease.ToString();
				spell.Uninterruptible = true;
				m_KelicRoot = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_KelicRoot);
			}
			return m_KelicRoot;
		}
	}
	#endregion
}