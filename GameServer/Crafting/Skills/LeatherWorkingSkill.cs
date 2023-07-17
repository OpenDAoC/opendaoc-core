using DOL.Language;

namespace DOL.GS
{

	public class LeatherWorkingSkill : AbstractCraftingSkill
	{

		public LeatherWorkingSkill()
		{
			Icon = 0x07;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Leathercrafting");
			eSkill = eCraftingSkill.LeatherCrafting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.LeatherCrafting) < subSkillCap)
                {
                    player.GainCraftingSkill(eCraftingSkill.LeatherCrafting, 1);
                }
				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}
