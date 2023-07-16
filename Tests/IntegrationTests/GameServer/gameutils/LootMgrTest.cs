using System;
using DOL.Database;
using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	/// <summary>
	/// Unit test for the LootMgr Class
	/// </summary>
	[TestFixture]
	public class LootManagerTest : ServerTests
	{
		public LootManagerTest()
		{
			
		}

		[Test] 
		public void TestLootGenerator()
		{						
			GameNPC mob = new GameNPC();
			mob.Level = 6;
			mob.Name="impling";

			for (int i=0;i< 15; i++) 
			{
				Console.WriteLine("Loot "+i);
				ItemTemplate[] loot = LootMgr.GetLoot(mob, null);
				foreach (ItemTemplate item in loot)
				{
					Console.WriteLine(mob.Name+" drops "+item.Name);
				}	
			}
			
			Console.WriteLine("Drops finished");
		}
	}
}
