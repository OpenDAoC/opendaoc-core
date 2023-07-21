namespace DOL.GS.Spells
{
    /// <summary>
    /// Handler for Flask use1: Heals are more effective on the target
    /// </summary>
    [SpellHandler("HealFlask")]
    public class HealFlaskSpellHandler : SpellHandler
    {
        public HealFlaskSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine)
        {
        }

        public override bool HasPositiveEffect
        {
            get { return true; }
        }
    }

    /// <summary>
    ///  Handler for Flask use2: Target gets chances not to die from the last hit.
    /// </summary>
    [SpellHandler("DeadFlask")]
    public class DeadFlaskSpellHandler : SpellHandler
    {
        public DeadFlaskSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine)
            : base(caster, spell, spellLine)
        {
        }

        public override bool HasPositiveEffect
        {
            get { return true; }
        }
    }
}