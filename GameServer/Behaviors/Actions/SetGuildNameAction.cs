using System;
using Core.Events;
using Core.GS.Behaviour;
using Core.GS.Enums;

namespace Core.GS.Behaviors;

[Action(ActionType = EActionType.SetGuildName,DefaultValueQ=EDefaultValueConstants.NPC)]
public class SetGuildNameAction : AAction<string,GameNpc>
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