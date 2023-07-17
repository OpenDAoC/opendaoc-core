namespace DOL.GS.Spells
{
	/// <summary>
	/// Palading heal chant works only in combat
	/// </summary>
	[SpellHandlerAttribute("CombatHeal")]
	public class CombatHealSpellHandler : HealSpellHandler
	{
		public CombatHealSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine) { }

		/// <summary>
		/// Execute heal spell
		/// </summary>
		/// <param name="target"></param>
		public override bool StartSpell(GameLiving target)
		{
			m_startReuseTimer = true;

			foreach (GameLiving member in GetGroupAndPets(Spell))
			{
				new CombatHealEcsEffect(new ECSGameEffectInitParams(member, Spell.Frequency, Caster.Effectiveness, this));
			}

			GamePlayer player = Caster as GamePlayer;

			if (!Caster.InCombat && (player==null || player.Group==null || !player.Group.IsGroupInCombat()))
				return false; // Do not start healing if not in combat

			return base.StartSpell(target);
		}
	}
}
