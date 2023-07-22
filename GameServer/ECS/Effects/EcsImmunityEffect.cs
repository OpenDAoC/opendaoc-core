using System;
using DOL.GS.Spells;

namespace DOL.GS
{
    public class EcsImmunityEffect : EcsGameSpellEffect
    {
        public override ushort Icon { get { return SpellHandler.Spell.Icon; } }
        public override string Name { get { return SpellHandler.Spell.Name; } }

        public EcsImmunityEffect(GameLiving owner, ISpellHandler handler, int duration, int pulseFreq, double effectiveness, ushort icon, bool cancelEffect = false)
            : base(new ECSGameEffectInitParams(owner, duration, effectiveness, handler))
        {
            // Some of this is already done in the base constructor and should be cleaned up
            Owner = owner;
            SpellHandler = handler;
            Duration = duration;
            PulseFreq = pulseFreq;
            Effectiveness = effectiveness;
            CancelEffect = cancelEffect;
            EffectType = MapImmunityEffect();
            ExpireTick = duration + GameLoop.GameLoopTime;
            StartTick = GameLoop.GameLoopTime;
            LastTick = 0;
            TriggersImmunity = false;

            EffectService.RequestStartEffect(this);
        }

        protected EEffect MapImmunityEffect()
        {
            switch (SpellHandler.Spell.SpellType)
            {
                case ESpellType.Mesmerize:
                    return EEffect.MezImmunity;
                case ESpellType.StyleStun:
                case ESpellType.Stun:
                    return EEffect.StunImmunity;
                case ESpellType.SpeedDecrease:
                case ESpellType.DamageSpeedDecreaseNoVariance:
                case ESpellType.DamageSpeedDecrease:
                    return EEffect.SnareImmunity;
                case ESpellType.Nearsight:
                    return EEffect.NearsightImmunity;
                default:
                    return EEffect.Unknown;
            }
        }
    }
    public class NPCECSStunImmunityEffect : EcsGameEffect
    {
        private int timesStunned = 1;
        public NPCECSStunImmunityEffect(ECSGameEffectInitParams initParams) : base()
        {
            Owner = initParams.Target;
            Duration = 60000;
            EffectType = EEffect.NPCStunImmunity;
            EffectService.RequestStartEffect(this);
        }

        public long CalclulateStunDuration(long duration)
        {
            var retVal = duration / (2 * timesStunned);
            timesStunned++;
            return retVal;
        }
    }

    public class NPCECSMezImmunityEffect : EcsGameEffect
    {
        private int timesStunned = 1;
        public NPCECSMezImmunityEffect(ECSGameEffectInitParams initParams) : base()
        {
            Owner = initParams.Target;
            Duration = 60000;
            EffectType = EEffect.NPCMezImmunity;
            EffectService.RequestStartEffect(this);
        }

        public long CalclulateStunDuration(long duration)
        {
            var retVal = duration / (2 * timesStunned);
            timesStunned++;
            return retVal;
        }
    }
}