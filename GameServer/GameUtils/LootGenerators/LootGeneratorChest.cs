using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// LootGeneratorChest
	/// Adds money chests to a Mobs droppable loot based on a chance set in server properties
	/// </summary>
	public class LootGeneratorChest : LootGeneratorBase
	{	
		int SMALLCHEST_CHANCE = ServerProperties.ServerProperties.BASE_SMALLCHEST_CHANCE;
		int LARGECHEST_CHANCE = ServerProperties.ServerProperties.BASE_LARGECHEST_CHANCE;
		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList loot = base.GenerateLoot(mob, killer);
			int small = SMALLCHEST_CHANCE;
			int large = LARGECHEST_CHANCE;
			if (UtilCollection.Chance(small))
			{
				int lvl = mob.Level + 1;
				if (lvl < 1) lvl = 1;
				int minLoot = ServerProperties.ServerProperties.SMALLCHEST_MULTIPLIER * (lvl * lvl); 
				long moneyCount = minLoot + UtilCollection.Random(minLoot >> 1);
				moneyCount = (long)((double)moneyCount * ServerProperties.ServerProperties.MONEY_DROP);
				DbItemTemplates money = new DbItemTemplates();
				money.Model = 488;
				money.Name = "small chest";
				money.Level = 0;
				money.Price = moneyCount;
				loot.AddFixed(money, 1);
			}
			if (UtilCollection.Chance(large))
			{
				int lvl = mob.Level + 1;
				if (lvl < 1) lvl = 1;
				int minLoot = ServerProperties.ServerProperties.LARGECHEST_MULTIPLIER * (lvl * lvl); 
				long moneyCount = minLoot + UtilCollection.Random(minLoot >> 1);
				moneyCount = (long)((double)moneyCount * ServerProperties.ServerProperties.MONEY_DROP);
				DbItemTemplates money = new DbItemTemplates();
				money.Model = 488;
				money.Name = "large chest";
				money.Level = 0;
				money.Price = moneyCount;
				loot.AddFixed(money, 1);
			}
			return loot;
		}
	}
}