using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	public class OfRaBunkerOfFaithHandler : NfRaBarrierOfFortitudeHandler
	{
		public OfRaBunkerOfFaithHandler(DbAbilities dba, int level) : base(dba, level) { }

        public override int MaxLevel { get { return 1; } }
        public override int CostForUpgrade(int level) { return 10; }
        public override int GetReUseDelay(int level) { return 1800; } // 30 mins
        protected override int GetAbsorbAmount() { return 50; } // in %
    }
}
