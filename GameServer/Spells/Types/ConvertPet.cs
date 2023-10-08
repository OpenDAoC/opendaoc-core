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

using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Based on HealSpellHandler.cs
	/// Spell calculates a percentage of the caster's health.
	/// Heals target for the full amount, Caster loses half that amount in health.
	/// </summary>
	[SpellHandler("PetConversion")]
	public class PetConversionSpellHandler : SpellHandler
	{
		// constructor
		public PetConversionSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

		/// <summary>
		/// Execute pet conversion spell
		/// </summary>
		public override bool StartSpell(GameLiving target)
		{
			var targets = SelectTargets(target);
			if (targets.Count <= 0)
				if (m_caster.ControlledBrain != null)
					targets.Add(m_caster.ControlledBrain.Body);
				else
				{
					return false;
				}
			
			int mana = 0;

			foreach (GameLiving living in targets)
			{
				ApplyEffectOnTarget(living);
				mana += (int)(living.Health * Spell.Value / 100);
			}

			int absorb = m_caster.ChangeMana(m_caster, EPowerChangeType.Spell, mana);

			if (m_caster is GamePlayer)
			{
				if (absorb > 0)
					MessageToCaster("You absorb " + absorb + " power points.", EChatType.CT_Spell);
				else
					MessageToCaster("Your power is already full!", EChatType.CT_SpellResisted);
				((GamePlayer)m_caster).CommandNpcRelease();
			}

			return true;
		}
	}
}
