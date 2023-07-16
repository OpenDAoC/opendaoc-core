namespace DOL.GS.Spells
{
    [SpellHandler("CloudsongAura")]
    public class CloudsongAuraSpellHandler : DualStatBuff
    {
        public CloudsongAuraSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
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
            get { return eProperty.SpellRange; }
        }

        public override eProperty Property2
        {
            get { return eProperty.ResistPierce; }
        }

    }

    /// <summary>
    /// [Freya] Nidel : Handler for Fall damage reduction.
    /// Calcul located in PlayerPositionUpdateHandler.cs
    /// </summary>
    [SpellHandler("CloudsongFall")]
    public class CloudsongFallSpellHandler : SpellHandler
    {
        public CloudsongFallSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
        {
        }

        public override bool HasPositiveEffect
        {
            get { return true; }
        }
    }
}
