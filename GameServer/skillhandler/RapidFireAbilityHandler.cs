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
		public void Execute(AbilityUtil ab, GamePlayer player)
		{

			RapidFireEcsEffect rapidFire = (RapidFireEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.RapidFire);
			if (rapidFire!=null)
			{
				EffectService.RequestImmediateCancelEffect(rapidFire, false);
				return;
			}

			if(!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.RapidFire.CannotUseDead"), EChatType.CT_System, EChatLoc.CL_SystemWindow);
                return;
			}

			SureShotEcsEffect sureShot = (SureShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.SureShot);
			if (sureShot != null)
				EffectService.RequestImmediateCancelEffect(sureShot);

			TrueShotEcsEffect trueshot = (TrueShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.TrueShot);
			if (trueshot != null)
				EffectService.RequestImmediateCancelEffect(trueshot, false);

			ECSGameEffect volley = EffectListService.GetEffectOnTarget(player, EEffect.Volley);
			if (volley != null)
			{
				player.Out.SendMessage("You can't use "+ab.Name+" while Volley is active!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				return;
			}

			new RapidFireEcsEffect(new ECSGameEffectInitParams(player, 0, 1));
		}
	}
}
