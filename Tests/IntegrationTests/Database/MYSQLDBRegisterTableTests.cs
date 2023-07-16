﻿using DOL.Database.Connection;
using DOL.Database.Attributes;

using NUnit.Framework;
using DOL.Database;

namespace DOL.Tests.Integration.Database.MySQL
{
	[TestFixture, Explicit]
	public class MYSQLDBRegisterTableTests : RegisterTableTests
	{
		public MYSQLDBRegisterTableTests()
		{
			Database = MySQLDBSetUp.Database;
		}
		
		protected override SQLObjectDatabase GetDatabaseV2 { get { return (SQLObjectDatabase)ObjectDatabase.GetObjectDatabase(ConnectionType.DATABASE_MYSQL, MySQLDBSetUp.ConnectionString); } }
	
		[Test]
		public void TestTableWithBrokenPrimaryKey()
		{
			// Destroy previous table
			Database.ExecuteNonQuery(string.Format("DROP TABLE IF EXISTS `{0}`", AttributesUtils.GetTableName(typeof(TestTableWithBrokenPrimaryV1))));
			// Create Table
			Database.RegisterDataObject(typeof(TestTableWithBrokenPrimaryV1));
			// Break Primary Key
			Database.ExecuteNonQuery(string.Format("ALTER TABLE `{0}` DROP PRIMARY KEY", AttributesUtils.GetTableName(typeof(TestTableWithBrokenPrimaryV1))));
			
			// Get a new Database Object to Trigger Migration
			var DatabaseV2 = GetDatabaseV2;
			
			// Trigger False Migration
			DatabaseV2.RegisterDataObject(typeof(TestTableWithBrokenPrimaryV2));
			
			var adds = DatabaseV2.AddObject(new [] {
			                                	new TestTableWithBrokenPrimaryV2 { PrimaryKey = 1 },
			                                	new TestTableWithBrokenPrimaryV2 { PrimaryKey = 1 },
			                                });
			
			Assert.IsFalse(adds, "Primary Key was not restored and duplicate key were inserted !");
		}
	}
}
