
using System;
using System.Collections;
using System.Collections.Generic;
using DOL.Events;

namespace DOL.GS.Effects
{

    public class BlissfulIgnoranceEffect : TimedEffect
    {
        private GamePlayer EffectOwner;

        public BlissfulIgnoranceEffect()
            : base(RealmAbilities.BlissfulIgnoranceAbility.DURATION)
        { }

        public override void Start(GameLiving target)
        {
            base.Start(target);
            if (target is GamePlayer)
            {
                EffectOwner = target as GamePlayer;
                foreach (GamePlayer p in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                {
                    p.Out.SendSpellEffectAnimation(EffectOwner, p, 7069, 0, false, 1);
                }
                GameEventMgr.AddHandler(EffectOwner, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLeftWorld));
            }
        }
        public override void Stop()
        {
            if (EffectOwner != null)
                GameEventMgr.RemoveHandler(EffectOwner, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLeftWorld));

            base.Stop();
        }

        /// <summary>
        /// Called when a player leaves the game
        /// </summary>
        /// <param name="e">The event which was raised</param>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">EventArgs associated with the event</param>
        protected void PlayerLeftWorld(DOLEvent e, object sender, EventArgs args)
        {
 			Cancel(false);
        }

        public override string Name { get { return "Blissful Ignorance"; } }
        public override ushort Icon { get { return 3068; } }

        // Delve Info
        public override IList<string> DelveInfo
        {
            get
            {
                var list = new List<string>();
                list.Add("No penality Hit from self buffs.");
                return list;
            }
        }
    }
}

