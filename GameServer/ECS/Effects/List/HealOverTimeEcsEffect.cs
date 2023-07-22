using DOL.GS.Spells;

namespace DOL.GS
{
    public class HealOverTimeEcsEffect : EcsGameSpellEffect
    {
        public HealOverTimeEcsEffect(ECSGameEffectInitParams initParams) : base(initParams) 
        {
            NextTick = StartTick;
        }

        public override void OnStartEffect()
        {
            // "You start healing faster."
            // "{0} starts healing faster."
            OnEffectStartsMsg(Owner, true, true, true);
        }

        public override void OnStopEffect()
        {
            //"Your meditative state fades."
            //"{0}'s meditative state fades."
            OnEffectExpiresMsg(Owner, true, false, true);
        }

        public override void OnEffectPulse()
        {
            ((HotHandler)SpellHandler).OnDirectEffect(Owner, Effectiveness);
            NextTick += PulseFreq;
        }
    }
}
