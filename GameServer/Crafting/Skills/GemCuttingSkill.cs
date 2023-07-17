using DOL.Language;

namespace DOL.GS
{
	public class GemCuttingSkill : AbstractCraftingSkill
	{
		public GemCuttingSkill()
		{
			Icon = 0x09;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Gemcutting");
			eSkill = eCraftingSkill.GemCutting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
		{
			if (UtilCollection.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.GemCutting) < subSkillCap)
                {
                    player.GainCraftingSkill(eCraftingSkill.GemCutting, 1);
                    player.Out.SendUpdateCraftingSkills();
                }
			}
		}
	}
}
