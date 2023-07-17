using System;
using DOL.AI.Brain;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.Language;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Handler for spells that are issued by the player, but cast
	/// by his pet.
	/// </summary>
	/// <author>Aredhel</author>
	[SpellHandler("PetSpell")]
	class PetSpellHandler : SpellHandler
	{
		/// <summary>
		/// Calculate casting time based on delve and dexterity stat bonus.
		/// Necromancers do not benefit from TrialsOfAtlantis Casting Speed Bonuses.
		/// </summary>
		/// <returns></returns>
		public override int CalculateCastingTime()
		{
			int ticks = m_spell.CastTime;
			ticks = (int)(ticks * Math.Max(m_caster.CastingSpeedReductionCap, m_caster.DexterityCastTimeReduction));
            if (ticks < m_caster.MinimumCastingSpeed)
                ticks = m_caster.MinimumCastingSpeed;
			return ticks;
		}

		/// <summary>
		/// Check if we have a pet to start with.
		/// </summary>
		/// <param name="selectedTarget"></param>
		/// <returns></returns>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			if (!base.CheckBeginCast(selectedTarget))
				return false;

			if (Caster.ControlledBrain == null)
			{
				MessageToCaster(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "PetSpellHandler.CheckBeginCast.NoControlledBrainForCast"), EChatType.CT_SpellResisted);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Called when spell has finished casting.
		/// </summary>
		/// <param name="target"></param>
		public override void FinishSpellCast(GameLiving target)
		{
			GamePlayer player = Caster as GamePlayer;

			if (player == null || player.ControlledBrain == null) 
                return;

			// No power cost, we'll drain power on the caster when
			// the pet actually starts casting it.
			// If there is an ID, create a sub spell for the pet.

			int powerCost = PowerCost(player);

			if (powerCost > 0)
				player.ChangeMana(player, EPowerChangeType.Spell, -powerCost);

			ControlledNpcBrain petBrain = player.ControlledBrain as ControlledNpcBrain;
			if (petBrain != null && Spell.SubSpellID > 0)
			{
				Spell spell = SkillBase.GetSpellByID(Spell.SubSpellID);
                if (spell != null && spell.SubSpellID == 0)
                {
                    spell.Level = Spell.Level;
                    petBrain.Notify(GameNpcEvent.PetSpell, this,
                        new PetSpellEventArgs(spell, SpellLine, target, Spell));
                }
			}

            // Facilitate Painworking.

            if (Spell.RecastDelay > 0 && m_startReuseTimer)
            {
                foreach (Spell spell in SkillBase.GetSpellList(SpellLine.KeyName))
                {
                    if (spell.SpellType == Spell.SpellType && 
                        spell.RecastDelay == Spell.RecastDelay 
                        && spell.Group == Spell.Group)
                        Caster.DisableSkill(spell, spell.RecastDelay);
                }
            }
        }

		/// <summary>
		/// Creates a new pet spell handler.
		/// </summary>
		public PetSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine)
			: base(caster, spell, spellLine)
		{
		}
	}
}
