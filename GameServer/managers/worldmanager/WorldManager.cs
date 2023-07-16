using System;

namespace DOL.GS
{
	/// <summary>
	/// GameServer Manager to Handle World Data and Region events for this GameServer.
	/// </summary>
	public sealed class WorldManager
	{
		/// <summary>
		/// Reference to the Instanced GameServer
		/// </summary>
		private GameServer GameServerInstance { get; set; }

		/// <summary>
		/// Reference to the World Weather Manager
		/// </summary>
		public WeatherManager WeatherManager { get; private set; }
		
		/// <summary>
		/// Create a new instance of <see cref="WorldManager"/>
		/// </summary>
		public WorldManager(GameServer GameServerInstance)
		{
			if (GameServerInstance == null)
				throw new ArgumentNullException("GameServerInstance");

			this.GameServerInstance = GameServerInstance;
			
			WeatherManager = new WeatherManager(this.GameServerInstance.Scheduler);
		}
	}
}
