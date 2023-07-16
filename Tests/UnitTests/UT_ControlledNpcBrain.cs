using DOL.AI.Brain;
using NUnit.Framework;

namespace DOL.Tests.Unit.Gameserver
{  
    [TestFixture]
    class UT_ControlledNpcBrain
    {
        [Test]
        public void GetPlayerOwner_InitWithPlayer_Player()
        {
            var player = new FakePlayer();
            var brain = new ControlledNpcBrain(player);

            var actual = brain.GetPlayerOwner();

            var expected = player;
            Assert.AreEqual(expected, actual);
        }
    }
}
