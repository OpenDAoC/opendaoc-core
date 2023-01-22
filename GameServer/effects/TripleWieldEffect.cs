using System;
using System.Collections.Generic;
using DOL.Events;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Effects;

/// <summary>
/// TripleWield
/// </summary>
public class TripleWieldEffect : TimedEffect
{
    public TripleWieldEffect(int duration) : base(duration)
    {
    }

    public override void Start(GameLiving target)
    {
        base.Start(target);
        var player = target as GamePlayer;
        foreach (GamePlayer p in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
        {
            p.Out.SendSpellEffectAnimation(player, player, 7102, 0, false, 1);
            p.Out.SendSpellCastAnimation(player, Icon, 0);
        }

        GameEventMgr.AddHandler(player, GameLivingEvent.AttackFinished, new DOLEventHandler(EventHandler));
    }

    public override void Stop()
    {
        base.Stop();
        var player = Owner as GamePlayer;
        GameEventMgr.RemoveHandler(player, GameLivingEvent.AttackFinished, new DOLEventHandler(EventHandler));
    }

    /// <summary>
    /// Handler fired on every melee attack by effect target
    /// </summary>
    /// <param name="e"></param>
    /// <param name="sender"></param>
    /// <param name="arguments"></param>
    protected void EventHandler(DOLEvent e, object sender, EventArgs arguments)
    {
        var atkArgs = arguments as AttackFinishedEventArgs;
        if (atkArgs == null) return;
        if (atkArgs.AttackData.AttackResult != eAttackResult.HitUnstyled
            && atkArgs.AttackData.AttackResult != eAttackResult.HitStyle) return;
        if (atkArgs.AttackData.Target == null) return;
        var target = atkArgs.AttackData.Target;
        if (target == null) return;
        if (target.ObjectState != GameObject.eObjectState.Active) return;
        if (target.IsAlive == false) return;
        var attacker = sender as GameLiving;
        if (attacker == null) return;
        if (attacker.ObjectState != GameObject.eObjectState.Active) return;
        if (attacker.IsAlive == false) return;
        if (atkArgs.AttackData.IsOffHand) return; // only react to main hand
        if (atkArgs.AttackData.Weapon == null) return; // no weapon attack

        var modifier = 100;
        //double dpsCap = (1.2 + 0.3 * attacker.Level) * 0.7;
        //double dps = Math.Min(atkArgs.AttackData.Weapon.DPS_AF/10.0, dpsCap);
        var baseDamage = atkArgs.AttackData.Weapon.DPS_AF / 10.0 *
                         atkArgs.AttackData.WeaponSpeed;

        modifier += (int) (25 * atkArgs.AttackData.Target.GetConLevel(atkArgs.AttackData.Attacker));
        modifier = Math.Min(300, modifier);
        modifier = Math.Max(75, modifier);

        var damage = baseDamage * modifier * 0.001; // attack speed is 10 times higher (2.5spd=25)			
        var damageResisted = damage * target.GetResist(eDamageType.Body) * -0.01;

        var ad = new AttackData();
        ad.Attacker = attacker;
        ad.Target = target;
        ad.Damage = (int) (damage + damageResisted);
        ad.Modifier = (int) damageResisted;
        ad.DamageType = eDamageType.Body;
        ad.AttackType = AttackData.eAttackType.MeleeOneHand;
        ad.AttackResult = eAttackResult.HitUnstyled;
        ad.WeaponSpeed = atkArgs.AttackData.WeaponSpeed;

        var owner = attacker as GamePlayer;
        if (owner != null)
        {
            owner.Out.SendMessage(
                LanguageMgr.GetTranslation(owner.Client, "Effects.TripleWieldEffect.MBHitsExtraDamage",
                    target.GetName(0, false), ad.Damage), eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
            var playerTarget = target as GamePlayer;
            if (playerTarget != null)
                playerTarget.Out.SendMessage(
                    LanguageMgr.GetTranslation(playerTarget.Client, "Effects.TripleWieldEffect.XMBExtraDamageToYou",
                        attacker.GetName(0, false), ad.Damage), eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
        }

        target.OnAttackedByEnemy(ad);
        attacker.DealDamage(ad);

        foreach (GamePlayer player in ad.Attacker.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            player.Out.SendCombatAnimation(null, target, 0, 0, 0, 0, 0x0A, target.HealthPercent);
    }

    public override string Name =>
        LanguageMgr.GetTranslation(((GamePlayer) Owner).Client, "Effects.TripleWieldEffect.Name");

    public override ushort Icon => 475;

    public override IList<string> DelveInfo
    {
        get
        {
            var list = new List<string>(4);
            ;
            list.Add(
                LanguageMgr.GetTranslation(((GamePlayer) Owner).Client, "Effects.TripleWieldEffect.InfoEffect"));
            list.AddRange(base.DelveInfo);

            return list;
        }
    }
}