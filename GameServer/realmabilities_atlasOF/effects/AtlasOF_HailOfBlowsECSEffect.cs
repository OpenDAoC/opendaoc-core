using DOL.GS.PacketHandler;
using DOL.GS.Spells;

namespace DOL.GS.Effects
{
    public class AtlasOF_HailOfBlowsECSEffect : StatBuffEcsEffect
    {
        public AtlasOF_HailOfBlowsECSEffect(ECSGameEffectInitParams initParams)
            : base(initParams)
        {
            EffectType = EEffect.MeleeHasteBuff;
        }
        
        public override ushort Icon { get { return 4240; } }
        public override string Name { get { return "Hail Of Blows"; } }
        public override bool HasPositiveEffect { get { return true; } }
    }
}
