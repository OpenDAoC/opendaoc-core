﻿
using System;

using DOL.GS;
using DOL.Events;

namespace GameServerScripts.Titles
{
	/// <summary>
	/// Administrator
	/// </summary>
	public class AdministratorTitle : TranslatedNoGenderGenericEventPlayerTitle
	{
		public override DOLEvent Event { get { return GamePlayerEvent.GameEntered; }}
		protected override Tuple<string, string> DescriptionValue { get { return new Tuple<string, string>("Titles.PrivLevel.Administrator", "Titles.PrivLevel.Administrator"); }}
		protected override Func<DOL.GS.GamePlayer, bool> SuitableMethod { get { return player => player.Client.Account.PrivLevel == (uint)ePrivLevel.Admin; }}
	}
	/// <summary>
	/// Game Master
	/// </summary>
	public class GamemasterTitle : TranslatedNoGenderGenericEventPlayerTitle
	{
		public override DOLEvent Event { get { return GamePlayerEvent.GameEntered; }}
		protected override Tuple<string, string> DescriptionValue { get { return new Tuple<string, string>("Titles.PrivLevel.Gamemaster", "Titles.PrivLevel.Gamemaster"); }}
		protected override Func<DOL.GS.GamePlayer, bool> SuitableMethod { get { return player => player.Client.Account.PrivLevel == (uint)ePrivLevel.GM; }}
	}
	
	public class Friend : NoGenderGenericEventPlayerTitle
	{
		public override DOLEvent Event { get { return GamePlayerEvent.GameEntered; }}
		protected override Tuple<string, string> DescriptionValue { get { return new Tuple<string, string>("My Uncle Works At Nintendo", "My Uncle Works At Nintendo"); }}
		protected override Func<DOL.GS.GamePlayer, bool> SuitableMethod { get { return player => player.GetAchievementProgress("NintendoDad") > 0; }}
	}
}
