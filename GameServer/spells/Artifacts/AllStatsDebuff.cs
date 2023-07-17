using DOL.AI.Brain;
using DOL.GS.Effects;

namespace DOL.GS.Spells.Atlantis
{
	/// <summary>
	/// All stats debuff spell handler
	/// </summary>
	[SpellHandlerAttribute("AllStatsDebuff")]
	public class AllStatsDebuff : SpellHandler
	{
		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
		public override void OnEffectStart(GameSpellEffect effect)
		{
			base.OnEffectStart(effect);
			effect.Owner.DebuffCategory[(int)EProperty.Dexterity] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Strength] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Constitution] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Acuity] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Piety] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Empathy] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Quickness] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Intelligence] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Charisma] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.ArmorAbsorption] += (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.MagicAbsorption] += (int)m_spell.Value;

			if (effect.Owner is GamePlayer)
			{
				GamePlayer player = effect.Owner as GamePlayer;
				player.Out.SendCharStatsUpdate();
				player.UpdateEncumberance();
				player.UpdatePlayerStatus();
				player.Out.SendUpdatePlayer();
			}
		}
		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			effect.Owner.DebuffCategory[(int)EProperty.Dexterity] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Strength] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Constitution] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Acuity] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Piety] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Empathy] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Quickness] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Intelligence] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.Charisma] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.ArmorAbsorption] -= (int)m_spell.Value;
			effect.Owner.DebuffCategory[(int)EProperty.MagicAbsorption] -= (int)m_spell.Value;

			if (effect.Owner is GamePlayer)
			{
				GamePlayer player = effect.Owner as GamePlayer;
				player.Out.SendCharStatsUpdate();
				player.UpdateEncumberance();
				player.UpdatePlayerStatus();
				player.Out.SendUpdatePlayer();
			}
			return base.OnEffectExpires(effect, noMessages);
		}

		/// <summary>
		/// Apply effect on target or do spell action if non duration spell
		/// </summary>
		/// <param name="target">target that gets the effect</param>
		/// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			base.ApplyEffectOnTarget(target, effectiveness);
			if (target.Realm == 0 || Caster.Realm == 0)
			{
				target.LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
				Caster.LastAttackTickPvE = GameLoop.GameLoopTime;
			}
			else
			{
				target.LastAttackedByEnemyTickPvP = GameLoop.GameLoopTime;
				Caster.LastAttackTickPvP = GameLoop.GameLoopTime;
			}
			if (target is GameNPC)
			{
				var aggroBrain = ((GameNPC)target).Brain as StandardMobBrain;
				if (aggroBrain != null)
					aggroBrain.AddToAggroList(Caster, (int)Spell.Value);
			}
		}
		public AllStatsDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}