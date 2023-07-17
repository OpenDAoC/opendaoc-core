using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DOL.Events;
using DOL.GS;
using DOL.GS.Effects;
using log4net;

namespace DOL.AI.Brain
{
	/// <summary>
	/// AI for dragon like NPCs.
	/// </summary>
	/// <author>Aredhel</author>
    public class DragonBrain : StandardMobBrain
    {
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		private long LastGlareUseTick;
		private int GlareUseCooldown = 30; //30 seconds?

        /// <summary>
        /// Create a new DragonBrain.
        /// </summary>
        public DragonBrain()
            : base()
        {
			FSM.ClearStates();

			FSM.Add(new DragonState_WakingUp(FSM, this));
			FSM.Add(new DragonState_ReturnToSpawn(FSM, this));
			FSM.Add(new DragonState_Idle(FSM, this));
			FSM.Add(new DragonState_Aggro(FSM, this));
			FSM.Add(new StandardMobState_Dead(FSM, this));

			FSM.SetCurrentState(EFsmStateType.WAKING_UP);
			LastGlareUseTick = 0;
        }

		/// <summary>
		/// The brain main loop. Do necessary health checks first and take
		/// any actions, if necessary. If everything's fine either pick a
		/// player to Glare at or to throw around.
		/// </summary>
        public override void Think()
        {
			FSM.Think();

			/*
			// Don't call base.Think() for now, I don't trust it...
			// While returning to the spawn point we don't need to think.

			if (Body.IsReturningHome) return;

			// When dragon regenerates to full health reset stage to 10.

			if (Body.HealthPercent == 100 && Stage < 10)
				Stage = 10;

			// If we aren't already aggroing something, look out for
			// someone we can aggro on and attack right away.

			if (!HasAggro && AggroLevel > 0)
			{
				CheckPlayerAggro();
				CheckNPCAggro();

				if (HasAggro)
				{
					AttackMostWanted();
					return;
				}
				else
				{
					if (Body.AttackState)
						Body.StopAttack();

					Body.TargetObject = null;
				}
			}
			
            // If dragon has run out of tether range, clear aggro list and let it 
            // return to its spawn point.
 
            if (CheckTether())
            {
				Body.StopFollowing();
				GameDragon dragon = Body as GameDragon;
				if (dragon != null)
				{
					dragon.PrepareToStun();
				}

                // ClearAggroList();
                Body.WalkToSpawn();
                return;
            }

			if (CheckHealth()) return;
			if (PickGlareTarget()) return;
			PickThrowTarget();
			*/
        }


		protected override void CheckNPCAggro()
		{
			if (Body.attackComponent.AttackState)
				return;

			foreach (GameNPC npc in Body.GetNPCsInRadius((ushort)AggroRange))
			{
				if (!npc.IsAlive || npc.ObjectState != GameObject.eObjectState.Active)
					continue;

				if (!GameServer.ServerRules.IsAllowedToAttack(Body, npc, true))
					continue;

				if (AggroTable.ContainsKey(npc))
					continue; // add only new NPCs

				if (npc.Brain != null && npc.Brain is IControlledBrain)
				{
					if (CanAggroTarget(npc))
					{
						AddToAggroList(npc, (npc.Level + 1) << 1);
					}
				}
			}
		}


		/// <summary>
		/// Called whenever the dragon's body sends something to its brain.
		/// </summary>
		/// <param name="e">The event that occured.</param>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The event details.</param>
		public override void Notify(CoreEvent e, object sender, EventArgs args)
		{
			base.Notify(e, sender, args);
			if (sender == Body)
			{
				GameDragon dragon = sender as GameDragon;
				if (e == GameObjectEvent.TakeDamage)
				{
					if (CheckHealth()) return;

					// Someone hit the dragon. If the attacker is in melee range, there
					// is a chance the dragon will cast a debuff specific to melee
					// classes on him, if not, well, dragon will try to get its Glare off...

					GameObject source = (args as TakeDamageEventArgs).DamageSource;
					if (source != null)
					{
						if (dragon.IsWithinRadius(source, dragon.AttackRange))
						{
							dragon.CheckMeleeDebuff(source as GamePlayer);
						}
						else
						{
							dragon.CheckGlare(source as GamePlayer);
						}
					}
					else
					{
						log.Error("Dragon takes damage from null source. args = " + (args == null ? "null" : args.ToString()));
					}
				}
				else if (e == GameLivingEvent.EnemyHealed)
				{
					// Someone healed an enemy. If the healer is in melee range, there
					// is a chance the dragon will cast a debuff specific to ranged
					// classes on him, if not, there's still Glare...

					GameObject source = (args as EnemyHealedEventArgs).HealSource;

					if (source != null)
					{
						if (dragon.IsWithinRadius(source, dragon.AttackRange))
						{
							dragon.CheckRangedDebuff(source as GamePlayer);
						}
						else
						{
							dragon.CheckGlare(source as GamePlayer);
						}
					}
					else
					{
						log.Error("Dragon heal source null. args = " + (args == null ? "null" : args.ToString()));
					}
				}
			}
			else if (e == GameNpcEvent.ArriveAtTarget && sender != null)
			{
				// Message from another NPC, such as a retriever,
				// for example.

				log.Info(String.Format("DragonBrain.Notify: ArriveAtTarget({0})", (sender as GameObject).Name));
				(Body as GameDragon).OnRetrieverArrived(sender as GameNPC);
			}
        }

