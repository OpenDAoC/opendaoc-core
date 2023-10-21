using System;
using System.Collections.Generic;
using Core.Database;
using Core.Database.Tables;
using Core.Events;
using Core.GS.ECS;
using Core.GS.Enums;
using Core.GS.Events;
using Core.GS.GameUtils;

namespace Core.GS.RealmAbilities
{
	public class NfRaDecimationTrapAbility : TimedRealmAbility
	{
		/*private ushort region;
		private int x;
		private int y;
		private int z;*/
		private Area.Circle traparea;
		private GameLiving owner;
		private EcsGameTimer ticktimer;
		private const int TICKS = 6;
		private int effectiveness;
		private ushort region;

		public NfRaDecimationTrapAbility(DbAbility dba, int level) : base(dba, level) { }

		/// <summary>
		/// Action
		/// </summary>
		/// <param name="living"></param>
		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;

			ushort Icon = 7026;
			effectiveness = 0;
			owner = living;

			if(ServerProperties.Properties.USE_NEW_ACTIVES_RAS_SCALING)
			{
				switch (Level)
				{
					case 1: effectiveness = 300; break;
					case 2: effectiveness = 450; break;
					case 3: effectiveness = 600; break;
					case 4: effectiveness = 750; break;
					case 5: effectiveness = 900; break;
				}
			}
			else
			{
				switch (Level)
				{
					case 1: effectiveness = 300; break;
					case 2: effectiveness = 600; break;
					case 3: effectiveness = 900; break;
				}
			}

			if (living.GroundTarget == null)
				return;
			if (!living.IsWithinRadius( living.GroundTarget, 1500 ))
				return;
			GamePlayer player = living as GamePlayer;
			if (player == null)
				return;

			if (player.RealmAbilityCastTimer != null)
			{
				player.RealmAbilityCastTimer.Stop();
				player.RealmAbilityCastTimer = null;
				player.Out.SendMessage("You cancel your Spell!", EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
			}

			foreach (GamePlayer p in living.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				p.Out.SendSpellCastAnimation(living, Icon, 20);

			player.RealmAbilityCastTimer = new EcsGameTimer(player);
			player.RealmAbilityCastTimer.Callback = new EcsGameTimer.EcsTimerCallback(startSpell);
			player.RealmAbilityCastTimer.Start(2000);
		}

		public override bool CheckRequirement(GamePlayer player)
		{
			return player.Level >= 40;
		}

		private int startSpell(EcsGameTimer timer)
		{
			if (!owner.IsAlive)
				return 0;

			traparea = new Area.Circle("decimation trap", owner.X, owner.Y, owner.Z, 50);

			owner.CurrentRegion.AddArea(traparea);
			region = owner.CurrentRegionID;

			GameEventMgr.AddHandler(traparea, AreaEvent.PlayerEnter, new CoreEventHandler(EventHandler));
			ticktimer = new EcsGameTimer(owner);
			ticktimer.Callback = new EcsGameTimer.EcsTimerCallback(onTick);
			ticktimer.Start(600000);
			getTargets();
			DisableSkill(owner);

			return 0;
		}

		private int onTick(EcsGameTimer timer)
		{
			removeHandlers();
			return 0;
		}

		protected void EventHandler(CoreEvent e, Object sender, EventArgs arguments)
		{
			AreaEventArgs args = arguments as AreaEventArgs;
			if (args == null)
				return;
			GameLiving living = args.GameObject as GameLiving;
			if (living == null)
				return;
			if (!GameServer.ServerRules.IsAllowedToAttack(owner, living, true))
				return;
			getTargets();
		}


		private void removeHandlers()
		{
			owner.CurrentRegion.RemoveArea(traparea);
			GameEventMgr.RemoveHandler(traparea, AreaEvent.PlayerEnter, new CoreEventHandler(EventHandler));
		}


		private void getTargets()
		{
			foreach (GamePlayer target in WorldMgr.GetPlayersCloseToSpot(region, traparea.X, traparea.Y, traparea.Z, 350))
			{
				if (GameServer.ServerRules.IsAllowedToAttack(owner, target, true))
				{
					DamageTarget(target);
				}
			}
			foreach (GameNpc target in WorldMgr.GetNPCsCloseToSpot(region, traparea.X, traparea.Y, traparea.Z, 350))
			{
				if (GameServer.ServerRules.IsAllowedToAttack(owner, target, true))
				{
					DamageTarget(target);
				}
			}
		}

		private void DamageTarget(GameLiving target)
		{
			if (!GameServer.ServerRules.IsAllowedToAttack(owner, target, true))
				return;
			if (!target.IsAlive)
				return;
			if (ticktimer.IsAlive)
			{
				ticktimer.Stop();
				removeHandlers();
			}
			int dist = target.GetDistanceTo( new Point3D( traparea.X, traparea.Y, traparea.Z ) );
			double mod = 1;
			if (dist > 0)
				mod = 1 - ((double)dist / 350);

			int basedamage = (int)(effectiveness * mod);
			int resist = (int)(basedamage * target.GetModified(EProperty.Resist_Energy) * -0.01);
			int damage = basedamage + resist;


			GamePlayer player = owner as GamePlayer;
			if (player != null)
			{
				player.Out.SendMessage("You hit " + target.Name + " for " + damage + "(" + resist + ") points of damage!", EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
			}

			GamePlayer targetPlayer = target as GamePlayer;
			if (targetPlayer != null)
			{
				if (targetPlayer.IsStealthed)
					targetPlayer.Stealth(false);
			}

			foreach (GamePlayer p in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				p.Out.SendSpellEffectAnimation(owner, target, 7026, 0, false, 1);
				p.Out.SendCombatAnimation(owner, target, 0, 0, 0, 0, 0x14, target.HealthPercent);
			}

			//target.TakeDamage(owner, eDamageType.Energy, damage, 0);
			AttackData ad = new AttackData();
			ad.AttackResult = EAttackResult.HitUnstyled;
			ad.Attacker = owner;
			ad.Target = target;
			ad.DamageType = EDamageType.Energy;
			ad.Damage = damage;
			target.OnAttackedByEnemy(ad);
			owner.DealDamage(ad);
		}

		public override int GetReUseDelay(int level)
		{
			return 900;
		}

		public override void AddEffectsInfo(IList<string> list)
		{
			if(ServerProperties.Properties.USE_NEW_ACTIVES_RAS_SCALING)
			{
				list.Add("Trap that deals the following damage in an 350 radius");
				list.Add("Level 1: 300 Damage");
				list.Add("Level 2: 450 Damage");
				list.Add("Level 3: 600 Damage");
				list.Add("Level 4: 750 Damage");
				list.Add("Level 5: 900 Damage");
				list.Add("");
				list.Add("Range 1500");
				list.Add("Target: Ground Target");
				list.Add("Radius: 350");
				list.Add("Casting time: 2 seconds");
			}
			else
			{
				list.Add("Trap that deals the following damage in an 350 radius");
				list.Add("Level 1: 300 Damage");
				list.Add("Level 2: 600 Damage");
				list.Add("Level 3: 900 Damage");
				list.Add("");
				list.Add("Range 1500");
				list.Add("Target: Ground Target");
				list.Add("Radius: 350");
				list.Add("Casting time: 2 seconds");
			}
		}
	}
}