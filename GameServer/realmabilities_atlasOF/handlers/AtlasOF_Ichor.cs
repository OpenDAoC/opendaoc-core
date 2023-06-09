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
	    private const string ichor = "Ichor of the Deep"; // Ability name
        private const int duration = 20; // Duration of root in seconds
        private const double damage = 400; // Base damage
        private const eDamageType damageType = eDamageType.Spirit;
        private const eEffect effect = eEffect.IchorOfTheDeep;
        private const eSpellType type = eSpellType.IchorOfTheDeep;
        private const int radius = 500; // Area of effect
        private const int range = 1875; // In units
        private const int recast = 900; // In seconds
        private const int reqLevel = 40; // Minimum required level to spec
        private const int interruptTime = 3000; // In milliseconds
        private const int effectiveness = 1; // Spell damage effectiveness
        private const int value = 99; // Root effectiveness
        private const int spellID = 7029;
        private DBSpell _dbSpell;
        private Spell _spell = null;
        private SpellLine _spellLine;
        private SpellHandler _ichorHandler;
        private GamePlayer _caster;
        private GameLiving _owner;

        public GamePlayer Caster => _caster;
        public GameLiving Owner => _owner;
        private DBSpell DBSpell => _dbSpell;
        public Spell Spell => _spell;
        public SpellLine SpellLine => _spellLine;
        public SpellHandler SpellHandler => _ichorHandler;
        public Ability Ability => this;

        public AtlasOF_Ichor(DBAbility dba, int level) : base(dba, level) { }

        public override string Name => ichor;
        public override ushort Icon => spellID;
        public override int MaxLevel => 1;
        public override int CostForUpgrade(int level) { return 14; }
        public override int GetReUseDelay(int level) { return recast; } // 15 minutes
        public override bool CheckRequirement(GamePlayer player)
        {
	        return AtlasRAHelpers.HasPlayerLevel(player, reqLevel);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        
        /// <summary>
        /// Sets the parameters of the realm ability and its effects.
        /// </summary>
        /// <param name="caster">The player casting/using the realm ability.</param>
        /// <returns></returns>
        public SpellHandler CreateSpell(GamePlayer caster)
        {
	        _dbSpell = new DBSpell();
	        _dbSpell.Name = ichor;
	        _dbSpell.Icon = spellID;
	        _dbSpell.ClientEffect = spellID;
	        _dbSpell.Damage = damage; // 400
	        _dbSpell.DamageType = (int)damageType; // Spirit
	        _dbSpell.Target = "Enemy";
	        _dbSpell.Radius = radius; // 500
	        _dbSpell.Type = type.ToString();
	        _dbSpell.Value = value;
	        _dbSpell.Duration = duration; // 20 seconds
	        _dbSpell.Pulse = 0;
	        _dbSpell.PulsePower = 0;
	        _dbSpell.Power = 0;
	        _dbSpell.CastTime = 0; // Instant cast
	        _dbSpell.EffectGroup = 0;
	        _dbSpell.Range = range; // 1875 units
	        _dbSpell.Frequency = 0;
	        _dbSpell.RecastDelay = recast; // 15 minutes
	        _dbSpell.InstrumentRequirement = 0;
	        _dbSpell.Concentration = 0;
	        _dbSpell.Description = "Damages and roots all enemies in the spell's radius. This ignores existing root immunities.";
	        _dbSpell.Message1 = "Constricting bonds surround your body!";
	        _dbSpell.Message2 = "{0} is surrounded by constricting bonds!";
	        _spell = new Spell(_dbSpell, reqLevel);
	        _spellLine = new SpellLine("RAs", "RealmAbilities", "RealmAbilities", true);
	        _ichorHandler = new SpellHandler(caster, new Spell(_dbSpell,  reqLevel) , _spellLine); // Make spell level 0 so that it bypasses the spec level adjustment code

	        return _ichorHandler;
        }

        public override void Execute(GameLiving living)
        {
	        // Regular null check
	        if (living == null)
		        return;

	        // Make sure the caster is an actual player
	        if (living is not GamePlayer)
		        return;

	        // Make sure the caster is not indisposed
	        if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED))
            {
	            if (CheckPreconditions(living, DEAD))
		            Caster.Out.SendMessage("You cannot use this ability while dead!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
	            
	            if (CheckPreconditions(living, SITTING))
		            Caster.Out.SendMessage("You cannot use this ability while sitting!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
	            
	            if (CheckPreconditions(living, MEZZED))
		            Caster.Out.SendMessage("You cannot use this ability while mesmerized!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
	            
	            if (CheckPreconditions(living, STUNNED))
		            Caster.Out.SendMessage("You cannot use this ability while stunned!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
	            
	            return;
            }
	        
            _caster = (GamePlayer)living;
            _ichorHandler = CreateSpell((GamePlayer)living);
            _owner = Caster.TargetObject as GameLiving;
            
            // Caster must have a target
			if (Caster.TargetObject == null)
			{
				Caster.Out.SendMessage("You must select a target for this ability!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}
			
			// Cannot use ability if timer is not expired
			if (Spell.HasRecastDelay && Caster.GetSkillDisabledDuration(Spell) > 0)
			{
				Caster.Out.SendMessage("You must wait " + Caster.GetSkillDisabledDuration(Spell) / 1000 + " seconds to recast this type of ability!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// Caster can't use objects as a target
			if (_owner == null || _owner is Keeps.GameKeepDoor or Keeps.GameKeepComponent)
			{
				Caster.Out.SendMessage("You have an invalid target!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// Target must be in front of the Player
			if (!Caster.TargetInView || !Caster.IsObjectInFront(_owner, 150))
			{
				Caster.Out.SendMessage(_owner.GetName(0, true) + " is not in view!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// Can't target self
			if (Caster == _owner)
			{
				Caster.Out.SendMessage("You can't attack yourself!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}
			
			// Target must be within range
			if (!Caster.IsWithinRadius(_owner, SpellHandler.CalculateSpellRange()))
			{
				Caster.Out.SendMessage(_owner.GetName(0, true) + " is too far away!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// Target must be alive
			if (!_owner.IsAlive)
			{
				Caster.Out.SendMessage(_owner.GetName(0, true) + " is dead!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// So they can't use Admins/GMs or Necros as a target
			if (!GameServer.ServerRules.IsAllowedToAttack(Caster, _owner, true) || (_owner is GamePlayer playerTarget && playerTarget.CharacterClass.ID == (int)eCharacterClass.Necromancer && playerTarget.IsShade))
			{
				Caster.Out.SendMessage(_owner.GetName(0, true) + " can't be attacked!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}

			// Target cannot be an ally or friendly
			if (Caster.Realm == _owner.Realm)
			{
				Caster.Out.SendMessage("You can't attack a member of your realm!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				Caster.DisableSkill(this, interruptTime);
				return;
			}
			
			var ad = DamageDetails(_owner, _owner);

			SendCastMessage(Caster);
			OnDirectEffect(ad.Target, ad.Target);
			var targetDur = CalculateEffectDuration(ad.Target, effectiveness);
			ApplyEffectOnTarget(ad.Target, targetDur);

			// Go through all applicable GamePlayer targets in ability radius and apply damage/effect
			foreach (GamePlayer aePlayers in _owner.GetPlayersInRadius((ushort) Spell.Radius))
			{
				if (aePlayers == null) continue;
				if (aePlayers.Realm == Caster.Realm) continue;
				if (aePlayers.Client.Account.PrivLevel > 1) continue;
				if (!aePlayers.IsAlive) continue;
				if (aePlayers == Caster) continue;

				var gpAD = DamageDetails(_owner, aePlayers);

				// Apply secondary/reduced damage to all applicable targets in RA radius
				if (gpAD.Target != _owner)
				{
					OnDirectEffect(_owner, gpAD.Target);
					var aeDur = CalculateEffectDuration(gpAD.Target, effectiveness);
					ApplyEffectOnTarget(gpAD.Target, aeDur);
				}
			}
			
			// Go through all applicable GameNPC targets in ability radius and apply damage/effect
			foreach (GameNPC aeNPCs in _owner.GetNPCsInRadius((ushort) Spell.Radius))
			{
				if (aeNPCs == null) continue;
				if (aeNPCs.Realm == Caster.Realm) continue;
				if (!aeNPCs.IsAlive) continue;

				var npcAD = DamageDetails(_owner, aeNPCs);

				if (npcAD.Target != _owner)
				{
					OnDirectEffect(_owner, npcAD.Target);
					var npcDur = CalculateEffectDuration(npcAD.Target, effectiveness);
					ApplyEffectOnTarget(npcAD.Target, npcDur);
				}
			}

			Caster.DisableSkill(this, Spell.RecastDelay);
        }

        public void SendAnimation(GameLiving target, ushort spellID, bool success)
        {
	        if (_caster == null)
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

	        var duration = Spell.Duration * effectiveness;
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
	        if (initTarget == null || aeTarget == null || _caster == null)
		        return null;

	        var ad = new AttackData();
	        ad.Attacker = _caster;
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
	        ad.Damage = CalculateDamageWithFalloff((int)Spell.Damage, initTarget, aeTarget, effectiveness);

	        return ad;
        }

        public void OnDirectEffect(GameLiving initTarget, GameLiving aeTarget)
        {
	        // Determining which targets cannot be hurt/affected by ability
	        
	        // Skip if target is null
	        if (initTarget == null)
		        return;
	        
	        // Skip if AE target is null
	        if (aeTarget == null)
		        return;
	        
	        // Skip if caster is null
	        if (Caster == null)
		        return;
	        
	        // Skip if target is caster
	        if (aeTarget == Caster)
		        return;
	        
	        // Skip if target is dead
	        if (!aeTarget.IsAlive)
		        return;
	        
	        // So they can't use Admins/GMs as a target
	        if (!GameServer.ServerRules.IsAllowedToAttack(Caster, aeTarget, true))
		        return;
	        
	        var targetShade = EffectListService.GetEffectOnTarget(aeTarget, eEffect.Shade);
	        
	        // Skip if player is in shade form
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
	        Caster.Out.SendMessage("You hit " + ad.Target.GetName(0, false) + " for " + ad.Damage + " damage!", eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
	        
	        // Display damage message to target if any damage is actually caused
	        if (ad.Damage > 0)
	        {
		        if (ad.Target is GamePlayer gpTarget)
					gpTarget.Out.SendMessage(ad.Attacker.Name + " hits you for " + ad.Damage + " damage!", eChatType.CT_Damaged, eChatLoc.CL_SystemWindow);
		        ad.Target.StartInterruptTimer(interruptTime, ad.AttackType, ad.Attacker);
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
	        
	        // Check for Prevent Flight
	        if (effect != null && effect.SpellHandler.Spell.Name.Equals("Prevent Flight"))
	        {
		        Caster.Out.SendMessage(target.GetName(0, true) + " is immune to this effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        SendAnimation(target, Spell.ClientEffect, false);
		        return;
	        }

	        var targetCharge = EffectListService.GetEffectOnTarget(target, eEffect.Charge);
	        
	        // Check for Charge
	        if (targetCharge != null)
	        {
		        SendAnimation(target, Spell.ClientEffect, false);
		        Caster.Out.SendMessage(target.Name + " is moving too fast for this spell to have any effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        return;
	        }

	        var sos = target.effectListComponent.Effects.ContainsKey(eEffect.SpeedOfSound);
	        
			// Check for Speed of Sound
	        if (sos)
	        {
		        SendAnimation(target, Spell.ClientEffect, false);
		        Caster.Out.SendMessage(target.Name + " is moving too fast for this spell to have any effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
		        return;
	        }

	        var snare = EffectListService.GetEffectOnTarget(target, eEffect.Snare);
	        
	        // Overwrite snare
	        if (snare != null)
		        EffectService.RequestImmediateCancelEffect(snare);
	        
	        var speedDebuff = EffectListService.GetEffectOnTarget(target, eEffect.MovementSpeedDebuff);
	        
	        // Overwrite speed debuff
	        if (speedDebuff != null)
		        EffectService.RequestImmediateCancelEffect(speedDebuff);
	        
	        var ichor = EffectListService.GetEffectOnTarget(target, AtlasOF_Ichor.effect);
	        
	        // Overwrite existing Ichor effect, if any exists
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
                delveInfoList.Add(ichor);
                delveInfoList.Add("");
                delveInfoList.Add("Level Requirement: " + reqLevel);
                delveInfoList.Add("");
                delveInfoList.Add("Function: direct damage & root ");
                delveInfoList.Add("Damages and roots all enemies in the spell's radius.");
                delveInfoList.Add("Damage: " + damage);
                delveInfoList.Add("Target: area of effect");
                delveInfoList.Add("Range: " + range);
                delveInfoList.Add("Duration: " + duration + " sec");
                delveInfoList.Add("Radius: " + radius);
                delveInfoList.Add("Damage: " + damageType);
                delveInfoList.Add("Casting time: instant");
                delveInfoList.Add("");
                delveInfoList.Add("Can use the ability every: " + FormatTimespan(recast / 60) + " min");

                return delveInfoList;
            }
        }

        public override void AddEffectsInfo(IList<string> list)
        {
	        list.Add(ichor);
	        list.Add("");
            list.Add("Level Requirement: " + reqLevel);
            list.Add("");
            list.Add("Function: direct damage & root ");
            list.Add("Damages and roots all enemies in the spell's radius.");
            list.Add("Damage: " + damage);
            list.Add("Target: area of effect");
            list.Add("Range: " + range);
            list.Add("Duration: " + duration + " sec");
            list.Add("Casting time: instant");
            list.Add("Radius: " + radius);
            list.Add("Damage: " + damageType);
            list.Add("Casting time: instant");
            list.Add("");
            list.Add("Can use the ability every: " + FormatTimespan(recast / 60) + " min");
        }
    }
}