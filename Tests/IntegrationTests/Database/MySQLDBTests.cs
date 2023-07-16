using NUnit.Framework;

namespace DOL.Tests.Integration.Database.MySQL
{
	[TestFixture, Explicit]
	public class MySQLDBTests : DatabaseTests
	{
		public MySQLDBTests()
		{
			Database = MySQLDBSetUp.Database;
		}
	}
}
