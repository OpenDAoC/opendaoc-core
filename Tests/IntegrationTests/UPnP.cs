using NUnit.Framework;

namespace DOL.Tests.Integration.Net
{
	[TestFixture]
	public class uPnP
	{
		UpnpNat upnp;
		
		public uPnP(){}
		
		[OneTimeSetUp]
		public void init()
		{
			upnp = new UpnpNat();
		}
		
		[Test, Explicit]
		public void Discover()
		{
			Assert.IsTrue(upnp.Discover());
		}
	}
}
