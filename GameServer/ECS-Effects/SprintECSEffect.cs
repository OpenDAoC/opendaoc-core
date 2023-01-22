﻿using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;
using DOL.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.GS;

public class SprintECSGameEffect : ECSGameAbilityEffect
{
    public SprintECSGameEffect(ECSGameEffectInitParams initParams)
        : base(initParams)
    {
        EffectType = eEffect.Sprint;
        NextTick = GameLoop.GameLoopTime + 1;
        EffectService.RequestStartEffect(this);
    }

    /// <summary>
    /// The amount of timer ticks player was not moving
    /// </summary>
    private int m_idleTicks = 0;

    public override ushort Icon => 0x199;

    public override string Name => LanguageMgr.GetTranslation(OwnerPlayer.Client, "Effects.SprintEffect.Name");

    public override bool HasPositiveEffect => true;

    public override long GetRemainingTimeForClient()
    {
        return 1000;
    }

    public override void OnStartEffect()
    {
        if (OwnerPlayer != null)
        {
            var regen = OwnerPlayer.GetModified(eProperty.EnduranceRegenerationRate);
            var endchant = OwnerPlayer.GetModified(eProperty.FatigueConsumption);
            var cost = -5 + regen;
            if (endchant > 1) cost = (int) Math.Ceiling(cost * endchant * 0.01);
            OwnerPlayer.Endurance += cost;

            OwnerPlayer.Out.SendUpdateMaxSpeed();
            OwnerPlayer.Out.SendMessage(
                LanguageMgr.GetTranslation(OwnerPlayer.Client.Account.Language, "GamePlayer.Sprint.PrepareSprint"),
                eChatType.CT_System, eChatLoc.CL_SystemWindow);
            Owner.StartEnduranceRegeneration();
        }
    }

    public override void OnStopEffect()
    {
        if (OwnerPlayer != null)
        {
            OwnerPlayer.Out.SendUpdateMaxSpeed();
            OwnerPlayer.Out.SendMessage(
                LanguageMgr.GetTranslation(OwnerPlayer.Client.Account.Language, "GamePlayer.Sprint.NoLongerReady"),
                eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }
    }

    public override void OnEffectPulse()
    {
        int nextInterval;

        if (Owner.IsMoving)
            m_idleTicks = 0;
        else m_idleTicks++;

        if (Owner.Endurance - 5 <= 0 || m_idleTicks >= 6)
        {
            EffectService.RequestImmediateCancelEffect(this);
            nextInterval = 0;
        }
        else
        {
            nextInterval = Util.Random(600, 1400);
            if (Owner.IsMoving)
            {
                var amount = 5;

                var ra = Owner.GetAbility<AtlasOF_LongWindAbility>();
                if (ra != null)
                    amount = 5 - ra.GetAmountForLevel(ra.Level);

                //m_owner.Endurance -= amount;
            }
        }

        NextTick += nextInterval;
    }
}