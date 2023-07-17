
using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Searing pet RA
    /// </summary>
    public class SearingPetAbility : RR5RealmAbility
    {
        public const int DURATION = 19 * 1000;

        public SearingPetAbility(DbAbilities dba, int level) : base(dba, level) { }

        /// <summary>
        /// Action
        /// </summary>
        /// <param name="living"></param>
        public override void Execute(GameLiving living)
        {
            if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;

            GamePlayer player = living as GamePlayer;
            if (player != null && player.ControlledBrain != null && player.ControlledBrain.Body != null)
            {
                GameNPC pet = player.ControlledBrain.Body as GameNPC;
                if (pet.IsAlive)
                {
					SearingPetEffect SearingPet = pet.EffectList.GetOfType<SearingPetEffect>();
                    if (SearingPet != null) SearingPet.Cancel(false);
                    new SearingPetEffect(player).Start(pet);
                }
                DisableSkill(living);
            }
            else if (player != null)
            {
                player.Out.SendMessage("You must have a controlled pet to use this ability!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                player.DisableSkill(this, 3 * 1000);
            }
        }

        public override int GetReUseDelay(int level)
        {
            return 120;
        }

        public override void AddEffectsInfo(IList<string> list)
        {
            list.Add(" PBAoE Pet pulsing effect, 350units, 25 damage, 6 ticks, 2min RUT.");
            list.Add("");
            list.Add("Target: Pet");
            list.Add("Duration: 18s");
            list.Add("Casting time: Instant");
        }

    }
}


