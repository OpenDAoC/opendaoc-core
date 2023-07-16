using System;
using System.IO;
using System.Reflection;

using DOL.Database;
using DOL.Database.Connection;

using NUnit.Framework;

namespace DOL.Tests.Integration.Database
{
	[SetUpFixture]
	public class DatabaseSetUp
	{
		public DatabaseSetUp()
		{
		}

		public static SQLObjectDatabase Database { get; set; }
		public static string ConnectionString { get; set; }

		[OneTimeSetUp]
		public void SetUp()
		{
			var CodeBase = new FileInfo(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath).Directory;
			ConnectionString = string.Format("Data Source={0};Version=3;Pooling=False;Cache Size=1073741824;Journal Mode=Off;Synchronous=Off;Foreign Keys=True;Default Timeout=60",
												 Path.Combine(CodeBase.Parent.FullName, "dol-database-tests-only.sqlite3.db"));

			Database = (SQLObjectDatabase)ObjectDatabase.GetObjectDatabase(ConnectionType.DATABASE_SQLITE, ConnectionString);

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
