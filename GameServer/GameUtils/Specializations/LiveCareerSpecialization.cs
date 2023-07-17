using System;
using System.Collections.Generic;

namespace DOL.GS
{
	/// <summary>
	/// Description of LiveCareerSpecialization.
	/// </summary>
	public class LiveCareerSpecialization : CareerSpecialization
	{
		public LiveCareerSpecialization(string keyname, string displayname, ushort icon, int ID)
			: base(keyname, displayname, icon, ID)
		{
		}
	}
	
	/// <summary>
	/// Special Career for handling base ability and special Event For all players (sprint for example)
	/// </summary>
	public class LiveWideCareerSpecialization : LiveCareerSpecialization
	{
		public LiveWideCareerSpecialization(string keyname, string displayname, ushort icon, int ID)
			: base(keyname, displayname, icon, ID)
		{
		}
	}
}
