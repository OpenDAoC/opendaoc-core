namespace DOL.GS
{
	public class BdPet : GameSummonedPet
	{
		/// <summary>
		/// Proc IDs for various pet weapons.
		/// </summary>
		private enum Procs
		{
			Cold = 32050,
			Disease = 32014,
			Heat = 32053,
			Poison = 32013,
			Stun = 2165
		};

		/// <summary>
		/// Create a commander.
		/// </summary>
		/// <param name="npcTemplate"></param>
		/// <param name="owner"></param>
		public BdPet(INpcTemplate npcTemplate) : base(npcTemplate) { }

	}
}