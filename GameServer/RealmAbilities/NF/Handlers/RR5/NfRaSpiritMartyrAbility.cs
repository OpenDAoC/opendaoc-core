using System;
using System.Collections;
using Core.Database;
using Core.Database.Tables;
using Core.GS.Enums;
using Core.GS.PacketHandler;

namespace Core.GS.RealmAbilities
{
	public class NfRaSpiritMartyrAbility : Rr5RealmAbility
    {
		public NfRaSpiritMartyrAbility(DbAbility dba, int level) : base(dba, level) { }

        const int m_healRange = 2000;
        double m_healthpool = 3200;

		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;
			
			GamePlayer player = living as GamePlayer;
			if (player == null) return;
			if (player.ControlledBrain == null) return;
			if (player.ControlledBrain.Body == null) return;
			
			ArrayList targets = new ArrayList();
			//select targets
            if (player.Group == null)
            {
                if(player.Health < player.MaxHealth)
                  targets.Add(player);
            }
            else
            {
                foreach (GamePlayer tplayer in player.Group.GetPlayersInTheGroup())
                {
                    if (tplayer.IsAlive && player.IsWithinRadius(tplayer, m_healRange )
                        && tplayer.Health < tplayer.MaxHealth)
                        targets.Add(player);
                }
            }

			if (targets.Count == 0)
			{
				player.Out.SendMessage(((player.Group != null) ? "Your group is" : "You are") + " fully healed!", EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
				return;
			}

			//send spelleffect
			foreach (GamePlayer visPlayer in player.GetPlayersInRadius((ushort)WorldMgr.VISIBILITY_DISTANCE))
				visPlayer.Out.SendSpellEffectAnimation(player, player, 7075, 0, false, 0x01);

			int petHealthPercent = player.ControlledBrain.Body.HealthPercent;
			m_healthpool *= (petHealthPercent * 0.01);

            //[StephenxPimentel]
            //1.108 - This ability will no longer stun or sacrifice the pet.

			//player.ControlledBrain.Body.Die(player);

			int pool = (int)m_healthpool;
			while (pool > 0 && targets.Count > 0)
			{
				//get most injured player
				GamePlayer mostInjuredPlayer = null;
				int LowestHealthPercent = 100;
				foreach (GamePlayer tp in targets)
				{
					byte tpHealthPercent = tp.HealthPercent;
					if (tpHealthPercent < LowestHealthPercent)
					{
						LowestHealthPercent = tpHealthPercent;
						mostInjuredPlayer = tp;
					}
				}
				if (mostInjuredPlayer == null)
					break;
				//target has been healed
				targets.Remove(mostInjuredPlayer);
				int healValue = Math.Min(600, (mostInjuredPlayer.MaxHealth - mostInjuredPlayer.Health));
				healValue = Math.Min(healValue, pool);
                mostInjuredPlayer.ChangeHealth(player, EHealthChangeType.Spell, healValue);
			}

			DisableSkill(living);
        }
		
		public override int GetReUseDelay(int level)
		{
			return 600;
		}
    }
}
