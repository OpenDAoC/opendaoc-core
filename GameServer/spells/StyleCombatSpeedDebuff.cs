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
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells;

/// <summary>
/// Style combat speed debuff effect spell handler
/// </summary>
[SpellHandler("StyleCombatSpeedDebuff")]
public class StyleCombatSpeedDebuff : CombatSpeedDebuff
{
    public override int CalculateSpellResistChance(GameLiving target)
    {
        return 0;
    }

    /// <summary>
    /// Calculates the effect duration in milliseconds
    /// </summary>
    /// <param name="target">The effect target</param>
    /// <param name="effectiveness">The effect effectiveness</param>
    /// <returns>The effect duration in milliseconds</returns>
    protected override int CalculateEffectDuration(GameLiving target, double effectiveness)
    {
        return Spell.Duration;
    }

    // constructor
    public StyleCombatSpeedDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
    {
    }
}