using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.Spells.Atlantis
{
    /// <summary>
    /// Arrogance spell handler
    /// </summary>
    [SpellHandlerAttribute("Arrogance")]
    public class Arrogance : SpellHandler
    {
    	GamePlayer playertarget = null;
    	
        /// <summary>
        /// The timer that will cancel the effect
        /// </summary>
        protected ECSGameTimer m_expireTimer;
        public override void OnEffectStart(GameSpellEffect effect)
        {
        	base.OnEffectStart(effect);
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Dexterity] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Strength] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Constitution] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Acuity] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Piety] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Empathy] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Quickness] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Intelligence] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Charisma] += (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.ArmorAbsorption] += (int)m_spell.Value;                       
            
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
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Dexterity] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Strength] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Constitution] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Acuity] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Piety] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Empathy] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Quickness] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Intelligence] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.Charisma] -= (int)m_spell.Value;
            effect.Owner.BaseBuffBonusCategory[(int)eProperty.ArmorAbsorption] -= (int)m_spell.Value;
             
            if (effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
                player.Out.SendCharStatsUpdate();
                player.UpdateEncumberance();
                player.UpdatePlayerStatus();
            	player.Out.SendUpdatePlayer();  
                Start(player);
            }
            return base.OnEffectExpires(effect,noMessages);
        }

        protected virtual void Start(GamePlayer player)
        {
        	playertarget = player;
            StartTimers();
            player.DebuffCategory[(int)eProperty.Dexterity] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Strength] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Constitution] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Acuity] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Piety] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Empathy] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Quickness] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Intelligence] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.Charisma] += (int)m_spell.Value;
            player.DebuffCategory[(int)eProperty.ArmorAbsorption] += (int)m_spell.Value;
            
            player.Out.SendCharStatsUpdate();
            player.UpdateEncumberance();
            player.UpdatePlayerStatus();
          	player.Out.SendUpdatePlayer(); 
        }

        protected virtual void Stop()
        {
            if (playertarget != null)
            {     
	            playertarget.DebuffCategory[(int)eProperty.Dexterity] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Strength] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Constitution] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Acuity] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Piety] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Empathy] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Quickness] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Intelligence] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.Charisma] -= (int)m_spell.Value;;
	            playertarget.DebuffCategory[(int)eProperty.ArmorAbsorption] -= (int)m_spell.Value;;
	            
            	playertarget.Out.SendCharStatsUpdate();
            	playertarget.UpdateEncumberance();
            	playertarget.UpdatePlayerStatus();
          		playertarget.Out.SendUpdatePlayer(); 
            }
            StopTimers();
        }
        protected virtual void StartTimers()
        {
            StopTimers();
            m_expireTimer = new ECSGameTimer(playertarget, new ECSGameTimer.ECSTimerCallback(ExpiredCallback), 10000);
        }
        protected virtual void StopTimers()
        {
            if (m_expireTimer != null)
            {
                m_expireTimer.Stop();
                m_expireTimer = null;
            }
        }
        protected virtual int ExpiredCallback(ECSGameTimer callingTimer)
        {
            Stop();
            return 0;
        }

        public Arrogance(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}
