using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	/// <summary>
	/// Unit Test for Default Invalid Names Startup Behavior.
	/// Need Ressource InvalidNames.txt set to Default.
	/// </summary>
	[TestFixture]
	public class InvalidNamesStartupTest
	{
		public InvalidNamesStartupTest()
		{
		}
		
		[Test]
		public void InvalidNamesStartup_CheckDefaultConstraintOneString_Match()
		{
			Assert.IsTrue(GameServer.Instance.PlayerManager.InvalidNames["fuck"]);
		}
		
		[Test]
		public void InvalidNamesStartup_CheckDefaultConstraintOneString_NoMatch()
		{
			Assert.IsFalse(GameServer.Instance.PlayerManager.InvalidNames["unicorn"]);
		}
		
		[Test]
		public void InvalidNamesStartup_CheckDefaultConstraintTwoString_Match()
		{
			Assert.IsTrue(GameServer.Instance.PlayerManager.InvalidNames["fu", "ck"]);
		}
		
		[Test]
		public void InvalidNamesStartup_CheckDefaultConstraintTwoString_NoMatch()
		{
			Assert.IsFalse(GameServer.Instance.PlayerManager.InvalidNames["uni", "corn"]);
		}
	}
}
