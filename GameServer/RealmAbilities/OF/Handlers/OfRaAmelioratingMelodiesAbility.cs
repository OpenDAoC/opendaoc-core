using System.Collections.Generic;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	public class OfRaAmelioratingMelodiesAbility : NfRaAmelioratingMelodiesAbility
	{
		public OfRaAmelioratingMelodiesAbility(DbAbility dba, int level) : base(dba, level) { }

        public override int MaxLevel { get { return 1; } }
        public override int CostForUpgrade(int level) { return 14; }
        public override int GetReUseDelay(int level) { return 1800; } // 30 mins
        protected override int GetHealAmountPerTick() { return 100; } // 100hp per tick @ 1.5s tick rate over 30s = 2000 total.

        public override void AddEffectsInfo(IList<string> list)
        {
            list.Add("Heals all group members for 100hp every 1.5 seconds.");
            list.Add("Range: 1500");
            list.Add("Target: Group");
            list.Add("Duration: 30 sec");
            list.Add("Casting time: instant");
        }
    }
}