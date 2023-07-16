using System;

using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	[TestFixture]
	public class ServerTest
	{
		public ServerTest()
		{
		}
		
		[Test]
		public void TestGameServerStartup()
		{
			Console.WriteLine("Test GameServer Startup...");
			Assert.NotNull(GameServer.Instance);
			Assert.IsTrue(GameServer.Instance.IsRunning);
			Assert.AreEqual(GameServer.Instance.ServerStatus, eGameServerStatus.GSS_Open);
		}
	}
}
