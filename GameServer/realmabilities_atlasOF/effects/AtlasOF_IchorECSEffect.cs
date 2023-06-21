using System;
using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;
using DOL.GS.Spells;

namespace DOL.GS.Effects
{
    public class AtlasOF_IchorOfTheDeepECSEffect : ECSGameAbilityEffect
    {
	    private ECSGameTimer m_rootExpire;
	    private static SpellHandler m_ichorHandler;
	    private int m_rootDur = m_ichorHandler.Spell.Duration;
	    private Spell m_spell = m_ichorHandler.Spell;
        private SpellLine m_spellline;
        public Spell Spell { get { return m_spell; } }
        public SpellLine SpellLine { get { return m_spellline; } }
        public SpellHandler SpellHandler { get { return m_ichorHandler; }}

        /// <summary>
        /// The Spell Caster
        /// </summary>
        private GamePlayer Caster { get; set; }

        public AtlasOF_IchorOfTheDeepECSEffect(ECSGameEffectInitParams initParams)
            : base(initParams)
        {
	        Caster = (GamePlayer)SpellHandler.Caster;
	        Owner = (GameLiving)m_ichorHandler.Caster.TargetObject;
            EffectType = eEffect.IchorOfTheDeep;
            Duration = m_rootDur;
            Effectiveness = 1;
            CancelEffect = false;
            RenewEffect = false;
            IsDisabled = false;
            IsBuffActive = false;
            ExpireTick = Duration + GameLoop.GameLoopTime;
            StartTick = GameLoop.GameLoopTime;
            LastTick = 0;
            NextTick = 0;

            EffectService.RequestStartEffect(this);
        }

        public override ushort Icon { get { return 7029; } }
        public override string Name { get { return "Ichor of the Deep"; } }
        public override bool HasPositiveEffect { get { return false; } }
        public override bool IsConcentrationEffect() { return false; }

        public override void OnStartEffect()
        {
	        var caster = Caster;
			var target = Owner;

			if (caster == null || target == null)
				return;

			var effect = this;

			// Send spell message to player if applicable
			if (target is GamePlayer gpMessage)
				gpMessage.Out.SendMessage("Constricting bonds surround your body!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);

			// Apply the snare
			m_rootExpire = new ECSGameTimer(target, new ECSGameTimer.ECSTimerCallback(RootExpires), (int)effect.Duration);
			target.BuffBonusMultCategory1.Set((int)eProperty.MaxSpeed, this, 1.0 - (Spell.Value * 0.01));
			target.TempProperties.setProperty(effect, m_rootExpire);
			SendUpdates(target);
			m_rootExpire.Interval = 650;
			m_rootExpire.Start((int)(1 + (effect.Duration >> 1)));
			GameEventMgr.AddHandler(target, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttacked));

			var upperCase = SpellHandler.Spell.Message2.StartsWith("{0}");
			if (target is GamePlayer pTarget)
				SpellHandler.MessageToLiving(pTarget, SpellHandler.Spell.Message1, eChatType.CT_Spell);

			if (caster.IsWithinRadius(target, Spell.Radius))
				SpellHandler.MessageToCaster(Util.MakeSentence(SpellHandler.Spell.Message2, target.GetName(0, true)), eChatType.CT_Spell);
			Message.SystemToArea(target, Util.MakeSentence(SpellHandler.Spell.Message2, target.GetName(0, upperCase)), eChatType.CT_Spell, target, caster);
        }

        public override void OnStopEffect()
        {
	        var effect = this;
	        var target = Owner;
	        if (target == null)
		        return;

	        ECSGameTimer timer = (ECSGameTimer)target.TempProperties.getProperty<object>(effect, m_rootExpire);
	        if (timer != null)
		        timer.Stop();

	        target.TempProperties.removeProperty(effect);
	        effect.Owner.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
	        SendUpdates(effect.Owner);

	        GameEventMgr.RemoveHandler(target, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttacked));
        }
        /// <summary>
        /// Sends updates on effect start/stop
        /// </summary>
        /// <param name="owner"></param>
        public static void SendUpdates(GameLiving owner)
        {
            if (owner.IsMezzed || owner.IsStunned)
                return;

            GamePlayer player = owner as GamePlayer;

            if (player != null)
                player.Out.SendUpdateMaxSpeed();

            GameNPC npc = owner as GameNPC;
            if (npc != null)
            {
                short maxSpeed = npc.MaxSpeed;
                if (npc.CurrentSpeed > maxSpeed)
                    npc.CurrentSpeed = maxSpeed;
            }
        }

