using NUnit.Framework;

namespace DOL.Tests.Integration.Database.MySQL
{
	[TestFixture, Explicit]
	public class MySQLDBCustomParamsTest : CustomParamsTest
	{
		public MySQLDBCustomParamsTest()
		{
			Database = MySQLDBSetUp.Database;
		}
	}
}
