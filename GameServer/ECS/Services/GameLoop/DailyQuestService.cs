using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Quests;
using ECS.Debug;

namespace DOL.GS
{
    public class DailyQuestService
    {
        private const string SERVICE_NAME = "DailyQuestService";
        private const string DAILY_INTERVAL_KEY = "DAILY";
        private static DateTime lastDailyRollover;

        static DailyQuestService()
        {
            IList<DbTaskRefreshIntervals> loadQuestsProp = GameServer.Database.SelectAllObjects<DbTaskRefreshIntervals>();

            foreach (DbTaskRefreshIntervals interval in loadQuestsProp)
            {
                if (interval.RolloverInterval.Equals(DAILY_INTERVAL_KEY))
                    lastDailyRollover = interval.LastRollover;
            }
        }

        public static void Tick()
        {
            GameLoop.CurrentServiceTick = SERVICE_NAME;
            Diagnostics.StartPerfCounter(SERVICE_NAME);
            //.WriteLine($"daily:{lastDailyRollover.Date.DayOfYear} weekly:{lastWeeklyRollover.Date.DayOfYear+7} now:{DateTime.Now.Date.DayOfYear}");

            if (lastDailyRollover.Date.DayOfYear < DateTime.Now.Date.DayOfYear || lastDailyRollover.Year < DateTime.Now.Year)
            {
                lastDailyRollover = DateTime.Now;
                DbTaskRefreshIntervals loadQuestsProp = GameServer.Database.SelectObject<DbTaskRefreshIntervals>(DB.Column("RolloverInterval").IsEqualTo(DAILY_INTERVAL_KEY));

                // Update the one we've got, or make a new one.
                if (loadQuestsProp != null)
                {
                    loadQuestsProp.LastRollover = DateTime.Now;
                    GameServer.Database.SaveObject(loadQuestsProp);
                }
                else
                {
                    DbTaskRefreshIntervals newTime = new();
                    newTime.LastRollover = DateTime.Now;
                    newTime.RolloverInterval = DAILY_INTERVAL_KEY;
                    GameServer.Database.AddObject(newTime);
                }

                List<GamePlayer> players = EntityMgr.UpdateAndGetAll<GamePlayer>(EntityMgr.EntityType.Player, out int lastNonNullIndex);

                for (int i = 0; i < lastNonNullIndex + 1; i++)
                {
                    GamePlayer player = players[i];

                    if (player == null)
                        continue;

                    List<AbstractQuest> questsToRemove = new();

                    foreach (AbstractQuest quest in player.QuestListFinished)
                    {
                        if (quest is Quests.DailyQuest)
                        {
                            quest.AbortQuest();
                            questsToRemove.Add(quest);
                        }
                    }

                    foreach (AbstractQuest quest in questsToRemove)
                    {
                        player.QuestList.Remove(quest);
                        player.QuestListFinished.Remove(quest);
                    }
                }

                IList<DbQuests> existingDailyQuests = GameServer.Database.SelectObjects<DbQuests>(DB.Column("Name").IsLike("%DailyQuest%"));

                foreach (DbQuests existingDailyQuest in existingDailyQuests)
                {
                    if (existingDailyQuest.Step <= -1)
                        GameServer.Database.DeleteObject(existingDailyQuest);
                }

                //Console.WriteLine($"Daily refresh");
            }

            Diagnostics.StopPerfCounter(SERVICE_NAME);
        }
    }
}
