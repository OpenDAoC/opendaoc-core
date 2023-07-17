using System.Reflection;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using log4net;
using DOL.Language;
using System.Linq;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Sprint Ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Engage)]
	public class EngageAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// wait 5 sec to engage after attack
		/// </summary>
		public const int ENGAGE_ATTACK_DELAY_TICK = 5000;

		/// <summary>
		/// Endurance lost on every attack
		/// </summary>
		public const int ENGAGE_DURATION_LOST = 15;

		/// <summary>
		/// Execute engage ability
		/// </summary>
		/// <param name="ab">The used ability</param>
		/// <param name="player">The player that used the ability</param>
		public void Execute(AbilityUtil ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in EngageAbilityHandler.");
				return;
			}

			//Cancel old engage effects on player
			if (player.IsEngaging)
			{
				EngageEcsEffect engage = EffectListService.GetEffectOnTarget(player, EEffect.Engage) as EngageEcsEffect;
				if (engage != null)
				{
					engage.Cancel(true);
					return;
				}
			}

			if (!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.CannotUseDead"), eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
                return;
			}

			if (player.IsSitting)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.CannotUseStanding"), eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
                return;
			}

			if (player.ActiveWeaponSlot == EActiveWeaponSlot.Distance)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.CannotUseNoCaCWeapons"), eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
                return;
			}

			GameLiving target = player.TargetObject as GameLiving;
			if (target == null)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.NoTarget"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			// You cannot engage a mob that was attacked within the last 5 seconds...
			if (target.LastAttackedByEnemyTick > GameLoop.GameLoopTime - EngageAbilityHandler.ENGAGE_ATTACK_DELAY_TICK)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.TargetAttackedRecently", target.GetName(0, true)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			if (!GameServer.ServerRules.IsAllowedToAttack(player, target, true))
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Engage.NotAllowedToEngageTarget", target.GetName(0, false)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			new EngageEcsEffect(new ECSGameEffectInitParams(player, 0, 1, null));
		}
	}
}