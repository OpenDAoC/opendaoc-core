using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests.Integration.Server
{
	/// <summary>
	/// Unit tests for the new Training System
	/// </summary>
	[TestFixture]
	public class Train1105 : ServerTests
	{
		[Test, Explicit]
		public void TrainNow()
		{
			GamePlayer player = CreateMockGamePlayer();
			Assert.IsNotNull(player);
			player.Out.SendTrainerWindow();
			return;
		}
	}
}