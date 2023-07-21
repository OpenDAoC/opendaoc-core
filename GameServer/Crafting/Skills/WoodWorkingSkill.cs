using DOL.Language;

namespace DOL.GS
{
	public class WoodWorkingSkill : AbstractCraftingSkill
	{
		public WoodWorkingSkill()
		{
			Icon = 0x0E;
			Name = LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "Crafting.Name.Woodworking");
			eSkill = eCraftingSkill.WoodWorking;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
		{
			if (UtilCollection.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.WoodWorking) < subSkillCap)
                {
                    player.GainCraftingSkill(eCraftingSkill.WoodWorking, 1);
                }
				player.Out.SendUpdateCraftingSkills();
			}

		}
	}
}
