using NUnit.Framework;

namespace DOL.Tests.Integration.Database.MySQL
{
	[TestFixture, Explicit]
	public class MySQLDBInterfaceTests : InterfaceTests
	{
		public MySQLDBInterfaceTests()
		{
			Database = MySQLDBSetUp.Database;
		}
		
		[Test]
		public override void TestEscape()
		{
			var test = "\\\"'’";
			
			Assert.AreEqual("\\\\\\\"\\'\\’", Database.Escape(test), "MySQL String Escape Test Failure...");
		}
	}
}
