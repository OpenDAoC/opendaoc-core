using System;
using DOL.AI.Brain;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Style taunt effect spell handler
	/// </summary>
	[SpellHandler("StyleTaunt")]
	public class StyleTauntHandler : SpellHandler 
	{
		public override int CalculateSpellResistChance(GameLiving target)
		{
			return 0;
		}

		/// <summary>
		/// Determines wether this spell is compatible with given spell
		/// and therefore overwritable by better versions
		/// spells that are overwritable cannot stack
		/// </summary>
		/// <param name="compare"></param>
		/// <returns></returns>
		public override bool IsOverwritable(GameSpellEffect compare)
		{
            return false;
		}

        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            if (target is GameNPC)
            {
                AttackData ad = Caster.TempProperties.getProperty<object>(GameLiving.LAST_ATTACK_DATA, null) as AttackData;
                if (ad != null)
                {
                    IOldAggressiveBrain aggroBrain = ((GameNPC)target).Brain as IOldAggressiveBrain;
					if (aggroBrain != null)
					{
						int aggro = Convert.ToInt32(ad.Damage * Spell.Value);
						aggroBrain.AddToAggroList(Caster, aggro);

						//log.DebugFormat("Damage: {0}, Taunt Value: {1}, (de)Taunt Amount {2}", ad.Damage, Spell.Value, aggro.ToString());
					}
                }
            }
        }

		// constructor
        public StyleTauntHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
	}
}