        #region Tether

        /// <summary>
        /// Check whether dragon is out of tether range.
        /// </summary>
        /// <returns>True if dragon has reached the end of its tether.</returns>
        public bool CheckTether()
        {
            GameDragon dragon = Body as GameDragon;
            if (dragon == null) return false;
            return !dragon.IsWithinRadius( dragon.SpawnPoint, dragon.TetherRange );
        }

        #endregion

        #region Health Check

		private int m_stage = 10;

		/// <summary>
		/// This keeps track of the stage the encounter is in, so players
		/// don't have to go through all the PBAoE etc. again, just because
		/// the dragon regains a small amount of health. Starts at 10 (full
		/// health) and drops to 0.
		/// </summary>
		public int Stage
		{
			get { return m_stage; }
			set { if (value >= 0 && value <= 10) m_stage = value; }
		}

        /// <summary>
		/// Actions to be taken into consideration when health drops.
		/// </summary>
		/// <returns>Whether any action was taken.</returns>
		public bool CheckHealth()
		{
			GameDragon dragon = Body as GameDragon;
			if (dragon == null) return false;

			int healthOld = dragon.HealthPercentOld / 10;
			int healthNow = dragon.HealthPercent / 10;

			// Stun when health drops below 30%.

			if (healthNow < 3)
			{
				if (dragon.CheckStun(healthNow < healthOld))
					return true;
			}

			if (healthNow < healthOld && Stage > healthNow)
			{
				Stage = healthNow;

				// Breathe at 89%/79%/69%/49% and 9%.

				switch (healthNow)
				{
					case 8:
					case 7:
					case 6:
					case 4:
					case 0: if (dragon.CheckBreath())
							return true;
						break;
				}

				// Spawn adds at 49% and 19% (hunch).

				switch (healthNow)
				{
					case 5:
					case 3: if (dragon.CheckAddSpawns())
							return true;
						break;
				}
			}
			return false;
		}

		#endregion

		#region Glare

		public bool GlareAvailable => LastGlareUseTick + GlareUseCooldown * 1000 < GameLoop.GameLoopTime;

		/// <summary>
		/// Try to find a potential target for Glare.
		/// </summary>
		/// <returns>Whether or not a target was picked.</returns>
		public bool PickGlareTarget()
		{
			if (!GlareAvailable) return false; //if we're on cooldown, return
			
			GameDragon dragon = Body as GameDragon;
			if (dragon == null) return false;

			ArrayList inRangeLiving = new ArrayList();

			lock ((AggroTable as ICollection).SyncRoot)
            {
				Dictionary<GameLiving, long>.Enumerator enumerator = AggroTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameLiving living = enumerator.Current.Key;
					if (living != null && 
						living.IsAlive &&
						living.EffectList.GetOfType<NecromancerShadeEffect>() == null && 
						!dragon.IsWithinRadius(living, dragon.AttackRange))
					{
						inRangeLiving.Add(living);
					}
				}
            }

			if (inRangeLiving.Count > 0)
			{
				return dragon.CheckGlare((GameLiving)(inRangeLiving[UtilCollection.Random(1, inRangeLiving.Count) - 1]));
			}

			return false;
		}

		#endregion

        #region Throw

		/// <summary>
		/// Pick a target to hurl into the air.
		/// </summary>
		/// <returns>Whether or not a target was picked.</returns>
		public bool PickThrowTarget()
		{
			GameDragon dragon = Body as GameDragon;
			if (dragon == null) return false;

			ArrayList inRangeLiving = new ArrayList();
			foreach (GamePlayer player in dragon.GetPlayersInRadius((ushort)dragon.AttackRange))
			{
				if (player.IsAlive && player.EffectList.GetOfType<NecromancerShadeEffect>() == null)
				{
					inRangeLiving.Add(player);
				}
			}

			foreach (GameNPC npc in dragon.GetNPCsInRadius((ushort)dragon.AttackRange))
			{
				if (npc.IsAlive && npc.Brain != null && npc.Brain is IControlledBrain)
				{
					inRangeLiving.Add(npc);
				}
			}

			if (inRangeLiving.Count > 0)
			{
				return dragon.CheckThrow((GameLiving)(inRangeLiving[UtilCollection.Random(1, inRangeLiving.Count) - 1]));
			}

			return false;
		}

        #endregion
    }
}
