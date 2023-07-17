using System;
using System.Collections.Generic;
using System.Reflection;

using log4net;

using DOL.Database;
using DOL.GS.ServerProperties;

namespace DOL.GS
{
    public class RecipeMgr
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Ingredient[] ingredients;

        public DbItemTemplates Product { get; }
        public eCraftingSkill RequiredCraftingSkill { get; }
        public int Level { get; }
        public List<Ingredient> Ingredients => new List<Ingredient>(ingredients);
        public bool IsForUniqueProduct { get; private set; } = false;

        public RecipeMgr(DbItemTemplates product, List<Ingredient> ingredients)
        {
            this.ingredients = ingredients.ToArray();
            Product = product;
        }

        public RecipeMgr(DbItemTemplates product, List<Ingredient> ingredients, eCraftingSkill requiredSkill, int level, bool makeTemplated)
            : this(product, ingredients)
        {
            RequiredCraftingSkill = requiredSkill;
            Level = level;
            IsForUniqueProduct = !makeTemplated;
        }

        public long CostToCraft
        {
            get
            {
                long result = 0;
                foreach (var ingredient in ingredients)
                {
                    result += ingredient.Cost;
                }
                return result;
            }
        }

        public void SetRecommendedProductPriceInDB()
        {
            var product = Product;
            var totalPrice = CostToCraft;
            bool updatePrice = !(product.Name.EndsWith("metal bars") ||
                                 product.Name.EndsWith("leather square") ||
                                 product.Name.EndsWith("cloth square") ||
                                 product.Name.EndsWith("wooden boards"));

            if (product.PackageID.Contains("NoPriceUpdate"))
                updatePrice = false;

            if (updatePrice)
            {
                long pricetoset;
                var secondaryCraftingSkills = new List<eCraftingSkill>() { 
                    eCraftingSkill.MetalWorking, eCraftingSkill.LeatherCrafting, eCraftingSkill.ClothWorking, eCraftingSkill.WoodWorking
                };

                if (secondaryCraftingSkills.Contains(RequiredCraftingSkill))
                    pricetoset = Math.Abs((long)(totalPrice * 2 * Properties.CRAFTING_SECONDARYCRAFT_SELLBACK_PERCENT) / 100);
                else
                    pricetoset = Math.Abs(totalPrice * 2 * Properties.CRAFTING_SELLBACK_PERCENT / 100);

                if (pricetoset > 0 && product.Price != pricetoset)
                {
                    long currentPrice = product.Price;
                    product.Price = pricetoset;
                    product.AllowUpdate = true;
                    product.Dirty = true;
                    product.Id_nb = product.Id_nb.ToLower();
                    if (GameServer.Database.SaveObject(product))
                        log.Warn("Craft Price Correction: " + product.Id_nb + " rawmaterials price= " + totalPrice + " Current Price= " + currentPrice + ". Corrected price to= " + pricetoset);
                    else
                        log.Warn("Craft Price Correction Not SAVED: " + product.Id_nb + " rawmaterials price= " + totalPrice + " Current Price= " + currentPrice + ". Corrected price to= " + pricetoset);
                    GameServer.Database.UpdateInCache<DbItemTemplates>(product.Id_nb);
                    product.Dirty = false;
                    product.AllowUpdate = false;
                }
            }
        }
    }

    public class Ingredient
    {
        public int Count { get; }
        public DbItemTemplates Material { get; }

        public Ingredient(int count, DbItemTemplates ingredient)
        {
            Count = count;
            Material = ingredient;
        }

        public long Cost => Count * Material.Price;
    }

    public class RecipeDB
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<ushort, RecipeMgr> recipeCache = new Dictionary<ushort, RecipeMgr>();

        public static RecipeMgr FindBy(ushort recipeDatabaseID)
        {
            RecipeMgr recipe;
            recipeCache.TryGetValue(recipeDatabaseID, out recipe);
            if (recipe != null)
            {
                //avoid repeated DB access for invalid recipes
                if (recipe.Product != null) return recipeCache[recipeDatabaseID];
                else throw new KeyNotFoundException("Recipe is marked as invalid. Check your logs for Recipe with ID " + recipeDatabaseID + ".");
            }

            try
            {
                recipe = LoadFromDB(recipeDatabaseID);
                return recipe;
            }
            catch (Exception e)
            {
                log.Error(e);
                recipe = NullRecipe;
                return recipe;
            }
            finally
            {
                if (Properties.CRAFTING_ADJUST_PRODUCT_PRICE)
                    recipe.SetRecommendedProductPriceInDB();
                recipeCache[recipeDatabaseID] = recipe;
            }

        }

        private static RecipeMgr NullRecipe => new RecipeMgr(null, null);

        private static RecipeMgr LoadFromDB(ushort recipeDatabaseID)
        {

            string craftingDebug = "";
            
            var dbRecipe = GameServer.Database.FindObjectByKey<DbCraftedItems>(recipeDatabaseID.ToString());
            if (dbRecipe == null)
            {
                craftingDebug = "[CRAFTING] No DBCraftedItem with ID " + recipeDatabaseID + " exists.";
                log.Warn(craftingDebug);
                return null;
                //throw new ArgumentException(craftingDebug);
            }
                
            

            DbItemTemplates product = GameServer.Database.FindObjectByKey<DbItemTemplates>(dbRecipe.Id_nb);
            if (product == null)
            {
                craftingDebug = "[CRAFTING] ItemTemplate " + dbRecipe.Id_nb + " for Recipe with ID " + dbRecipe.CraftedItemID + " does not exist.";
                log.Warn(craftingDebug);
                return null;
                //throw new ArgumentException(craftingDebug);
            }

            var rawMaterials = CoreDb<DbCraftedXItems>.SelectObjects(DB.Column("CraftedItemId_nb").IsEqualTo(dbRecipe.Id_nb));
            if (rawMaterials.Count == 0)
            {
                craftingDebug = "[CRAFTING] Recipe with ID " + dbRecipe.CraftedItemID + " has no ingredients.";
                log.Warn(craftingDebug);
                return null;
                //throw new ArgumentException(craftingDebug);
            }

            bool isRecipeValid = true;

            var ingredients = new List<Ingredient>();
            foreach (DbCraftedXItems material in rawMaterials)
            {
                DbItemTemplates template = GameServer.Database.FindObjectByKey<DbItemTemplates>(material.IngredientId_nb);

                if (template == null)
                {
                    craftingDebug = "[CRAFTING] Cannot find raw material ItemTemplate: " + material.IngredientId_nb + ") needed for recipe: " + dbRecipe.CraftedItemID + "\n";
                    isRecipeValid = false;
                }
                ingredients.Add(new Ingredient(material.Count, template));
            }

            if (!isRecipeValid)
            {
                log.Warn(craftingDebug);
                return null;
                //throw new ArgumentException(errorText);
            }

            var recipe = new RecipeMgr(product, ingredients, (eCraftingSkill)dbRecipe.CraftingSkillType, dbRecipe.CraftingLevel, dbRecipe.MakeTemplated);
            return recipe;
        }
    }
}
