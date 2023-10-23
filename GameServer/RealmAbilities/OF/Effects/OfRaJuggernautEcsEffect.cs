using Core.GS.AI;
using Core.GS.ECS;
using Core.GS.Enums;
using Core.GS.Skills;
using Core.GS.Spells;

namespace Core.GS.RealmAbilities;

public class OfRaJuggernautEcsEffect : EcsGameAbilityEffect
{
    public OfRaJuggernautEcsEffect(EcsGameEffectInitParams initParams) : base(initParams)
    {
        EffectType = EEffect.Juggernaut;
        EffectService.RequestStartEffect(this);
    }

    public override ushort Icon => 4261;
    public override string Name => "Juggernaut";
    public override bool HasPositiveEffect => true;

    public override void OnStartEffect()
    {
        SpellLine RAspellLine = new SpellLine("RAs", "RealmAbilities", "RealmAbilities", true);
        Spell Juggernaut = SkillBase.GetSpellByID(90801);

        if (Juggernaut != null)
            Owner.CastSpell(Juggernaut, RAspellLine);

        base.OnStartEffect();
    }

    public override void OnStopEffect()
    {
        if (Owner.ControlledBrain is JuggernautBrain juggernautBrain)
            juggernautBrain.Body.TakeDamage(null, EDamageType.Natural, int.MaxValue, 0);

        base.OnStopEffect();
    }

    public void Cancel(bool playerCancel)
    {
        EffectService.RequestImmediateCancelEffect(this, playerCancel);
        OnStopEffect();
    }
}