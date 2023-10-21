using System.Collections.Generic;
using Core.Database;
using Core.Database.Tables;
using Core.GS.Effects;
using Core.GS.Enums;
using Core.GS.PacketHandler;

namespace Core.GS.RealmAbilities
{
	public class NfRaWhirlingStaffAbility : Rr5RealmAbility
	{
		public NfRaWhirlingStaffAbility(DbAbility dba, int level) : base(dba, level) { }

		/// <summary>
		/// Action
		/// </summary>
		/// <param name="living"></param>
		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;

			SendCasterSpellEffectAndCastMessage(living, 7043, true);

			bool deactivate = false;
			foreach (GamePlayer player in living.GetPlayersInRadius(350))
			{
				if (GameServer.ServerRules.IsAllowedToAttack(living, player, true))
				{
					DamageTarget(player, living);
					deactivate = true;
				}
			}

			foreach (GameNpc npc in living.GetNPCsInRadius(350))
			{
				if (GameServer.ServerRules.IsAllowedToAttack(living, npc, true))
				{
					DamageTarget(npc, living);
					deactivate = true;
				}
			}
			if (deactivate)
				DisableSkill(living);
		}

		private void DamageTarget(GameLiving target, GameLiving caster)
		{
			int resist = 251 * target.GetResist(EDamageType.Crush) / -100;
			int damage = 251 + resist;

			GamePlayer player = caster as GamePlayer;
			if (player != null)
				player.Out.SendMessage("You hit " + target.Name + " for " + damage + "(" + resist + ") points of damage!", EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);

			GamePlayer targetPlayer = target as GamePlayer;
			if (targetPlayer != null)
			{
				if (targetPlayer.IsStealthed)
					targetPlayer.Stealth(false);
			}

			foreach (GamePlayer p in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				p.Out.SendSpellEffectAnimation(caster, target, 7043, 0, false, 1);
				p.Out.SendCombatAnimation(caster, target, 0, 0, 0, 0, 0x14, target.HealthPercent);
			}

			//target.TakeDamage(caster, eDamageType.Spirit, damage, 0);
			AttackData ad = new AttackData();
			ad.AttackResult = EAttackResult.HitUnstyled;
			ad.Attacker = caster;
			ad.Target = target;
			ad.DamageType = EDamageType.Crush;
			ad.Damage = damage;
			target.OnAttackedByEnemy(ad);
			caster.DealDamage(ad);

			if (target.EffectList.GetOfType<NfRaWhirlingStaffEffect>() == null)
			{
				NfRaWhirlingStaffEffect effect = new NfRaWhirlingStaffEffect();
				effect.Start(target);
			}

		}

		public override int GetReUseDelay(int level)
		{
			return 600;
		}

		public override void AddEffectsInfo(IList<string> list)
		{
			list.Add("A 350 radius PBAE attack that deals medium crushing damage and disarms your opponents for 6 seconds");
			list.Add("");
			list.Add("Radius: 350");
			list.Add("Target: Enemy");
			list.Add("Duration: 6 sec");
			list.Add("Casting time: instant");
		}

	}
}