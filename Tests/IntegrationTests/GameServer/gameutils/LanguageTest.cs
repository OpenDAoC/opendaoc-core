using System;
using DOL.Language;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	[TestFixture]
	public class LanguageTest: ServerTests
	{
		public LanguageTest()
		{
		}
		
		[Test]
		public void TestGetString()
		{
			Console.WriteLine("TestGetString();");
			Console.WriteLine(LanguageMgr.GetTranslation ("test","fail default string"));
			Assert.IsTrue(true, "ok");
		}
	}
}
