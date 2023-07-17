using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
    /// Handler for Quick Cast Ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Quickcast)]
	public class QuickCastAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// The ability disable duration in milliseconds
		/// </summary>
		public const int DISABLE_DURATION = 30000;

		/// <summary>
		/// Executes the ability
		/// </summary>
		/// <param name="ab">The used ability</param>
		/// <param name="player">The player that used the ability</param>
		public void Execute(Ability ab, GamePlayer player)
		{									
			// Cannot change QC state if already casting a spell (can't turn it off!)
			if(player.CurrentSpellHandler != null)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.QuickCast.CannotUseIsCasting"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			QuickCastEcsEffect quickcast = (QuickCastEcsEffect)EffectListService.GetAbilityEffectOnTarget(player, EEffect.QuickCast);
			if (quickcast!=null)
			{
				quickcast.Cancel(true);
				return;
			}			

			// Dead can't quick cast
			if(!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.QuickCast.CannotUseDead"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			// Can't quick cast if in attack mode
			if(player.attackComponent.AttackState)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.QuickCast.CannotUseInMeleeCombat"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			long quickcastChangeTick = player.TempProperties.getProperty<long>(GamePlayer.QUICK_CAST_CHANGE_TICK);
			long changeTime = player.CurrentRegion.Time - quickcastChangeTick;
			if(changeTime < DISABLE_DURATION)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.QuickCast.CannotUseChangeTick", ((DISABLE_DURATION - changeTime) / 1000)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                //30 sec is time between 2 quick cast 
				return;
			}

			//TODO: more checks in this order

			//player.DisableSkill(ab,DURATION / 10);

			new QuickCastEcsEffect(new ECSGameEffectInitParams(player, QuickCastEcsEffect.DURATION, 1));
		}
	}
}
