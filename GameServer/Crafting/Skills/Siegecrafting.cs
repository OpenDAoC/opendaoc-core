using System;
using DOL.Language;

namespace DOL.GS;

public class Siegecrafting : AProfession
{
	public Siegecrafting()
		: base()
	{
		Icon = 0x03;
		Name = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, "Crafting.Name.Siegecraft");
		eSkill = ECraftingSkill.SiegeCrafting;
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

	public override void GainCraftingSkillPoints(GamePlayer player, RecipeMgr recipe)
	{
		if (Util.Chance(CalculateChanceToGainPoint(player, recipe.Level)))
		{
			player.GainCraftingSkill(ECraftingSkill.SiegeCrafting, 1);
			player.Out.SendUpdateCraftingSkills();
		}
	}

	public override void BuildCraftedItem(GamePlayer player, RecipeMgr recipe)
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