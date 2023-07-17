using System.Reflection;
using DOL.GS;
using DOL.GS.RealmAbilities;
using log4net;

namespace DOL.AI.Brain
{
	/// <summary>
	/// A brain that can be controlled
	/// </summary>
	public class BdMeleeBrain : BdPetBrain
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructs new controlled npc brain
		/// </summary>
		/// <param name="owner"></param>
		public BdMeleeBrain(GameLiving owner) : base(owner) { }

		#region AI

		/// <summary>
		/// Checks the Abilities
		/// </summary>
		public override void CheckAbilities()
		{
			//load up abilities
			if (Body.Abilities != null && Body.Abilities.Count > 0)
			{
				foreach (Ability ab in Body.Abilities.Values)
				{
					switch (ab.KeyName)
					{
						case Abilities.ChargeAbility:
							if (Body.TargetObject != null && !Body.IsWithinRadius(Body.TargetObject, 500 ))
							{
								ChargeAbility charge = Body.GetAbility<ChargeAbility>();
								if (charge != null && Body.GetSkillDisabledDuration(charge) <= 0)
								{
									charge.Execute(Body);
								}
							}
							break;
					}
				}
			}
		}

		protected override bool CheckDefensiveSpells(Spell spell) { return false; }
		protected override bool CheckOffensiveSpells(Spell spell) { return false; }

		/// <summary>
		/// Checks Instant Spells.  Handles Taunts, shouts, stuns, etc.
		/// </summary>
		protected override bool CheckInstantSpells(Spell spell)
		{
			GameObject lastTarget = Body.TargetObject;
			Body.TargetObject = null;
			switch (spell.SpellType)
			{
				case ESpellType.Taunt:
					Body.TargetObject = lastTarget;
					break;
			}

			if (Body.TargetObject != null)
			{
				if (LivingHasEffect((GameLiving)Body.TargetObject, spell))
					return false;
				Body.CastSpell(spell, m_mobSpellLine);
				//Body.TargetObject = lastTarget;
				return true;
			}
			Body.TargetObject = lastTarget;
			return false;
		}

		#endregion
	}
}
