using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Quests;
using ECS.Debug;

namespace DOL.GS
{
    public class WeeklyQuestService
    {
        private const string SERVICE_NAME = "WeeklyQuestService";
        private const string WEEKLY_INTERVAL_KEY = "WEEKLY";
        private static DateTime lastWeeklyRollover;

        static WeeklyQuestService()
        {
            IList<DbTaskRefreshIntervals> loadQuestsProp = GameServer.Database.SelectAllObjects<DbTaskRefreshIntervals>();

            foreach (DbTaskRefreshIntervals interval in loadQuestsProp)
            {
                if (interval.RolloverInterval.Equals(WEEKLY_INTERVAL_KEY))
                    lastWeeklyRollover = interval.LastRollover;
            }
        }

        public static void Tick()
        {
            GameLoop.CurrentServiceTick = SERVICE_NAME;
            Diagnostics.StartPerfCounter(SERVICE_NAME);
            //.WriteLine($"daily:{lastDailyRollover.Date.DayOfYear} weekly:{lastWeeklyRollover.Date.DayOfYear+7} now:{DateTime.Now.Date.DayOfYear}");

            // This is where the weekly check will go once testing is finished.
            if (lastWeeklyRollover.Date.DayOfYear + 7 < DateTime.Now.Date.DayOfYear || lastWeeklyRollover.Year < DateTime.Now.Year)
            {
                lastWeeklyRollover = DateTime.Now;
                DbTaskRefreshIntervals loadQuestsProp = GameServer.Database.SelectObject<DbTaskRefreshIntervals>(DB.Column("RolloverInterval").IsEqualTo(WEEKLY_INTERVAL_KEY));

                // Update the one we've got, or make a new one.
                if (loadQuestsProp != null)
                {
                    loadQuestsProp.LastRollover = DateTime.Now;
                    GameServer.Database.SaveObject(loadQuestsProp);
                }
                else
                {
                    DbTaskRefreshIntervals newTime = new DbTaskRefreshIntervals();
                    newTime.LastRollover = DateTime.Now;
                    newTime.RolloverInterval = WEEKLY_INTERVAL_KEY;
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
                        if (quest is Quests.WeeklyQuest)
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

                IList<DbQuests> existingWeeklyQuests = GameServer.Database.SelectObjects<DbQuests>(DB.Column("Name").IsLike("%WeeklyQuest%"));

                foreach (DbQuests existingWeeklyQuest in existingWeeklyQuests)
                {
                    if (existingWeeklyQuest.Step <= -1)
                        GameServer.Database.DeleteObject(existingWeeklyQuest);
                }
            }

            Diagnostics.StopPerfCounter(SERVICE_NAME);
        }
    }
}
