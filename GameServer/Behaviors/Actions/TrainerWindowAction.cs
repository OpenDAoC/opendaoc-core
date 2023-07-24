using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;
using DOL.Database;
using DOL.AI.Brain;
using log4net;
using System.Reflection;

namespace DOL.GS.Behaviour.Actions
{
	[ActionAttribute(ActionType = EActionType.TrainerWindow, IsNullableP = true)]
	public class TrainerWindowAction : AbstractAction<Nullable<Int32>, GameNpc>
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public TrainerWindowAction(GameNpc defaultNPC)
			: base(defaultNPC, EActionType.TrainerWindow)
		{
		}
		public override void Perform(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
			if (player != null)
				player.Out.SendTrainerWindow();
		}
	}
}
