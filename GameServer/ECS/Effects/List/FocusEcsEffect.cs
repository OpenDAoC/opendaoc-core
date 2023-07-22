namespace DOL.GS
{
    public class FocusEcsEffect : EcsGameSpellEffect
    {
        public FocusEcsEffect(ECSGameEffectInitParams initParams)
            : base(initParams) { }

        public override void OnStartEffect()
        {
            // "Lashing energy ripples around you."
            // "Dangerous energy surrounds {0}."
            OnEffectStartsMsg(Owner, false, true, true);
        }

        public override void OnStopEffect()
        {
            // "Your energy field dissipates."
            // "{0}'s energy field dissipates."
            OnEffectExpiresMsg(Owner, false, true, true);
        }
    }
}
