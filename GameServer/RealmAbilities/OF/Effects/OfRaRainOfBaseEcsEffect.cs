namespace DOL.GS.Effects
{
    public class OfRaRainOfBaseEcsEffect : DamageAddEcsEffect
    {
        public OfRaRainOfBaseEcsEffect(ECSGameEffectInitParams initParams) : base(initParams) { }

        public override bool HasPositiveEffect => true;
    }
}
