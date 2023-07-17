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
	/// Handler for Triple Wield clicks
	/// </summary>
	[SkillHandler(Abilities.Triple_Wield)]
	public class TripleWieldAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The ability reuse time in seconds
		/// </summary>
		protected const int REUSE_TIMER = 7 * 60;

		/// <summary>
		/// The ability effect duration in seconds
		/// </summary>
		public const int DURATION = 30;

		public void Execute(AbilityUtil ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in TripleWieldAbilityHandler.");
				return;
			}

            if (!player.IsAlive)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseDead"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            if (player.IsMezzed)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseMezzed"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            if (player.IsStunned)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStunned"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            if (player.IsSitting)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStanding"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
			TripleWieldEcsEffect tw = (TripleWieldEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.TripleWield);
			if (tw != null)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseAlreadyActive"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}
			new TripleWieldEcsEffect(new ECSGameEffectInitParams(player, DURATION * 1000, 1));

			player.DisableSkill(ab, REUSE_TIMER * 1000);
		}
	}
}
