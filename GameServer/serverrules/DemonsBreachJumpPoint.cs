using System;

using DOL.Database;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.Language;

namespace DOL.GS.ServerRules
{
	public class NergalsBreachJumpPoint : IJumpPointHandler
	{
		public bool IsAllowedToJump(ZonePoint targetPoint, GamePlayer player)
		{
			if(player.Client.Account.PrivLevel > 1)
            {
                return true;
            }
            if(player.Level < 5)
			{
				return true;
			}
            player.Client.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "DemonsBreachJumpPoint.Requirements"), eChatType.CT_System, eChatLoc.CL_ChatWindow);
			return false;
		}
		
	}

	public class BalbansBreachJumpPoint : IJumpPointHandler
	{
		public bool IsAllowedToJump(ZonePoint targetPoint, GamePlayer player)
		{
            if (player.Client.Account.PrivLevel > 1)
            {
                return true;
            }
            if (player.Level < 10 && player.Level > 4)
			{
				return true;
			}
            player.Client.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "DemonsBreachJumpPoint.Requirements"), eChatType.CT_System, eChatLoc.CL_ChatWindow);
			return false;
		}
	}
}
