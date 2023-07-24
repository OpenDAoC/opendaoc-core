using DOL.GS;

namespace DOL.AI.Brain
{
	/// <summary>
	/// The brains for alluvian mobs. No need to manually assign this.
	/// /mob create DOL.GS.Alluvian and this will be attached automatically.
	/// </summary>
    public class AlluvianBrain : StandardMobBrain
	{
		/// <summary>
		/// Determine if we have less than 12, if not, spawn one.
		/// </summary>
		public override void Think()
		{
			NpcAlluvian mob = Body as NpcAlluvian;
			if (NpcAlluvian.GlobuleNumber < 12)
			{
				mob.SpawnGlobule();
			}
			base.Think();
		}
	}
}
