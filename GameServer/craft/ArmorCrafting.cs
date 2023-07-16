using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;
using System;
using System.Collections.Generic;

namespace DOL.GS
{
	public class ArmorCrafting : AbstractProfession
	{
		public ArmorCrafting()
		{
			Icon = 0x02;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, 
                "Crafting.Name.Armorcraft");
			eSkill = eCraftingSkill.ArmorCrafting;
		}

        protected override String Profession
        {
            get
            {
                return "CraftersProfession.Armorer";
            }
        }

		protected override bool CheckForTools(GamePlayer player, Recipe recipe)
		{
			foreach (GameStaticItem item in player.GetItemsInRadius(CRAFT_DISTANCE))
			{
				if (item.Name.ToLower() == "forge" || item.Model == 478) // Forge
					return true;
			}

			player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Crafting.CheckTool.NotHaveTools", recipe.Product.Name), eChatType.CT_System, eChatLoc.CL_SystemWindow);
			player.Out.SendMessage(LanguageMgr.GetTranslation(ServerProperties.Properties.DB_LANGUAGE, "Crafting.CheckTool.FindForge"), eChatType.CT_System, eChatLoc.CL_SystemWindow);

			if (player.Client.Account.PrivLevel > 1)
				return true;

			return false;
		}

		public override int GetSecondaryCraftingSkillMinimumLevel(Recipe recipe)
		{
			switch(recipe.Product.Object_Type)
			{
				case (int)eObjectType.Studded:
				case (int)eObjectType.Chain:
				case (int)eObjectType.Plate:
				case (int)eObjectType.Reinforced:
				case (int)eObjectType.Scale:
					return recipe.Level - 60;
			}

			return base.GetSecondaryCraftingSkillMinimumLevel(recipe);
		}

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance( CalculateChanceToGainPoint(player, recipe.Level)))
			{
				player.GainCraftingSkill(eCraftingSkill.ArmorCrafting, 1);
                base.GainCraftingSkillPoints(player, recipe);
				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}
