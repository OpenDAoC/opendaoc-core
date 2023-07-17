using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.Teleport,DefaultValueQ=0)]
    public class TeleportAction : AbstractAction<GameLocation,int>
    {               

        public TeleportAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.Teleport, p, q)
        {            
            }


        public TeleportAction(GameNPC defaultNPC,  GameLocation location, int fuzzyRadius)
            : this(defaultNPC,  (object)location, (object)fuzzyRadius) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            GameLocation location = P;
            int radius = Q;

            if (location.Name != null)
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.TeleportAction.TeleportedToLoc", player, location.Name), eChatType.CT_System, eChatLoc.CL_SystemWindow);
            }

            location.X += Util.Random(-radius, radius);
            location.Y += Util.Random(-radius, radius);
            player.MoveTo(location.RegionID, location.X, location.Y, location.Z, location.Heading);
        }
    }
}
