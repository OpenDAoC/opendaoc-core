
namespace DOL.GS.Effects
{
    public class MasteryOfConcentrationECSEffect : EcsGameAbilityEffect
    {
        public MasteryOfConcentrationECSEffect(EcsGameEffectInitParams initParams)
            : base(initParams)
        {
            EffectType = eEffect.MasteryOfConcentration;
            EffectService.RequestStartEffect(this);
        }

        public override ushort Icon { get { return 4238; } }
        public override string Name { get { return "Mastery of Concentration"; } }
        public override bool HasPositiveEffect { get { return true; } }
    }
}
