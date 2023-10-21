using System.Collections.Generic;
using System.Reflection;
using Core.Database;
using Core.Database.Enums;
using Core.Database.Tables;
using Core.GS.Database;
using log4net;

namespace Core.GS.Movement
{
    /// <summary>
    /// TODO: instead movement manager we need AI when npc should travel on path and attack 
    /// enemies if they are near and after that return to pathing for example.
    /// this current implementation is incomplete but usable for horses
    /// </summary>
    public class MovementMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static Dictionary<string, DbPath> m_pathCache = new Dictionary<string, DbPath>();
		private static Dictionary<string, SortedList<int, DbPathPoint>> m_pathpointCache = new Dictionary<string, SortedList<int, DbPathPoint>>();
		private static object LockObject = new object();
		/// <summary>
		/// Cache all the paths and pathpoints
		/// </summary>
		private static void FillPathCache()
		{
			IList<DbPath> allPaths = GameServer.Database.SelectAllObjects<DbPath>();
			foreach (DbPath path in allPaths)
			{
				m_pathCache.Add(path.PathID, path);
			}

			int duplicateCount = 0;

			IList<DbPathPoint> allPathPoints = GameServer.Database.SelectAllObjects<DbPathPoint>();
			foreach (DbPathPoint pathPoint in allPathPoints)
			{
				if (m_pathpointCache.ContainsKey(pathPoint.PathID))
				{
					if (m_pathpointCache[pathPoint.PathID].ContainsKey(pathPoint.Step) == false)
					{
						m_pathpointCache[pathPoint.PathID].Add(pathPoint.Step, pathPoint);
					}
					else
					{
						duplicateCount++;
					}
				}
				else
				{
					SortedList<int, DbPathPoint> pList = new SortedList<int, DbPathPoint>();
					pList.Add(pathPoint.Step, pathPoint);
					m_pathpointCache.Add(pathPoint.PathID, pList);
				}
			}
            if (duplicateCount > 0)
                log.ErrorFormat("{0} duplicate steps ignored while loading paths.", duplicateCount);
			log.InfoFormat("Path cache filled with {0} paths.", m_pathCache.Count);
		}

		public static void UpdatePathInCache(string pathID)
		{
			log.DebugFormat("Updating path {0} in path cache.", pathID);

			var dbpath = CoreDb<DbPath>.SelectObject(DB.Column("PathID").IsEqualTo(pathID));
			if (dbpath != null)
			{
				if (m_pathCache.ContainsKey(pathID))
				{
					m_pathCache[pathID] = dbpath;
				}
				else
				{
					m_pathCache.Add(dbpath.PathID, dbpath);
				}
			}

			var pathPoints = CoreDb<DbPathPoint>.SelectObjects(DB.Column("PathID").IsEqualTo(pathID));
			SortedList<int, DbPathPoint> pList = new SortedList<int, DbPathPoint>();
			if (m_pathpointCache.ContainsKey(pathID))
			{
				m_pathpointCache[pathID] = pList;
			}
			else
			{
				m_pathpointCache.Add(pathID, pList);
			}

			foreach (DbPathPoint pathPoint in pathPoints)
			{
				m_pathpointCache[pathPoint.PathID].Add(pathPoint.Step, pathPoint);
			}
		}

        /// <summary>
        /// loads a path from the cache
        /// </summary>
        /// <param name="pathID">path to load</param>
        /// <returns>first pathpoint of path or null if not found</returns>
        public static PathPoint LoadPath(string pathID)
        {
        	lock(LockObject)
        	{
	        	if (m_pathCache.Count == 0)
				{
					FillPathCache();
				}
	
				DbPath dbpath = null;
	
				if (m_pathCache.ContainsKey(pathID))
				{
					dbpath = m_pathCache[pathID];
				}
	
				// even if path entry not found see if pathpoints exist and try to use it
	
	            EPathType pathType = EPathType.Once;
	
	            if (dbpath != null)
	            {
	                pathType = (EPathType)dbpath.PathType;
	            }
	
				SortedList<int, DbPathPoint> pathPoints = null;
	
				if (m_pathpointCache.ContainsKey(pathID))
				{
					pathPoints = m_pathpointCache[pathID];
				}
				else
				{
					pathPoints = new SortedList<int, DbPathPoint>();
				}
	
	            PathPoint prev = null;
	            PathPoint first = null;
	
				foreach (DbPathPoint pp in pathPoints.Values)
				{
					PathPoint p = new PathPoint(pp.X, pp.Y, pp.Z, (short) pp.MaxSpeed, pathType);
					p.WaitTime = pp.WaitTime;
	
					if (first == null)
					{
						first = p;
					}
					p.Prev = prev;
					if (prev != null)
					{
						prev.Next = p;
					}
					prev = p;
				}

            	return first;
        	}
        }

        /// <summary>
        /// Saves the path into the database
        /// </summary>
        /// <param name="pathID">The path ID</param>
        /// <param name="path">The path waypoint</param>
        public static void SavePath(string pathID, PathPoint path)
        {
            if (path == null)
                return;

            pathID.Replace('\'', '/');

			// First delete any path with this pathID from the database

			var dbpath = CoreDb<DbPath>.SelectObject(DB.Column("PathID").IsEqualTo(pathID));
			if (dbpath != null)
			{
				GameServer.Database.DeleteObject(dbpath);
			}

			GameServer.Database.DeleteObject(CoreDb<DbPathPoint>.SelectObjects(DB.Column("PathID").IsEqualTo(pathID)));

			// Now add this path and iterate through the PathPoint linked list to add all the path points

            PathPoint root = FindFirstPathPoint(path);

            //Set the current pathpoint to the rootpoint!
            path = root;
            dbpath = new DbPath(pathID, root.Type);
            GameServer.Database.AddObject(dbpath);

            int i = 1;
            do
            {
                DbPathPoint dbpp = new DbPathPoint(path.X, path.Y, path.Z, path.MaxSpeed);
                dbpp.Step = i++;
                dbpp.PathID = pathID;
                dbpp.WaitTime = path.WaitTime;
                GameServer.Database.AddObject(dbpp);
                path = path.Next;
            }
			while (path != null && path != root);

			UpdatePathInCache(pathID);
        }

        /// <summary>
        /// Searches for the first point in the waypoints chain
        /// </summary>
        /// <param name="path">One of the pathpoints</param>
        /// <returns>The first pathpoint in the chain or null</returns>
        public static PathPoint FindFirstPathPoint(PathPoint path)
        {
            PathPoint root = path;
            // avoid circularity
            int iteration = 50000;
            while (path.Prev != null && path.Prev != root)
            {
                path = path.Prev;
                iteration--;
                if (iteration <= 0)
                {
                    if (log.IsErrorEnabled)
                        log.Error("Path cannot be saved, it seems endless");
                    return null;
                }
            }
            return path;
        }
    }
}