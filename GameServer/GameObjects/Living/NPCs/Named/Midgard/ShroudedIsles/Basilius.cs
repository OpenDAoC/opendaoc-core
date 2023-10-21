﻿using System;
using Core.AI.Brain;
using Core.Database;
using Core.Database.Tables;
using Core.Events;
using Core.GS.AI.Brains;
using Core.GS.Enums;

namespace Core.GS;

public class Basilius : GameEpicBoss
{
	public Basilius() : base() { }

	[ScriptLoadedEvent]
	public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
	{
		if (log.IsInfoEnabled)
			log.Info("Basilius Initializing...");
	}
	public override int GetResist(EDamageType damageType)
	{
		switch (damageType)
		{
			case EDamageType.Slash: return 20;// dmg reduction for melee dmg
			case EDamageType.Crush: return 20;// dmg reduction for melee dmg
			case EDamageType.Thrust: return 20;// dmg reduction for melee dmg
			default: return 30;// dmg reduction for rest resists
		}
	}
	public override double AttackDamage(DbInventoryItem weapon)
	{
		return base.AttackDamage(weapon) * Strength / 100;
	}
	public override int AttackRange
	{
		get { return 350; }
		set { }
	}
	public override bool HasAbility(string keyName)
	{
		if (IsAlive && keyName == GS.Abilities.CCImmunity)
			return true;

		return base.HasAbility(keyName);
	}
	public override double GetArmorAF(EArmorSlot slot)
	{
		return 350;
	}
	public override double GetArmorAbsorb(EArmorSlot slot)
	{
		// 85% ABS is cap.
		return 0.20;
	}
	public override int MaxHealth
	{
		get { return 30000; }
	}
	public override bool AddToWorld()
	{
		INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60158315);
		LoadTemplate(npcTemplate);
		Strength = npcTemplate.Strength;
		Dexterity = npcTemplate.Dexterity;
		Constitution = npcTemplate.Constitution;
		Quickness = npcTemplate.Quickness;
		Piety = npcTemplate.Piety;
		Intelligence = npcTemplate.Intelligence;
		Empathy = npcTemplate.Empathy;

		RespawnInterval = ServerProperties.Properties.SET_EPIC_GAME_ENCOUNTER_RESPAWNINTERVAL * 60000;//1min is 60000 miliseconds
		BasiliusBrain sbrain = new BasiliusBrain();
		SetOwnBrain(sbrain);
		LoadedFromScript = false;//load from database
		SaveIntoDatabase();
		base.AddToWorld();
		return true;
	}
	public override void OnAttackEnemy(AttackData ad) //on enemy actions
	{
		if (Util.Chance(35) && !ad.Target.effectListComponent.ContainsEffectForEffectType(EEffect.Bleed))
		{
			if (ad != null && (ad.AttackResult == EAttackResult.HitUnstyled || ad.AttackResult == EAttackResult.HitStyle))
				CastSpell(BasiliusBleed, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
		}
		if (ad != null && (ad.AttackResult == EAttackResult.HitUnstyled || ad.AttackResult == EAttackResult.HitStyle))
		{
			if (Util.Chance(35) && !ad.Target.effectListComponent.ContainsEffectForEffectType(EEffect.StunImmunity) && !ad.Target.effectListComponent.ContainsEffectForEffectType(EEffect.Stun) && ad.Target.IsAlive)
				CastSpell(Basilius_stun, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
		}
		base.OnAttackEnemy(ad);
	}
    #region Spells
    private Spell m_Basilius_stun;
	private Spell Basilius_stun
	{
		get
		{
			if (m_Basilius_stun == null)
			{
				DbSpell spell = new DbSpell();
				spell.AllowAdd = false;
				spell.CastTime = 0;
				spell.RecastDelay = 2;
				spell.ClientEffect = 2165;
				spell.Icon = 2132;
				spell.TooltipId = 2132;
				spell.Duration = 6;
				spell.Description = "Target is stunned and cannot move or take any other action for the duration of the spell.";
				spell.Name = "Stun";
				spell.Range = 400;
				spell.SpellID = 11893;
				spell.Target = "Enemy";
				spell.Type = ESpellType.Stun.ToString();
				m_Basilius_stun = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_Basilius_stun);
			}
			return m_Basilius_stun;
		}
	}
	private Spell m_BasiliusBleed;
	private Spell BasiliusBleed
	{
		get
		{
			if (m_BasiliusBleed == null)
			{
				DbSpell spell = new DbSpell();
				spell.AllowAdd = false;
				spell.CastTime = 0;
				spell.RecastDelay = 2;
				spell.ClientEffect = 2130;
				spell.Icon = 3411;
				spell.TooltipId = 3411;
				spell.Damage = 100;
				spell.Name = "Bleed";
				spell.Description = "Does 100 bleeding damage to a target every 3 seconds for 30 seconds.";
				spell.Message1 = "You are bleeding! ";
				spell.Message2 = "{0} is bleeding! ";
				spell.Duration = 30;
				spell.Frequency = 30;
				spell.Range = 350;
				spell.SpellID = 11892;
				spell.Target = ESpellTarget.ENEMY.ToString();
				spell.Type = ESpellType.StyleBleeding.ToString();
				spell.Uninterruptible = true;
				spell.MoveCast = true;
				spell.DamageType = (int)EDamageType.Body;
				m_BasiliusBleed = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_BasiliusBleed);
			}
			return m_BasiliusBleed;
		}
	}
	#endregion
}
