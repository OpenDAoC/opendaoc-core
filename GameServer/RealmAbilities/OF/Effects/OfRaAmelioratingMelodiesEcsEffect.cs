using System.Collections.Generic;
using Core.GS.ECS;
using Core.GS.Enums;
using Core.GS.PacketHandler;

namespace Core.GS.Effects
{
    public class OfRaAmelioratingMelodiesEcsEffect : EcsGameAbilityEffect
    {
        private const int m_range = 1500;
        private int m_heal;
        
        public OfRaAmelioratingMelodiesEcsEffect(EcsGameEffectInitParams initParams)
            : base(initParams)
        {
            EffectType = EEffect.AmelioratingMelodies;
            PulseFreq = 1500; // 1.5s. Effect lasts 30s so that is 20 ticks.
            NextTick = StartTick;
            m_heal = (int)Effectiveness; // Effectiveness value is used as a heal value per tick.
            EffectService.RequestStartEffect(this);
        }

        public override ushort Icon { get { return 4250; } }
        public override string Name { get { return "Ameliorating Melodies"; } }
        public override bool HasPositiveEffect { get { return true; } }

        public override void OnEffectPulse()
        {
            if (OwnerPlayer == null) return;

            ICollection<GamePlayer> playersToHeal = new List<GamePlayer>();

            // OF AM works on the caster as well, unlike NF AM.
            if (OwnerPlayer.Group == null)
            {
                playersToHeal.Add(OwnerPlayer);
            }
            else
            {
                playersToHeal = OwnerPlayer.Group.GetPlayersInTheGroup();
            }

            foreach (GamePlayer p in playersToHeal)
            {
                if ((p.Health < p.MaxHealth) && OwnerPlayer.IsWithinRadius(p, m_range) && (p.IsAlive))
                {
                    int heal = m_heal;
                    if (p.Health + heal > p.MaxHealth) heal = p.MaxHealth - p.Health;
                    p.ChangeHealth(OwnerPlayer, EHealthChangeType.Regenerate, heal);
                    OwnerPlayer.Out.SendMessage("Your Ameliorating Melodies heal " + p.Name + " for " + heal.ToString() + " hit points.", EChatType.CT_Spell, EChatLoc.CL_SystemWindow);
                    p.Out.SendMessage(OwnerPlayer.Name + "'s Ameliorating Melodies heals you for " + heal.ToString() + " hit points.", EChatType.CT_Spell, EChatLoc.CL_SystemWindow);
                }
            }

            NextTick += PulseFreq;
        }
    }
}
