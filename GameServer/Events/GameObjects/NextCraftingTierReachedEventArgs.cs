using DOL.GS;

namespace DOL.Events
{
	public class NextCraftingTierReachedEventArgs : System.EventArgs
	{
		private eCraftingSkill m_skill;
		private int m_points;
		public eCraftingSkill Skill
		{
			get
			{
				return m_skill;
			}
		}
		public int Points
		{
			get
			{
				return m_points;
			}
		}
		public NextCraftingTierReachedEventArgs(eCraftingSkill skill, int points)
			: base()
		{
			m_skill = skill;
			m_points = points;
		}
	}
}
