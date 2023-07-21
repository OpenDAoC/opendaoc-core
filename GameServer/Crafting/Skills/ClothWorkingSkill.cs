using DOL.Language;

namespace DOL.GS
{
	public class ClothWorkingSkill : AbstractCraftingSkill
	{
		public ClothWorkingSkill()
		{
			Icon = 0x08;
			Name = LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, "Crafting.Name.Clothworking");
			eSkill = eCraftingSkill.ClothWorking;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
		{
			if (UtilCollection.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.ClothWorking) < subSkillCap)
                {
                    player.GainCraftingSkill(eCraftingSkill.ClothWorking, 1);
                    player.Out.SendUpdateCraftingSkills();
                }
			}
		}
	}
}
