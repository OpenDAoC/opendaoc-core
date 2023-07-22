using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Soldier's Citadel RA
    /// </summary>
    public class NfRaSoldiersCitadelHandler : Rr5RealmAbility
    {
        public const int DURATION = 30 * 1000;
        public const int SECOND_DURATION = 15 * 1000;

        public NfRaSoldiersCitadelHandler(DbAbilities dba, int level) : base(dba, level) { }

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
            	NfRaSoldiersCitadelEffect soldiersCitadel = player.EffectList.GetOfType<NfRaSoldiersCitadelEffect>();
                if (soldiersCitadel != null)
                    soldiersCitadel.Cancel(false);

                new NfRaSoldiersCitadelEffect().Start(player);
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
