using System;
using System.Reflection;
using Core.Database;
using Core.Database.Tables;
using Core.Events;
using Core.GS.Enums;
using Core.GS.Events;
using Core.GS.GameUtils;
using Core.GS.Packets;
using Core.GS.Packets.Server;
using Core.GS.Quests;
using Core.GS.Server;
using Core.GS.World;
using log4net;

namespace Core.GS.DailyQuest.Albion
{
	public class DailyCaptureKeepLvl50AlbQuest : Quests.DailyQuest
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private const string questTitle = "[Daily] Frontier Conquerer";
		private const int minimumLevel = 50;
		private const int maximumLevel = 50;

		// Capture Goal
		private const int MAX_CAPTURED = 1;
		
		private static GameNpc Haszan = null; // Start NPC

		private int _isCaptured = 0;

		// Constructors
		public DailyCaptureKeepLvl50AlbQuest() : base()
		{
		}

		public DailyCaptureKeepLvl50AlbQuest(GamePlayer questingPlayer) : base(questingPlayer, 1)
		{
		}

		public DailyCaptureKeepLvl50AlbQuest(GamePlayer questingPlayer, int step) : base(questingPlayer, step)
		{
		}

		public DailyCaptureKeepLvl50AlbQuest(GamePlayer questingPlayer, DbQuest dbQuest) : base(questingPlayer, dbQuest)
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
			if (!ServerProperty.LOAD_QUESTS)
				return;
			

			#region defineNPCs

			GameNpc[] npcs = WorldMgr.GetNPCsByName("Haszan", ERealm.Albion);

			if (npcs.Length > 0)
				foreach (GameNpc npc in npcs)
					if (npc.CurrentRegionID == 1 && npc.X == 583866 && npc.Y == 477497)
					{
						Haszan = npc;
						break;
					}

