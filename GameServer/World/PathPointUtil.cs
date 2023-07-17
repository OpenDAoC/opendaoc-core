
using DOL.Database;

namespace DOL.GS.Movement
{
	/// <summary>
	/// represents a point in a way path
	/// </summary>
	public class PathPointUtil : Point3D
	{
		protected short m_maxspeed;
		protected PathPointUtil m_next = null;
		protected PathPointUtil m_prev = null;
		protected ePathType m_type;
		protected bool m_flag;
		protected int m_waitTime = 0;

		public PathPointUtil(PathPointUtil pp) : this(pp, pp.MaxSpeed,pp.Type) {}

		public PathPointUtil(Point3D p, short maxspeed, ePathType type) : this(p.X,  p.Y,  p.Z, maxspeed, type) {}

		public PathPointUtil(int x, int y, int z, short maxspeed, ePathType type) : base(x, y, z)
		{
			m_maxspeed = maxspeed;
			m_type = type;
			m_flag = false;
			m_waitTime = 0;
		}

		/// <summary>
		/// Speed allowed after that waypoint in forward direction
		/// </summary>
		public short MaxSpeed
		{
			get { return m_maxspeed; }
			set { m_maxspeed = value; }
		}

		/// <summary>
		/// next waypoint in path
		/// </summary>
		public PathPointUtil Next
		{
			get { return m_next; }
			set { m_next = value; }
		}

		/// <summary>
		/// previous waypoint in path
		/// </summary>
		public PathPointUtil Prev
		{
			get { return m_prev; }
			set { m_prev = value; }
		}

		/// <summary>
		/// flag toggle when go through pathpoint
		/// </summary>
		public bool FiredFlag
		{
			get { return m_flag; }
			set { m_flag = value; }
		}

		/// <summary>
		/// path type
		/// </summary>
		public ePathType Type
		{
			get { return m_type; }
			set { m_type = value; }
		}

		/// <summary>
		/// path type
		/// </summary>
		public int WaitTime
		{
			get { return m_waitTime; }
			set { m_waitTime = value; }
		}
	}
}
