
using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Soldier's Citadel RA
    /// </summary>
    public class SoldiersCitadelAbility : RR5RealmAbility
    {
        public const int DURATION = 30 * 1000;
        public const int SECOND_DURATION = 15 * 1000;

        public SoldiersCitadelAbility(DBAbility dba, int level) : base(dba, level) { }

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
            	SoldiersCitadelEffect SoldiersCitadel = player.EffectList.GetOfType<SoldiersCitadelEffect>();
                if (SoldiersCitadel != null)
                    SoldiersCitadel.Cancel(false);

                new SoldiersCitadelEffect().Start(player);
            }
            DisableSkill(living);
        }

        public override int GetReUseDelay(int level)
        {
            return 600;
        }

        public override void AddEffectsInfo(IList<string> list)
        {
            list.Add("+50% block/parry 30s, -10% block/parry 15s.");
            list.Add("");
            list.Add("Target: Self");
            list.Add("Duration: 45 sec");
            list.Add("Casting time: Instant");
        }

    }
}
