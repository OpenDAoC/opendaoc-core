using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS
{
    public class TailoringSkill : AbstractProfession
    {
        public TailoringSkill()
        {
            Icon = 0x0B;
            Name = LanguageMgr.GetTranslation(ServerProperties.ServerProperties.SERV_LANGUAGE, 
                "Crafting.Name.Tailoring");
            eSkill = eCraftingSkill.Tailoring;
        }

        protected override String Profession
        {
            get
            {
                return "CraftersProfession.Tailor";
            }
        }

		protected override bool CheckForTools(GamePlayer player, RecipeMgr recipe)
		{
            bool needForge = false;

            foreach (var ingredient in recipe.Ingredients)
            {
                if (ingredient.Material.Model == 519) // metal bar
                {
                    needForge = true;
                    break;
                }
            }

            if (needForge)
            {
                foreach (GameStaticItem item in player.GetItemsInRadius(CRAFT_DISTANCE))
                {
                    if (item.Name == "forge" || item.Model == 478) // Forge
                        return true;
                }

                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Crafting.CheckTool.NotHaveTools", recipe.Product.Name), EChatType.CT_System, EChatLoc.CL_SystemWindow);
                player.Out.SendMessage(LanguageMgr.GetTranslation(ServerProperties.ServerProperties.DB_LANGUAGE, "Crafting.CheckTool.FindForge"), EChatType.CT_System, EChatLoc.CL_SystemWindow);

				if (player.Client.Account.PrivLevel > 1)
					return true;

				return false;
            }

            return true;
        }

        public override int GetSecondaryCraftingSkillMinimumLevel(RecipeMgr recipe)
        {
            switch (recipe.Product.Object_Type)
            {
                case (int)EObjectType.Cloth:
                case (int)EObjectType.Leather:
                case (int)EObjectType.Studded:
                    return recipe.Level - 30;
            }

            return base.GetSecondaryCraftingSkillMinimumLevel(recipe);
        }

		public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
        {
            if (UtilCollection.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
            {
                player.GainCraftingSkill(eCraftingSkill.Tailoring, 1);
                base.GainCraftingSkillPoints(player, recipe);
                player.Out.SendUpdateCraftingSkills();
            }
        }
    }
}
