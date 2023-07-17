using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
	[ActionAttribute(ActionType = EActionType.PlaySound, IsNullableQ=true)]
	public class PlaySoundAction : AbstractAction<ushort, eSoundType>
	{

		public PlaySoundAction(GameNPC defaultNPC, Object p, Object q)
			: base(defaultNPC, EActionType.PlaySound, p, q) { }


		public PlaySoundAction(GameNPC defaultNPC, ushort id, eSoundType type)
			: this(defaultNPC, (object)id, (object)type) { }


		public PlaySoundAction(GameNPC defaultNPC, ushort id)
			: this(defaultNPC, (object)id, (object)eSoundType.Divers) { }


		public override void Perform(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
			ushort message = P;
			player.Out.SendPlaySound(Q, P);
		}
	}
}