namespace DOL.GS.Spells
{
    /// <summary>
    /// [Freya] Nidel : Handler for Flask use1: Heals are more effective on the target
    /// </summary>
    [SpellHandler("HealFlask")]
    public class HealFlask : SpellHandler
    {
        public HealFlask(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine)
        {
        }

        public override bool HasPositiveEffect
        {
            get { return true; }
        }
    }

    /// <summary>
    ///  [Freya] Nidel : Handler for Flask use2: Target gets chances not to die from the last hit.
    /// </summary>
    [SpellHandler("DeadFlask")]
    public class DeadFlask : SpellHandler
    {
        public DeadFlask(GameLiving caster, Spell spell, SpellLine spellLine)
            : base(caster, spell, spellLine)
        {
        }

        public override bool HasPositiveEffect
        {
            get { return true; }
        }
    }
}