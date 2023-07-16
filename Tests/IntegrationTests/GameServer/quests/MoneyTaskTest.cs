using System;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.Quests;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	/// <summary>
	/// Unit Test for Money Task.
	/// </summary>
	[TestFixture]
	public class MoneyTaskTest : ServerTests
	{
		public MoneyTaskTest()
		{
		}

		[Test, Explicit]
		public void CreateMoneyTask()
		{
			GamePlayer player = CreateMockGamePlayer();

			GameMerchant merchant = new GameMerchant();
			merchant.Name = "Tester";
			merchant.Realm = eRealm.Albion;
			Console.WriteLine(player.Name);

			if (MoneyTask.CheckAvailability(player, merchant))
			{
				if (MoneyTask.BuildTask(player, merchant))
				{
					MoneyTask task = (MoneyTask)player.Task;


					Assert.IsNotNull(task);
					Console.WriteLine("XP" + task.RewardXP);
					Console.WriteLine("Item:" + task.ItemName);
					Console.WriteLine("Item:" + task.Name);
					Console.WriteLine("Item:" + task.Description);

					// Check Notify Event handling
					InventoryItem item = GameInventoryItem.Create(new ItemTemplate());
					item.Name = task.ItemName;

					GameNPC npc = new GameNPC();
					npc.Name = task.ReceiverName;
					task.Notify(GamePlayerEvent.GiveItem, player, new GiveItemEventArgs(player, npc, item));

					if (player.Task.TaskActive || player.Task == null)
						Assert.Fail("Task did not finished proper in Notify");
				}
			}
		}
	}
}