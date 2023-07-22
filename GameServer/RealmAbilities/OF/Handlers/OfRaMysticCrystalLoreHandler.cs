using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	/// <summary>
	/// Mystic Crystal Lore, power heal
	/// </summary>
	public class OfRaMysticCrystalLoreHandler : NfRaMysticCrystalLoreHandler
	{
		public OfRaMysticCrystalLoreHandler(DbAbilities dba, int level) : base(dba, level) { }

        public override int CostForUpgrade(int currentLevel) { return OfRaHelpers.GetCommonUpgradeCostFor3LevelsRA(currentLevel); }
        
        public override int GetReUseDelay(int level)
        {
	        return 300;
        }
    }
}