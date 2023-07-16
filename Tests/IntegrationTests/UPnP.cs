using NUnit.Framework;

namespace DOL.Tests.Integration.Net
{
	[TestFixture]
	public class uPnP
	{
		UPnPNat upnp;
		
		public uPnP(){}
		
		[OneTimeSetUp]
		public void init()
		{
			upnp = new UPnPNat();
		}
		
		[Test, Explicit]
		public void Discover()
		{
			Assert.IsTrue(upnp.Discover());
		}
	}
}
