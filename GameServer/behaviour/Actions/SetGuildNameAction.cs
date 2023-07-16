using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.SetGuildName,DefaultValueQ=eDefaultValueConstants.NPC)]
    public class SetGuildNameAction : AbstractAction<string,GameNPC>
    {               
                
        public SetGuildNameAction(GameNPC defautNPC,  Object p, Object q)
            : base(defautNPC, eActionType.SetGuildName, p, q)
        { }

        public SetGuildNameAction(GameNPC defaultNPC, string guildName, GameNPC npc)
            : this(defaultNPC, (object)guildName, (object)npc) { }

        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {            
            Q.GuildName = P;
        }
    }
}
