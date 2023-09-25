using DOL.GS;

namespace DOL.AI.Brain
{
    public class TurretMainTankBrain : TurretBrain
    {
        public TurretMainTankBrain(GameLiving owner) : base(owner) { }

        protected override bool TrustCast(Spell spell, eCheckSpellType type, GameLiving target)
        {
            // Tank turrets don't check for spells if their target is close, but attack in melee instead.
            if (Body.IsWithinRadius(target, Body.attackComponent.AttackRange))
            {
                Body.StopCurrentSpellcast();
                Body.StartAttack(target);
                return true;
            }
            else
            {
                Body.StopAttack();
                return base.TrustCast(spell, type, target);
            }
        }
    }
}