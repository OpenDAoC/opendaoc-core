using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Rapid Fire ability
	/// </summary>
	[SkillHandlerAttribute(Abilities.RapidFire)]
	public class RapidFireAbilityHandler : IAbilityActionHandler
	{
		public void Execute(Ability ab, GamePlayer player)
		{

			RapidFireECSGameEffect rapidFire = (RapidFireECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.RapidFire);
			if (rapidFire!=null)
			{
				EffectService.RequestImmediateCancelEffect(rapidFire, false);
				return;
			}

			if(!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.RapidFire.CannotUseDead"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			SureShotECSGameEffect sureShot = (SureShotECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.SureShot);
			if (sureShot != null)
				EffectService.RequestImmediateCancelEffect(sureShot);

			TrueShotECSGameEffect trueshot = (TrueShotECSGameEffect)EffectListService.GetAbilityEffectOnTarget(player, eEffect.TrueShot);
			if (trueshot != null)
				EffectService.RequestImmediateCancelEffect(trueshot, false);

			ECSGameEffect volley = EffectListService.GetEffectOnTarget(player, eEffect.Volley);
			if (volley != null)
			{
				player.Out.SendMessage("You can't use "+ab.Name+" while Volley is active!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			new RapidFireECSGameEffect(new ECSGameEffectInitParams(player, 0, 1));
		}
	}
}
