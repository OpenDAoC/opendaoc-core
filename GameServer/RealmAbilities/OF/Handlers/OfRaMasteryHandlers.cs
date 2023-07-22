using Core.GS.RealmAbilities;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	/// <summary>
	/// Mastery of Pain ability
	/// </summary>
	public class OfRaMasteryOfPainHandler : NfRaMasteryOfPainHandler
	{
		public OfRaMasteryOfPainHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 2; }

        // MoP is 5% per level unlike most other Mastery RAs.
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); } 
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Parry ability
    /// </summary>
    public class OfRaMasteryOfParryingHandler : NfRaMasteryOfParryingHandler
	{
        public OfRaMasteryOfParryingHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Blocking ability
    /// </summary>
    public class OfRaMasteryOfBlockingHandler : NfRaMasteryOfBlockingHandler
    {
        public OfRaMasteryOfBlockingHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Healing ability
    /// </summary>
    public class OfRaMasteryOfHealingHandler : MasteryOfHealingHandler
    {
        public OfRaMasteryOfHealingHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Arms ability
    /// </summary>
    public class OfRaMasteryOfArmsHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfArmsHandler(DbAbilities dba, int level) : base(dba, level, EProperty.MeleeSpeed) { }
        protected override string ValueUnit { get { return "%"; } }

        public override bool CheckRequirement(GamePlayer player)
        { 
            // Atlas custom change - Friar pre-req is AugDex3 instead of a 100% useless AugStr3.
            if (player.CharacterClass.ID == (byte)ECharacterClass.Friar)
            {
                return OfRaHelpers.GetAugDexLevel(player) >= 3;
            }
            
            return OfRaHelpers.GetAugStrLevel(player) >= 3;
        }

        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Archery ability
    /// </summary>
    public class OfRaMasteryOfArcheryHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfArcheryHandler(DbAbilities dba, int level) : base(dba, level, EProperty.ArcherySpeed) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 3; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of the Art ability
    /// </summary>
    public class OfRaMasteryOfTheArtHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfTheArtHandler(DbAbilities dba, int level) : base(dba, level, EProperty.CastingSpeed) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 3; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Magery ability
    /// </summary>
    public class OfRaMasteryOfMageryHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfMageryHandler(DbAbilities dba, int level) : base(dba, level, EProperty.SpellDamage) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of the Arcane ability
    /// </summary>
    public class OfRaMasteryOfTheArcaneHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfTheArcaneHandler(DbAbilities dba, int level) : base(dba, level, EProperty.BuffEffectiveness) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Water ability. The best of all abilities.
    /// </summary>
    public class OfRaMasteryOfWaterHandler : RaPropertyEnhancer
    {
        public OfRaMasteryOfWaterHandler(DbAbilities dba, int level) : base(dba, level, EProperty.WaterSpeed) { }
        protected override string ValueUnit { get { return "%"; } }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Dodger ability
    /// </summary>
    public class OfRaDodgerHandler : RaPropertyEnhancer
    {
        public OfRaDodgerHandler(DbAbilities dba, int level) : base(dba, level, EProperty.EvadeChance) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugQuiLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Mastery of Stealth ability
    /// </summary>
    public class OfRaMasteryOfStealthHandler : NfRaMasteryOfStealthHandler
	{
        public OfRaMasteryOfStealthHandler(DbAbilities dba, int level) : base(dba, level) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugQuiLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor3LevelsRA(level); }
    }
    
    /// <summary>
    /// Dualist's Reflexes ability
    /// </summary>
    public class OfRaDuelistsReflexesHandler : RaPropertyEnhancer
    {
        public OfRaDuelistsReflexesHandler(DbAbilities dba, int level) : base(dba, level, EProperty.OffhandDamageAndChance) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }
    
    /// <summary>
    /// Arrow Salvaging ability
    /// </summary>
    public class OfRaArrowSalvagingHandler : RaPropertyEnhancer
    {
        public OfRaArrowSalvagingHandler(DbAbilities dba, int level) : base(dba, level, EProperty.ArrowRecovery) { }
        protected override string ValueUnit { get { return "%"; } }
        public override bool CheckRequirement(GamePlayer player) { return true; }
        public override int GetAmountForLevel(int level) { return level * 10; }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }
}