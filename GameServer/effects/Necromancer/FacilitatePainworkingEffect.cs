using System;

using DOL.GS.Spells;

namespace DOL.GS.Effects
{
	/// <summary>
	/// The Facilitate Painworking effect.
	/// </summary>
	/// <author>Aredhel</author>
    public class FacilitatePainworkingEffect : GameSpellEffect
    {
        public FacilitatePainworkingEffect(ISpellHandler handler, int duration, int pulseFreq, double effectiveness)
            : base(handler, duration, pulseFreq, effectiveness)
        {
        }
    }
}
