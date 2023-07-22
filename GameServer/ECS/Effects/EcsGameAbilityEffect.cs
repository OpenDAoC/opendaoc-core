namespace DOL.GS
{
    /// <summary>
    /// Ability-Based Effect
    /// </summary>
    public class EcsGameAbilityEffect : EcsGameEffect
    {
        public override string Name { get { return "Default Ability Name"; } }

        public EcsGameAbilityEffect(ECSGameEffectInitParams initParams) : base(initParams)
        {
            //EffectService.RequestStartEffect(this);
        }
    }
}