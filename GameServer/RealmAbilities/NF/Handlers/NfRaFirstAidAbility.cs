using System.Collections.Generic;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.RealmAbilities
{
	public class NfRaFirstAidAbility : TimedRealmAbility
	{
		public NfRaFirstAidAbility(DbAbility dba, int level) : base(dba, level) { }

		/// <summary>
		/// Action
		/// </summary>
		/// <param name="living"></param>
		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED | INCOMBAT)) return;

			int heal = 0;
			
			if(ServerProperties.Properties.USE_NEW_ACTIVES_RAS_SCALING)
			{
				switch (Level)
				{
					case 1: heal = 25; break;
					case 2: heal = 40; break;
					case 3: heal = 60; break;
					case 4: heal = 80; break;
					case 5: heal = 100; break;
				}				
			}
			else
			{
				switch (Level)
				{
					case 1: heal = 25; break;
					case 2: heal = 60; break;
					case 3: heal = 100; break;
				}				
			}
			
			

			int healed = living.ChangeHealth(living, EHealthChangeType.Spell, living.MaxHealth * heal / 100);

			SendCasterSpellEffectAndCastMessage(living, 7001, healed > 0);

			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				if (healed > 0) player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FirstAidAbility.Execute.HealYourself", healed), EChatType.CT_Spell, EChatLoc.CL_SystemWindow);
				if (heal > healed)
				{
					if (healed == 0)
					{
						player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FirstAidAbility.Execute.AlreadyFullyHealed"), EChatType.CT_Spell, EChatLoc.CL_SystemWindow);
					}
					else
					{
						player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "FirstAidAbility.Execute.FullyHealed"), EChatType.CT_Spell, EChatLoc.CL_SystemWindow);
					}
				}
			}
			if (healed > 0) DisableSkill(living);
		}

		public override int GetReUseDelay(int level)
		{
			return 180;
		}
		
		public override int CostForUpgrade(int level)
		{
			switch (level)
			{
				case 1:
					return 3;
				case 2:
					return 6;
				case 3:
					return 10;
				default:	// default must return value for lvl 1
					return 3;
			}
		}

		public override void AddEffectsInfo(IList<string> list)
		{
			if(ServerProperties.Properties.USE_NEW_ACTIVES_RAS_SCALING)
			{
				//TODO Translate new descriptions
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info1"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info2"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info3"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info4"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info5"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info6"));				
			}
			else
			{
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info1"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info2"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info3"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info4"));
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info5"));
				list.Add("");
				list.Add(LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "FirstAidAbility.AddEffectsInfo.Info6"));
			}

		}
	}
}