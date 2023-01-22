using DOL.Database;
using System.Collections.Generic;

namespace DOL.GS.RealmAbilities;

public class AtlasOF_NegativeMaelstrom : NegativeMaelstromAbility
{
    public AtlasOF_NegativeMaelstrom(DBAbility dba, int level) : base(dba, level)
    {
    }

    public override int MaxLevel => 1;

    public override int CostForUpgrade(int level)
    {
        return 14;
    }

    public override int GetReUseDelay(int level)
    {
        return 1800;
    } // 30 mins
}