using System;
using System.Linq;
using System.Collections;

using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS
{
	/// <summary>
	/// Faction of mob
	/// </summary>
	public class FactionUtil
	{
		private const int DECREASE_AGGRO_AMOUNT = -1;
		private const int INCREASE_AGGRO_AMOUNT = 1;
		private const int MAX_AGGRO_VALUE = 100;
		private const int MIN_AGGRO_VALUE = -100;

		public FactionUtil()
		{
			m_name = String.Empty;
			m_friendFactions = new ArrayList();
			m_enemyFactions = new ArrayList();
			m_playerxFaction = new Hashtable();
			m_updatePlayer = new ArrayList();
		}

		#region DB
		/// <summary>
		/// load faction from DB
		/// </summary>
		/// <param name="dbfaction"></param>
		public void LoadFromDatabase(DbFactions dbfaction)
		{
			m_name = dbfaction.Name;
			m_id = dbfaction.ID;
			m_baseAggroLevel = dbfaction.BaseAggroLevel;
		}


		public void SaveAggroToFaction()
		{
			lock ( m_updatePlayer.SyncRoot )
			{
				foreach ( string charID in m_updatePlayer )
				{
					SaveAggroToFaction( charID );
				}

				m_updatePlayer.Clear();
			}
		}


		public void SaveAggroToFaction(string charID)
		{
			if (charID == null) return;
			var dbfactionAggroLevel = CoreDb<DbFactionAggro>.SelectObject(DB.Column("CharacterID").IsEqualTo(charID).And(DB.Column("FactionID").IsEqualTo(ID)));
			if (dbfactionAggroLevel == null)
			{
				dbfactionAggroLevel = new DbFactionAggro();
				dbfactionAggroLevel.AggroLevel = (int)m_playerxFaction[charID];
				dbfactionAggroLevel.CharacterID = charID;
				dbfactionAggroLevel.FactionID = this.ID;
				GameServer.Database.AddObject(dbfactionAggroLevel);
			}
			else
			{
				dbfactionAggroLevel.AggroLevel = (int)m_playerxFaction[charID];
				GameServer.Database.SaveObject(dbfactionAggroLevel);
			}
		}
		#endregion

		#region Properties


		/// <summary>
		/// hold name of faction
		/// </summary>
		private string m_name;
		/// <summary>
		/// name of faction
		/// </summary>
		public string Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// hold friend factions
		/// </summary>
		private ArrayList m_friendFactions;
		/// <summary>
		/// friend factions
		/// </summary>
		public ArrayList FriendFactions
		{
			get { return m_friendFactions; }
		}

		/// <summary>
		/// hold enemy factions
		/// </summary>
		private ArrayList m_enemyFactions;
		/// <summary>
		/// enemy factions
		/// </summary>
		public ArrayList EnemyFactions
		{
			get { return m_enemyFactions; }
		}

		/// <summary>
		/// hold id of faction
		/// </summary>
		private int m_id;
		/// <summary>
		/// id of faction
		/// </summary>
		public int ID
		{
			get { return m_id; }
		}

		/// <summary>
		/// hold base aggro level
		/// </summary>
		private int m_baseAggroLevel;
		/// <summary>
		/// base aggro when player have never meet faction before
		/// </summary>
		public int BaseAggroLevel
		{
			get { return m_baseAggroLevel; }
		}

		/// <summary>
		/// this is the table of player aggrolevel
		/// </summary>
		private Hashtable m_playerxFaction;

		private ArrayList m_updatePlayer;

		/// <summary>
		/// table of player and aggrolevel (characterid/aggrolevel)
		/// </summary>
		public Hashtable PlayerxFaction
		{
			get { return m_playerxFaction; }
		}
		#endregion

		#region Friend/enemy Faction
		/// <summary>
		/// add friend faction to this faction
		/// </summary>
		/// <param name="faction"></param>
		public void AddFriendFaction(FactionUtil faction)
		{
			if (!m_friendFactions.Contains(faction))
				m_friendFactions.Add(faction);
		}

		/// <summary>
		/// remove friend faction
		/// </summary>
		/// <param name="faction"></param>
		public void RemoveFriendFaction(FactionUtil faction)
		{
			if (m_friendFactions.Contains(faction))
				m_friendFactions.Remove(faction);
		}

		/// <summary>
		/// add enemy faction
		/// </summary>
		/// <param name="faction"></param>
		public void AddEnemyFaction(FactionUtil faction)
		{
			if (!m_enemyFactions.Contains(faction))
				m_enemyFactions.Add(faction);
		}

		/// <summary>
		/// remove enemy faction
		/// </summary>
		/// <param name="faction"></param>
		public void RemoveEnemyFaction(FactionUtil faction)
		{
			if (m_enemyFactions.Contains(faction))
				m_enemyFactions.Remove(faction);
		}
		#endregion

		#region changes for interactions with faction members
		/// <summary>
		/// called when a player kills a mob from the faction
		/// </summary>
		/// <param name="killer"></param>
		public void KillMember(GamePlayer killer)
		{
			foreach (FactionUtil faction in m_friendFactions)
			{
				faction.ChangeAggroLevel(killer, INCREASE_AGGRO_AMOUNT);
			}
			foreach (FactionUtil faction in m_enemyFactions)
			{
				faction.ChangeAggroLevel(killer, DECREASE_AGGRO_AMOUNT);
			}
		}

		/// <summary>
        /// changes aggro of faction and related factions to player
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public void ChangeAggroLevel(GamePlayer player, int amount)
		{
			// remember the player
			if (!m_updatePlayer.Contains(player.ObjectId))
			{
				m_updatePlayer.Add(player.ObjectId);
			}
			int oldAggro;
			// remember the player's relation to the faction
			if (m_playerxFaction.ContainsKey(player.ObjectId))
			{
				oldAggro = (int)m_playerxFaction[player.ObjectId];
			}
			else
			{
				oldAggro = BaseAggroLevel;
				m_playerxFaction.Add(player.ObjectId, BaseAggroLevel);
			}
			// get the new relation
			int newAggro = oldAggro + amount;
			// clamp it between MIN and MAX
			if (newAggro < MIN_AGGRO_VALUE)
			{
				newAggro = MIN_AGGRO_VALUE;
			}
			else if (newAggro > MAX_AGGRO_VALUE)
			{
				newAggro = MAX_AGGRO_VALUE;
			}
			// check if changed
			if (newAggro != oldAggro)
			{
				
				// save the change
				if(UtilCollection.Chance(20))
					m_playerxFaction[player.ObjectId] = newAggro;
			}
			// tell the player
			string msg = "Your relationship with " + this.Name + " has ";
			if (amount > 0)
			{
				msg += "decreased.";
			}
			else
			{
				msg += "increased.";
			}
			player.Out.SendMessage(msg, eChatType.CT_System, eChatLoc.CL_SystemWindow);
		}

		/// <summary>
		/// gets aggro level of player with faction
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public int GetAggroToFaction(GamePlayer player)
		{
			if (m_playerxFaction.ContainsKey(player.ObjectId))
				return (int)m_playerxFaction[player.ObjectId];
			else
				return BaseAggroLevel;
		}
		#endregion
	}
}
