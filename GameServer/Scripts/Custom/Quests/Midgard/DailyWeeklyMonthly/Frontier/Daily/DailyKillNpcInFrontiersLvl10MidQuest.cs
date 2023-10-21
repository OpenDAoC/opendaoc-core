using System;
using System.Reflection;
using Core.Database;
using Core.Database.Tables;
using Core.Events;
using Core.GS.Enums;
using Core.GS.PacketHandler;
using Core.GS.Quests;
using log4net;

namespace Core.GS.DailyQuest.Hibernia
{
	public class DailyKillNpcInFrontiersLvl10MidQuest : Quests.DailyQuest
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected const string questTitle = "[Daily] A Bit of Bravery";
		protected const int minimumLevel = 10;
		protected const int maximumLevel = 50;

		// Kill Goal
		protected const int MAX_KILLED = 25;
		
		private static GameNpc Herou = null; // Start NPC

		private int FrontierMobsKilled = 0;

		// Constructors
		public DailyKillNpcInFrontiersLvl10MidQuest() : base()
		{
		}

		public DailyKillNpcInFrontiersLvl10MidQuest(GamePlayer questingPlayer) : base(questingPlayer)
		{
		}

		public DailyKillNpcInFrontiersLvl10MidQuest(GamePlayer questingPlayer, int step) : base(questingPlayer, step)
		{
		}

		public DailyKillNpcInFrontiersLvl10MidQuest(GamePlayer questingPlayer, DbQuest dbQuest) : base(questingPlayer, dbQuest)
		{
		}
		
		public override int Level
		{
			get
			{
				// Quest Level
				return minimumLevel;
			}
		}

		[ScriptLoadedEvent]
		public static void ScriptLoaded(CoreEvent e, object sender, EventArgs args)
		{
			if (!ServerProperties.Properties.LOAD_QUESTS)
				return;
			

			#region defineNPCs

			GameNpc[] npcs = WorldMgr.GetNPCsByName("Herou", ERealm.Midgard);

			if (npcs.Length > 0)
				foreach (GameNpc npc in npcs)
					if (npc.CurrentRegionID == 100 && npc.X == 766401 && npc.Y == 670349)
					{
						Herou = npc;
						break;
					}

			if (Herou == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Herou , creating it ...");
				Herou = new GameNpc();
				Herou.Model = 142;
				Herou.Name = "Herou";
				Herou.GuildName = "Realm Logistics";
				Herou.Realm = ERealm.Midgard;
				//Svasud Faste Location
				Herou.CurrentRegionID = 100;
				Herou.Size = 50;
				Herou.Level = 59;
				Herou.X = 766401;
				Herou.Y = 670349;
				Herou.Z = 5736;
				Herou.Heading = 2835;
				Herou.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Herou.SaveIntoDatabase();
				}
			}

			#endregion

			#region defineItems
			#endregion

			#region defineObject
			#endregion

			GameEventMgr.AddHandler(GamePlayerEvent.AcceptQuest, new CoreEventHandler(SubscribeQuest));
			GameEventMgr.AddHandler(GamePlayerEvent.DeclineQuest, new CoreEventHandler(SubscribeQuest));

			GameEventMgr.AddHandler(Herou, GameObjectEvent.Interact, new CoreEventHandler(TalkToHerou));
			GameEventMgr.AddHandler(Herou, GameLivingEvent.WhisperReceive, new CoreEventHandler(TalkToHerou));

			/* Now we bring to Herou the possibility to give this quest to players */
			Herou.AddQuestToGive(typeof (DailyKillNpcInFrontiersLvl10MidQuest));

