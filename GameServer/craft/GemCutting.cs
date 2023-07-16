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
	public class GemCutting : AbstractCraftingSkill
	{
		public GemCutting()
		{
			Icon = 0x09;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Gemcutting");
			eSkill = eCraftingSkill.GemCutting;
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
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
