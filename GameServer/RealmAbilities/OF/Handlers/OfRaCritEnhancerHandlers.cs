using Core.GS.RealmAbilities;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Spell crit chance %.
    /// </summary>
    public class OfRaWildPowerHandler : NfRaWildPowerHandler
    {
        public OfRaWildPowerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Heal crit chance %.
    /// </summary>
    public class OfRaWildHealingHandler : WildHealingHandler
    {
        public OfRaWildHealingHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// DoT & Debuff crit chance %.
    /// </summary>
         public class OfRaWildArcanaHandler : RaPropertyEnhancer
         {
             public OfRaWildArcanaHandler(DbAbilities dba, int level) : base(dba, level, EProperty.CriticalDotHitChance) { }
             public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
             public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
             public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
         }

    /// <summary>
    /// Pet crit chance %.
    /// </summary>
    public class OfRaWildMinionHandler : RaPropertyEnhancer
    {
        public OfRaWildMinionHandler(DbAbilities dba, int level) : base(dba, level, EProperty.Undefined) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Archery crit chance %.
    /// </summary>
    public class OfRaFalconsEyeHandler : RaPropertyEnhancer // We don't want to piggyback on the NF FalconsEye because it increases spell crit chance and not archery for some reason...
    {
        public OfRaFalconsEyeHandler(DbAbilities dba, int level) : base(dba, level, EProperty.CriticalArcheryHitChance) { }
        public override bool CheckRequirement(GamePlayer player) { return OfRaHelpers.GetAugDexLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }
}
