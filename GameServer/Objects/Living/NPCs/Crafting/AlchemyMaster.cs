 
using DOL.Language;

namespace DOL.GS
{
	[NpcGuild("Alchemists Master")]
	public class AlchemistsMaster : CraftNpc
	{
		private static readonly eCraftingSkill[] m_trainedSkills = 
		{
			eCraftingSkill.SpellCrafting,
			eCraftingSkill.Alchemy,
			eCraftingSkill.GemCutting,
			eCraftingSkill.HerbalCrafting,
			eCraftingSkill.SiegeCrafting,
		};

		public override eCraftingSkill[] TrainedSkills
		{
			get { return m_trainedSkills; }
		}

		public override string GUILD_ORDER
		{
			get
			{
                return LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "AlchemistsMaster.GuildOrder");
            }
		}

		public override string ACCEPTED_BY_ORDER_NAME
		{
			get
			{
                return LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "AlchemistsMaster.AcceptedByOrderName");
            }
		}
		public override eCraftingSkill TheCraftingSkill
		{
			get { return eCraftingSkill.Alchemy; }
		}
		public override string InitialEntersentence
		{
			get
			{
                return LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "AlchemistsMaster.InitialEntersentence");
            }
		}
	}
}