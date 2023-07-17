using System;

using DOL.Database;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.Events;
using JNogueira.Discord.Webhook.Client;

namespace DOL.GS.ServerRules
{
	/// <summary>
	/// Handles DF entrance jump point allowing only one realm to enter on Normal server type.
	/// </summary>
	public class DFEnterJumpPoint : IJumpPointHandler
	{
		/// <summary>
		/// Decides whether player can jump to the target point.
		/// All messages with reasons must be sent here.
		/// Can change destination too.
		/// </summary>
		/// <param name="targetPoint">The jump destination</param>
		/// <param name="player">The jumping player</param>
		/// <returns>True if allowed</returns>
		public bool IsAllowedToJump(DbZonePoints targetPoint, GamePlayer player)
		{
            if (player.Client.Account.PrivLevel > 1)
            {
                return true;
            }
			if (GameServer.Instance.Configuration.ServerType != EGameServerType.GST_Normal)
				return true;
			if (ServerProperties.Properties.ALLOW_ALL_REALMS_DF)
				return true;
			if (player.Realm == PreviousOwner && LastRealmSwapTick + GracePeriod >= GameLoop.GameLoopTime)
				return true;
			return (player.Realm == DarknessFallOwner);
		}

		public static ERealm DarknessFallOwner = ERealm.None;
		public static ERealm PreviousOwner = ERealm.None;

		public static long GracePeriod = 900 * 1000; //15 mins
		public static long LastRealmSwapTick = 0;
		
		/// <summary>
		/// initialize the darkness fall entrance system
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		[ScriptLoadedEvent]
		public static void OnScriptLoaded(CoreEvent e, object sender, EventArgs args)
		{
			CheckDFOwner();
			GameEventMgr.AddHandler(KeepEvent.KeepTaken,new CoreEventHandler(OnKeepTaken));
		}

		/// <summary>
		/// check if a realm have more keep at start
		/// to know the DF owner
		/// </summary>
		
		private static void CheckDFOwner()
		{
			int albcount = GameServer.KeepManager.GetKeepCountByRealm(ERealm.Albion);
			int midcount = GameServer.KeepManager.GetKeepCountByRealm(ERealm.Midgard);
			int hibcount = GameServer.KeepManager.GetKeepCountByRealm(ERealm.Hibernia);
			
			if (albcount > midcount && albcount > hibcount)
			{
				DarknessFallOwner = ERealm.Albion;
			}
			if (midcount > albcount && midcount > hibcount)
			{
				DarknessFallOwner = ERealm.Midgard;
			}
			if (hibcount > midcount && hibcount > albcount)
			{
				DarknessFallOwner = ERealm.Hibernia;
			}
		}



		/// <summary>
		/// when  keep is taken it check if the realm which take gain the control of DF
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="arguments"></param>
		public static void OnKeepTaken(CoreEvent e, object sender, EventArgs arguments)
		{
			KeepEventArgs args = arguments as KeepEventArgs;
			ERealm realm = (ERealm) args.Keep.Realm ;
			if (realm != DarknessFallOwner )
			{
				// TODO send message to chat and discord web hook
				string oldDFOwner = GlobalConstants.RealmToName(DarknessFallOwner);
				int currentDFOwnerTowerCount = GameServer.KeepManager.GetKeepCountByRealm(DarknessFallOwner);
				int challengerOwnerTowerCount = GameServer.KeepManager.GetKeepCountByRealm(realm);
				if (currentDFOwnerTowerCount < challengerOwnerTowerCount)
                {
					PreviousOwner = DarknessFallOwner;
					LastRealmSwapTick = GameLoop.GameLoopTime;
					DarknessFallOwner = realm;
				}
					
				string realmName = "";

				string messageDFGetControl = string.Format("The forces of {0} have gained access to Darkness Falls!", GlobalConstants.RealmToName(realm));
				string messageDFLostControl = string.Format("{0} will lose access to Darkness Falls in 15 minutes!", oldDFOwner);

				if (oldDFOwner != GlobalConstants.RealmToName(DarknessFallOwner))
				{ 
					BroadcastMessage(messageDFLostControl, ERealm.None);
					BroadcastMessage(messageDFGetControl, ERealm.None);
				}
			}
		}
		
		/// <summary>
		/// Method to broadcast messages, if eRealm.None all can see,
		/// else only the right realm can see
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="realm">The realm</param>
		public static void BroadcastMessage(string message, ERealm realm)
		{
			foreach (GameClient client in WorldMgr.GetAllClients())
			{
				if (client.Player == null)
					continue;
				if ((client.Account.PrivLevel != 1 || realm == ERealm.None) || client.Player.Realm == realm)
				{
					client.Out.SendMessage(message, eChatType.CT_Important, eChatLoc.CL_SystemWindow);
				}
			}
			
			if (ServerProperties.Properties.DISCORD_ACTIVE && !string.IsNullOrEmpty(ServerProperties.Properties.DISCORD_RVR_WEBHOOK_ID))
			{
				var client = new DiscordWebhookClient(ServerProperties.Properties.DISCORD_RVR_WEBHOOK_ID);

				// Create your DiscordMessage with all parameters of your message.
				var discordMessage = new DiscordMessage(
					"",
					username: "Atlas RvR",
					avatarUrl: "https://cdn.discordapp.com/attachments/919610633656369214/928728399449571388/keep.png",
					tts: false,
					embeds: new[]
					{
						new DiscordMessageEmbed(
							author: new DiscordMessageEmbedAuthor("Darkness Falls"),
							color: 0,
							description: message
						)
					}
				);
				
				client.SendToDiscord(discordMessage);
			}
			
		}

		public static void SetDFOwner(GamePlayer p, ERealm NewDFOwner)
		{
			if (DarknessFallOwner != NewDFOwner)
			{
				foreach (GameClient pp in WorldMgr.GetClientsOfRegion(249))
				{
					if (pp == null) break;
					if (pp.Player == null) continue;
					if (pp.IsPlaying && pp.Player.Realm == DarknessFallOwner)
					{
						pp.Out.SendSoundEffect(217, 0, 0, 0, 0, 0);
					}

				}

				DarknessFallOwner = NewDFOwner;

				foreach (GameClient pp in WorldMgr.GetClientsOfRegion(249))
				{
					if (pp == null) break;
					if (pp.Player == null) continue;
					if (pp.IsPlaying && pp.Player.Realm == DarknessFallOwner)
					{
						pp.Out.SendSoundEffect(216, 0, 0, 0, 0, 0);
					}

				}

				p.Out.SendMessage(string.Format("New DF Owner set to {0}", GlobalConstants.RealmToName(NewDFOwner)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
			else
				p.Out.SendMessage(string.Format("DF Owner is already set to {0}", GlobalConstants.RealmToName(NewDFOwner)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
		}
	}
}
