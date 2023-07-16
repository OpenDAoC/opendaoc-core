
using System.Collections.Generic;
using DOL.Events;
using DOL.GS.PacketHandler;
using System;

namespace DOL.GS.Effects
{
    public class RezDmgImmunityEffect : TimedEffect
    {
        private GamePlayer m_player = null;

        public RezDmgImmunityEffect() : base(6000) { }

        public override void Start(GameLiving target)
        {
            base.Start(target);
            m_player = target as GamePlayer;
            GameEventMgr.AddHandler(m_player, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttacked));
            GameEventMgr.AddHandler(m_player, GameLivingEvent.Dying, new DOLEventHandler(OnRemove));
            GameEventMgr.AddHandler(m_player, GamePlayerEvent.Quit, new DOLEventHandler(OnRemove));
            GameEventMgr.AddHandler(m_player, GamePlayerEvent.Linkdeath, new DOLEventHandler(OnRemove));
            GameEventMgr.AddHandler(m_player, GamePlayerEvent.RegionChanged, new DOLEventHandler(OnRemove));
        }

        private void OnAttacked(DOLEvent e, object sender, EventArgs args)
        {
            AttackedByEnemyEventArgs attackArgs = args as AttackedByEnemyEventArgs;
            if (attackArgs == null) return;
            AttackData ad = null;
            ad = attackArgs.AttackData;

            int damageAbsorbed = (int)(ad.Damage + ad.CriticalDamage);

            //They shouldn't take any damamge at all
            //if (m_player.Health < (damageAbsorbed + (int)Math.Round((double)m_player.MaxHealth / 20))) m_player.Health += damageAbsorbed;
            m_player.Health += damageAbsorbed;
        }

        private void OnRemove(DOLEvent e, object sender, EventArgs args)
        {
            //((GamePlayer)Owner).Out.SendMessage("Sputins Legacy grants you a damage immunity!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);

            Stop();
        }

        public override void Stop()
        {
			if (m_player.EffectList.GetOfType<SputinsLegacyEffect>() != null) m_player.EffectList.Remove(this);
            GameEventMgr.RemoveHandler(m_player, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttacked));
            GameEventMgr.RemoveHandler(m_player, GameLivingEvent.Dying, new DOLEventHandler(OnRemove));
            GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.Quit, new DOLEventHandler(OnRemove));
            GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.Linkdeath, new DOLEventHandler(OnRemove));
            GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.RegionChanged, new DOLEventHandler(OnRemove));
            base.Stop();
        }









        public override string Name { get { return "Resurrection Damage Immunity"; } }

        public override ushort Icon { get { return 3069; } }

        public override IList<string> DelveInfo
        {
            get
            {
                var list = new List<string>();
                list.Add("Newly resserected players can't take damage for 5 seconds.");
                return list;
            }
        }
    }
}