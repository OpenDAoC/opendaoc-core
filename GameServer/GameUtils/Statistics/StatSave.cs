using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using DOL.Database;
using DOL.Events;
using log4net;

namespace DOL.GS.GameEvents
{
	class StatSave
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly int INITIAL_DELAY = 60000;
		
		private static long m_lastBytesIn = 0;
		private static long m_lastBytesOut = 0;
		private static long m_lastMeasureTick = DateTime.Now.Ticks;
		private static int m_statFrequency = 60 * 1000; // 1 minute
		private static PerformanceCounter m_systemCpuUsedCounter = null;
		private static PerformanceCounter m_processCpuUsedCounter = null;
		
		private static volatile Timer m_timer = null;
		
		[GameServerStartedEvent]
		public static void OnScriptCompiled(CoreEvent e, object sender, EventArgs args)
		{
			// Desactivated
			if (ServerProperties.ServerProperties.STATSAVE_INTERVAL == -1)
				return;
			
			// try
			// {
			// 	m_systemCpuUsedCounter = new PerformanceCounter("Processor", "% processor time", "_total");
			// 	m_systemCpuUsedCounter.NextValue();
			// }
			// catch (Exception ex)
			// {
			// 	m_systemCpuUsedCounter = null;
			// 	if (log.IsWarnEnabled)
			// 		log.Warn(ex.GetType().Name + " SystemCpuUsedCounter won't be available: " + ex.Message);
			// }
			// try
			// {
			// 	m_processCpuUsedCounter = new PerformanceCounter("Process", "% processor time", GetProcessCounterName());
			// 	m_processCpuUsedCounter.NextValue();
			// }
			// catch (Exception ex)
			// {
			// 	m_processCpuUsedCounter = null;
			// 	if (log.IsWarnEnabled)
			// 		log.Warn(ex.GetType().Name + " ProcessCpuUsedCounter won't be available: " + ex.Message);
			// }
			// 1 min * INTERVAL
			m_statFrequency *= ServerProperties.ServerProperties.STATSAVE_INTERVAL;
			lock (typeof(StatSave))
			{
				m_timer = new Timer(new TimerCallback(SaveStats), null, INITIAL_DELAY, Timeout.Infinite);
			}
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(CoreEvent e, object sender, EventArgs args)
		{
			lock (typeof(StatPrint))
			{
				if (m_timer != null)
				{
					m_timer.Change(Timeout.Infinite, Timeout.Infinite);
					m_timer.Dispose();
					m_timer = null;
				}
			}
		}

		/// <summary>
		/// Find the process counter name
		/// </summary>
		/// <returns></returns>
		public static string GetProcessCounterName()
		{
			Process process = Process.GetCurrentProcess();
			int id = process.Id;
			PerformanceCounterCategory perfCounterCat = new PerformanceCounterCategory("Process");
			foreach (DictionaryEntry entry in perfCounterCat.ReadCategory()["id process"])
			{
				string processCounterName = (string)entry.Key;
				if (((InstanceData)entry.Value).RawValue == id)
					return processCounterName;
			}
			return "";
		}
		
		public static void SaveStats(object state)
		{
			try
			{
				long ticks = DateTime.Now.Ticks;
				long time = ticks - m_lastMeasureTick;
				m_lastMeasureTick = ticks;
				time /= 10000000L;
				if (time < 1)
				{
					log.Warn("Time has not changed since last call of SaveStats");
					time = 1; // prevent division by zero?
				}
				long inRate = (Statistics.BytesIn - m_lastBytesIn) / time;
				long outRate = (Statistics.BytesOut - m_lastBytesOut) / time;

				m_lastBytesIn = Statistics.BytesIn;
				m_lastBytesOut = Statistics.BytesOut;

				int clients = WorldMgr.GetAllPlayingClientsCount();
				int AlbPlayers = WorldMgr.GetClientsOfRealmCount(ERealm.Albion);
				int MidPlayers = WorldMgr.GetClientsOfRealmCount(ERealm.Midgard);
				int HibPlayers = WorldMgr.GetClientsOfRealmCount(ERealm.Hibernia);

				float cpu = 0;
				// if (m_systemCpuUsedCounter != null)
				// 	cpu = m_systemCpuUsedCounter.NextValue(); 
				// if (m_processCpuUsedCounter != null)
				// 	cpu = m_processCpuUsedCounter.NextValue();

				// long totalmem = GC.GetTotalMemory(false);

				long totalmem = 0;
				
				DbServerStats newstat = new DbServerStats();
				newstat.CPU = cpu;
				newstat.Clients = clients;
				newstat.Upload = (int)outRate/1024;
				newstat.Download = (int)inRate / 1024;
				newstat.Memory = totalmem;
				newstat.AlbionPlayers = AlbPlayers;
				newstat.MidgardPlayers = MidPlayers;
				newstat.HiberniaPlayers = HibPlayers;
				GameServer.Database.AddObject(newstat);
				GameServer.Database.SaveObject(newstat);
			}
			catch (Exception e)
			{
				log.Error("Updating server stats", e);
			}
			finally
			{
				lock (typeof(StatSave))
				{
					if (m_timer != null)
					{
						m_timer.Change(m_statFrequency, Timeout.Infinite);
					}
				}
			}
		}
	}
}