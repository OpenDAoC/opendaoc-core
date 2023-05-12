using System;
using DOL.Database;
using System.Collections.Generic;
using System.Reflection;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Spells;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.ServerProperties;
using DOL.Language;
using log4net;

namespace DOL.GS.RealmAbilities
{
    public class AtlasOF_Ichor : TimedRealmAbility, ISpellCastingAbilityHandler
    {
	    private const string m_ichor = "Ichor of the Deep";
        private const int m_duration = 20;
        private const double m_damage = 400;
        private const eDamageType m_damageType = eDamageType.Spirit;
        private const eEffect m_effect = eEffect.IchorOfTheDeep;
        private const eSpellType m_type = eSpellType.IchorOfTheDeep;
        private const int m_radius = 500;
        private const int m_range = 1875;
        private const int m_recast = 900;
        private const int m_reqLevel = 40;
        private const int m_interruptTime = 3000;
        private const int m_effectiveness = 1;
        private const int m_value = 99;
        private const int m_spellID = 7029;
        private DBSpell m_dbspell;
        private Spell m_spell = null;
        private SpellLine m_spellline;
        private SpellHandler m_ichorHandler;
        private GamePlayer m_caster;
        private GameLiving m_owner;

        public GamePlayer Caster => m_caster;
        public GameLiving Owner => m_owner;
        private DBSpell DBSpell => m_dbspell;
        public Spell Spell => m_spell;
        public SpellLine SpellLine => m_spellline;
        public SpellHandler SpellHandler => m_ichorHandler;
        public Ability Ability => this;

        public AtlasOF_Ichor(DBAbility dba, int level) : base(dba, level) { }

        public override string Name => m_ichor;
        public override ushort Icon => m_spellID;
        public override int MaxLevel => 1;
        public override int CostForUpgrade(int level) { return 14; }
        public override int GetReUseDelay(int level) { return m_recast; } // 15 minutes
        public override bool CheckRequirement(GamePlayer player)
        {
	        return AtlasRAHelpers.HasPlayerLevel(player, m_reqLevel);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        public SpellHandler CreateSpell(GamePlayer caster)
        {
	        m_dbspell = new DBSpell();
	        m_dbspell.Name = m_ichor;
	        m_dbspell.Icon = m_spellID;
	        m_dbspell.ClientEffect = m_spellID;
	        m_dbspell.Damage = m_damage; // 400
	        m_dbspell.DamageType = (int)m_damageType; // Spirit
	        m_dbspell.Target = "Enemy";
	        m_dbspell.Radius = m_radius; // 500
	        m_dbspell.Type = m_type.ToString();
	        m_dbspell.Value = m_value;
	        m_dbspell.Duration = m_duration; // 20 seconds
	        m_dbspell.Pulse = 0;
	        m_dbspell.PulsePower = 0;
	        m_dbspell.Power = 0;
	        m_dbspell.CastTime = 0;
	        m_dbspell.EffectGroup = 0;
	        m_dbspell.Range = m_range; // 1875
	        m_dbspell.Frequency = 0;
	        m_dbspell.RecastDelay = m_recast; // 15 minutes
	        m_dbspell.InstrumentRequirement = 0;
	        m_dbspell.Concentration = 0;
	        m_dbspell.Description = "Damages and roots all enemies in the spell's radius. This ignores existing root immunities.";
	        m_dbspell.Message1 = "Constricting bonds surround your body!";
	        m_dbspell.Message2 = "{0} is surrounded by constricting bonds!";
	        m_spell = new Spell(m_dbspell, m_reqLevel);
	        m_spellline = new SpellLine("RAs", "RealmAbilities", "RealmAbilities", true);
	        m_ichorHandler = new SpellHandler(caster, new Spell(m_dbspell,  m_reqLevel) , m_spellline); // make spell level 0 so it bypasses the spec level adjustment code

	        return m_ichorHandler;
        }

        public override void Execute(GameLiving living)
        {
	        if (living == null)
	        {
		        log.Warn("No living at execute");
		        // Message: 
		        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
		        return;
	        }

	        if (living is not GamePlayer)
	        {
		        log.Warn("Living is not GamePlayer");
		        // Message: 
		        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
		        return;
	        }

            if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED))
            {
	            log.Warn("Precondition failed");
	            // Message: 
	            ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
	            return;
            }
            m_caster = (GamePlayer)living;

