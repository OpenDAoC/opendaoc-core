using System;

using DOL.GS.Friends;

namespace DOL.GS
{
	/// <summary>
	/// GameServer Manager to Handle Player Data and restriction for this GameServer.
	/// </summary>
	public sealed class PlayerManager
	{
		/// <summary>
		/// Reference to the Instanced GameServer
		/// </summary>
		private GameServer GameServerInstance { get; set; }
		
		/// <summary>
		/// Reference to the Invalid Names Manager
		/// </summary>
		public InvalidNamesManager InvalidNames { get; private set; }
		
		/// <summary>
		/// Reference to the Friends List Manager
		/// </summary>
		public FriendsManager Friends { get; private set; }

		/// <summary>
		/// Create a new Instance of <see cref="PlayerManager"/>
		/// </summary>
		public PlayerManager(GameServer GameServerInstance)
		{
			if (GameServerInstance == null)
				throw new ArgumentNullException("GameServerInstance");
			
			this.GameServerInstance = GameServerInstance;
			
			InvalidNames = new InvalidNamesManager(this.GameServerInstance.Configuration.InvalidNamesFile);
			Friends = new FriendsManager(GameServerInstance.IDatabase);
		}
	}
}
