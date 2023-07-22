﻿using DOL.AI.Brain;
using DOL.Database;
using DOL.GS;

namespace Core.GS.Scripts.mobs.AlbionMobs;

public class CrusaderAntithesis : GameEpicDungeonNPC
{
	public CrusaderAntithesis() : base()
	{
	}
	public override bool AddToWorld()
	{
		INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(50041);
		LoadTemplate(npcTemplate);
		Strength = npcTemplate.Strength;
		Dexterity = npcTemplate.Dexterity;
		Constitution = npcTemplate.Constitution;
		Quickness = npcTemplate.Quickness;
		Piety = npcTemplate.Piety;
		Intelligence = npcTemplate.Intelligence;
		Empathy = npcTemplate.Empathy;

		CrusaderAntithesisBrain sbrain = new CrusaderAntithesisBrain();
		SetOwnBrain(sbrain);
		base.AddToWorld();
		return true;
	}
	public override void OnAttackEnemy(AttackData ad) //on enemy actions
	{
		if (UtilCollection.Chance(35) && ad != null)
		{
			CastSpell(CrusaderDD, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
		}
		base.OnAttackEnemy(ad);
	}
	private Spell m_CrusaderDD;
	private Spell CrusaderDD
	{
		get
		{
			if (m_CrusaderDD == null)
			{
				DBSpell spell = new DBSpell();
				spell.AllowAdd = false;
				spell.CastTime = 0;
				spell.Power = 0;
				spell.RecastDelay = 3;
				spell.ClientEffect = 0;
				spell.Icon = 0;
				spell.Damage = UtilCollection.Random(350,450);
				spell.DamageType = (int)EDamageType.Slash;
				spell.Name = "Melee Swing";
				spell.Range = 400;
				spell.SpellID = 12016;
				spell.Target = "Enemy";
				spell.Type = ESpellType.DirectDamageNoVariance.ToString();
				m_CrusaderDD = new Spell(spell, 60);
				SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_CrusaderDD);
			}
			return m_CrusaderDD;
		}
	}
}

public class CrusaderAntithesisBrain : StandardMobBrain
{
	private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	public CrusaderAntithesisBrain() : base()
	{
		AggroLevel = 100;
		AggroRange = 600;
		ThinkInterval = 1500;
	}

	public override void Think()
	{
		if (UtilCollection.Chance(20))
		{
			SetFlags();
		}
		base.Think();
	}
	private protected void SetFlags()
	{
		if (HasAggro)
		{
			switch (UtilCollection.Random(8))
			{
				case 0:
					if (Body.Model != 667)
					{
						Body.Model = 667;
						Body.Flags = (GameNPC.eFlags)12;//NONAME + NOTARGET
						Body.BroadcastLivingEquipmentUpdate();
					}
					break;
				case 1:
					if (Body.Model != 927)
					{
						Body.Model = 927;
						Body.Flags = (GameNPC.eFlags)1;
						Body.BroadcastLivingEquipmentUpdate();
					}
					break;
			}
		}
		else
		{
			if (Body.Model != 927)
			{
				Body.Model = 927;
				Body.Flags = (GameNPC.eFlags)1;
				Body.BroadcastLivingEquipmentUpdate();
			}
		}
	}
}