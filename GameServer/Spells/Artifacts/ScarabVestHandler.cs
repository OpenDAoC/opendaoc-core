using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
    /// <summary>
    /// Scarab proc spell handler
    /// Snare and morph target. Cecity is a subspell.
    /// </summary>
    [SpellHandlerAttribute("ScarabProc")]
    public class ScarabVestHandler : UnbreakableSpeedDecreaseHandler
    {
 		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}
        public override void OnEffectStart(GameSpellEffect effect)
        {        	           
            if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
            	player.Model = (ushort)Spell.LifeDrainReturn; // 1200 is official id
            }     
            base.OnEffectStart(effect);
        }
        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            if(effect.Owner is GamePlayer)
            {
            	GamePlayer player = effect.Owner as GamePlayer;
 				player.Model = player.CreationModel;     
            }                       
            return base.OnEffectExpires(effect, noMessages);
        }
        public ScarabVestHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}
