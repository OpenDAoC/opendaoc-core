using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS
{

	public class LeatherCrafting : AbstractCraftingSkill
	{

		public LeatherCrafting()
		{
			Icon = 0x07;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Leathercrafting");
			eSkill = eCraftingSkill.LeatherCrafting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
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
