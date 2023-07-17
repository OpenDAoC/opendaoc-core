using System;

using DOL.Database;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Disease always debuffs the target by 7.5% movement
	/// and 15% total hit points, and prevents health regeneration.
	/// http://www.camelotherald.com/article.php?id=63
	/// Here they say hit points but spell description states that
	/// it is strength, what should I use hmm...
	/// </summary>
	[SpellHandlerAttribute("Disease")]
	public class DiseaseSpellHandler : SpellHandler
	{
        public override void CreateECSEffect(ECSGameEffectInitParams initParams)
        {
            new DiseaseEcsEffect(initParams);
        }

        /// <summary>
        /// called after normal spell cast is completed and effect has to be started
        /// </summary>
        public override void FinishSpellCast(GameLiving target)
		{
			m_caster.Mana -= PowerCost(target);
			base.FinishSpellCast(target);
		}
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
			target.StartInterruptTimer(target.SpellInterruptDuration, AttackData.eAttackType.Spell, Caster);
			base.ApplyEffectOnTarget(target, effectiveness);
        }

        /// <summary>
        /// When an applied effect starts
        /// duration spells only
        /// </summary>
        /// <param name="effect"></param>
        public override void OnEffectStart(GameSpellEffect effect)
		{
			//base.OnEffectStart(effect);

			//if (effect.Owner.Realm == 0 || Caster.Realm == 0)
			//{
			//	effect.Owner.LastAttackedByEnemyTickPvE = effect.Owner.CurrentRegion.Time;
			//	Caster.LastAttackTickPvE = Caster.CurrentRegion.Time;
			//}
			//else
			//{
			//	effect.Owner.LastAttackedByEnemyTickPvP = effect.Owner.CurrentRegion.Time;
			//	Caster.LastAttackTickPvP = Caster.CurrentRegion.Time;
			//}

			//GameSpellEffect mezz = SpellHandler.FindEffectOnTarget(effect.Owner, "Mesmerize");
 		//	if(mezz != null) mezz.Cancel(false);
			//effect.Owner.Disease(true);
			//effect.Owner.BuffBonusMultCategory1.Set((int)eProperty.MaxSpeed, this, 1.0 - 0.15);
			//effect.Owner.BuffBonusMultCategory1.Set((int)eProperty.Strength, this, 1.0 - 0.075);

			//SendUpdates(effect);

			//MessageToLiving(effect.Owner, Spell.Message1, eChatType.CT_Spell);
			//Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message2, effect.Owner.GetName(0, true)), eChatType.CT_System, effect.Owner);

			//effect.Owner.StartInterruptTimer(effect.Owner.SpellInterruptDuration, AttackData.eAttackType.Spell, Caster);
			//if (effect.Owner is GameNPC)
			//{
			//	IOldAggressiveBrain aggroBrain = ((GameNPC)effect.Owner).Brain as IOldAggressiveBrain;
			//	if (aggroBrain != null)
			//		aggroBrain.AddToAggroList(Caster, 1);
			//}
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
			//base.OnEffectExpires(effect, noMessages);
			//effect.Owner.Disease(false);
			//effect.Owner.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
			//effect.Owner.BuffBonusMultCategory1.Remove((int)eProperty.Strength, this);

			//if (!noMessages)
			//{
			//	MessageToLiving(effect.Owner, Spell.Message3, eChatType.CT_SpellExpires);
			//	Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message4, effect.Owner.GetName(0, true)), eChatType.CT_SpellExpires, effect.Owner);
			//}

			//SendUpdates(effect);

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
			double duration = base.CalculateEffectDuration(target, effectiveness);
			duration -= duration * target.GetResist(Spell.DamageType) * 0.01;

			if (duration < 1)
				duration = 1;
			else if (duration > (Spell.Duration * 4))
				duration = (Spell.Duration * 4);
			return (int)duration;
		}

		/// <summary>
		/// Sends needed updates on start/stop
		/// </summary>
		/// <param name="effect"></param>
		public virtual void SendUpdates(ECSGameEffect effect)
		{
            GamePlayer player = effect.Owner as GamePlayer;
            if (player != null)
            {
                if (!player.IsMezzed && !player.IsStunned)
                    player.Out.SendUpdateMaxSpeed();
                player.Out.SendCharStatsUpdate();
                player.Out.SendUpdateWeaponAndArmorStats();
            }

            GameNPC npc = effect.Owner as GameNPC;
            if (npc != null)
            {
                short maxSpeed = npc.MaxSpeed;
                if (npc.CurrentSpeed > maxSpeed)
                    npc.CurrentSpeed = maxSpeed;
            }
        }

		public override DbPlayerXEffects GetSavedEffect(GameSpellEffect e)
		{
			if ( //VaNaTiC-> this cannot work, cause PulsingSpellEffect is derived from object and only implements IConcEffect
			     //e is PulsingSpellEffect ||
			     //VaNaTiC<-
			    Spell.Pulse != 0 || Spell.Concentration != 0 || e.RemainingTime < 1)
				return null;
			DbPlayerXEffects eff = new DbPlayerXEffects();
			eff.Var1 = Spell.ID;
			eff.Duration = e.RemainingTime;
			eff.IsHandler = true;
			eff.SpellLine = SpellLine.KeyName;
			return eff;
		}

		public override void OnEffectRestored(GameSpellEffect effect, int[] vars)
		{
			effect.Owner.Disease(true);
			effect.Owner.BuffBonusMultCategory1.Set((int)EProperty.MaxSpeed, this, 1.0 - 0.15);
			effect.Owner.BuffBonusMultCategory1.Set((int)EProperty.Strength, this, 1.0 - 0.075);
		}

		public override int OnRestoredEffectExpires(GameSpellEffect effect, int[] vars, bool noMessages)
		{
			return this.OnEffectExpires(effect, noMessages);
		}

		// constructor
		public DiseaseSpellHandler(GameLiving caster, Spell spell, SpellLine spellLine) : base(caster, spell, spellLine) {}
	}
}
