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
			SureShotEcsEffect sureShot = (SureShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.SureShot);
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

			RapidFireEcsEffect rapidFire = (RapidFireEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.RapidFire);
			if (rapidFire != null)
				EffectService.RequestImmediateCancelEffect(rapidFire, false);

			TrueShotEcsEffect trueshot = (TrueShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.TrueShot);
			if (trueshot != null)
				EffectService.RequestImmediateCancelEffect(trueshot, false);

			new SureShotEcsEffect(new ECSGameEffectInitParams(player, 0, 1));
		}
	}
}
