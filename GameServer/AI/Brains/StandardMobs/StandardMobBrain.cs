
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.GS.SkillHandler;
using DOL.GS.Spells;
using DOL.Language;

namespace DOL.AI.Brain
{
    /// <summary>
    /// Standard brain for standard mobs
    /// </summary>
    public class StandardMobBrain : APlayerVicinityBrain, IOldAggressiveBrain
    {
        public const int MAX_AGGRO_DISTANCE = 3600;
        public const int MAX_AGGRO_LIST_DISTANCE = 6000;

        // Used for AmbientBehaviour "Seeing" - maintains a list of GamePlayer in range
        public List<GamePlayer> PlayersSeen = new List<GamePlayer>();

        public new StandardMobFsm FSM { get; private set; }

        /// <summary>
        /// Constructs a new StandardMobBrain
        /// </summary>
        public StandardMobBrain() : base()
        {
            AggroLevel = 0;
            AggroRange = 0;

            FSM = new StandardMobFsm();
            FSM.Add(new StandardMobState_Idle(FSM, this));
            FSM.Add(new StandardMobState_WakingUp(FSM, this));
            FSM.Add(new StandardMobState_Aggro(FSM, this));
            FSM.Add(new StandardMobState_ReturnToSpawn(FSM, this));
            FSM.Add(new StandardMobState_Patrolling(FSM, this));
            FSM.Add(new StandardMobState_Roaming(FSM, this));
            FSM.Add(new StandardMobState_Dead(FSM, this));

            FSM.SetCurrentState(EFsmStateType.WAKING_UP);
        }

        /// <summary>
        /// Returns the string representation of the StandardMobBrain
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + ", AggroLevel=" + AggroLevel.ToString() + ", AggroRange=" + AggroRange.ToString();
        }

        public override bool Stop()
        {
            // tolakram - when the brain stops, due to either death or no players in the vicinity, clear the aggro list
            if (base.Stop())
            {
                ClearAggroList();
                return true;
            }

            return false;
        }

        public override void KillFSM()
        {
            FSM.KillFSM();
        }

        #region AI

        public override void Think()
        {
            FSM.Think();
        }

        public virtual bool CheckProximityAggro()
        {
            FireAmbientSentence();

            // Check aggro only if our aggro list is empty and we're not in combat.
            if (AggroRange > 0 && !HasAggro && !Body.AttackState && Body.CurrentSpellHandler == null)
            {
                // Don't check aggro if we spawned less than X seconds ago. This is to prevent clients from sending positive LoS check
                // when they shouldn't, which can happen right after 'SendNPCCreate' and makes mobs aggro through walls.
                // TODO: Find a way to delay the first tick of 'Think()' instead.
                if (GameLoop.GameLoopTime - Body.SpawnTick < 1250)
                    return false;

                CheckPlayerAggro();
                CheckNPCAggro();
            }

            // Some calls rely on this method to return if there's something in the aggro list, not necesarilly to perform a proximity aggro check.
            // But this doesn't necessarily return wheter or not the check was positive, only the current state (LoS checks take time).
            return HasAggro;
        }

