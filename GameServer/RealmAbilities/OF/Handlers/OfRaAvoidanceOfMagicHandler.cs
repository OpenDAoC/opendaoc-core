using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	/// <summary>
	/// Avoidance of Magic RA, reduces magical damage
	/// </summary>
	public class OfRaAvoidanceOfMagicHandler : NfRaAvoidanceOfMagicHandler
	{
		public OfRaAvoidanceOfMagicHandler(DbAbilities dba, int level) : base(dba, level) { }
		public override int GetAmountForLevel(int level) { return OfRaHelpers.GetPropertyEnhancer3AmountForLevel(level); }
		public override int CostForUpgrade(int level) { return OfRaHelpers.GetCommonUpgradeCostFor5LevelsRA(level); }
	}
}