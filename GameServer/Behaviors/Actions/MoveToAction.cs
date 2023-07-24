using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
	[ActionAttribute(ActionType = EActionType.MoveTo)]
	public class MoveToAction : AbstractAction<GameLocation,GameLiving>
	{

		public MoveToAction(GameNpc defaultNPC,  Object p, Object q)
			: base(defaultNPC, EActionType.MoveTo, p, q)
		{ }

		public MoveToAction(GameNpc defaultNPC, GameLocation location, GameLiving npc)
			: this(defaultNPC, (object)location,(object) npc) { }
		
		public override void Perform(CoreEvent e, object sender, EventArgs args)
		{
			GameLiving npc = Q;

			if (P is GameLocation)
			{
				GameLocation location = (GameLocation)P;
				npc.MoveTo(location.RegionID, location.X, location.Y, location.Z, location.Heading);
			}
			else
			{
				GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
				npc.MoveTo(player.CurrentRegionID, player.X, player.Y, player.Z, (ushort)player.Heading);
			}
		}
	}
}
