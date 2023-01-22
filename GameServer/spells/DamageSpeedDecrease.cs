/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;
using DOL.Language;

namespace DOL.GS.Spells;

/// <summary>
/// Damages target and decreases speed after
/// </summary>
[SpellHandlerAttribute("DamageSpeedDecrease")]
public class DamageSpeedDecreaseSpellHandler : SpeedDecreaseSpellHandler
{
    /// <summary>
    /// Apply effect on target or do spell action if non duration spell
    /// </summary>
    /// <param name="target">target that gets the effect</param>
    /// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
    public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
    {
        // do damage even if immune to duration effect
        OnDirectEffect(target, effectiveness);

        if (target is Keeps.GameKeepDoor == false && target is Keeps.GameKeepComponent == false)
            /*
            if (Caster.HasAbilityType(typeof(AtlasOF_WildArcanaAbility)))
            {
                if (Util.Chance(Caster.SpellCriticalChance))
                {
                    effectiveness *= 2;
                    if(Caster is GamePlayer c) c.Out.SendMessage($"Your {Spell.Name} critically hits the enemy for 100% additional effect!", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
                }
            }*/
            base.ApplyEffectOnTarget(target, effectiveness);
    }

    /// <summary>
    /// execute non duration spell effect on target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="effectiveness"></param>
    public override void OnDirectEffect(GameLiving target, double effectiveness)
    {
        base.OnDirectEffect(target, effectiveness);
        // calc damage
        var ad = CalculateDamageToTarget(target, effectiveness);
        SendDamageMessages(ad);
        DamageTarget(ad, true);
        if (Spell.LifeDrainReturn != 0)
            StealLife(ad);
    }

    /// <summary>
    /// Uses percent of damage to heal the caster
    /// </summary>
    public virtual void StealLife(AttackData ad)
    {
        if (ad == null) return;
        if (!m_caster.IsAlive) return;

        if (ad.Target is Keeps.GameKeepDoor || ad.Target is Keeps.GameKeepComponent) return;

        var heal = (ad.Damage + ad.CriticalDamage) * m_spell.LifeDrainReturn / 100;
        if (m_caster.IsDiseased)
        {
            MessageToCaster("You are diseased!", eChatType.CT_SpellResisted);
            heal >>= 1;
        }

        if (heal <= 0) return;
        heal = m_caster.ChangeHealth(m_caster, eHealthChangeType.Spell, heal);

        if (heal > 0)
            MessageToCaster("You steal " + heal + " hit point" + (heal == 1 ? "." : "s."), eChatType.CT_Spell);
        else
            MessageToCaster("You cannot absorb any more life.", eChatType.CT_SpellResisted);
    }

    /// <summary>
    /// When an applied effect expires.
    /// Duration spells only.
    /// </summary>
    /// <param name="effect">The expired effect</param>
    /// <param name="noMessages">true, when no messages should be sent to player and surrounding</param>
    /// <returns>immunity duration in milliseconds</returns>
    public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
    {
        base.OnEffectExpires(effect, noMessages);
        return 0;
    }

    /// <summary>
    /// Creates the corresponding spell effect for the spell
    /// </summary>
    /// <param name="target"></param>
    /// <param name="effectiveness"></param>
    /// <returns></returns>
    protected override GameSpellEffect CreateSpellEffect(GameLiving target, double effectiveness)
    {
        var duration = CalculateEffectDuration(target, effectiveness);
        return new GameSpellEffect(this, duration, 0, effectiveness);
    }

    /// <summary>
    /// Delve Info
    /// </summary>
    public override IList<string> DelveInfo
    {
        get
        {
            /*
            <Begin Info: Lesser Constricting Jolt>
            Function: damage/speed decrease

            Target is damaged, and also moves slower for the spell's duration.

            Speed decrease: 35%
            Damage: 64
            Target: Targetted
            Range: 1500
            Duration: 30 sec
            Power cost: 10
            Casting time:      3.0 sec
            Damage: Matter

            <End Info>
            */

            var list = new List<string>();
            list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client,
                "DamageSpeedDecrease.DelveInfo.Function"));
            list.Add(" "); //empty line
            list.Add(Spell.Description);
            list.Add(" "); //empty line
            list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client,
                "DamageSpeedDecrease.DelveInfo.Decrease", Spell.Value));
            if (Spell.Damage != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Damage",
                    Spell.Damage.ToString("0.###;0.###'%'")));
            if (Spell.LifeDrainReturn != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.HealthReturned",
                    Spell.LifeDrainReturn));
            list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Target", Spell.Target));
            if (Spell.Range != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Range", Spell.Range));
            if (Spell.Duration >= ushort.MaxValue * 1000)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Duration") +
                         " Permanent.");
            else if (Spell.Duration > 60000)
                list.Add(string.Format(
                    LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Duration") +
                    Spell.Duration / 60000 + ":" + (Spell.Duration % 60000 / 1000).ToString("00") + " min"));
            else if (Spell.Duration != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Duration") +
                         (Spell.Duration / 1000).ToString("0' sec';'Permanent.';'Permanent.'"));
            if (Spell.Frequency != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Frequency",
                    (Spell.Frequency * 0.001).ToString("0.0")));
            if (Spell.Power != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.PowerCost",
                    Spell.Power.ToString("0;0'%'")));
            list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.CastingTime",
                (Spell.CastTime * 0.001).ToString("0.0## sec;-0.0## sec;'instant'")));
            if (Spell.RecastDelay > 60000)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.RecastTime") +
                         (Spell.RecastDelay / 60000).ToString() + ":" +
                         (Spell.RecastDelay % 60000 / 1000).ToString("00") + " min");
            else if (Spell.RecastDelay > 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.RecastTime") +
                         (Spell.RecastDelay / 1000).ToString() + " sec");
            if (Spell.Concentration != 0)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.ConcentrationCost",
                    Spell.Concentration));
            if (Spell.Radius != 0)
                list.Add(
                    LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Radius", Spell.Radius));
            if (Spell.DamageType != eDamageType.Natural)
                list.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "DelveInfo.Damage",
                    GlobalConstants.DamageTypeToName(Spell.DamageType)));

            return list;
        }
    }

    // counstructor
    public DamageSpeedDecreaseSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell,
        line)
    {
    }
}