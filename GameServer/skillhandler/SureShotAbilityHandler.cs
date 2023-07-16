using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Sure Shot ability
	/// </summary>
	[SkillHandlerAttribute(Abilities.SureShot)]
	public class SureShotAbilityHandler : IAbilityActionHandler
	{
		public void Execute(Ability ab, GamePlayer player)
		{
			SureShotECSGameEffect sureShot = (SureShotECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.SureShot);
			if (sureShot != null)
			{
				EffectService.RequestImmediateCancelEffect(sureShot);
				return;
			}

			if (!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.SureShot.CannotUseDead"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			RapidFireECSGameEffect rapidFire = (RapidFireECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.RapidFire);
			if (rapidFire != null)
				EffectService.RequestImmediateCancelEffect(rapidFire, false);

			TrueShotECSGameEffect trueshot = (TrueShotECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.TrueShot);
			if (trueshot != null)
				EffectService.RequestImmediateCancelEffect(trueshot, false);

			new SureShotECSGameEffect(new ECSGameEffectInitParams(player, 0, 1));
		}
	}
}
