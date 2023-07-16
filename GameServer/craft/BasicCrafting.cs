using System;
using System.Collections.Generic;

using DOL.Database;
using DOL.Language;

namespace DOL.GS
{
	public class BasicCrafting : AbstractProfession
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BasicCrafting()
		{
			Icon = 0x0F;
            Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.BasicCrafting");
            eSkill = eCraftingSkill.BasicCrafting;
		}

        protected override String Profession
        {
            get
            {
                return "CraftersProfession.BasicCrafter";
            }
        }

        public override string CRAFTER_TITLE_PREFIX
		{
			get
			{
				return "Crafter's";
            }
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
				player.GainCraftingSkill(eCraftingSkill.BasicCrafting, 1);
				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}
