using System;
using System.Reflection;

using DOL.Database;
using DOL.Events;
using DOL.Language;
using DOL.GS.ServerProperties;

using log4net;

namespace DOL.GS.GameEvents
{
	/// <summary>
	/// This class makes sure that all the startup guilds are created in the database
	/// </summary>
	public static class StartupGuilds
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		
		/// <summary>
		/// Enable Starter Guilds
		/// </summary>
		[ServerProperty("startup", "starting_guild", "Starter Guild - Edit this to enable/disable the starter guilds", true)]
		public static bool STARTING_GUILD;

		/// <summary>
		/// This method runs the checks and listen to new player creation.
		/// </summary>
		/// <param name="e">The event</param>
		/// <param name="sender">The sender</param>
		/// <param name="args">The arguments</param>
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
            GameEventMgr.AddHandler(DatabaseEvent.CharacterCreated, new DOLEventHandler(AddNewbieToStarterGuild));

            if (!STARTING_GUILD)
                return;
            
            CheckStartupGuilds();            
		}
		
		/// <summary>
		/// Try to recreate Startup Guild
		/// </summary>
		[RefreshCommandAttribute]
		public static void CheckStartupGuilds()
		{
            foreach (eRealm currentRealm in Enum.GetValues(typeof(eRealm)))
			{
				if (currentRealm == eRealm.None || currentRealm == eRealm.Door)
					continue;
				
				CheckGuild(currentRealm,LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, string.Format("Guild.StartupGuild.{0}", GlobalConstants.RealmToName(currentRealm))));
			}
		}
		
		/// <summary>
		/// Remove event handler on server shutdown.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(DatabaseEvent.CharacterCreated, new DOLEventHandler(AddNewbieToStarterGuild));
		}
		
		/// <summary>
		/// Add newly created player to startup guild.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public static void AddNewbieToStarterGuild(DOLEvent e, object sender, EventArgs args)
		{
			if (!STARTING_GUILD)
				return;
			
			// Check Args
			var chArgs = args as CharacterEventArgs;
			
			if (chArgs == null)
				return;
			
			DOLCharacters ch = chArgs.Character;
			Account account = chArgs.GameClient.Account;
			

			var guildname = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE, string.Format("Guild.StartupGuild.{0}", GlobalConstants.RealmToName((eRealm)ch.Realm)));
			ch.GuildID = GuildMgr.GuildNameToGuildID(guildname);

			if (ch.GuildID != "")
				ch.GuildRank = 8;
			
		}

		/// <summary>
		/// This method checks if a guild exists
		/// if not, the guild is created with default values
		/// </summary>
		/// <param name="currentRealm">Current Realm being checked</param>
		/// <param name="guildName">The guild name that is being checked</param>
		private static void CheckGuild(eRealm currentRealm, string guildName)
		{
			if (!GuildMgr.DoesGuildExist(guildName))
			{
				Guild newguild = GuildMgr.CreateGuild(currentRealm, guildName);
				newguild.Ranks[8].OcHear = true;
				newguild.Motd = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,"Guild.StartupGuild.Motd");
				newguild.Omotd = LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,"Guild.StartupGuild.Omotd");
				newguild.BonusType = Guild.eBonusType.Experience;
				newguild.BonusStartTime = DateTime.Now;
				newguild.Ranks[8].Title =  LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,"Guild.StartupGuild.Title");
				newguild.Ranks[8].Invite = true;
				newguild.IsStartingGuild = true;
			}
		}
	}
}