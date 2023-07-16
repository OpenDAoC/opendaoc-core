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
	public class WoodWorking : AbstractCraftingSkill
	{
		public WoodWorking()
		{
			Icon = 0x0E;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Woodworking");
			eSkill = eCraftingSkill.WoodWorking;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
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