			if (Haszan == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Haszan , creating it ...");
				Haszan = new GameNpc();
				Haszan.Model = 51;
				Haszan.Name = "Haszan";
				Haszan.GuildName = "Realm Logistics";
				Haszan.Realm = ERealm.Albion;
				//Castle Sauvage Location
				Haszan.CurrentRegionID = 1;
				Haszan.Size = 50;
				Haszan.Level = 59;
				Haszan.X = 583866;
				Haszan.Y = 477497;
				Haszan.Z = 2600;
				Haszan.Heading = 3111;
				Haszan.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Haszan.SaveIntoDatabase();
				}
			}

			#endregion

			#region defineItems
			#endregion

			#region defineObject
			#endregion

			GameEventMgr.AddHandler(GamePlayerEvent.AcceptQuest, new CoreEventHandler(SubscribeQuest));
			GameEventMgr.AddHandler(GamePlayerEvent.DeclineQuest, new CoreEventHandler(SubscribeQuest));

			GameEventMgr.AddHandler(Haszan, GameObjectEvent.Interact, new CoreEventHandler(TalkToHaszan));
			GameEventMgr.AddHandler(Haszan, GameLivingEvent.WhisperReceive, new CoreEventHandler(TalkToHaszan));

			/* Now we bring to Haszan the possibility to give this quest to players */
			Haszan.AddQuestToGive(typeof (DailyCaptureKeepLvl50AlbQuest));

			if (log.IsInfoEnabled)
				log.Info("Quest \"" + questTitle + "\" initialized");
		}

		[ScriptUnloadedEvent]
		public static void ScriptUnloaded(CoreEvent e, object sender, EventArgs args)
		{
			//if not loaded, don't worry
			if (Haszan == null)
				return;
			// remove handlers
			GameEventMgr.RemoveHandler(GamePlayerEvent.AcceptQuest, new CoreEventHandler(SubscribeQuest));
			GameEventMgr.RemoveHandler(GamePlayerEvent.DeclineQuest, new CoreEventHandler(SubscribeQuest));

			GameEventMgr.RemoveHandler(Haszan, GameObjectEvent.Interact, new CoreEventHandler(TalkToHaszan));
			GameEventMgr.RemoveHandler(Haszan, GameLivingEvent.WhisperReceive, new CoreEventHandler(TalkToHaszan));

			/* Now we remove to Haszan the possibility to give this quest to players */
			Haszan.RemoveQuestToGive(typeof (DailyCaptureKeepLvl50AlbQuest));
		}

		private static void TalkToHaszan(CoreEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			if(Haszan.CanGiveQuest(typeof (DailyCaptureKeepLvl50AlbQuest), player)  <= 0)
				return;

			//We also check if the player is already doing the quest
			DailyCaptureKeepLvl50AlbQuest quest = player.IsDoingQuest(typeof (DailyCaptureKeepLvl50AlbQuest)) as DailyCaptureKeepLvl50AlbQuest;

			if (e == GameObjectEvent.Interact)
			{
				if (quest != null)
				{
					switch (quest.Step)
					{
						case 1:
							Haszan.SayTo(player, "Find an enemy occupied keep and capture it. If you succeed come back for your reward.");
							break;
						case 2:
							Haszan.SayTo(player, "Hello " + player.Name + ", did you [capture] a keep?");
							break;
					}
				}
				else
				{
					Haszan.SayTo(player, "Hello "+ player.Name +", I am Haszan. I help the king with logistics, and he's tasked me with getting things done around here. "+
											"I've seen you battling in our frontiers. Do you think you're strong enough to help me with some real estate matters? \n"+
					                     "\nThe king wants us to [reclaim a keep] that he's particularly fond of.");
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
						case "reclaim a keep":
							player.Out.SendQuestSubscribeCommand(Haszan, QuestMgr.GetIDForQuestType(typeof(DailyCaptureKeepLvl50AlbQuest)), "Will you help Haszan "+questTitle+"");
							break;
					}
				}
				else
				{
					switch (wArgs.Text)
					{
						case "capture":
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
			if (player.IsDoingQuest(typeof (DailyCaptureKeepLvl50AlbQuest)) != null)
				return true;

			// This checks below are only performed is player isn't doing quest already

			//if (player.HasFinishedQuest(typeof(Academy_47)) == 0) return false;

			//if (!CheckPartAccessible(player,typeof(CityOfCamelot)))
			//	return false;

			if (player.Level < minimumLevel || player.Level > maximumLevel)
				return false;

			return true;
		}

		private static void CheckPlayerAbortQuest(GamePlayer player, byte response)
		{
			DailyCaptureKeepLvl50AlbQuest quest = player.IsDoingQuest(typeof (DailyCaptureKeepLvl50AlbQuest)) as DailyCaptureKeepLvl50AlbQuest;

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

			if (qargs.QuestID != QuestMgr.GetIDForQuestType(typeof(DailyCaptureKeepLvl50AlbQuest)))
				return;

			if (e == GamePlayerEvent.AcceptQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x01);
			else if (e == GamePlayerEvent.DeclineQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x00);
		}

		private static void CheckPlayerAcceptQuest(GamePlayer player, byte response)
		{
			if(Haszan.CanGiveQuest(typeof (DailyCaptureKeepLvl50AlbQuest), player)  <= 0)
				return;

			if (player.IsDoingQuest(typeof (DailyCaptureKeepLvl50AlbQuest)) != null)
				return;

			if (response == 0x00)
			{
				player.Out.SendMessage("Thank you for your help.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
			}
			else
			{
				//Check if we can add the quest!
				if (!Haszan.GiveQuest(typeof (DailyCaptureKeepLvl50AlbQuest), player, 1))
					return;

				Haszan.SayTo(player, "Thank you "+player.Name+", be an enrichment for our realm!");

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
						return "Go to the battlefield and conquer a keep. \nCaptured: Keep ("+ _isCaptured +" | 1)";
					case 2:
						return "Return to Haszan for your Reward.";
				}
				return base.Description;
			}
		}

		public override void Notify(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = sender as GamePlayer;

			if (player?.IsDoingQuest(typeof(DailyCaptureKeepLvl50AlbQuest)) == null)
				return;
			
			if (sender != m_questPlayer)
				return;

			if (Step != 1 || e != GamePlayerEvent.CapturedKeepsChanged) return;
			_isCaptured = 1;
			player.Out.SendMessage("[Daily] Captured Keep: ("+_isCaptured+" | "+MAX_CAPTURED+")", EChatType.CT_ScreenCenter, EChatLoc.CL_SystemWindow);
			player.Out.SendQuestUpdate(this);
					
			if (_isCaptured >= MAX_CAPTURED)
			{
				// FinishQuest or go back to Dean
				Step = 2;
			}

		}
		
		public override string QuestPropertyKey
		{
			get => "CaptureKeepQuestAlb";
			set { ; }
		}
		
		public override void LoadQuestParameters()
		{
			
		}

		public override void SaveQuestParameters()
		{
			
		}

		public override void AbortQuest()
		{
			base.AbortQuest(); //Defined in Quest, changes the state, stores in DB etc ...
		}

		public override void FinishQuest()
		{
			int reward = ServerProperty.DAILY_RVR_REWARD;
			
			m_questPlayer.ForceGainExperience((m_questPlayer.ExperienceForNextLevel - m_questPlayer.ExperienceForCurrentLevel)/5);
			m_questPlayer.AddMoney(MoneyMgr.GetMoney(0,0,m_questPlayer.Level*2,0,Util.Random(50)), "You receive {0} as a reward.");
			CoreRogMgr.GenerateReward(m_questPlayer, 250);
			CoreRogMgr.GenerateJewel(m_questPlayer, (byte)(m_questPlayer.Level + 1), m_questPlayer.Level + Util.Random(5, 11));
			_isCaptured = 0;
			
			if (reward > 0)
			{
				m_questPlayer.Out.SendMessage($"You have been rewarded {reward} Realmpoints for finishing Daily Quest.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
				m_questPlayer.GainRealmPoints(reward, false);
				m_questPlayer.Out.SendUpdatePlayer();
			}
			base.FinishQuest(); //Defined in Quest, changes the state, stores in DB etc ...
		}
	}
}
