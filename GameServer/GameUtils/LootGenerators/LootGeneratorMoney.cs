using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// MoneyLootGenerator
	/// At the moment this generaotr only adds money to the loot
	/// </summary>
	public class LootGeneratorMoney : LootGeneratorBase
	{
		/// <summary>
        /// Generate loot for given mob
		/// </summary>
		/// <param name="mob"></param>
		/// <param name="killer"></param>
		/// <returns></returns>
		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList loot = base.GenerateLoot(mob, killer);

			int lvl = mob.Level + 1;
			if (lvl < 1) lvl = 1;
			int minLoot = 2 + ((lvl * lvl * lvl) >> 3);

			long moneyCount = minLoot + UtilCollection.Random(minLoot >> 1);
			moneyCount = (long)((double)moneyCount * ServerProperties.Properties.MONEY_DROP);

			DbItemTemplates money = new DbItemTemplates();
			money.Model = 488;
			money.Name = "bag of coins";
			money.Level = 0;

			money.Price = moneyCount;
			
			loot.AddFixed(money, 1);
			return loot;
		}
	}
}
