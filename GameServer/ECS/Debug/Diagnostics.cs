using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using ECS.Debug;

namespace ECS.Debug
{
    public static class Diagnostics
    {
        private const string SERVICE_NAME = "Diagnostics";
        private static StreamWriter _perfStreamWriter;
        private static bool _streamWriterInitialized = false;
        private static object _GameEventMgrNotifyLock = new();
        private static bool PerfCountersEnabled = false;
        private static bool stateMachineDebugEnabled = false;
        private static bool aggroDebugEnabled = false;
        private static Dictionary<string, Stopwatch> PerfCounters = new();
        private static object _PerfCountersLock = new();
        private static bool GameEventMgrNotifyProfilingEnabled = false;
        private static int GameEventMgrNotifyTimerInterval = 0;
        private static long GameEventMgrNotifyTimerStartTick = 0;
        private static Stopwatch GameEventMgrNotifyStopwatch;
        private static Dictionary<string, List<double>> GameEventMgrNotifyTimes = new();

        public static bool StateMachineDebugEnabled { get => stateMachineDebugEnabled; private set => stateMachineDebugEnabled = value; }
        public static bool AggroDebugEnabled { get => aggroDebugEnabled; private set => aggroDebugEnabled = value; }

        public static void TogglePerfCounters(bool enabled)
        {
            if (enabled == false)
            {
                _perfStreamWriter.Close();
                _streamWriterInitialized = false;
            }

            PerfCountersEnabled = enabled;
        }

        public static void ToggleStateMachineDebug(bool enabled)
        {
            StateMachineDebugEnabled = enabled;
        }

        public static void ToggleAggroDebug(bool enabled)
        {
            AggroDebugEnabled = enabled;
        }

        public static void Tick()
        {
            GameLoop.CurrentServiceTick = SERVICE_NAME;
            ReportPerfCounters();

            if (GameEventMgrNotifyProfilingEnabled)
            {
                if ((GameLoop.GetCurrentTime() - GameEventMgrNotifyTimerStartTick) > GameEventMgrNotifyTimerInterval)
                    ReportGameEventMgrNotifyTimes();
            }
        }

        private static void InitializeStreamWriter()
        {
            if (_streamWriterInitialized)
                return;
            else
            {
                string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PerfLog" + DateTime.Now.ToFileTime());
                _perfStreamWriter = new StreamWriter(_filePath, false);
                _streamWriterInitialized = true;
            }
        }

        public static void StartPerfCounter(string uniqueID)
        {
            if (!PerfCountersEnabled)
                return;

            InitializeStreamWriter();
            Stopwatch stopwatch = Stopwatch.StartNew();
            lock(_PerfCountersLock)
            {
                PerfCounters.TryAdd(uniqueID, stopwatch);
            }
        }

        public static void StopPerfCounter(string uniqueID)
        {
            if (!PerfCountersEnabled)
                return;

            lock (_PerfCountersLock)
            {
                if (PerfCounters.TryGetValue(uniqueID, out Stopwatch stopwatch))
                    stopwatch.Stop();
            }
        }

        private static void ReportPerfCounters()
        {
            if (!PerfCountersEnabled)
                return;

            // Report perf counters that were active this frame and then flush them.
            lock(_PerfCountersLock)
            {
                if (PerfCounters.Count > 0)
                {
                    string logString = "[PerfCounters] ";

                    foreach (var counter in PerfCounters)
                    {
                        string counterName = counter.Key;
                        float elapsed = (float)counter.Value.Elapsed.TotalMilliseconds;
                        string elapsedString = elapsed.ToString();
                        elapsedString = Util.TruncateString(elapsedString, 4);
                        logString += $"{counterName} {elapsedString}ms | ";
                    }

                    _perfStreamWriter.WriteLine(logString);
                    PerfCounters.Clear();
                }
            }
        }

        public static void BeginGameEventMgrNotify()
        {
            if (!GameEventMgrNotifyProfilingEnabled)
                return;

            GameEventMgrNotifyStopwatch = Stopwatch.StartNew();
        }

        public static void EndGameEventMgrNotify(CoreEvent e)
        {
            if (!GameEventMgrNotifyProfilingEnabled)
                return;

            GameEventMgrNotifyStopwatch.Stop();

            lock (_GameEventMgrNotifyLock)
            {
                if (GameEventMgrNotifyTimes.TryGetValue(e.Name, out List<double> EventTimeValues))
                    EventTimeValues.Add(GameEventMgrNotifyStopwatch.Elapsed.TotalMilliseconds);
                else
                {
                    EventTimeValues = new();
                    EventTimeValues.Add(GameEventMgrNotifyStopwatch.Elapsed.TotalMilliseconds);
                    GameEventMgrNotifyTimes.TryAdd(e.Name, EventTimeValues);
                }
            }
        }

        public static void StartGameEventMgrNotifyTimeReporting(int IntervalMilliseconds)
        {
            if (GameEventMgrNotifyProfilingEnabled)
                return;

            GameEventMgrNotifyProfilingEnabled = true;
            GameEventMgrNotifyTimerInterval = IntervalMilliseconds;
            GameEventMgrNotifyTimerStartTick = GameLoop.GetCurrentTime();
        }

        public static void StopGameEventMgrNotifyTimeReporting()
        {
            if (!GameEventMgrNotifyProfilingEnabled)
                return;

            GameEventMgrNotifyProfilingEnabled = false;
            GameEventMgrNotifyTimes.Clear();
        }

        private static void ReportGameEventMgrNotifyTimes()
        {
            string ActualInterval = Util.TruncateString((GameLoop.GetCurrentTime() - GameEventMgrNotifyTimerStartTick).ToString(), 5);
            Console.WriteLine($"==== GameEventMgr Notify() Costs (Requested Interval: {GameEventMgrNotifyTimerInterval}ms | Actual Interval: {ActualInterval}ms) ====");

            lock (_GameEventMgrNotifyLock)
            {
                foreach (var NotifyData in GameEventMgrNotifyTimes)
                {
                    List<double> EventTimeValues = NotifyData.Value;
                    string EventNameString = NotifyData.Key.PadRight(30);
                    double TotalCost = 0;
                    double MinCost = 0;
                    double MaxCost = 0;

                    foreach (double time in EventTimeValues)
                    {
                        TotalCost += time;

                        if (time < MinCost)
                            MinCost = time;

                        if (time > MaxCost)
                            MaxCost = time;
                    }

                    int NumValues = EventTimeValues.Count;
                    double AvgCost = TotalCost / NumValues;
                    string NumValuesString = NumValues.ToString().PadRight(4);
                    string TotalCostString = Util.TruncateString(TotalCost.ToString(), 5);
                    string MinCostString = Util.TruncateString(MinCost.ToString(), 5);
                    string MaxCostString = Util.TruncateString(MaxCost.ToString(), 5);
                    string AvgCostString = Util.TruncateString(AvgCost.ToString(), 5);
                    Console.WriteLine($"{EventNameString} - # Calls: {NumValuesString} | Total: {TotalCostString}ms | Avg: {AvgCostString}ms | Min: {MinCostString}ms | Max: {MaxCostString}ms");
                }

                GameEventMgrNotifyTimes.Clear();
                GameEventMgrNotifyTimerStartTick = GameLoop.GetCurrentTime();
                Console.WriteLine("---------------------------------------------------------------------------");
            }
        }
    }
}
