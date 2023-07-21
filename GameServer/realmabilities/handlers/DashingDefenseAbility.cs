using System.Reflection;
using System.Collections;
using DOL.Database;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Language;
using System.Collections.Generic;

namespace DOL.GS.RealmAbilities
{
    public class DashingDefenseAbility : TimedRealmAbility
    {
        public DashingDefenseAbility(DbAbilities dba, int level) : base(dba, level) { }

        public const string Dashing = "Dashing";

        //private RegionTimer m_expireTimer;
        int m_duration = 1;
        int m_range = 1000;
        //private GamePlayer m_player;

        public override void Execute(GameLiving living)
        {
            GamePlayer player = living as GamePlayer;
            if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;
            if (player.TempProperties.getProperty(Dashing, false))
            {
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "DashingDefenseAbility.Execute.AlreadyEffect"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
				return;
            }
			
            if(ServerProperties.ServerProperties.USE_NEW_ACTIVES_RAS_SCALING)
            {
 	            switch (Level)
	            {
	                case 1: m_duration = 10; break;
	                case 2: m_duration = 20; break;
	                case 3: m_duration = 30; break;
	                case 4: m_duration = 45; break;
	                case 5: m_duration = 60; break;
	                default: return;
	            }            	           	
            }
            else
            {
	            switch (Level)
	            {
	                case 1: m_duration = 10; break;
	                case 2: m_duration = 30; break;
	                case 3: m_duration = 60; break;
	                default: return;
	            }            	
            }

            DisableSkill(living);

            ArrayList targets = new ArrayList();
            if (player.Group == null)
                {
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "DashingDefenseAbility.Execute.MustInGroup"), EChatType.CT_SpellResisted, EChatLoc.CL_SystemWindow);
					return;
                }
            else foreach (GamePlayer grpMate in player.Group.GetPlayersInTheGroup())
                    if (player.IsWithinRadius(grpMate, m_range) && grpMate.IsAlive)
                        targets.Add(grpMate);

            bool success;
            foreach (GamePlayer target in targets)
            {
                //send spelleffect
                if (!target.IsAlive) continue;
                success = !target.TempProperties.getProperty(Dashing, false);
                if (success)
                    if (target != null && target != player)
                    {
                        new DashingDefenseEffect().Start(player, target, m_duration);
                    }
            }

        }

        public override int GetReUseDelay(int level)
        {
            return 420;
        }

		public override void AddEffectsInfo(IList<string> list)
		{
			//TODO Translate
			if(ServerProperties.ServerProperties.USE_NEW_ACTIVES_RAS_SCALING)
			{
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info1"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info2"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info3"));
			}
			else
			{
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info1"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info2"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "DashingDefenseAbility.AddEffectsInfo.Info3"));
			}
		}
    }
}
