using System;
using System.IO;
using System.Reflection;

using DOL.Database;
using DOL.Database.Connection;

using NUnit.Framework;

namespace DOL.Tests.Integration.Database.MySQL
{
	[SetUpFixture]
	public class MySQLDBSetUp
	{
		public MySQLDBSetUp()
		{
		}

		public static SqlObjectDatabase Database { get; set; }
		public static string ConnectionString { get; set; }

		[OneTimeSetUp]
		public void SetUp()
		{
			var CodeBase = new FileInfo(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath).Directory;
			ConnectionString = "Server=localhost;Port=3306;Database=test_dol_database;User ID=root;Password=;Treat Tiny As Boolean=False";

			Database = (SqlObjectDatabase)ObjectDatabase.GetObjectDatabase(EConnectionType.DATABASE_MYSQL, ConnectionString);

			Console.WriteLine("DB Configured : {0}, {1}", Database.ConnectionType, ConnectionString);

			log4net.Config.BasicConfigurator.Configure(
				new log4net.Appender.ConsoleAppender
				{
					Layout = new log4net.Layout.SimpleLayout(),
					Threshold = log4net.Core.Level.Info
				});
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			log4net.LogManager.Shutdown();
		}
	}
}
