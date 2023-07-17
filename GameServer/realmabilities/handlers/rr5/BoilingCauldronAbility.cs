using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Boiling Cauldron RA
    /// </summary>
    public class BoilingCauldronAbility : RR5RealmAbility
    {
        public const int DURATION = 4500;

        public BoilingCauldronAbility(DbAbilities dba, int level) : base(dba, level) { }

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
            	BoilingCauldronEffect BoilingCauldron = player.EffectList.GetOfType<BoilingCauldronEffect>();
                if (BoilingCauldron != null)
                    BoilingCauldron.Cancel(false);

                new BoilingCauldronEffect().Start(player);
            }
            DisableSkill(living);
        }

        public override int GetReUseDelay(int level)
        {
            return 900;
        }

        public override void AddEffectsInfo(IList<string> list)
        {
            list.Add("Summon a cauldron that boil in place for 3.5s before spilling and doing damage to all those nearby. 15min RUT.");
            list.Add("");
            list.Add("Target: Enemy");
            list.Add("Duration: 3.5s");
            list.Add("Casting time: Instant");
        }

    }
}
