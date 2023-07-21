using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.GS.Spells
{
    [SpellHandler("PiercingMagic")]
    public class PiercingMagicHandler : SpellHandler
    {
        public override void CreateECSEffect(ECSGameEffectInitParams initParams)
        {
            new PiercingMagicEcsEffect(initParams);
        }
        // constructor
        public PiercingMagicHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
        {
        }
    }   
}
