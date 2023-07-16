using System;
using DOL.Database;
using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	[TestFixture]
	public class DatabaseTest : ServerTests
	{				

		public DatabaseTest()
		{
		}

		[Test]
		public void TestSelect()
		{
			Console.WriteLine("TestSelect();");

			var obs = GameServer.Database.SelectAllObjects<ItemTemplate>();
			Console.WriteLine("ItemTemplates Type="+obs.GetType());

			var items = GameServer.Database.SelectAllObjects<MerchantItem>();
			Console.WriteLine("MerchantItems Type="+items.GetType());
			
		}			

	}
}
