using DOL.Database;

namespace DOL.GS.RealmAbilities
{

    public class OfRaThornweedFieldHandler : NfRaThornweedFieldHandler
    {
        public OfRaThornweedFieldHandler(DbAbilities dba, int level) : base(dba, level) { }

        public override int MaxLevel { get { return 1; } }
        public override int CostForUpgrade(int level) { return 14; }
        public override int GetReUseDelay(int level) { return 1800; } // 30 mins
    }
}