        /// <summary>
        /// Handles attack on buff owner
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        protected virtual void OnAttacked(DOLEvent e, object sender, EventArgs arguments)
        {
	        var attackArgs = arguments as AttackedByEnemyEventArgs;
	        var living = sender as GameLiving;
	        if (attackArgs == null) return;
	        if (living == null) return;

	        switch (attackArgs.AttackData.AttackResult)
	        {
		        case eAttackResult.HitStyle:
		        case eAttackResult.HitUnstyled:
		        {
			        var ichor = EffectListService.GetEffectOnTarget(living, eEffect.IchorOfTheDeep);

			        if (ichor != null)
			        {
				        if (attackArgs.AttackData.Damage > 0)
					        EffectService.RequestImmediateCancelEffect(ichor);

				        var speedDebuff = EffectListService.GetSpellEffectOnTarget(living, eEffect.MovementSpeedDebuff);
				        if (speedDebuff != null && ichor != null)
				        {
					        if (speedDebuff.Duration < ichor.Duration && attackArgs.AttackData.Damage < 1 && !Spell.Name.Equals("Prevent Flight"))
								EffectService.RequestImmediateCancelEffect(speedDebuff);
					        else
								EffectService.RequestImmediateCancelEffect(ichor);
				        }

				        var mez = EffectListService.GetEffectOnTarget(living, eEffect.Mez);
				        if (mez != null && ichor != null)
					        EffectService.RequestImmediateCancelEffect(ichor);

				        var stun = EffectListService.GetEffectOnTarget(living, eEffect.Stun);
				        if (stun != null && ichor != null)
					        EffectService.RequestImmediateCancelEffect(ichor);
			        }
			        RootExpires(m_rootExpire);
		        }
			        break;
	        }
        }

        protected virtual int RootExpires(ECSGameTimer timer)
        {
	        if (timer == null)
		        return 0;

	        if (Owner == null)
		        return 0;

	        var target = Owner;

	        var ichor = EffectListService.GetEffectOnTarget(target, eEffect.IchorOfTheDeep);

	        if (ichor == null)
	        {
		        target.TempProperties.removeProperty(timer);
		        target.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
		        SendUpdates(target);
		        timer.Stop();
		        timer = null;
		        return 0;
	        }

	        var sos = target.EffectList.CountOfType<SpeedOfSoundEffect>() == 1;
	        if (sos)
	        {
		        EffectService.RequestImmediateCancelEffect(ichor);
		        target.TempProperties.removeProperty(timer);
		        target.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
		        SendUpdates(target);
		        timer.Stop();
		        timer = null;
		        return 0;
	        }

	        if (GetRemainingTimeForClient() > 0)
	        {
				OnTick();
		        return timer.Interval;
	        }

	        EffectService.RequestImmediateCancelEffect(ichor);
	        target.TempProperties.removeProperty(timer);
	        target.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
	        SendUpdates(target);

	        timer.Stop();
	        timer = null;
	        return 0;
        }

        /// <summary>
        /// Called on every timer tick
        /// </summary>
        private void OnTick()
        {
	        var effect = this;
	        var owner = Owner;
	        if (owner == null)
		        return;
	        var factor = 2.0 - (effect.Duration - (GetRemainingTimeForClient() / 1000)) / (double)(effect.Duration >> 1);
	        if (factor < 0) factor = 0;
	        else if (factor > 1) factor = 1;

	        owner.BuffBonusMultCategory1.Set((int)eProperty.MaxSpeed, effect, 1.0 - ((SpellHandler.Spell.Value * factor) * 0.01));

	        if (factor <= 0)
	        {
		        var ichor = EffectListService.GetEffectOnTarget(owner, eEffect.IchorOfTheDeep);
		        if (ichor != null)
			        EffectService.RequestImmediateCancelEffect(ichor);
		        if (m_rootExpire != null)
		        {
			        m_rootExpire.Stop();
			        m_rootExpire = null;
		        }
	        }

	        SendUpdates(effect.Owner);
        }
    }
}
