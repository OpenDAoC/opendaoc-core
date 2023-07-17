using System;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Stealth Spec clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Camouflage)]
	public class CamouflageSpecHandler : IAbilityActionHandler
	{
		public const int DISABLE_DURATION = 300000; //atlas 5min cooldown

		/// <summary>
		/// Executes the stealth ability
		/// </summary>
		/// <param name="ab"></param>
		/// <param name="player"></param>
		public void Execute(Ability ab, GamePlayer player)
		{
			if (!player.IsStealthed)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUse.Camouflage.NotStealthed"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}
			 
			CamouflageEcsEffect camouflage = (CamouflageEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.Camouflage);
			
			if (camouflage != null)
			{				
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Camouflage.UseCamo"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			
			new CamouflageEcsEffect(new ECSGameEffectInitParams(player, 0, 1));
			player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.Camouflage.UseCamo"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
		}
	}
}