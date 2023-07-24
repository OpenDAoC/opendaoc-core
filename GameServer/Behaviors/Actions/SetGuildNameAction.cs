using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = EActionType.SetGuildName,DefaultValueQ=EDefaultValueConstants.NPC)]
    public class SetGuildNameAction : AbstractAction<string,GameNpc>
    {               
                
        public SetGuildNameAction(GameNpc defautNPC,  Object p, Object q)
            : base(defautNPC, EActionType.SetGuildName, p, q)
        { }

        public SetGuildNameAction(GameNpc defaultNPC, string guildName, GameNpc npc)
            : this(defaultNPC, (object)guildName, (object)npc) { }

        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {            
            Q.GuildName = P;
        }
    }
}
