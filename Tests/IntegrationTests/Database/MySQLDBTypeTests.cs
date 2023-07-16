using NUnit.Framework;

namespace DOL.Tests.Integration.Database.MySQL
{
	[TestFixture, Explicit]
	public class MySQLDatabaseTypeTests : DatabaseTypeTests
	{
		public MySQLDatabaseTypeTests()
		{
			Database = MySQLDBSetUp.Database;
		}
	}
}
