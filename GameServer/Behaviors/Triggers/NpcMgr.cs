using System;

namespace DOL.GS
{
	/// <summary>
	/// GameServer Manager to handle Npc Data and Other Behavior for the whole Instance.
	/// </summary>
	public sealed class NpcMgr
	{
		/// <summary>
		/// Reference to the Instanced GameServer
		/// </summary>
		private GameServer GameServerInstance { get; set; }
		
		public MobAmbientBehaviorMgr AmbientBehaviour { get; private set; }
		
		/// <summary>
		/// Create a new Instance of <see cref="NpcMgr"/>
		/// </summary>
		public NpcMgr(GameServer GameServerInstance)
		{
			if (GameServerInstance == null)
				throw new ArgumentNullException("GameServerInstance");

			this.GameServerInstance = GameServerInstance;
			
			AmbientBehaviour = new MobAmbientBehaviorMgr(this.GameServerInstance.IDatabase);
		}
	}
}
