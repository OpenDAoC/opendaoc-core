using DOL.AI.Brain;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Spell handler for power trasnfer.
	/// </summary>
	/// <author>Aredhel</author>
	[SpellHandler("PowerTransferPet")]
	class NecroPowerTransferHandler : PowerTransferHandler
	{
		public override void OnDirectEffect(GameLiving target, double effectiveness)
		{
			if (!(Caster is NecromancerPet))
				return;
			base.OnDirectEffect(target, effectiveness);
		}
		
				/// <summary>
		/// Returns a reference to the shade.
		/// </summary>
		/// <returns></returns>
		protected override GamePlayer Owner()
		{
			if (!(Caster is NecromancerPet))
				return null;

			return (((Caster as NecromancerPet).Brain) as IControlledBrain).Owner as GamePlayer;
		}

		/// <summary>
        /// Create a new handler for the power transfer spell.
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="spell"></param>
		/// <param name="line"></param>
		public NecroPowerTransferHandler (GameLiving caster, Spell spell, SpellLine line) 
            : base(caster, spell, line) { }
	}
}