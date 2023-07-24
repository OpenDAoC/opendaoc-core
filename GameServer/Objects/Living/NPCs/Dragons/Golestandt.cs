using System;
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// The Albion dragon.
	/// </summary> 
	public class Golestandt : GameDragon
    {
		#region Add Spawns

		/// <summary>
		/// Spawn adds that will despawn again after 30 seconds.
		/// For Golestandt, these will be level 57-60 GameNPCs and 
		/// their numbers will depend on the number of players inside 
		/// the lair.
		/// </summary>
		/// <returns>Whether or not any adds were spawned.</returns>
		public override bool CheckAddSpawns()
		{
			base.CheckAddSpawns();	// In order to reset HealthPercentOld.

			int numAdds = Math.Max(1, PlayersInLair / 2);
			for (int add = 1; add <= numAdds; ++add)
			{
				SpawnTimedAdd(600, UtilCollection.Random(57, 60),	X + UtilCollection.Random(300, 600), Y + UtilCollection.Random(300, 600), 30, false);	// granite giant pounder lvl 57-60
			}
			return true;
		}

		#endregion

		#region Glare

        /// <summary>
        /// The Glare spell.
        /// </summary>
        protected override Spell Glare
        {
            get
            {
                if (m_glareSpell == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 5700;
                    spell.Description = "Glare";
                    spell.Name = "Dragon Glare";
                    spell.Range = 2500;
                    spell.Radius = 700;
                    spell.Damage = 2000* DragonDifficulty /100;
					spell.RecastDelay = 10;
                    spell.DamageType = (int)EDamageType.Heat;
                    spell.SpellID = 6001;
                    spell.Target = "Enemy";
                    spell.Type = ESpellType.DirectDamage.ToString();
                    m_glareSpell = new Spell(spell, 70);
                    SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_glareSpell);
                }
                return m_glareSpell;
            }
        }

        #endregion

        #region Breath

        /// <summary>
        /// The Breath spell.
        /// </summary>
        protected override Spell Breath
        {
            get
            {
                if (m_breathSpell == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Uninterruptible = true;
                    spell.ClientEffect = 2308;
                    spell.Description = "Nuke";
                    spell.Name = "Dragon Nuke";
                    spell.Range = 700;
                    spell.Radius = 700;
                    spell.Damage = 2000* DragonDifficulty /100;
                    spell.DamageType = (int)EDamageType.Heat;
                    spell.SpellID = 6002;
                    spell.Target = "Enemy";
                    spell.Type = ESpellType.DirectDamage.ToString();
                    m_breathSpell = new Spell(spell, 70);
                    SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_breathSpell);
                }
                return m_breathSpell;
            }
        }

        /// <summary>
        /// The resist debuff spell.
        /// </summary>
        protected override Spell ResistDebuff
        {
            get
            {
                if (m_resistDebuffSpell == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Uninterruptible = true;
                    spell.ClientEffect = 777;
                    spell.Icon = 777;
                    spell.Description = "Heat Resist Debuff";
                    spell.Name = "Melt Armor";
                    spell.Range = 700;
                    spell.Radius = 700;
                    spell.Value = 30* DragonDifficulty/100;
                    spell.Duration = 30;
                    spell.Damage = 0;
                    spell.DamageType = (int)EDamageType.Heat;
                    spell.SpellID = 6003;
                    spell.Target = "Enemy";
                    spell.Type = ESpellType.HeatResistDebuff.ToString();
                    spell.Message1 = "You feel more vulnerable to heat!";
                    spell.Message2 = "{0} seems vulnerable to heat!";
					m_resistDebuffSpell = new Spell(spell, 70);
					SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_resistDebuffSpell);
                }
				return m_resistDebuffSpell;
            }
        }

        #endregion

		#region Melee Debuff

		/// <summary>
		/// The melee debuff spell.
		/// </summary>
		protected override Spell MeleeDebuff
		{
			get
			{
				if (m_meleeDebuffSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AllowAdd = false;
					spell.CastTime = 0;
					spell.Uninterruptible = true;
					spell.ClientEffect = 13082;
					spell.Icon = 13082;
					spell.Description = "Fumble Chance Debuff";
					spell.Name = "Growing Trepidation";
					spell.Range = 700;
					spell.Radius = 700;
					spell.Value = 50;
					spell.Duration = 90* DragonDifficulty /100;
					spell.Damage = 0;
					spell.DamageType = (int)EDamageType.Heat;
					spell.SpellID = 6003;
					spell.Target = "Enemy";
					spell.Type = ESpellType.FumbleChanceDebuff.ToString();
					m_meleeDebuffSpell = new Spell(spell, 70);
					SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_meleeDebuffSpell);
				}
				return m_meleeDebuffSpell;
			}
		}

		#endregion

		#region Ranged Debuff

		/// <summary>
		/// The ranged debuff spell.
		/// </summary>
		protected override Spell RangedDebuff
		{
			get
			{
				if (m_rangedDebuffSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AllowAdd = false;
					spell.CastTime = 0;
					spell.Uninterruptible = true;
					spell.ClientEffect = 590;
					spell.Icon = 590;
					spell.Description = "Nearsight";
					spell.Name = "Dazzling Light";
					spell.Range = 700;
					spell.Radius = 700;
					spell.Value = 100;
					spell.Duration = 90* DragonDifficulty /100;
					spell.Damage = 0;
					spell.DamageType = (int)EDamageType.Heat;
					spell.SpellID = 6003;
					spell.Target = "Enemy";
					spell.Type = ESpellType.Nearsight.ToString();
					spell.Message1 = "You are blinded!";
					spell.Message2 = "{0} is blinded!";
					m_rangedDebuffSpell = new Spell(spell, 70);
					SkillBase.AddScriptedSpell(GlobalSpellsLines.Mob_Spells, m_rangedDebuffSpell);
				}
				return m_rangedDebuffSpell;
			}
		}

		#endregion
	}
}
