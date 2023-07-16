using System;
using System.Collections.Generic;
using System.Reflection;

using DOL.Events;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using log4net;
using DOL.Language;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Sprint Ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Distraction)]
	public class DistractionAbilityHandler : IAbilityActionHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		/// <summary>
		/// The ability reuse time in milliseconds
		/// </summary>
		protected const int REUSE_TIMER = 10000;

		/// <summary>
		/// The ability effect duration in milliseconds
		/// </summary>
		public const int DURATION = 4000;

		private List<GameNPC> m_distractedNPCs = new List<GameNPC>();

		/// <summary>
		/// Execute dirtytricks ability
		/// </summary>
		/// <param name="ab">The used ability</param>
		/// <param name="player">The player that used the ability</param>
		public void Execute(Ability ab, GamePlayer player)
		{
			if (player == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not retrieve player in DistractionAbilityHandler.");
				return;
			}

			if (!player.IsAlive)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseDead"), eChatType.CT_YouHit, eChatLoc.CL_SystemWindow);
                return;
			}

			if (player.IsMezzed)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseMezzed"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}
			if (player.IsStunned)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStunned"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}
			if (player.IsSitting)
			{
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Skill.Ability.CannotUseStanding"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
			}

			var GameLoc = player.GroundTarget;
			if (GameLoc == null)
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "SummonAnimistPet.CheckBeginCast.GroundTargetNull"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (!player.GroundTargetInView)
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "SummonAnimistPet.CheckBeginCast.GroundTargetNotInView"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (GameLoc.GetDistance(player) > 750)
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "SummonAnimistPet.CheckBeginCast.GroundTargetNotInSpellRange"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			m_distractedNPCs = new List<GameNPC>();

			foreach (GameNPC npc in player.GetNPCsInRadius(1500))
			{
				if (npc.GetDistanceTo(GameLoc) < 400 && GameServer.ServerRules.IsAllowedToAttack(player, npc, true) && !(npc is GameTrainingDummy))
				{
					m_distractedNPCs.Add(npc);
				}
			}

			foreach (var distractedNpC in m_distractedNPCs)
			{
				distractedNpC.TurnTo(GameLoc.X, GameLoc.Y);
			}
			
			var DistractTimer = new ECSGameTimer(player, TurnBackToNormal, DURATION);
			DistractTimer.Start();

			player.DisableSkill(ab, REUSE_TIMER);
			//new DirtyTricksECSGameEffect(new ECSGameEffectInitParams(player, DURATION * 1000, 1));
		}
		
		protected virtual int TurnBackToNormal(ECSGameTimer timer)
		{
			foreach (var mDistractedNpC in m_distractedNPCs)
			{
				mDistractedNpC.TurnTo(mDistractedNpC.SpawnHeading);
			}
			m_distractedNPCs.Clear();
			return 0;
		}
	}
	
}
