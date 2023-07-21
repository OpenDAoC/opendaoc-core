using System;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
	/// <summary>
	/// All stats debuff spell handler
	/// </summary>
	[SpellHandlerAttribute("AllStatsPercentDebuff")]
	public class AllStatsPercentDebuffHandler : SpellHandler
	{
        protected int StrDebuff = 0;
        protected int DexDebuff = 0;
        protected int ConDebuff = 0;
        protected int EmpDebuff = 0;
        protected int QuiDebuff = 0;
        protected int IntDebuff = 0;
        protected int ChaDebuff = 0;
        protected int PieDebuff = 0;

		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
		public override void OnEffectStart(GameSpellEffect effect)
		{
			base.OnEffectStart(effect); 
			//effect.Owner.DebuffCategory[(int)eProperty.Dexterity] += (int)m_spell.Value;
            double percentValue = (m_spell.Value) / 100;
            StrDebuff = (int)((double)effect.Owner.GetModified(EProperty.Strength) * percentValue);
            DexDebuff = (int)((double)effect.Owner.GetModified(EProperty.Dexterity) * percentValue);
            ConDebuff = (int)((double)effect.Owner.GetModified(EProperty.Constitution) * percentValue);
            EmpDebuff = (int)((double)effect.Owner.GetModified(EProperty.Empathy) * percentValue);
            QuiDebuff = (int)((double)effect.Owner.GetModified(EProperty.Quickness) * percentValue);
            IntDebuff = (int)((double)effect.Owner.GetModified(EProperty.Intelligence) * percentValue);
            ChaDebuff = (int)((double)effect.Owner.GetModified(EProperty.Charisma) * percentValue);
            PieDebuff = (int)((double)effect.Owner.GetModified(EProperty.Piety) * percentValue);
            

            effect.Owner.DebuffCategory[(int)EProperty.Dexterity] += DexDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Strength] += StrDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Constitution] += ConDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Piety] += PieDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Empathy] += EmpDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Quickness] += QuiDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Intelligence] += IntDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Charisma] += ChaDebuff;

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
            double percentValue = (m_spell.Value) / 100;

            effect.Owner.DebuffCategory[(int)EProperty.Dexterity] -= DexDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Strength] -= StrDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Constitution] -= ConDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Piety] -= PieDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Empathy] -= EmpDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Quickness] -= QuiDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Intelligence] -= IntDebuff;
            effect.Owner.DebuffCategory[(int)EProperty.Charisma] -= ChaDebuff;

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
				IOldAggressiveBrain aggroBrain = ((GameNPC)target).Brain as IOldAggressiveBrain;
				if (aggroBrain != null)
					aggroBrain.AddToAggroList(Caster, (int)Spell.Value);
			}
		}
        public AllStatsPercentDebuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}