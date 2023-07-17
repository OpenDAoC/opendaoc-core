
using System;
using System.Collections;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
    [SpellHandlerAttribute("Fury")]
    public class FuryHandler : SpellHandler
    {
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            base.ApplyEffectOnTarget(target, 1);
        }

        public override void OnEffectStart(GameSpellEffect effect)
        {
            int value = (int)m_spell.Value;

            SendEffectAnimation(effect.Owner, 0, false, 1);
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Body] += value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Cold] += value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Energy] += value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Heat] += value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Matter] += value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Spirit] += value;

            GamePlayer player = effect.Owner as GamePlayer;
            if (player != null)
            {
                player.Out.SendCharStatsUpdate();
                player.UpdatePlayerStatus();
                player.Out.SendCharResistsUpdate();
            }
        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            int value = (int)m_spell.Value;

            effect.Owner.AbilityBonus[(int)EProperty.Resist_Body] -= value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Cold] -= value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Energy] -= value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Heat] -= value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Matter] -= value;
            effect.Owner.AbilityBonus[(int)EProperty.Resist_Spirit] -= value;

            GamePlayer player = effect.Owner as GamePlayer;
            if (player != null)
            {
                player.Out.SendCharStatsUpdate();
                player.UpdatePlayerStatus();
                player.Out.SendCharResistsUpdate();
            }

            return 0;
        }

        public FuryHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
}