using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.Animation, IsNullableQ = true)]
    public class AnimationAction : AbstractAction<eEmote,GameLiving>
    {               

        public AnimationAction(GameNPC defaultNPC, Object p, Object q)
            : base(defaultNPC, eActionType.Animation, p, q) { }
        

        public AnimationAction(GameNPC defaultNPC, eEmote emote, GameLiving actor)
            : this(defaultNPC, (object) emote, (object)actor) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);

            GameLiving actor = Q != null ? Q : player;

            foreach (GamePlayer nearPlayer in actor.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            {
                nearPlayer.Out.SendEmoteAnimation(actor, P);
            }
            
        }
    }
}
