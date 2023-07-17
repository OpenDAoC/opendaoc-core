using System.Reflection;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using log4net;
using DOL.Language;
using System.Linq;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Intercept ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Intercept)]
	public class InterceptAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The intercept distance
		/// </summary>
		public const int INTERCEPT_DISTANCE = 128;

		/// <summary>
		/// Intercept reuse timer in milliseconds
		/// </summary>
		public const int REUSE_TIMER = 60 * 1000;

		/// <summary>
		/// Executes the ability
		/// </summary>
		/// <param name="ab">The ability used</param>
		/// <param name="player">The player that used the ability</param>
		public void Execute(Ability ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in InterceptAbilityHandler.");
				return;
			}

			GameObject targetObject = player.TargetObject;
			if (targetObject == null)
			{
				//foreach (InterceptEffect intercept in player.EffectList.GetAllOfType<InterceptEffect>())
				foreach (InterceptEcsEffect intercept in player.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Intercept))
				{
					if (intercept.InterceptSource != player)
						continue;
					intercept.Cancel(false);
				}
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Intercept.CancelTargetNull"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			// Only attacks on other players may be intercepted. 
			// You cannot intercept attacks on yourself            
			Group group = player.Group;
			GamePlayer interceptTarget = targetObject as GamePlayer;
			if (interceptTarget == null || group == null || !group.IsInTheGroup(interceptTarget) || interceptTarget == player)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Intercept.CannotUse.NotInGroup"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			// check if someone is already intercepting for that target
			//foreach (InterceptEffect intercept in interceptTarget.EffectList.GetAllOfType<InterceptEffect>())
			foreach (InterceptEcsEffect intercept in interceptTarget.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Intercept))
			{
				if (intercept.InterceptTarget != interceptTarget)
					continue;
				if (intercept.InterceptSource != player && !(intercept.InterceptSource is GameNPC))
				{
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Intercept.CannotUse.InterceptTargetAlreadyInterceptedEffect", intercept.InterceptSource.GetName(0, true), intercept.InterceptTarget.GetName(0, false)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                    return;
				}
			}

			// cancel all intercepts by this player
			//foreach (InterceptEffect intercept in player.EffectList.GetAllOfType<InterceptEffect>())
			foreach (InterceptEcsEffect intercept in player.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Intercept))
			{
				if (intercept.InterceptSource != player)
					continue;
				intercept.Cancel(false);
			}

			player.DisableSkill(ab, REUSE_TIMER);

			//new InterceptEffect().Start(player, interceptTarget);
			new InterceptEcsEffect(new ECSGameEffectInitParams(player, 0, 1), player, (GameLiving)player.TargetObject);
		}
	}
}