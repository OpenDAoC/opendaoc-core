namespace DOL.GS.Spells
{
    [SpellHandler("Traldor")]
    public class TraldorSpellHandler : DualStatBuff
    {
        public TraldorSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
        {
        }

        /// <summary>
        /// SpecBuffBonusCategory
        /// </summary>
		public override eBuffBonusCategory BonusCategory1 { get { return eBuffBonusCategory.SpecBuff; } }

        /// <summary>
        /// BaseBuffBonusCategory
        /// </summary>
		public override eBuffBonusCategory BonusCategory2 { get { return eBuffBonusCategory.BaseBuff; } }

        public override eProperty Property1
        {
            get { return eProperty.SpellDamage; }
        }

        public override eProperty Property2
        {
            get { return eProperty.ResistPierce; }
        }

    }
}
