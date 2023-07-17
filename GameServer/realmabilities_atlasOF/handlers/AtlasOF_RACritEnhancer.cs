using System;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
    /// <summary>
    /// Spell crit chance %.
    /// </summary>
    public class AtlasOF_WildPowerAbility : WildPowerAbility
    {
        public AtlasOF_WildPowerAbility(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return AtlasRAHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return AtlasRAHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return AtlasRAHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Heal crit chance %.
    /// </summary>
    public class AtlasOF_WildHealingAbility : WildHealingAbility
    {
        public AtlasOF_WildHealingAbility(DbAbilities dba, int level) : base(dba, level) { }
        public override bool CheckRequirement(GamePlayer player) { return AtlasRAHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return AtlasRAHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return AtlasRAHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// DoT & Debuff crit chance %.
    /// </summary>
         public class AtlasOF_WildArcanaAbility : RAPropertyEnhancer
         {
             public AtlasOF_WildArcanaAbility(DbAbilities dba, int level) : base(dba, level, EProperty.CriticalDotHitChance) { }
             public override bool CheckRequirement(GamePlayer player) { return AtlasRAHelpers.GetAugAcuityLevel(player) >= 2; }
             public override int GetAmountForLevel(int level) { return AtlasRAHelpers.GetPropertyEnhancer5AmountForLevel(level); }
             public override int CostForUpgrade(int level) { return AtlasRAHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
         }

    /// <summary>
    /// Pet crit chance %.
    /// </summary>
    public class AtlasOF_WildMinionAbility : RAPropertyEnhancer
    {
        public AtlasOF_WildMinionAbility(DbAbilities dba, int level) : base(dba, level, EProperty.Undefined) { }
        public override bool CheckRequirement(GamePlayer player) { return AtlasRAHelpers.GetAugAcuityLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return AtlasRAHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return AtlasRAHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }

    /// <summary>
    /// Archery crit chance %.
    /// </summary>
    public class AtlasOF_FalconsEye : RAPropertyEnhancer // We don't want to piggyback on the NF FalconsEye because it increases spell crit chance and not archery for some reason...
    {
        public AtlasOF_FalconsEye(DbAbilities dba, int level) : base(dba, level, EProperty.CriticalArcheryHitChance) { }
        public override bool CheckRequirement(GamePlayer player) { return AtlasRAHelpers.GetAugDexLevel(player) >= 2; }
        public override int GetAmountForLevel(int level) { return AtlasRAHelpers.GetPropertyEnhancer5AmountForLevel(level); }
        public override int CostForUpgrade(int level) { return AtlasRAHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
    }
}
