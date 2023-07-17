using DOL.Language;

namespace DOL.GS
{

	public class HerbCraftingSkill : AbstractCraftingSkill
	{
		public HerbCraftingSkill()
		{
			Icon = 0x0A;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Herbcrafting");
			eSkill = eCraftingSkill.HerbalCrafting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.HerbalCrafting) < subSkillCap)
                {
                    player.GainCraftingSkill(eCraftingSkill.HerbalCrafting, 1);
                }
				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}
