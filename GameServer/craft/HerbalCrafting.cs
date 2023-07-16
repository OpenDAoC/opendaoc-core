using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;
using log4net;

namespace DOL.GS
{

	public class HerbalCrafting : AbstractCraftingSkill
	{
		public HerbalCrafting()
		{
			Icon = 0x0A;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Herbcrafting");
			eSkill = eCraftingSkill.HerbalCrafting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
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
