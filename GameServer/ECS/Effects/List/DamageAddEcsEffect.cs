namespace DOL.GS
{
    public class DamageAddEcsEffect : EcsGameSpellEffect
    {
        public DamageAddEcsEffect(ECSGameEffectInitParams initParams)
            : base(initParams) { }

        public override void OnStartEffect()
        {
            // "Lashing energy ripples around you."
            // "Dangerous energy surrounds {0}."
            OnEffectStartsMsg(Owner, true, true, true);
        }

        public override void OnStopEffect()
        {
            // "Your energy field dissipates."
            // "{0}'s energy field dissipates."
            OnEffectExpiresMsg(Owner, true, false, true);
        }
    }
}
