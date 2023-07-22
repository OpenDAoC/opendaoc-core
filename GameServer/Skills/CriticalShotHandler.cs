using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Critical Shot ability
	/// </summary>
	[SkillHandlerAttribute(Abilities.Critical_Shot)]
	public class CriticalShotHandler : IAbilityActionHandler
	{
		public void Execute(AbilityUtil ab, GamePlayer player)
		{
			if (player.ActiveWeaponSlot != EActiveWeaponSlot.Distance)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUse.CriticalShot.NoRangedWeapons"), EChatType.CT_Important, EChatLoc.CL_SystemWindow);
                return;
			}
			if (player.IsSitting)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUse.CriticalShot.MustBeStanding"), EChatType.CT_YouHit, EChatLoc.CL_SystemWindow);
                return;
			}

			// cancel rapid fire effect
			RapidFireEcsEffect rapidFire = (RapidFireEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.RapidFire);
			if (rapidFire != null)
				EffectService.RequestImmediateCancelEffect(rapidFire, false);

			// cancel sure shot effect
			SureShotEcsEffect sureShot = (SureShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.SureShot);
			if (sureShot != null)
				EffectService.RequestImmediateCancelEffect(sureShot);

			TrueShotEcsEffect trueshot = (TrueShotEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.TrueShot);
			if (trueshot != null)
				EffectService.RequestImmediateCancelEffect(trueshot, false);

			EcsGameEffect volley = EffectListService.GetEffectOnTarget(player, EEffect.Volley);
			if (volley != null)
            {
				player.Out.SendMessage("You can't use Critical-Shot while Volley is active!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
				return;
			}

			if (player.attackComponent.AttackState)
			{
				if (player.rangeAttackComponent.RangedAttackType == ERangedAttackType.Critical)
				{
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CriticalShot.SwitchToRegular"), EChatType.CT_System, EChatLoc.CL_SystemWindow);
					player.rangeAttackComponent.RangedAttackType = ERangedAttackType.Normal;
				}
				else
				{
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CriticalShot.AlreadyFiring"), EChatType.CT_Important, EChatLoc.CL_SystemWindow);
				}
				return;
			}
			player.rangeAttackComponent.RangedAttackType = ERangedAttackType.Critical;
			player.attackComponent.RequestStartAttack(player.TargetObject);
		}
	}
}