			if (log.IsInfoEnabled)
				log.Info("Quest \"" + questTitle + "\" initialized");
		}

		[ScriptUnloadedEvent]
		public static void ScriptUnloaded(CoreEvent e, object sender, EventArgs args)
		{
			//if not loaded, don't worry
			if (Herou == null)
				return;
			// remove handlers
			GameEventMgr.RemoveHandler(GamePlayerEvent.AcceptQuest, new CoreEventHandler(SubscribeQuest));
			GameEventMgr.RemoveHandler(GamePlayerEvent.DeclineQuest, new CoreEventHandler(SubscribeQuest));

			GameEventMgr.RemoveHandler(Herou, GameObjectEvent.Interact, new CoreEventHandler(TalkToHerou));
			GameEventMgr.RemoveHandler(Herou, GameLivingEvent.WhisperReceive, new CoreEventHandler(TalkToHerou));

			/* Now we remove to Herou the possibility to give this quest to players */
			Herou.RemoveQuestToGive(typeof (DailyKillNpcInFrontiersLvl10MidQuest));
		}

		protected static void TalkToHerou(CoreEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			if(Herou.CanGiveQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest), player)  <= 0)
				return;

			//We also check if the player is already doing the quest
			DailyKillNpcInFrontiersLvl10MidQuest quest = player.IsDoingQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest)) as DailyKillNpcInFrontiersLvl10MidQuest;

			if (e == GameObjectEvent.Interact)
			{
				if (quest != null)
				{
					switch (quest.Step)
					{
						case 1:
							Herou.SayTo(player, "Kill creatures in any RvR zone to help us clear more room for the armies to maneuver around.");
							break;
						case 2:
							Herou.SayTo(player, "Hello " + player.Name + ", did you [tidy the realm]?");
							break;
					}
				}
				else
				{
					Herou.SayTo(player, "Hello "+ player.Name +", I am Herou, Fen\'s friend. I serve the realm and ensure its borders are always protected. "+
					                    "I heard you are strong. Do you think you're strong enough to help me with some trouble we've been having? \n\n"+
					                    "I need an adventurer to help me [clear the frontiers].");
				}
			}
				// The player whispered to the NPC
			else if (e == GameLivingEvent.WhisperReceive)
			{
				WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
				if (quest == null)
				{
					switch (wArgs.Text)
					{
						case "clear the frontiers":
							player.Out.SendQuestSubscribeCommand(Herou, QuestMgr.GetIDForQuestType(typeof(DailyKillNpcInFrontiersLvl10MidQuest)), "Will you help Herou "+questTitle+"");
							break;
					}
				}
				else
				{
					switch (wArgs.Text)
					{
						case "tidy the realm":
							if (quest.Step == 2)
							{
								player.Out.SendMessage("Thank you for your contribution!", EChatType.CT_Chat, EChatLoc.CL_PopupWindow);
								quest.FinishQuest();
							}
							break;
						case "abort":
							player.Out.SendCustomDialog("Do you really want to abort this quest, \nall items gained during quest will be lost?", new CustomDialogResponse(CheckPlayerAbortQuest));
							break;
					}
				}
			}
		}
		
		public override bool CheckQuestQualification(GamePlayer player)
		{
			// if the player is already doing the quest his level is no longer of relevance
			if (player.IsDoingQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest)) != null)
				return true;

			// This checks below are only performed is player isn't doing quest already

			//if (player.HasFinishedQuest(typeof(Academy_47)) == 0) return false;

			//if (!CheckPartAccessible(player,typeof(CityOfCamelot)))
			//	return false;

			if (player.Level < minimumLevel || player.Level > maximumLevel)
				return false;

			return true;
		}
		
		public override void LoadQuestParameters()
		{
			FrontierMobsKilled = GetCustomProperty(QuestPropertyKey) != null ? int.Parse(GetCustomProperty(QuestPropertyKey)) : 0;
		}

		public override void SaveQuestParameters()
		{
			SetCustomProperty(QuestPropertyKey, FrontierMobsKilled.ToString());
		}


		private static void CheckPlayerAbortQuest(GamePlayer player, byte response)
		{
			DailyKillNpcInFrontiersLvl10MidQuest quest = player.IsDoingQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest)) as DailyKillNpcInFrontiersLvl10MidQuest;

			if (quest == null)
				return;

			if (response == 0x00)
			{
				SendSystemMessage(player, "Good, now go out there and finish your work!");
			}
			else
			{
				SendSystemMessage(player, "Aborting Quest " + questTitle + ". You can start over again if you want.");
				quest.AbortQuest();
			}
		}

		protected static void SubscribeQuest(CoreEvent e, object sender, EventArgs args)
		{
			QuestEventArgs qargs = args as QuestEventArgs;
			if (qargs == null)
				return;

			if (qargs.QuestID != QuestMgr.GetIDForQuestType(typeof(DailyKillNpcInFrontiersLvl10MidQuest)))
				return;

			if (e == GamePlayerEvent.AcceptQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x01);
			else if (e == GamePlayerEvent.DeclineQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x00);
		}

		private static void CheckPlayerAcceptQuest(GamePlayer player, byte response)
		{
			if(Herou.CanGiveQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest), player)  <= 0)
				return;

			if (player.IsDoingQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest)) != null)
				return;

			if (response == 0x00)
			{
				player.Out.SendMessage("Thank you for helping our realm.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
			}
			else
			{
				//Check if we can add the quest!
				if (!Herou.GiveQuest(typeof (DailyKillNpcInFrontiersLvl10MidQuest), player, 1))
					return;

				Herou.SayTo(player, "Killing creatures in any RvR zone will work. Thanks for your service!");

			}
		}

		//Set quest name
		public override string Name
		{
			get { return questTitle; }
		}

		// Define Steps
		public override string Description
		{
			get
			{
				switch (Step)
				{
					case 1:
						return "Kill yellow con or higher mobs in any RvR zone. \nKilled: ("+ FrontierMobsKilled +" | "+MAX_KILLED+")";
					case 2:
						return "Return to Herou in Svasud Faste for your Reward.";
				}
				return base.Description;
			}
		}

		public override void Notify(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = sender as GamePlayer;
			
			if (player?.IsDoingQuest(typeof(DailyKillNpcInFrontiersLvl10MidQuest)) == null)
				return;
			
			if (sender != m_questPlayer)
				return;

			if (e != GameLivingEvent.EnemyKilled || Step != 1) return;
			
			EnemyKilledEventArgs gArgs = (EnemyKilledEventArgs) args;
			
			if (gArgs.Target is GameSummonedPet)
				return;

			if (!(player.GetConLevel(gArgs.Target) > -1) || !gArgs.Target.CurrentZone.IsRvR ||
			    !player.CurrentZone.IsRvR) return;
			FrontierMobsKilled++;
			player.Out.SendMessage("[Daily] Monster Killed: (" + FrontierMobsKilled + " | " + MAX_KILLED + ")", EChatType.CT_ScreenCenter, EChatLoc.CL_SystemWindow);
			player.Out.SendQuestUpdate(this);
					
			if (FrontierMobsKilled >= MAX_KILLED)
			{
				// FinishQuest or go back to npc
				Step = 2;
			}

		}
		
		public override string QuestPropertyKey
		{
			get => "KillNPCInFrontiersMid";
			set { ; }
		}

		public override void AbortQuest()
		{
			base.AbortQuest(); //Defined in Quest, changes the state, stores in DB etc ...
		}

		public override void FinishQuest()
		{
			m_questPlayer.ForceGainExperience((m_questPlayer.ExperienceForNextLevel - m_questPlayer.ExperienceForCurrentLevel)/10);
			m_questPlayer.AddMoney(MoneyMgr.GetMoney(0,0,m_questPlayer.Level,50,Util.Random(50)), "You receive {0} as a reward.");
			CoreRoGMgr.GenerateReward(m_questPlayer, 150);
			CoreRoGMgr.GenerateJewel(m_questPlayer, (byte)(m_questPlayer.Level + 1), m_questPlayer.Level + Util.Random(5, 11));
			FrontierMobsKilled = 0;
			base.FinishQuest(); //Defined in Quest, changes the state, stores in DB etc ...
		}
	}
}
