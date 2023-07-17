using System.Reflection;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using log4net;
using DOL.Language;
using System.Linq;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for protect ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Protect)]
	public class ProtectAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The Protect Distance
		/// </summary>
		public const int PROTECT_DISTANCE = 1000;

		public void Execute(AbilityUtil ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in ProtectAbilityHandler.");
				return;
			}

			GameObject targetObject = player.TargetObject;
			if (targetObject == null)
			{
				foreach (ProtectEcsEffect protect in player.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Protect))
				{
					if (protect.ProtectSource == player)
						protect.Cancel(false);
				}
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Protect.CancelTargetNull"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			// You cannot protect attacks on yourself            
			GamePlayer protectTarget = player.TargetObject as GamePlayer;
			if (protectTarget == player)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Protect.CannotUse.CantProtectYourself"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			// Only attacks on other players may be protected. 
			// protect may only be used on other players in group
			Group group = player.Group;
			if (protectTarget == null || group == null || !group.IsInTheGroup(protectTarget))
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Protect.CannotUse.NotInGroup"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			// check if someone is protecting the target
			foreach (ProtectEcsEffect protect in protectTarget.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Protect))
			{
				if (protect.ProtectTarget != protectTarget)
					continue;
				if (protect.ProtectSource == player)
				{
					protect.Cancel(false);
					return;
				}
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Protect.CannotUse.ProtectTargetAlreadyProtectEffect", protect.ProtectSource.GetName(0, true), protect.ProtectTarget.GetName(0, false)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			// cancel all guard effects by this player before adding a new one
			foreach (ProtectEcsEffect protect in player.effectListComponent.GetAbilityEffects().Where(e => e.EffectType == EEffect.Protect))
			{
				if (protect.ProtectSource == player)
					protect.Cancel(false);
			}

			//new ProtectEffect().Start(player, protectTarget);
			new ProtectEcsEffect(new ECSGameEffectInitParams(player, 0, 1), player, protectTarget);
		}
	}
}