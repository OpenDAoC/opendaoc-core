using DOL.Database;
using DOL.Language;
using System.Collections.Generic;

namespace DOL.GS
{
	public class ClothWorking : AbstractCraftingSkill
	{
		public ClothWorking()
		{
			Icon = 0x08;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Clothworking");
			eSkill = eCraftingSkill.ClothWorking;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
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
