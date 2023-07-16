using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;
using System;
using System.Collections.Generic;

namespace DOL.GS
{
	public class SiegeCrafting : AbstractProfession
	{
		public SiegeCrafting()
			: base()
		{
			Icon = 0x03;
			Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Siegecraft");
			eSkill = eCraftingSkill.SiegeCrafting;
		}
		public override string CRAFTER_TITLE_PREFIX
		{
			get
			{
				return "Siegecrafter";
			}
		}

        protected override String Profession
        {
            get
            {
                return "CraftersProfession.Siegecrafter";
            }
        }

		public override void GainCraftingSkillPoints(GamePlayer player, Recipe recipe)
		{
			if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
			{
				player.GainCraftingSkill(eCraftingSkill.SiegeCrafting, 1);
				player.Out.SendUpdateCraftingSkills();
			}
		}

		public override void BuildCraftedItem(GamePlayer player, Recipe recipe)
		{
			var product = recipe.Product;
			GameSiegeWeapon siegeweapon;
			switch ((eObjectType)product.Object_Type)
			{
				case eObjectType.SiegeBalista:
					{
						siegeweapon = new GameSiegeBallista();
					}
					break;
				case eObjectType.SiegeCatapult:
					{
						siegeweapon = new GameSiegeCatapult();
					}
					break;
				case eObjectType.SiegeCauldron:
					{
						siegeweapon = new GameSiegeCauldron();
					}
					break;
				case eObjectType.SiegeRam:
					{
						siegeweapon = new GameSiegeRam();
					}
					break;
				case eObjectType.SiegeTrebuchet:
					{
						siegeweapon = new GameSiegeTrebuchet();
					}
					break;
				default:
					{
						base.BuildCraftedItem(player, recipe);
						return;
					}
			}

			//actually stores the Id_nb of the siegeweapon
			siegeweapon.ItemId = product.Id_nb;

			siegeweapon.LoadFromDatabase(product);
			siegeweapon.CurrentRegion = player.CurrentRegion;
			siegeweapon.Heading = player.Heading;
			siegeweapon.X = player.X;
			siegeweapon.Y = player.Y;
			siegeweapon.Z = player.Z;
			siegeweapon.Realm = player.Realm;
			siegeweapon.AddToWorld();
		}
	}
}
