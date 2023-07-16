namespace DOL.spells
{
    using AI.Brain;
    using GS;
    using GS.PacketHandler;
    using GS.Spells;

    /// <summary>
    /// Return life to Player Owner
    /// </summary>
    [SpellHandler("PetLifedrain")]
    public class PetLifedrainSpellHandler : LifedrainSpellHandler
    {
        public PetLifedrainSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            if(Caster == null || !(Caster is GameSummonedPet) || !(((GameSummonedPet) Caster).Brain is IControlledBrain))
                return;
            base.OnDirectEffect(target, effectiveness);
        }

        public override void StealLife(AttackData ad)
        {
            if(ad == null) return;
            GamePlayer player = ((IControlledBrain) ((GameSummonedPet) Caster).Brain).GetPlayerOwner();
            if(player == null || !player.IsAlive) return;
            int heal = ((ad.Damage + ad.CriticalDamage)*m_spell.LifeDrainReturn)/100;
            if(player.IsDiseased)
            {
                MessageToLiving(player, "You are diseased !", eChatType.CT_SpellResisted);
                heal >>= 1;
            }
            if(heal <= 0) return;

            heal = player.ChangeHealth(player, eHealthChangeType.Spell, heal);
            if(heal > 0)
            {
                MessageToLiving(player, "You steal " + heal + " hit point" + (heal == 1 ? "." :"s."), eChatType.CT_Spell);
            } else
            {
                MessageToLiving(player, "You cannot absorb any more life.", eChatType.CT_SpellResisted);
            }
        }
    }
}