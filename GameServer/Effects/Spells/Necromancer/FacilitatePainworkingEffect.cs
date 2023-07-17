using DOL.GS.Spells;

namespace DOL.GS.Effects
{
	/// <summary>
	/// The Facilitate Painworking effect.
	/// </summary>
    public class FacilitatePainworkingEffect : GameSpellEffect
    {
        public FacilitatePainworkingEffect(ISpellHandler handler, int duration, int pulseFreq, double effectiveness)
            : base(handler, duration, pulseFreq, effectiveness)
        {
        }
    }
}
