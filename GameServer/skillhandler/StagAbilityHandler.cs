using System;
using System.Collections;
using System.Reflection;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Events;
using log4net;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Stag Ability clicks
	/// </summary>
	[SkillHandler(Abilities.Stag)]
	public class StagAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The ability reuse time in milliseconds
		/// </summary>
		protected const int REUSE_TIMER = 60000 * 15; // clait: 15 minutes [og: 20]

		/// <summary>
		/// The ability effect duration in milliseconds
		/// </summary>
		public const int DURATION = 30 * 1000; // 30 seconds

		public void Execute(Ability ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in StagAbilityHandler.");
				return;
			}

            if (!player.IsAlive)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseDead"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            //if (player.IsMezzed)
            //{
            //    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseMezzed"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
            //    return;
            //}
            //if (player.IsStunned)
            //{
            //    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStunned"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
            //    return;
            //}
            if (player.IsSitting)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStanding"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
			//Cancel old stag effects on player
			StagECSGameEffect stag = (StagECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.Stag);
			if (stag != null)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseAlreadyActive"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}
			player.DisableSkill(ab, REUSE_TIMER);

			new StagECSGameEffect(new ECSGameEffectInitParams(player, DURATION, 1), ab.Level);
		}
	}
}
