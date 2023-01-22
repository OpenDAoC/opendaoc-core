using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;

namespace DOL.GS.Effects;

/// <summary>
/// Adrenaline Rush
/// </summary>
public class RemedyEffect : TimedEffect
{
    public RemedyEffect()
        : base(60000)
    {
        ;
    }

    private GameLiving owner;
    private int healthdrain;

    public override void Start(GameLiving target)
    {
        base.Start(target);
        owner = target;
        var player = target as GamePlayer;
        if (player != null)
            foreach (GamePlayer p in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                p.Out.SendSpellEffectAnimation(player, player, Icon, 0, false, 1);

        healthdrain = (int) (target.MaxHealth * 0.1);
        if (target.Health <= healthdrain)
            return;
        target.TakeDamage(target, eDamageType.Body, healthdrain, 0);
    }

    public override string Name => "Remedy";

    public override ushort Icon => 3059;

    public override void Stop()
    {
        if (!owner.IsAlive)
        {
            base.Stop();
            return;
        }

        owner.Health += healthdrain;
        base.Stop();
    }

    public int SpellEffectiveness => 100;

    public override IList<string> DelveInfo
    {
        get
        {
            var list = new List<string>();
            list.Add("For 60 seconds you're immune to all weapon poisons");
            return list;
        }
    }
}