            m_ichorHandler = CreateSpell((GamePlayer)living);

            // Player must have a target
			if (Caster.TargetObject == null)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage("You must select a target for this ability!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			m_owner = Caster.TargetObject as GameLiving;

			// So they can't use Admins or objects as a target
			if (Owner == null || m_owner is Keeps.GameKeepDoor or Keeps.GameKeepComponent)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage("You have an invalid target!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Target must be in front of the Player
			if (!Caster.TargetInView || !Caster.IsObjectInFront(m_owner, 150))
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage(m_owner.GetName(0, true) + " is not in view!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Can't target self
			if (Caster == m_owner)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage("You can't attack yourself!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Target must be alive
			if (!m_owner.IsAlive)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage(m_owner.GetName(0, true) + " is dead!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// So they can't use Admins or objects as a target
			if (!GameServer.ServerRules.IsAllowedToAttack(Caster, m_owner, true) || (m_owner is GamePlayer playerTarget && playerTarget.CharacterClass.ID == (int)eCharacterClass.Necromancer && playerTarget.IsShade))
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage(m_owner.GetName(0, true) + " can't be attacked", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Target must be within range
			if (!Caster.IsWithinRadius(m_owner, SpellHandler.CalculateSpellRange()))
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage(m_owner.GetName(0, true) + " is too far away!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Target cannot be an ally or friendly
			if (Caster.Realm == m_owner.Realm)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage("You can't attack a member of your realm!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}

			// Cannot use ability if timer is not expired
			if (Spell.HasRecastDelay && Caster.GetSkillDisabledDuration(Spell) > 0)
			{
				// Message: 
				ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
				Caster.Out.SendMessage("You must wait" + Caster.GetSkillDisabledDuration(Spell) / 1000 + " seconds to recast this type of ability!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, m_interruptTime);
				return;
			}



			var ad = DamageDetails(m_owner, m_owner);

			SendCastMessage(Caster);
			OnDirectEffect(ad.Target, ad.Target);
			var targetDur = CalculateEffectDuration(ad.Target, m_effectiveness);
			ApplyEffectOnTarget(ad.Target, targetDur);

			foreach (GamePlayer aePlayers in m_owner.GetPlayersInRadius((ushort) Spell.Radius))
			{
				if (aePlayers == null) continue;

				var gpAD = DamageDetails(m_owner, aePlayers);

				if (gpAD.Target != m_owner && gpAD.Target != Caster)
				{
					OnDirectEffect(m_owner, gpAD.Target);
					var aeDur = CalculateEffectDuration(gpAD.Target, m_effectiveness);
					ApplyEffectOnTarget(gpAD.Target, aeDur);
				}
			}
			foreach (GameNPC aeNPCs in m_owner.GetNPCsInRadius((ushort) Spell.Radius))
			{
				if (aeNPCs == null) continue;

				var npcAD = DamageDetails(m_owner, aeNPCs);

				if (npcAD.Target != m_owner)
				{
					OnDirectEffect(m_owner, npcAD.Target);
					var npcDur = CalculateEffectDuration(npcAD.Target, m_effectiveness);
					ApplyEffectOnTarget(npcAD.Target, npcDur);
				}
			}

			Caster.DisableSkill(this, Spell.RecastDelay);
        }

        public void SendAnimation(GameLiving target, ushort spellID, bool success)
        {
	        if (m_caster == null)
		        return;

	        if (Caster.GetSkillDisabledDuration(Spell) > 0 || target == null)
		        return;

	        foreach (GamePlayer player in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
	        {
		        if (player != null)
			        player.Out.SendSpellEffectAnimation(Caster, target, spellID, 0, false, success ? (byte)1 : (byte)0);
	        }
        }

        private int CalculateDamageWithFalloff(int initialDamage, GameLiving initTarget, GameLiving aeTarget, double effectiveness)
        {
	        int modDamage = (int)Math.Round((decimal) (initialDamage * effectiveness * ((500-(initTarget.GetDistance(new Point2D(aeTarget.X, aeTarget.Y)))) / 500.0)));
	        var primaryResistModifier = aeTarget.GetResist(Spell.DamageType) * 0.01;
	        var secondaryResistModifier = aeTarget.SpecBuffBonusCategory[(int)eProperty.Resist_Spirit] * 0.01;

	        modDamage *= (int)primaryResistModifier;
	        modDamage *= (int)secondaryResistModifier;

	        return modDamage;
        }
        private int CalculateEffectDuration(GameLiving target, double effectiveness)
        {
	        if (target == null)
		        return 0;

	        double duration = Spell.Duration * effectiveness;
	        var primaryResistModifier = target.GetResist(Spell.DamageType) * 0.01;
	        var secondaryResistModifier = target.SpecBuffBonusCategory[(int)eProperty.Resist_Spirit] * 0.01;
	        var rootdet = target.GetModified(eProperty.SpeedDecreaseDurationReduction) * 0.01;

			duration *= (int)primaryResistModifier;
			duration *= (int)rootdet;
			duration *= (int)secondaryResistModifier;

	        if (duration < 1)
		        duration = 1;
	        else if (duration > (Spell.Duration * 4))
		        duration = (Spell.Duration * 4);
	        return (int)duration;
        }

        public AttackData DamageDetails(GameLiving initTarget, GameLiving aeTarget)
        {
	        if (initTarget == null || aeTarget == null || m_caster == null)
		        return null;

	        var ad = new AttackData();
	        ad.Attacker = m_caster;
	        ad.Target = aeTarget;
	        ad.AttackType = AttackData.eAttackType.Spell;
	        ad.SpellHandler = SpellHandler;
	        ad.AttackResult = eAttackResult.HitUnstyled;
	        ad.IsSpellResisted = false;
	        ad.DamageType = Spell.DamageType;
	        ad.BlockChance = 0;
	        ad.CausesCombat = true;
	        ad.CriticalDamage = 0;
	        ad.EvadeChance = 0;
	        ad.ParryChance = 0;
	        ad.IsSpellResisted = false;
	        ad.Modifier = 0;
	        ad.Damage = CalculateDamageWithFalloff((int)Spell.Damage, initTarget, aeTarget, m_effectiveness);
	        ad.UncappedDamage = ad.Damage;

	        return ad;
        }

        public void OnDirectEffect(GameLiving initTarget, GameLiving aeTarget)
        {
	        if (initTarget == null)
		        return;
	        if (aeTarget == null)
		        return;
	        if (Caster == null)
		        return;
	        if (aeTarget == Caster)
		        return;
	        if (!aeTarget.IsAlive)
		        return;
	        // So they can't use Admins or objects as a target
	        if (!GameServer.ServerRules.IsAllowedToAttack(Caster, aeTarget, true))
		        return;
	        var targetShade = EffectListService.GetEffectOnTarget(aeTarget, eEffect.Shade);
	        if (aeTarget is GamePlayer aePlayer && aePlayer.CharacterClass.ID == (int) eCharacterClass.Necromancer && aePlayer.IsShade && targetShade != null)
		        return;
	        // Target cannot be an ally or friendly
	        if (Caster.Realm == aeTarget.Realm)
		        return;

	        var ad = DamageDetails(initTarget, aeTarget);
	        ad.Target.TakeDamage(ad.Attacker, ad.DamageType, ad.Damage, 0);
	        if (ad.Target.IsStealthed && ad.Target is GamePlayer stealther && ad.Damage > 0)
		        stealther.Stealth(false);
	        ad.Target.LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
	        ad.Target.LastAttackedByEnemyTickPvP = GameLoop.GameLoopTime;

	        // Spell damage messages
	        // Message: 
	        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
	        Caster.Out.SendMessage("You hit " + ad.Target.GetName(0, false) + " for " + ad.Damage + " damage!", eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
	        // Display damage message to target if any damage is actually caused
	        if (ad.Damage > 0)
	        {
		        if (ad.Target is GamePlayer gpTarget)
			        // Message: 
			        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
					gpTarget.Out.SendMessage(ad.Attacker.Name + " hits you for " + ad.Damage + " damage!", eChatType.CT_Damaged, eChatLoc.CL_SystemWindow);
		        ad.Target.StartInterruptTimer(m_interruptTime, ad.AttackType, ad.Attacker);
	        }
        }

        private void ApplyEffectOnTarget(GameLiving target, int duration)
		{
	        if (target == null)
		        return;
	        // Target must be alive
	        if (!target.IsAlive)
		        return;

	        var effect = EffectListService.GetSpellEffectOnTarget(target, eEffect.MovementSpeedDebuff);
	        if (effect != null && effect.SpellHandler.Spell.Name.Equals("Prevent Flight"))
	        {
		        // Message: 
		        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
		        Caster.Out.SendMessage(target.GetName(0, true) + " is immune to this effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        SendAnimation(target, Spell.ClientEffect, false);
		        return;
	        }

	        var targetCharge = EffectListService.GetEffectOnTarget(target, eEffect.Charge);
	        if (targetCharge != null)
	        {
		        SendAnimation(target, Spell.ClientEffect, false);
		        // Message: 
		        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
		        Caster.Out.SendMessage(target.Name + " is moving too fast for this spell to have any effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        return;
	        }

	        var sos = target.EffectList.CountOfType<SpeedOfSoundEffect>() == 1;
	        if (sos)
	        {
		        SendAnimation(target, Spell.ClientEffect, false);
		        // Message: 
		        ChatUtil.SendTypeMessage((int)eMsg., client, "", null);
		        Caster.Out.SendMessage(target.Name + " is moving too fast for this spell to have any effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        return;
	        }

	        var snare = EffectListService.GetEffectOnTarget(target, eEffect.Snare);
	        if (snare != null)
		        EffectService.RequestImmediateCancelEffect(snare);
	        var speedDebuff = EffectListService.GetEffectOnTarget(target, eEffect.MovementSpeedDebuff);
	        if (speedDebuff != null)
		        EffectService.RequestImmediateCancelEffect(speedDebuff);
	        var ichor = EffectListService.GetEffectOnTarget(target, m_effect);
	        if (ichor != null)
		        EffectService.RequestImmediateCancelEffect(ichor);

	        SendAnimation(target, Spell.ClientEffect, true);
	        new AtlasOF_IchorOfTheDeepECSEffect(new ECSGameEffectInitParams(target, duration, Level, CreateSpell(Caster)));
		}

        public override IList<string> DelveInfo
        {
            get
            {
                var delveInfoList = new List<string>();
                delveInfoList.Add(m_ichor);
                delveInfoList.Add("");
                delveInfoList.Add("Level Requirement: " + m_reqLevel);
                delveInfoList.Add("");
                delveInfoList.Add("Function: direct damage & root ");
                delveInfoList.Add("Damages and roots all enemies in the spell's radius.");
                delveInfoList.Add("Damage: " + m_damage);
                delveInfoList.Add("Target: area of effect");
                delveInfoList.Add("Range: " + m_range);
                delveInfoList.Add("Duration: " + m_duration + " sec");
                delveInfoList.Add("Radius: " + m_radius);
                delveInfoList.Add("Damage: " + m_damageType);
                delveInfoList.Add("Casting time: instant");
                delveInfoList.Add("");
                delveInfoList.Add("Can use the ability every: " + FormatTimespan(m_recast));

                return delveInfoList;
            }
        }

        public override void AddEffectsInfo(IList<string> list)
        {
	        list.Add(m_ichor);
	        list.Add("");
            list.Add("Level Requirement: " + m_reqLevel);
            list.Add("");
            list.Add("Function: direct damage & root ");
            list.Add("Damages and roots all enemies in the spell's radius.");
            list.Add("Damage: " + m_damage);
            list.Add("Target: Targeted");
            list.Add("Range: " + m_range);
            list.Add("Duration: " + m_duration + " sec");
            list.Add("Casting time: instant");
            list.Add("Radius: " + m_radius);
            list.Add("Damage: " + m_damageType);
            list.Add("Casting time: instant");
            list.Add("");
            list.Add("Can use the ability every: " + FormatTimespan(m_recast));
        }
    }
}