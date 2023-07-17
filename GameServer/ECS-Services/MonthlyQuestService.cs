using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Quests;
using ECS.Debug;

namespace DOL.GS
{
    public class MonthlyQuestService
    {
        private const string SERVICE_NAME = "MonthlyQuestService";
        private const string MONTHLY_INTERVAL_KEY = "MONTHLY";
        private static DateTime lastMonthlyRollover;

        static MonthlyQuestService()
        {
            IList<DbTaskRefreshIntervals> loadQuestsProp = GameServer.Database.SelectAllObjects<DbTaskRefreshIntervals>();

            foreach (DbTaskRefreshIntervals interval in loadQuestsProp)
            {
                if (interval.RolloverInterval.Equals(MONTHLY_INTERVAL_KEY))
                    lastMonthlyRollover = interval.LastRollover;
            }
        }

        public static void Tick()
        {
            Diagnostics.StartPerfCounter(SERVICE_NAME);
            //.WriteLine($"daily:{lastDailyRollover.Date.DayOfYear} weekly:{lastWeeklyRollover.Date.DayOfYear+7} now:{DateTime.Now.Date.DayOfYear}");

            // This is where the weekly check will go once testing is finished.
            if (lastMonthlyRollover.Date.Month < DateTime.Now.Date.Month || lastMonthlyRollover.Year < DateTime.Now.Year)
            {
                lastMonthlyRollover = DateTime.Now;
                DbTaskRefreshIntervals loadQuestsProp = GameServer.Database.SelectObject<DbTaskRefreshIntervals>(DB.Column("RolloverInterval").IsEqualTo(MONTHLY_INTERVAL_KEY));

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
                    newTime.RolloverInterval = MONTHLY_INTERVAL_KEY;
                    GameServer.Database.AddObject(newTime);
                }

                List<GamePlayer> players = EntityManager.UpdateAndGetAll<GamePlayer>(EntityManager.EntityType.Player, out int lastNonNullIndex);

                for (int i = 0; i < lastNonNullIndex + 1; i++)
                {
                    GamePlayer player = players[i];

                    if (player == null)
                        continue;

                    List<AbstractQuest> questsToRemove = new();

                    foreach (AbstractQuest quest in player.QuestListFinished)
                    {
                        if (quest is Quests.MonthlyQuest)
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

                IList<DbQuests> existingMonthlyQuests = GameServer.Database.SelectObjects<DbQuests>(DB.Column("Name").IsLike("%MonthlyQuest%"));

                foreach (DbQuests existingMonthlyQuest in existingMonthlyQuests)
                {
                    if (existingMonthlyQuest.Step <= -1)
                        GameServer.Database.DeleteObject(existingMonthlyQuest);
                }
            }

            Diagnostics.StopPerfCounter(SERVICE_NAME);
        }
    }
}
