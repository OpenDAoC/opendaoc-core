using Core.GS.ECS;
using Core.GS.Enums;
using Core.GS.Spells;

namespace Core.GS.Effects
{
    public class OfRaShadowRunEcsEffect : EcsGameAbilityEffect
    {
        public new SpellHandler SpellHandler;
        public OfRaShadowRunEcsEffect(EcsGameEffectInitParams initParams)
            : base(initParams)
        {
            EffectType = EEffect.ShadowRun;
            EffectService.RequestStartEffect(this);
        }

        public override void OnStartEffect()
        {
            base.OnStartEffect();
            if (Owner is GamePlayer p)
            {
                p.Out.SendUpdateMaxSpeed();
            }
        }

        public override void OnStopEffect()
        {
            base.OnStopEffect();
            if (Owner is GamePlayer p)
            {
                p.Out.SendUpdateMaxSpeed();
            }
        }

        public override ushort Icon { get { return 4278; } }
        public override string Name { get { return "Shadow Run"; } }
        public override bool HasPositiveEffect { get { return true; } }
    }
}
