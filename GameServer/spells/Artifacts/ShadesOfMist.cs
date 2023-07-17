﻿
using System;
using DOL.GS.Effects;
using DOL.Events;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
    /// <summary>
    /// Shades of Mist spell handler: Shape change (shade) on self with
    /// a defensive proc (200 pt. melee health buffer).
    /// </summary>
    /// <author>Aredhel</author>
    [SpellHandlerAttribute("ShadesOfMist")]
    public class ShadeOfMistDefensiveProcSpellHandler : SpellHandler
    {
        private int ablativehp = 0;
        public override void OnEffectStart(GameSpellEffect effect)
        {

            base.OnEffectStart(effect);
            if (effect.Owner is GamePlayer)
            {
                GamePlayer player = effect.Owner as GamePlayer;
                foreach (GameSpellEffect Effect in player.EffectList.GetAllOfType<GameSpellEffect>())
                {
                    if (Effect.SpellHandler.Spell.SpellType.Equals("TraitorsDaggerProc") ||
                        Effect.SpellHandler.Spell.SpellType.Equals("DreamMorph") ||
                        Effect.SpellHandler.Spell.SpellType.Equals("DreamGroupMorph") ||
                        Effect.SpellHandler.Spell.SpellType.Equals("MaddeningScalars") ||
                        Effect.SpellHandler.Spell.SpellType.Equals("AtlantisTabletMorph") ||
                        Effect.SpellHandler.Spell.SpellType.Equals("AlvarusMorph"))
                    {
                        player.Out.SendMessage("You already have an active morph!", DOL.GS.PacketHandler.eChatType.CT_SpellResisted, DOL.GS.PacketHandler.eChatLoc.CL_ChatWindow);
                        return;
                    }
                }
                player.Model = player.ShadeModel;
                player.Out.SendUpdatePlayer();
                GameEventMgr.AddHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new CoreEventHandler(EventHandler));
            }
        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            if (effect.Owner is GamePlayer)
            {
                GamePlayer player = effect.Owner as GamePlayer;
                player.Model = player.CreationModel;
                player.Out.SendUpdatePlayer();
            }
            GameEventMgr.RemoveHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new CoreEventHandler(EventHandler));
            return base.OnEffectExpires(effect, noMessages);
            
        }

        public void EventHandler(CoreEvent e, object sender, EventArgs arguments)
        {
            AttackedByEnemyEventArgs args = arguments as AttackedByEnemyEventArgs;
            if (args == null) return;
            
            if (args.AttackData == null) return;
            if (args.AttackData.SpellHandler != null) return;
            if (args.AttackData.AttackResult != EAttackResult.HitUnstyled
                && args.AttackData.AttackResult != EAttackResult.HitStyle)
                return;

            int baseChance = Spell.Frequency / 100;
            if (baseChance < 1)
                baseChance = 1;

            if (Util.Chance(baseChance))
            {
                ablativehp = 200;
                GamePlayer player = sender as GamePlayer;
                foreach (GamePlayer players in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                    players.Out.SendSpellEffectAnimation(player, player, 9103, 0, false, 1);
            }

            if (ablativehp != 0)
            {
                AttackData ad = args.AttackData;
                GameLiving living = sender as GameLiving;
                if (living == null) return;
                if (!MatchingDamageType(ref ad)) return;

                double absorbPercent = 100;

                int damageAbsorbed = (int)(0.01 * absorbPercent * (ad.Damage + ad.CriticalDamage));
                if (damageAbsorbed > ablativehp)
                    damageAbsorbed = ablativehp;
                ablativehp -= damageAbsorbed;
                ad.Damage -= damageAbsorbed;
                OnDamageAbsorbed(ad, damageAbsorbed);

                //TODO correct messages
                MessageToLiving(ad.Target, string.Format("Your ablative absorbs {0} damage!", damageAbsorbed), eChatType.CT_Spell);//since its not always Melee absorbing
                MessageToLiving(ad.Attacker, string.Format("A barrier absorbs {0} damage of your attack!", damageAbsorbed), eChatType.CT_Spell);  
            }
            
        }

        protected virtual void OnDamageAbsorbed(AttackData ad, int DamageAmount)
        {
        }

        // Check if Melee
        protected virtual bool MatchingDamageType(ref AttackData ad)
        {

            if (ad == null || (ad.AttackResult != EAttackResult.HitStyle && ad.AttackResult != EAttackResult.HitUnstyled))
                return false;
            if (!ad.IsMeleeAttack && ad.AttackType != AttackData.eAttackType.Ranged)
                return false;

            return true;
        }

        public ShadeOfMistDefensiveProcSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

}
