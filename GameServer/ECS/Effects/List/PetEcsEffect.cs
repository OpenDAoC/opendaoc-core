namespace DOL.GS
{
    public class PetEcsEffect : EcsGameSpellEffect
    {
        public PetEcsEffect(ECSGameEffectInitParams initParams)
            : base(initParams) { }

        public override void OnStopEffect()
        {
            if (SpellHandler.Caster.PetCount > 0)
                SpellHandler.Caster.PetCount--;
            Owner.Health = 0; // to send proper remove packet
            Owner.Delete();
        }
    }
}
