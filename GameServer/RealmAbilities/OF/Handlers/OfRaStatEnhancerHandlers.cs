using System.Collections.Generic;
using Core.GS.RealmAbilities;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	public class OfRaStrengthEnhancerHandler : RaStrengthEnhancer
	{
		public OfRaStrengthEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
		public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetStatEnhancerAmountForLevel(level); }
    }

	public class OfRaConstitutionEnhancerHandler : RaConstitutionEnhancer
	{
		public OfRaConstitutionEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetStatEnhancerAmountForLevel(level); }
    }

	public class OfRaQuicknessEnhancerHandler : RaQuicknessEnhancer
	{
		public OfRaQuicknessEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetStatEnhancerAmountForLevel(level); }
    }

	public class OfRaDexterityEnhancerHandler : RaDexterityEnhancer
	{
		public OfRaDexterityEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetStatEnhancerAmountForLevel(level); }
    }

	public class OfRaAcuityEnhancerHandler : RaAcuityEnhancer
	{
		public OfRaAcuityEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetStatEnhancerAmountForLevel(level); }
    }

	public class OfRaMaxPowerEnhancerHandler : RaMaxPowerEnhancer
	{
		public OfRaMaxPowerEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
    }

	public class OfRaMaxHealthEnhancerHandler : RaMaxHealthEnhancer
	{
		public OfRaMaxHealthEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
        public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
        public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
        public override bool CheckRequirement(GamePlayer player) { return true; } // Override NF level 40 requirement.
    }
	
	public class OfRaEndRegenEnhancerHandler : RaEndRegenEnhancer
	{
		public OfRaEndRegenEnhancerHandler(DbAbilities dba, int level) : base(dba, level) { }
		
		public override int MaxLevel
		{
			get
			{
				return 1;
			}
		}
		public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
		public override int GetAmountForLevel(int level) { return 1; }
		public override bool CheckRequirement(GamePlayer player) { return true; } // Override NF level 40 requirement.

		public override IList<string> DelveInfo { 			
			get
			{
				var delveInfoList = new List<string>();
				delveInfoList.Add(m_description);

				return delveInfoList;
			}
			
		}
	}
}