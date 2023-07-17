
using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Minion Rescue RA
    /// </summary>
    public class CallOfDarknessAbility : RR5RealmAbility
    {
        public const int DURATION = 60 * 1000;

        public CallOfDarknessAbility(DbAbilities dba, int level) : base(dba, level) { }

        /// <summary>
        /// Action
        /// </summary>
        /// <param name="living"></param>
        public override void Execute(GameLiving living)
        {
            if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;

            GamePlayer player = living as GamePlayer;
            if (player != null)
            {
            	CallOfDarknessEffect CallOfDarkness = player.EffectList.GetOfType<CallOfDarknessEffect>();
                if (CallOfDarkness != null)
                    CallOfDarkness.Cancel(false);

                new CallOfDarknessEffect().Start(player);
            }
            DisableSkill(living);
        }

        public override int GetReUseDelay(int level)
        {
            return 900;
        }

        public override void AddEffectsInfo(IList<string> list)
        {
            list.Add("Summon pet in 3 seconds, 1 minute duration, 15 minute RUT.");
            list.Add("");
            list.Add("Target: Self");
            list.Add("Duration: 1 min");
            list.Add("Casting time: Instant");
        }

    }
}
