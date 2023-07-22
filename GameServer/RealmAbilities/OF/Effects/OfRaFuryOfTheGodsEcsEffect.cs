namespace DOL.GS.Effects
{
    public class OfRaFuryOfTheGodsEcsEffect : DamageAddEcsEffect
    {
        public OfRaFuryOfTheGodsEcsEffect(ECSGameEffectInitParams initParams)
            : base(initParams)
        {
        }
        
        public override ushort Icon { get { return 4251; } }
        public override string Name { get { return "Fury Of The Gods"; } }
        public override bool HasPositiveEffect { get { return true; } }
    }
}