        public virtual bool IsBeyondTetherRange()
        {
            if (Body.MaxDistance != 0)
            {
                int distance = Body.GetDistanceTo(Body.SpawnPoint);
                int maxdistance = Body.MaxDistance > 0 ? Body.MaxDistance : -Body.MaxDistance * AggroRange / 100;

                if (maxdistance > 0 && distance > maxdistance)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public virtual bool HasPatrolPath()
        {
            if (Body.MaxSpeedBase > 0
                && Body.CurrentSpellHandler == null
                && !Body.IsMoving
                && !Body.attackComponent.AttackState
                && !Body.InCombat
                && !Body.IsMovingOnPath
                && Body.PathID != null
                && Body.PathID != ""
                && Body.PathID != "NULL")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check for aggro against players
        /// </summary>
        protected virtual void CheckPlayerAggro()
        {
            foreach (GamePlayer player in Body.GetPlayersInRadius((ushort)AggroRange, !Body.CurrentZone.IsDungeon))
            {
                if (!CanAggroTarget(player))
                    continue;

                if (player.IsStealthed || player.Steed != null)
                    continue;

                if (player.EffectList.GetOfType<NecromancerShadeEffect>() != null)
                    continue;

                if (GS.ServerProperties.ServerProperties.ALWAYS_CHECK_LOS)
                    // We don't know if the LoS check will be positive, so we have to ask other players
                    player.Out.SendCheckLOS(Body, player, new CheckLOSResponse(LosCheckForAggroCallback));
                else
                {
                    AddToAggroList(player, 0);
                    return;
                }
            }
        }

        /// <summary>
        /// Check for aggro against close NPCs
        /// </summary>
        protected virtual void CheckNPCAggro()
        {
            foreach (GameNpc npc in Body.GetNPCsInRadius((ushort)AggroRange, !Body.CurrentRegion.IsDungeon))
            {
                if (!CanAggroTarget(npc))
                    continue;

                if (npc is GameTaxi or GameTrainingDummy)
                    continue;

                if (GS.ServerProperties.ServerProperties.ALWAYS_CHECK_LOS)
                {
                    // Check LoS if either the target or the current mob is a pet
                    if (npc.Brain is ControlledNpcBrain theirControlledNpcBrain && theirControlledNpcBrain.GetPlayerOwner() is GamePlayer theirOwner)
                    {
                        theirOwner.Out.SendCheckLOS(Body, npc, new CheckLOSResponse(LosCheckForAggroCallback));
                        continue;
                    }
                    else if (this is ControlledNpcBrain ourControlledNpcBrain && ourControlledNpcBrain.GetPlayerOwner() is GamePlayer ourOwner)
                    {
                        ourOwner.Out.SendCheckLOS(Body, npc, new CheckLOSResponse(LosCheckForAggroCallback));
                        continue;
                    }
                }

                AddToAggroList(npc, 0);
                return;
            }
        }

        public virtual void FireAmbientSentence()
        {
            if (Body.ambientTexts != null && Body.ambientTexts.Any(item => item.Trigger == "seeing"))
            {
                // Check if we can "see" players and fire off ambient text
                var currentPlayersSeen = new List<GamePlayer>();
                foreach (GamePlayer player in Body.GetPlayersInRadius((ushort)AggroRange, true))
                {
                    if (!PlayersSeen.Contains(player))
                    {
                        Body.FireAmbientSentence(GameNpc.eAmbientTrigger.seeing, player);
                        PlayersSeen.Add(player);
                    }
                    currentPlayersSeen.Add(player);
                }

                for (int i = 0; i < PlayersSeen.Count; i++)
                {
                    if (!currentPlayersSeen.Contains(PlayersSeen[i]))
                        PlayersSeen.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// The interval for thinking, min 1.5 seconds
        /// 10 seconds for 0 aggro mobs
        /// </summary>
        public override int ThinkInterval
        {
            get
            { 
                if (Body is GameMerchant or GameTrainer or GameHastener)
                    return 5000; //Merchants and other special NPCs don't need to think that often

                return Math.Max(500, 1500 - (AggroLevel / 10 * 100));
            }
        }

        /// <summary>
        /// If this brain is part of a formation, it edits it's values accordingly.
        /// </summary>
        /// <param name="x">The x-coordinate to refer to and change</param>
        /// <param name="y">The x-coordinate to refer to and change</param>
        /// <param name="z">The x-coordinate to refer to and change</param>
        public virtual bool CheckFormation(ref int x, ref int y, ref int z)
        {
            return false;
        }

        /// <summary>
        /// Checks the Abilities
        /// </summary>
        public virtual void CheckAbilities()
        {
            //See CNPC
        }

        #endregion

        #region Aggro

        protected int _aggroRange;

        /// <summary>
        /// Max Aggro range in that this npc searches for enemies
        /// </summary>
        public virtual int AggroRange
        {
            get => Math.Min(_aggroRange, MAX_AGGRO_DISTANCE);
            set => _aggroRange = value;
        }

        /// <summary>
        /// Aggressive Level in % 0..100, 0 means not Aggressive
        /// </summary>
        public virtual int AggroLevel { get; set; }

        /// <summary>
        /// List of livings that this npc has aggro on, living => aggroAmount
        /// </summary>
        public Dictionary<GameLiving, long> AggroTable { get; private set; } = new Dictionary<GameLiving, long>();

        /// <summary>
        /// Checks whether living has someone on its aggrolist
        /// </summary>
        public virtual bool HasAggro
        {
            get
            {
                lock ((AggroTable as ICollection).SyncRoot)
                {
                    return AggroTable.Count > 0;
                }
            }
        }

        /// <summary>
        /// Add aggro table of this brain to that of another living.
        /// </summary>
        public void AddAggroListTo(StandardMobBrain brain)
        {
            if (!brain.Body.IsAlive)
                return;

            KeyValuePair<GameLiving, long>[] aggroTable = Array.Empty<KeyValuePair<GameLiving, long>>();

            lock ((AggroTable as ICollection).SyncRoot)
                aggroTable = AggroTable.ToArray();

            foreach (var aggro in aggroTable)
                brain.AddToAggroList(aggro.Key, Body.MaxHealth);
        }

        /// <summary>
        /// Add living to the aggrolist
        /// aggroAmount can be negative to lower amount of aggro
        /// </summary>
        public virtual void AddToAggroList(GameLiving living, int aggroAmount)
        {
            // tolakram - duration spell effects will attempt to add to aggro after npc is dead
            if (Body.IsConfused || !Body.IsAlive || living == null)
                return;

            BringFriends(living);

            // Handle trigger to say sentance on first aggro.
            if (AggroTable.Count < 1)
                Body.FireAmbientSentence(GameNpc.eAmbientTrigger.aggroing, living);

            // Only protect if gameplayer and aggroAmount > 0
            if (living is GamePlayer player && aggroAmount > 0)
            {
                // If player is in group, add whole group to aggro list
                if (player.Group != null)
                { 
                    lock ((AggroTable as ICollection).SyncRoot)
                    {
                        foreach (GamePlayer p in player.Group.GetPlayersInTheGroup())
                        {
                            if (!AggroTable.ContainsKey(p))
                                AggroTable[p] = 1L; // Add the missing group member on aggro table
                        }
                    }
                }

                foreach (ProtectEcsEffect protect in player.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Protect))
                {
                    if (aggroAmount <= 0)
                        break;

                    if (protect.ProtectTarget != living)
                        continue;

                    GamePlayer protectSource = protect.ProtectSource;

                    if (protectSource.IsStunned
                        || protectSource.IsMezzed
                        || protectSource.IsSitting
                        || protectSource.ObjectState != GameObject.eObjectState.Active
                        || !protectSource.IsAlive
                        || !protectSource.InCombat)
                        continue;

                    if (!living.IsWithinRadius(protectSource, ProtectHandler.PROTECT_DISTANCE))
                        continue;

                    // P I: prevents 10% of aggro amount
                    // P II: prevents 20% of aggro amount
                    // P III: prevents 30% of aggro amount
                    // guessed percentages, should never be higher than or equal to 50%
                    int abilityLevel = protectSource.GetAbilityLevel(Abilities.Protect);
                    int protectAmount = (int)(abilityLevel * 0.10 * aggroAmount);

                    if (protectAmount > 0)
                    {
                        aggroAmount -= protectAmount;
                        protectSource.Out.SendMessage(LanguageMgr.GetTranslation(protectSource.Client.Account.Language, "AI.Brain.StandardMobBrain.YouProtDist", player.GetName(0, false),
                                                                                 Body.GetName(0, false, protectSource.Client.Account.Language, Body)), EChatType.CT_System, EChatLoc.CL_SystemWindow);

                        lock ((AggroTable as ICollection).SyncRoot)
                        {
                            if (AggroTable.ContainsKey(protectSource))
                                AggroTable[protectSource] += protectAmount;
                            else
                                AggroTable[protectSource] = protectAmount;
                        }
                    }
                }
            }

            lock ((AggroTable as ICollection).SyncRoot)
            {
                if (AggroTable.ContainsKey(living))
                {
                    long amount = AggroTable[living];
                    amount += aggroAmount;

                    // can't be removed this way, set to minimum
                    if (amount <= 0)
                        amount = 1L;

                    AggroTable[living] = amount;
                }
                else
                {
                    if (aggroAmount > 0)
                        AggroTable[living] = aggroAmount;
                    else
                        AggroTable[living] = 1L;
                }
            }
        }

        public void PrintAggroTable()
        {
            StringBuilder sb = new StringBuilder();
            foreach (GameLiving gl in AggroTable.Keys)
                sb.AppendLine("Living: " + gl.Name + ", aggro: " + AggroTable[gl].ToString());
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Get current amount of aggro on aggrotable.
        /// </summary>
        public virtual long GetAggroAmountForLiving(GameLiving living)
        {
            lock ((AggroTable as ICollection).SyncRoot)
            {
                if (AggroTable.ContainsKey(living))
                    return AggroTable[living];
                return 0;
            }
        }

        /// <summary>
        /// Remove one living from aggro list.
        /// </summary>
        public virtual void RemoveFromAggroList(GameLiving living)
        {
            lock ((AggroTable as ICollection).SyncRoot)
                AggroTable.Remove(living);
        }

        /// <summary>
        /// Remove all livings from the aggrolist.
        /// </summary>
        public virtual void ClearAggroList()
        {
            CanBAF = true; // Mobs that drop out of combat can BAF again

            lock ((AggroTable as ICollection).SyncRoot)
            {
                AggroTable.Clear();
                Body.TempProperties.removeProperty(Body.attackComponent.Attackers);
            }
        }

        /// <summary>
        /// Selects and attacks the next target or does nothing.
        /// </summary>
        public virtual void AttackMostWanted()
        {
            if (!IsActive)
                return;

            if (ECS.Debug.Diagnostics.AggroDebugEnabled)
                PrintAggroTable();

            Body.TargetObject = CalculateNextAttackTarget();

            if (Body.TargetObject != null)
            {
                if (!CheckSpells(eCheckSpellType.Offensive))
                    Body.StartAttack(Body.TargetObject);
            }
        }

        protected virtual void LosCheckForAggroCallback(GamePlayer player, ushort response, ushort targetOID)
        {
            // If we kept adding to the aggro list it would make mobs go from one target immediately to another.
            if (HasAggro || targetOID == 0)
                return;

            if ((response & 0x100) == 0x100)
            {
                GameObject gameObject = Body.CurrentRegion.GetObject(targetOID);

                if (gameObject is GameLiving gameLiving)
                    AddToAggroList(gameLiving, 0);
            }
        }

        /// <summary>
        /// Returns whether or not 'living' is still a valid target.
        /// </summary>
        protected virtual bool ShouldThisLivingBeFilteredOutFromAggroList(GameLiving living)
        {
            return !living.IsAlive ||
                   living.ObjectState != GameObject.eObjectState.Active ||
                   living.IsStealthed ||
                   living.CurrentRegion != Body.CurrentRegion ||
                   !Body.IsWithinRadius(living, MAX_AGGRO_LIST_DISTANCE) ||
                   !GameServer.ServerRules.IsAllowedToAttack(Body, living, true);
        }

        /// <summary>
        /// Returns a copy of 'aggroList' ordered by aggro amount (descending), modified by range.
        /// </summary>
        protected virtual List<KeyValuePair<GameLiving, long>> OrderAggroListByModifiedAggroAmount(Dictionary<GameLiving, long> aggroList)
        {
            return aggroList.OrderByDescending(x => x.Value * Math.Min(500.0 / Body.GetDistanceTo(x.Key), 1)).ToList();
        }

        /// <summary>
        /// Filters out invalid targets from the current aggro list and returns a copy.
        /// </summary>
        protected virtual Dictionary<GameLiving, long> FilterOutInvalidLivingsFromAggroList()
        {
            Dictionary<GameLiving, long> tempAggroList;
            bool modified = false;

            lock ((AggroTable as ICollection).SyncRoot)
            {
                tempAggroList = new Dictionary<GameLiving, long>(AggroTable);
            }

            foreach (KeyValuePair<GameLiving, long> pair in tempAggroList.ToList())
            {
                GameLiving living = pair.Key;

                if (living == null)
                    continue;

                // Check to make sure this living is still a valid target.
                if (ShouldThisLivingBeFilteredOutFromAggroList(living))
                {
                    // Keep Necromancer shades so that we can attack them if their pets die.
                    if (EffectListService.GetEffectOnTarget(living, EEffect.Shade) != null)
                        continue;

                    modified = true;
                    tempAggroList.Remove(living);
                }
            }

            if (modified)
            {
                // Body.attackComponent.RemoveAttacker(removable.Key); ???

                lock ((AggroTable as ICollection).SyncRoot)
                {
                    AggroTable = tempAggroList.ToDictionary(x => x.Key, x => x.Value);
                }
            }

            return tempAggroList;
        }

        /// <summary>
        /// Returns the best target to attack from the current aggro list.
        /// </summary>
        protected virtual GameLiving CalculateNextAttackTarget()
        {
            // Filter out invalid entities (updates the list), then order the returned copy by (modified) aggro amount.
            List<KeyValuePair<GameLiving, long>> aggroList = OrderAggroListByModifiedAggroAmount(FilterOutInvalidLivingsFromAggroList());

            // We keep shades in aggro lists so that mobs attack them after their pet dies, but we must never return one.
            GameLiving nextTarget = aggroList.Find(x => EffectListService.GetEffectOnTarget(x.Key, EEffect.Shade) == null).Key;

            if (nextTarget != null)
                return nextTarget;

            // The list is either empty or full of shades.
            // If it's empty, return null.
            // If we found a shade, return the pet instead (if there's one). Ideally this should never happen.
            // If it does, it means we added the shade to the aggro list instead of the pet.
            // Which is currently the case due to the way 'AddToAggroList' propagates aggro to group members, and maybe other places.
            return aggroList.FirstOrDefault().Key?.ControlledBrain?.Body;
        }

        public virtual bool CanAggroTarget(GameLiving target)
        {
            if (!GameServer.ServerRules.IsAllowedToAttack(Body, target, true))
                return false;

            // Get owner if target is pet or subpet
            GameLiving realTarget = target;

            if (realTarget is GameNpc npcTarget && npcTarget.Brain is IControlledBrain npcTargetBrain)
                realTarget = npcTargetBrain.GetPlayerOwner();

            // Only attack if green+ to target
            if (realTarget.IsObjectGreyCon(Body))
                return false;

            // If this npc have Faction return the AggroAmount to Player
            if (Body.Faction != null)
            {
                if (realTarget is GamePlayer)
                    return Body.Faction.GetAggroToFaction((GamePlayer)realTarget) > 75;
                else if (realTarget is GameNpc && Body.Faction.EnemyFactions.Contains(((GameNpc)realTarget).Faction))
                    return true;
            }

            // We put this here to prevent aggroing non-factions npcs
            if (Body.Realm == ERealm.None && realTarget is GameNpc)
                return false;

            return AggroLevel > 0;
        }

        protected virtual void OnFollowLostTarget(GameObject target)
        {
            AttackMostWanted();
            if (!Body.attackComponent.AttackState)
                Body.ReturnToSpawnPoint();
        }

		public virtual void OnAttackedByEnemy(AttackData ad)
		{
			if (!Body.IsAlive || Body.ObjectState != GameObject.eObjectState.Active)
				return;

			if (FSM.GetCurrentState() == FSM.GetState(EFsmStateType.PASSIVE))
				return;

			int damage = ad.Damage + ad.CriticalDamage + Math.Abs(ad.Modifier);

			if (!Body.attackComponent.AttackState && FSM.GetCurrentState() != FSM.GetState(EFsmStateType.AGGRO))
			{
				// Aggro must be generated before calling Think(), otherwise the mob won't attack immediately.
				// Ensure that non damaging hits still result in the mob reacting.
				ConvertDamageToAggroAmount(ad.Attacker, Math.Max(1, damage));
				FSM.SetCurrentState(EFsmStateType.AGGRO);
				FSM.Think();
			}
			else if (damage > 0)
				ConvertDamageToAggroAmount(ad.Attacker, damage);
		}

		/// <summary>
		/// Converts a damage amount into an aggro amount, and splits it between the pet and its owner if necessary.
		/// Assumes damage to be superior than 0.
		/// </summary>
		protected virtual void ConvertDamageToAggroAmount(GameLiving attacker, int damage)
		{
			if (attacker is GameNpc NpcAttacker && NpcAttacker.Brain is ControlledNpcBrain controlledBrain)
			{
				damage = controlledBrain.ModifyDamageWithTaunt(damage);

				// Aggro is split between the owner (15%) and their pet (85%).
				int aggroForOwner = (int)(damage * 0.15);
				
				// We must ensure that the same amount of aggro isn't added for both, otherwise an out-of-combat mob could attack the owner when their pet engages it.
				// The owner must also always generate at least 1 aggro.
				// This works as long as the split isn't 50 / 50.
				if (aggroForOwner == 0)
				{
					AddToAggroList(controlledBrain.Owner, 1);
					AddToAggroList(NpcAttacker, Math.Max(2, damage));
				}
				else
				{
					AddToAggroList(controlledBrain.Owner, aggroForOwner);
					AddToAggroList(NpcAttacker, damage - aggroForOwner);
				}
			}
			else
				AddToAggroList(attacker, damage);
		}

        #endregion

        #region Bring a Friend

        /// <summary>
        /// Initial range to try to get BAFs from.
        /// May be overloaded for specific brain types, ie. dragons or keep guards
        /// </summary>
        protected virtual ushort BAFInitialRange {
            get { return 250; }
        }

        /// <summary>
        /// Max range to try to get BAFs from.
        /// May be overloaded for specific brain types, ie.dragons or keep guards
        /// </summary>
        protected virtual ushort BAFMaxRange {
            get { return 2000; }
        }

        /// <summary>
        /// Max range to try to look for nearby players.
        /// May be overloaded for specific brain types, ie.dragons or keep guards
        /// </summary>
        protected virtual ushort BAFPlayerRange {
            get { return 5000; }
        }

        /// <summary>
        /// Can the mob bring a friend?
        /// Set to false when a mob BAFs or is brought by a friend.
        /// </summary>
        public virtual bool CanBAF { get; set; } = true;

        /// <summary>
        /// Bring friends when this mob aggros
        /// </summary>
        /// <param name="attacker">Whoever triggered the BAF</param>
        protected virtual void BringFriends(GameLiving attacker)
        {
            if (!CanBAF)
                return;

            GamePlayer puller;  // player that triggered the BAF
            GameLiving actualPuller;

            // Only BAF on players and pets of players
            if (attacker is GamePlayer)
            {
                puller = (GamePlayer)attacker;
                actualPuller = puller;
            }
            else if (attacker is GameSummonedPet pet && pet.Owner is GamePlayer owner)
            {
                puller = owner;
                actualPuller = attacker;
            }
            else if (attacker is BdSubPet bdSubPet && bdSubPet.Owner is GameSummonedPet bdPet && bdPet.Owner is GamePlayer bdOwner)
            {
                puller = bdOwner;
                actualPuller = bdPet;
            }
            else
                return;

            CanBAF = false; // Mobs only BAF once per fight

            int numAttackers = 0;

            List<GamePlayer> victims = null; // Only instantiated if we're tracking potential victims

            // These are only used if we have to check for duplicates
            HashSet<String> countedVictims = null;
            HashSet<String> countedAttackers = null;

            BattleGroup bg = puller.TempProperties.getProperty<object>(BattleGroup.BATTLEGROUP_PROPERTY, null) as BattleGroup;

            // Check group first to minimize the number of HashSet.Add() calls
            if (puller.Group is Group group)
            {
                if (DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_COUNT_BG_MEMBERS && bg != null)
                    countedAttackers = new HashSet<String>(); // We have to check for duplicates when counting attackers

                if (!DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_PULLER)
                {
                    if (DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_BG_MEMBERS && bg != null)
                    {
                        // We need a large enough victims list for group and BG, and also need to check for duplicate victims
                        victims = new List<GamePlayer>(group.MemberCount + bg.PlayerCount - 1);
                        countedVictims = new HashSet<String>();
                    }
                    else
                        victims = new List<GamePlayer>(group.MemberCount);
                }

                foreach (GamePlayer player in group.GetPlayersInTheGroup())
                    if (player != null && (player.InternalID == puller.InternalID || player.IsWithinRadius(puller, BAFPlayerRange, true)))
                    {
                        numAttackers++;

                        if (countedAttackers != null)
                            countedAttackers.Add(player.InternalID);

                        if (victims != null)
                        {
                            victims.Add(player);

                            if (countedVictims != null)
                                countedVictims.Add(player.InternalID);
                        }
                    }
            } // if (puller.Group is Group group)

            // Do we have to count BG members, or add them to victims list?
            if ((bg != null) && (DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_COUNT_BG_MEMBERS
                || (DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_BG_MEMBERS && !DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_PULLER)))
            {
                if (victims == null && DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_BG_MEMBERS && !DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_ATTACK_PULLER)
                    // Puller isn't in a group, so we have to create the victims list for the BG
                    victims = new List<GamePlayer>(bg.PlayerCount);

                foreach (GamePlayer player in bg.Members.Keys)
                    if (player != null && (player.InternalID == puller.InternalID || player.IsWithinRadius(puller, BAFPlayerRange, true)))
                    {
                        if (DOL.GS.ServerProperties.ServerProperties.BAF_MOBS_COUNT_BG_MEMBERS
                            && (countedAttackers == null || !countedAttackers.Contains(player.InternalID)))
                            numAttackers++;

                        if (victims != null && (countedVictims == null || !countedVictims.Contains(player.InternalID)))
                            victims.Add(player);
                    }
            } // if ((bg != null) ...

            if (numAttackers == 0)
                // Player is alone
                numAttackers = 1;

            int percentBAF = DOL.GS.ServerProperties.ServerProperties.BAF_INITIAL_CHANCE
                + ((numAttackers - 1) * DOL.GS.ServerProperties.ServerProperties.BAF_ADDITIONAL_CHANCE);

            int maxAdds = percentBAF / 100; // Multiple of 100 are guaranteed BAFs

            // Calculate chance of an addition add based on the remainder
            if (UtilCollection.Chance(percentBAF % 100))
                maxAdds++;

            if (maxAdds > 0)
            {
                int numAdds = 0; // Number of mobs currently BAFed
                ushort range = BAFInitialRange; // How far away to look for friends

                // Try to bring closer friends before distant ones.
                while (numAdds < maxAdds && range <= BAFMaxRange)
                {
                    foreach (GameNpc npc in Body.GetNPCsInRadius(range))
                    {
                        if (numAdds >= maxAdds)
                            break;

                        // If it's a friend, have it attack
                        if (npc.IsFriend(Body) && npc.IsAggressive && npc.IsAvailable && npc.Brain is StandardMobBrain brain)
                        {
                            brain.CanBAF = false; // Mobs brought cannot bring friends of their own

                            GameLiving target;
                            if (victims != null && victims.Count > 0)
                                target = victims[UtilCollection.Random(0, victims.Count - 1)];
                            else
                                target = actualPuller;

                            brain.AddToAggroList(target, 0);
                            brain.AttackMostWanted();
                            numAdds++;
                        }
                    }// foreach

                    // Increase the range for finding friends to join the fight.
                    range *= 2;
                } // while
            } // if (maxAdds > 0)
        } // BringFriends()

        #endregion

        #region Spells

        public enum eCheckSpellType
        {
            Offensive,
            Defensive
        }

        /// <summary>
        /// Checks if any spells need casting
        /// </summary>
        /// <param name="type">Which type should we go through and check for?</param>
        public virtual bool CheckSpells(eCheckSpellType type)
        {
            bool casted = false;

            if (Body != null && Body.Spells != null && Body.Spells.Count > 0)
            {
                ArrayList spell_rec = new ArrayList();
                Spell spellToCast = null;
                bool needpet = false;
                bool needheal = false;

                if (type == eCheckSpellType.Defensive)
                {
                    foreach (Spell spell in Body.Spells)
                    {
                        if (Body.GetSkillDisabledDuration(spell) > 0) continue;
                        if (spell.Target.ToLower() == "enemy" || spell.Target.ToLower() == "area" || spell.Target.ToLower() == "cone") continue;
                        // If we have no pets
                        if (Body.ControlledBrain == null)
                        {
                            if (spell.SpellType == ESpellType.Pet) continue;

                            // TODO: Need to fix this bit
                            //if (spell.SpellType.ToLower().Contains("summon"))
                            //{
                            //	spell_rec.Add(spell);
                            //	needpet = true;
                            //}
                        }
                        if (Body.ControlledBrain != null && Body.ControlledBrain.Body != null)
                        {
                            if (UtilCollection.Chance(30) && Body.ControlledBrain != null && spell.SpellType == ESpellType.Heal &&
                                Body.GetDistanceTo(Body.ControlledBrain.Body) <= spell.Range &&
                                Body.ControlledBrain.Body.HealthPercent < DOL.GS.ServerProperties.ServerProperties.NPC_HEAL_THRESHOLD
                                && spell.Target.ToLower() != "self")
                            {
                                spell_rec.Add(spell);
                                needheal = true;
                            }
                            if (LivingHasEffect(Body.ControlledBrain.Body, spell) && (spell.Target.ToLower() != "self")) continue;
                        }
                        if (!needpet && !needheal)
                            spell_rec.Add(spell);
                    }
                    if (spell_rec.Count > 0)
                    {
                        spellToCast = (Spell)spell_rec[UtilCollection.Random((spell_rec.Count - 1))];
                        if (!Body.IsReturningToSpawnPoint)
                        {
                            if ((spellToCast.Uninterruptible || !Body.IsBeingInterrupted) && CheckDefensiveSpells(spellToCast))
                                casted = true;
                        }
                    }
                }
                else if (type == eCheckSpellType.Offensive)
                {
                    foreach (Spell spell in Body.Spells)
                    {
                        if (Body.GetSkillDisabledDuration(spell) == 0)
                        {
                            if (spell.CastTime > 0)
                            {
                                if (spell.Target.ToLower() == "enemy" || spell.Target.ToLower() == "area" || spell.Target.ToLower() == "cone")
                                    spell_rec.Add(spell);
                            }
                        }
                    }
                    if (spell_rec.Count > 0)
                    {
                        spellToCast = (Spell)spell_rec[UtilCollection.Random((spell_rec.Count - 1))];

                        if ((spellToCast.Uninterruptible || !Body.IsBeingInterrupted) && CheckOffensiveSpells(spellToCast))
                            casted = true;
                    }
                }
            }

            return casted || Body.IsCasting;
        }

        protected bool CanCastDefensiveSpell(Spell spell)
        {
            if (spell == null || spell.IsHarmful)
                return false;

            // Make sure we're currently able to cast the spell.
            if (spell.CastTime > 0 && Body.IsBeingInterrupted && !spell.Uninterruptible)
                return false;

            // Make sure the spell isn't disabled.
            if (spell.HasRecastDelay && Body.GetSkillDisabledDuration(spell) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks defensive spells. Handles buffs, heals, etc.
        /// </summary>
        protected virtual bool CheckDefensiveSpells(Spell spell)
        {
            if (!CanCastDefensiveSpell(spell))
                return false;

            bool casted = false;

            // clear current target, set target based on spell type, cast spell, return target to original target
            GameObject lastTarget = Body.TargetObject;

            Body.TargetObject = null;
            switch (spell.SpellType)
            {
                #region Buffs
                case ESpellType.AcuityBuff:
                case ESpellType.AFHitsBuff:
                case ESpellType.AllMagicResistBuff:
                case ESpellType.ArmorAbsorptionBuff:
                case ESpellType.ArmorFactorBuff:
                case ESpellType.BodyResistBuff:
                case ESpellType.BodySpiritEnergyBuff:
                case ESpellType.Buff:
                case ESpellType.CelerityBuff:
                case ESpellType.ColdResistBuff:
                case ESpellType.CombatSpeedBuff:
                case ESpellType.ConstitutionBuff:
                case ESpellType.CourageBuff:
                case ESpellType.CrushSlashTrustBuff:
                case ESpellType.DexterityBuff:
                case ESpellType.DexterityQuicknessBuff:
                case ESpellType.EffectivenessBuff:
                case ESpellType.EnduranceRegenBuff:
                case ESpellType.EnergyResistBuff:
                case ESpellType.FatigueConsumptionBuff:
                case ESpellType.FlexibleSkillBuff:
                case ESpellType.HasteBuff:
                case ESpellType.HealthRegenBuff:
                case ESpellType.HeatColdMatterBuff:
                case ESpellType.HeatResistBuff:
                case ESpellType.HeroismBuff:
                case ESpellType.KeepDamageBuff:
                case ESpellType.MagicResistBuff:
                case ESpellType.MatterResistBuff:
                case ESpellType.MeleeDamageBuff:
                case ESpellType.MesmerizeDurationBuff:
                case ESpellType.MLABSBuff:
                case ESpellType.PaladinArmorFactorBuff:
                case ESpellType.ParryBuff:
                case ESpellType.PowerHealthEnduranceRegenBuff:
                case ESpellType.PowerRegenBuff:
                case ESpellType.SavageCombatSpeedBuff:
                case ESpellType.SavageCrushResistanceBuff:
                case ESpellType.SavageDPSBuff:
                case ESpellType.SavageParryBuff:
                case ESpellType.SavageSlashResistanceBuff:
                case ESpellType.SavageThrustResistanceBuff:
                case ESpellType.SpiritResistBuff:
                case ESpellType.StrengthBuff:
                case ESpellType.StrengthConstitutionBuff:
                case ESpellType.SuperiorCourageBuff:
                case ESpellType.ToHitBuff:
                case ESpellType.WeaponSkillBuff:
                case ESpellType.DamageAdd:
                case ESpellType.OffensiveProc:
                case ESpellType.DefensiveProc:
                case ESpellType.DamageShield:
                    {
                        // Buff self, if not in melee, but not each and every mob
                        // at the same time, because it looks silly.
                        if (!LivingHasEffect(Body, spell) && !Body.attackComponent.AttackState && UtilCollection.Chance(40) && spell.Target.ToLower() != "pet")
                        {
                            Body.TargetObject = Body;
                            break;
                        }
                        if (Body.ControlledBrain != null && Body.ControlledBrain.Body != null && UtilCollection.Chance(40) && Body.GetDistanceTo(Body.ControlledBrain.Body) <= spell.Range && !LivingHasEffect(Body.ControlledBrain.Body, spell) && spell.Target.ToLower() != "self")
                        {
                            Body.TargetObject = Body.ControlledBrain.Body;
                            break;
                        }
                        break;
                    }
                #endregion Buffs

                #region Disease Cure/Poison Cure/Summon
                case ESpellType.CureDisease:
                    if (Body.IsDiseased)
                    {
                        Body.TargetObject = Body;
                        break;
                    }
                    if (Body.ControlledBrain != null && Body.ControlledBrain.Body != null && Body.ControlledBrain.Body.IsDiseased
                        && Body.GetDistanceTo(Body.ControlledBrain.Body) <= spell.Range && spell.Target.ToLower() != "self")
                    {
                        Body.TargetObject = Body.ControlledBrain.Body;
                        break;
                    }
                    break;
                case ESpellType.CurePoison:
                    if (LivingIsPoisoned(Body))
                    {
                        Body.TargetObject = Body;
                        break;
                    }
                    if (Body.ControlledBrain != null && Body.ControlledBrain.Body != null && LivingIsPoisoned(Body.ControlledBrain.Body)
                        && Body.GetDistanceTo(Body.ControlledBrain.Body) <= spell.Range && spell.Target.ToLower() != "self")
                    {
                        Body.TargetObject = Body.ControlledBrain.Body;
                        break;
                    }
                    break;
                case ESpellType.Summon:
                    Body.TargetObject = Body;
                    break;
                case ESpellType.SummonMinion:
                    //If the list is null, lets make sure it gets initialized!
                    if (Body.ControlledNpcList == null)
                        Body.InitControlledBrainArray(2);
                    else
                    {
                        //Let's check to see if the list is full - if it is, we can't cast another minion.
                        //If it isn't, let them cast.
                        IControlledBrain[] icb = Body.ControlledNpcList;
                        int numberofpets = 0;
                        for (int i = 0; i < icb.Length; i++)
                        {
                            if (icb[i] != null)
                                numberofpets++;
                        }
                        if (numberofpets >= icb.Length)
                            break;
                    }
                    Body.TargetObject = Body;
                    break;
                #endregion Disease Cure/Poison Cure/Summon

                #region Heals
                case ESpellType.CombatHeal:
                case ESpellType.Heal:
                case ESpellType.HealOverTime:
                case ESpellType.MercHeal:
                case ESpellType.OmniHeal:
                case ESpellType.PBAoEHeal:
                case ESpellType.SpreadHeal:
                    if (spell.Target.ToLower() == "self")
                    {
                        // if we have a self heal and health is less than 75% then heal, otherwise return false to try another spell or do nothing
                        if (Body.HealthPercent < DOL.GS.ServerProperties.ServerProperties.NPC_HEAL_THRESHOLD)
                        {
                            Body.TargetObject = Body;
                        }
                        break;
                    }

                    // Chance to heal self when dropping below 30%, do NOT spam it.
                    if (Body.HealthPercent < (DOL.GS.ServerProperties.ServerProperties.NPC_HEAL_THRESHOLD / 2.0)
                        && UtilCollection.Chance(10) && spell.Target.ToLower() != "pet")
                    {
                        Body.TargetObject = Body;
                        break;
                    }

                    if (Body.ControlledBrain != null && Body.ControlledBrain.Body != null
                        && Body.GetDistanceTo(Body.ControlledBrain.Body) <= spell.Range
                        && Body.ControlledBrain.Body.HealthPercent < DOL.GS.ServerProperties.ServerProperties.NPC_HEAL_THRESHOLD
                        && spell.Target.ToLower() != "self")
                    {
                        Body.TargetObject = Body.ControlledBrain.Body;
                        break;
                    }
                    break;
                #endregion

                //case "SummonAnimistFnF":
                //case "SummonAnimistPet":
                case ESpellType.SummonCommander:
                case ESpellType.SummonDruidPet:
                case ESpellType.SummonHunterPet:
                case ESpellType.SummonNecroPet:
                case ESpellType.SummonUnderhill:
                case ESpellType.SummonSimulacrum:
                case ESpellType.SummonSpiritFighter:
                    //case "SummonTheurgistPet":
                    if (Body.ControlledBrain != null)
                        break;
                    Body.TargetObject = Body;
                    break;

                default:
                    //log.Warn($"CheckDefensiveSpells() encountered an unknown spell type [{spell.SpellType}]");
                    break;
            }

            if (Body.TargetObject != null && (spell.Duration == 0 || (Body.TargetObject is GameLiving living && LivingHasEffect(living, spell) == false)))
                casted = Body.CastSpell(spell, m_mobSpellLine);

            Body.TargetObject = lastTarget;

            return casted;
        }

        /// <summary>
        /// Checks offensive spells.  Handles dds, debuffs, etc.
        /// </summary>
        protected virtual bool CheckOffensiveSpells(Spell spell)
        {
            if (spell.Target.ToLower() != "enemy" && spell.Target.ToLower() != "area" && spell.Target.ToLower() != "cone")
                return false;

            bool casted = false;

            if (Body.TargetObject is GameLiving living && (spell.Duration == 0 || (!LivingHasEffect(living,spell) || spell.SpellType == ESpellType.DirectDamageWithDebuff || spell.SpellType == ESpellType.DamageSpeedDecrease)))
            {
                // Offensive spells require the caster to be facing the target
                if (Body.TargetObject != Body)
                    Body.TurnTo(Body.TargetObject);

                casted = Body.CastSpell(spell, m_mobSpellLine);

                // if (casted && spell.CastTime > 0 && Body.IsMoving)
                //Stopfollowing if spell casted and the cast time > 0 (non-instant spells)
                if (casted && spell.CastTime > 0)
                    Body.StopFollowing();
                //If instant cast and spell casted, and current follow target is not the target object, then switch follow target to current TargetObject
                else if(casted && (spell.CastTime == 0 && Body.FollowTarget != Body.TargetObject))
                {
                    Body.Follow(Body.TargetObject, GameNpc.STICK_MINIMUM_RANGE, GameNpc.STICK_MAXIMUM_RANGE);
                }
            }
            return casted;
        }

        /// <summary>
        /// Checks Instant Spells.  Handles Taunts, shouts, stuns, etc.
        /// </summary>
        protected virtual bool CheckInstantSpells(Spell spell)
        {
            GameObject lastTarget = Body.TargetObject;
            Body.TargetObject = null;

            switch (spell.SpellType)
            {
                #region Enemy Spells
                case ESpellType.DirectDamage:
                case ESpellType.Lifedrain:
                case ESpellType.DexterityDebuff:
                case ESpellType.StrengthConstitutionDebuff:
                case ESpellType.CombatSpeedDebuff:
                case ESpellType.DamageOverTime:
                case ESpellType.MeleeDamageDebuff:
                case ESpellType.AllStatsPercentDebuff:
                case ESpellType.CrushSlashThrustDebuff:
                case ESpellType.EffectivenessDebuff:
                case ESpellType.Disease:
                case ESpellType.Stun:
                case ESpellType.Mez:
                case ESpellType.Taunt:
                    if (!LivingHasEffect(lastTarget as GameLiving, spell))
                    {
                        Body.TargetObject = lastTarget;
                    }
                    break;
                #endregion

                #region Combat Spells
                case ESpellType.CombatHeal:
                case ESpellType.DamageAdd:
                case ESpellType.ArmorFactorBuff:
                case ESpellType.DexterityQuicknessBuff:
                case ESpellType.EnduranceRegenBuff:
                case ESpellType.CombatSpeedBuff:
                case ESpellType.AblativeArmor:
                case ESpellType.Bladeturn:
                case ESpellType.OffensiveProc:
                    if (!LivingHasEffect(Body, spell))
                    {
                        Body.TargetObject = Body;
                    }
                    break;
                    #endregion
            }

            if (Body.TargetObject != null && (spell.Duration == 0 || (Body.TargetObject is GameLiving living && LivingHasEffect(living, spell) == false)))
            {
                Body.CastSpell(spell, m_mobSpellLine);
                Body.TargetObject = lastTarget;
                return true;
            }

            Body.TargetObject = lastTarget;
            return false;
        }

        protected static SpellLine m_mobSpellLine = SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells);

        /// <summary>
        /// Checks if the living target has a spell effect.
        /// Only to be used for spell casting purposes.
        /// </summary>
        /// <returns>True if the living has the effect of will receive it by our current spell.</returns>
        public bool LivingHasEffect(GameLiving target, Spell spell)
        {
            if (target == null)
                return true;

            /* all my homies hate vampires
            if (target is GamePlayer && (target as GamePlayer).CharacterClass.ID == (int)eCharacterClass.Vampiir)
            {
                switch (spell.SpellType)
                {
                    case eSpellType.StrengthConstitutionBuff:
                    case eSpellType.DexterityQuicknessBuff:
                    case eSpellType.StrengthBuff:
                    case eSpellType.DexterityBuff:
                    case eSpellType.ConstitutionBuff:
                    case eSpellType.AcuityBuff:
                        return true;
                }
            }*/

            ISpellHandler spellHandler = Body.castingComponent.SpellHandler;

            // If we're currently casting 'spell' on 'target', assume it already has the effect.
            // This allows spell queuing while preventing casting on the same target more than once.
            if (spellHandler != null && spellHandler.Spell.ID == spell.ID && spellHandler.Target == target)
                return true;

            // May not be the right place for that, but without that check NPCs with more than one offensive or defensive proc will only buff themselves once.
            if (spell.SpellType is ESpellType.OffensiveProc or ESpellType.DefensiveProc)
            {
                if (target.effectListComponent.Effects.TryGetValue(EffectService.GetEffectFromSpell(spell, m_mobSpellLine.IsBaseLine), out List<EcsGameEffect> existingEffects))
                {
                    if (existingEffects.FirstOrDefault(e => e.SpellHandler.Spell.ID == spell.ID || (spell.EffectGroup > 0 && e.SpellHandler.Spell.EffectGroup == spell.EffectGroup)) != null)
                        return true;
                }
            }
            else if (EffectListService.GetEffectOnTarget(target, EffectService.GetEffectFromSpell(spell, m_mobSpellLine.IsBaseLine)) != null)
                return true;

            return false;
        }

        protected bool LivingIsPoisoned(GameLiving target)
        {
            foreach (IGameEffect effect in target.EffectList)
            {
                //If the effect we are checking is not a gamespelleffect keep going
                if (effect is GameSpellEffect == false)
                    continue;

                GameSpellEffect speffect = effect as GameSpellEffect;

                // if this is a DOT then target is poisoned
                if (speffect.Spell.SpellType == ESpellType.DamageOverTime)
                    return true;
            }

            return false;
        }

        #endregion

        #region DetectDoor

        public virtual void DetectDoor()
        {
            ushort range = (ushort)((ThinkInterval / 800) * Body.CurrentWaypoint.MaxSpeed);

            foreach (GameDoorBase door in Body.CurrentRegion.GetDoorsInRadius(Body.X, Body.Y, Body.Z, range, false))
            {
                if (door is GameKeepDoor)
                {
                    if (Body.Realm != door.Realm) return;
                    door.Open();
                    //Body.Say("GameKeep Door is near by");
                    //somebody can insert here another action for GameKeep Doors
                    return;
                }
                else
                {
                    door.Open();
                    return;
                }
            }
            return;
        }
        #endregion
    }
}
