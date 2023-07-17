using System;

using DOL.Database;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Abstract Vampiir Ability using Level Based Ability to enable stat changing with Ratio Preset.
	/// </summary>
	public abstract class VampiirAbility : LevelBasedStatChangingAbility
	{
		/// <summary>
		/// Multiplier for Ability Level to adjust Stats for given Ability 
		/// </summary>
		public abstract int RatioByLevel { get; }
				
		/// <summary>
		/// Return Amount for this Stat Changing Ability
		/// Based on Current Ability Level and Ratio Multiplier
		/// </summary>
		/// <param name="level">Targeted Ability Level</param>
		/// <returns>Stat Changing amount</returns>
		public override int GetAmountForLevel(int level)
		{
			//(+stats every level starting level 6),
			return level < 6 ? 0 : (level - 5) * RatioByLevel;
		}
		
		protected VampiirAbility(DbAbilities dba, int level, eProperty property)
			: base(dba, level, property)
		{
		}
	}

	/// <summary>
	/// Vampiir Ability for Strength Stat
	/// </summary>
	public class VampiirStrength : VampiirAbility
	{
		/// <summary>
		/// Ratio Preset to *3
		/// </summary>
		public override int RatioByLevel { get { return 3; } }
		
		public VampiirStrength(DbAbilities dba, int level)
			: base(dba, level, eProperty.Strength)
		{
		}
	}

	/// <summary>
	/// Vampiir Ability for Strength Stat
	/// </summary>
	public class VampiirDexterity : VampiirAbility
	{
		/// <summary>
		/// Ratio Preset to *3
		/// </summary>
		public override int RatioByLevel { get { return 3; } }

		public VampiirDexterity(DbAbilities dba, int level)
			: base(dba, level, eProperty.Dexterity)
		{
		}
	}

	/// <summary>
	/// Vampiir Ability for Strength Stat
	/// </summary>
	public class VampiirConstitution : VampiirAbility
	{
		/// <summary>
		/// Ratio Preset to *3
		/// </summary>
		public override int RatioByLevel { get { return 3; } }

		public VampiirConstitution(DbAbilities dba, int level)
			: base(dba, level, eProperty.Constitution)
		{
		}
	}

	/// <summary>
	/// Vampiir Ability for Strength Stat
	/// </summary>
	public class VampiirQuickness : VampiirAbility
	{
		/// <summary>
		/// Ratio Preset to *2
		/// </summary>
		public override int RatioByLevel { get { return 2; } }

		public VampiirQuickness(DbAbilities dba, int level)
			: base(dba, level, eProperty.Quickness)
		{
		}
	}
}
