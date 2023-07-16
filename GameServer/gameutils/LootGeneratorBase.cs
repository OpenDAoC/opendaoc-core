using System;
using System.Collections;
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// Base for all loot generators
	/// </summary>
	public class LootGeneratorBase : ILootGenerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected int m_exclusivePriority = 0;

		public LootGeneratorBase()
		{
		}

		public int ExclusivePriority
		{
			get{ return m_exclusivePriority; }
			set{ m_exclusivePriority = value; }
		}

		public virtual void Refresh(GameNPC mob)
		{
		}

		/// <summary>
		/// Generate loot for given mob
		/// </summary>
		/// <param name="mob"></param>
		/// <param name="killer"></param>
		/// <returns></returns>
		public virtual LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList loot = new LootList();
			return loot;
		}
	}
}
