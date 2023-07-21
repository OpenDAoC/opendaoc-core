namespace DOL.GS.Spells
{
    [SpellHandler("MultiTarget")]
    public class MultiTargetHandler : SpellHandler
    {
        public MultiTargetHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine)
        {
        }
    }
}
