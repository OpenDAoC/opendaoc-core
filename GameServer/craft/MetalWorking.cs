using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;
using System.Collections.Generic;

namespace DOL.GS
{
	public class MetalWorking : AbstractCraftingSkill
	{
		public MetalWorking()
		{
			Icon = 0x06;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, 
				"Crafting.Name.Metalworking");
			eSkill = eCraftingSkill.MetalWorking;
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

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
                if (player.GetCraftingSkillValue(eCraftingSkill.MetalWorking) < subSkillCap)
                    player.GainCraftingSkill(eCraftingSkill.MetalWorking, 1);

				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}
