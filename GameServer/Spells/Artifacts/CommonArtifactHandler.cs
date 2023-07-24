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
    [SpellHandlerAttribute("AllStatsDebuff")]
    public class AllStatsDebuffHandler : SpellHandler
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
            
            if(effect.Owner is GamePlayer)
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
 
            if(effect.Owner is GamePlayer)
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
			if(target is GameNpc) 
			{
				IOldAggressiveBrain aggroBrain = ((GameNpc)target).Brain as IOldAggressiveBrain;
				if (aggroBrain != null)
					aggroBrain.AddToAggroList(Caster, (int)Spell.Value);
			}
		}		
        public AllStatsDebuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
 
    /// <summary>
    /// Lore debuff spell handler (Magic resist debuff)
    /// </summary>
    [SpellHandlerAttribute("LoreDebuff")]
    public class LoreDebuffHandler : SpellHandler
    {
 		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
        public override void OnEffectStart(GameSpellEffect effect)
        {
        	base.OnEffectStart(effect);      
        	effect.Owner.DebuffCategory[(int)EProperty.SpellDamage] += (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Heat] += (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Cold] += (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Matter] += (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Spirit] += (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Energy] += (int)Spell.Value;
            
            if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
             	player.Out.SendCharResistsUpdate(); 
             	player.UpdatePlayerStatus();
            }                       
        }
        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            effect.Owner.DebuffCategory[(int)EProperty.SpellDamage] -= (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Heat] -= (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Cold] -= (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Matter] -= (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Spirit] -= (int)Spell.Value;
            effect.Owner.DebuffCategory[(int)EProperty.Resist_Energy] -= (int)Spell.Value;
            
            if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
             	player.Out.SendCharResistsUpdate(); 
             	player.UpdatePlayerStatus();
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
			if(target is GameNpc) 
			{
				IOldAggressiveBrain aggroBrain = ((GameNpc)target).Brain as IOldAggressiveBrain;
				if (aggroBrain != null)
					aggroBrain.AddToAggroList(Caster, (int)Spell.Value);
			}
		}	
        public LoreDebuffHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// Strength/Constitution drain spell handler
    /// </summary>
    [SpellHandlerAttribute("StrengthConstitutionDrain")]
    public class StrConDrainHandler : StrengthConDebuff
    {  	
		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
        public override void OnEffectStart(GameSpellEffect effect)
        {
        	base.OnEffectStart(effect);         
            Caster.BaseBuffBonusCategory[(int)EProperty.Strength] += (int)m_spell.Value;
            Caster.BaseBuffBonusCategory[(int)EProperty.Constitution] += (int)m_spell.Value;
 
            if(Caster is GamePlayer)
            {
            	GamePlayer player = Caster as GamePlayer;          	
             	player.Out.SendCharStatsUpdate(); 
             	player.UpdateEncumberance();
             	player.UpdatePlayerStatus();
            } 
        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {           
            Caster.BaseBuffBonusCategory[(int)EProperty.Strength] -= (int)m_spell.Value;
            Caster.BaseBuffBonusCategory[(int)EProperty.Constitution] -= (int)m_spell.Value;          
 
            if(Caster is GamePlayer)
            {
            	GamePlayer player = Caster as GamePlayer;          	
             	player.Out.SendCharStatsUpdate(); 
             	player.UpdateEncumberance();
             	player.UpdatePlayerStatus();
            } 
            return base.OnEffectExpires(effect,noMessages);
        } 
        
        public StrConDrainHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    /// <summary>
    /// ABS Damage shield spell handler
    /// </summary>
    [SpellHandlerAttribute("ABSDamageShield")]
    public class AbsDamageShieldHandler : AblativeArmorHandler
    {
        public override void OnDamageAbsorbed(AttackData ad, int DamageAmount)
        {
            AttackData newad = new AttackData();
            newad.Attacker = ad.Target;
            newad.Target = ad.Attacker;
            newad.Damage = DamageAmount;
            newad.DamageType = Spell.DamageType;
            newad.AttackType = AttackData.EAttackType.Spell;
            newad.AttackResult = EAttackResult.HitUnstyled;
            newad.SpellHandler = this;
            newad.Target.OnAttackedByEnemy(newad);
            newad.Attacker.DealDamage(newad);
        }
        public AbsDamageShieldHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    
    /// <summary>
    /// Morph spell handler
    /// </summary>
    [SpellHandlerAttribute("Morph")]
    public class MorphHandler : SpellHandler
    {
        public override void OnEffectStart(GameSpellEffect effect)
        {    
           if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;  
            	player.Model = (ushort)Spell.LifeDrainReturn;     
            	player.Out.SendUpdatePlayer();  
            }       	
     		base.OnEffectStart(effect); 
        }
        
        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
           if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
                GameClient client = player.Client;
 				player.Model = (ushort)client.Account.Characters[client.ActiveCharIndex].CreationModel;            	
 				player.Out.SendUpdatePlayer();  
            }                       
            return base.OnEffectExpires(effect, noMessages);         	
        }    	
    	public MorphHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }   
 
    /// <summary>
    /// Arcane leadership spell handler (range+resist pierce)
    /// </summary>
    [SpellHandlerAttribute("ArcaneLeadership")]
    public class ArcaneLeadershipHandler : CloudsongAuraSpellHandler
    {
    	public ArcaneLeadershipHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }   
}
