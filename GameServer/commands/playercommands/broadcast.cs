
using System.Collections;
using System.Reflection;
using DOL.Language;
using DOL.GS;
using DOL.GS.ServerProperties;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		 "&broadcast",
		 new string[] { "&b" },
		 ePrivLevel.Player,
		 "Broadcast something to other players in the same zone",
		 "/b <message>")]
	public class BroadcastCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		private enum eBroadcastType : int
		{
			Area = 1,
			Visible = 2,
			Zone = 3,
			Region = 4,
			Realm = 5,
			Server = 6,
		}

		public void OnCommand(GameClient client, string[] args)
		{
			const string BROAD_TICK = "Broad_Tick";
			if (args.Length < 2)
			{
				DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Broadcast.NoText"));
				return;
			}
			if (client.Player.IsMuted)
			{
				client.Player.Out.SendMessage("You have been muted. You cannot broadcast.", eChatType.CT_Staff, eChatLoc.CL_SystemWindow);
				return;
			}
			string message = string.Join(" ", args, 1, args.Length - 1);

			long BroadTick = client.Player.TempProperties.getProperty<long>(BROAD_TICK);
			if (BroadTick > 0 && BroadTick - client.Player.CurrentRegion.Time <= 0)
			{
				client.Player.TempProperties.removeProperty(BROAD_TICK);
			}
			long changeTime = client.Player.CurrentRegion.Time - BroadTick;
			if (changeTime < 800 && BroadTick > 0)
			{
				client.Player.Out.SendMessage("Slow down! Think before you say each word!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Player.TempProperties.setProperty(BROAD_TICK, client.Player.CurrentRegion.Time);
				return;
			}
			Broadcast(client.Player, message);

			client.Player.TempProperties.setProperty(BROAD_TICK, client.Player.CurrentRegion.Time);
		}

		private void Broadcast(GamePlayer player, string message)
		{
			foreach (GamePlayer p in GetTargets(player))
			{
				if (GameServer.ServerRules.IsAllowedToUnderstand(p, player) || ((eBroadcastType)ServerProperties.Properties.BROADCAST_TYPE == eBroadcastType.Server))
				{
					p.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Scripts.Players.Broadcast.Message", player.Name, message), eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
				}
			}

		}

		private ArrayList GetTargets(GamePlayer player)
		{
			ArrayList list = new ArrayList();
			eBroadcastType type = (eBroadcastType)ServerProperties.Properties.BROADCAST_TYPE;
			switch (type)
			{
				case eBroadcastType.Area:
					{
						bool found = false;
						foreach (AbstractArea area in player.CurrentAreas)
						{
							if (area.CanBroadcast)
							{
								found = true;
								foreach (GameClient thisClient in WorldMgr.GetClientsOfRegion(player.CurrentRegionID))
								{
									if (thisClient.Player.CurrentAreas.Contains(area))
									{
										list.Add(thisClient.Player);
									}
								}
							}
						}
						if (!found)
						{
							player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Scripts.Players.Broadcast.NoHere"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
					}
				case eBroadcastType.Realm:
					{
						foreach (GameClient thisClient in WorldMgr.GetClientsOfRealm(player.Realm))
						{
							list.Add(thisClient.Player);
						}
						break;
					}
				case eBroadcastType.Region:
					{
						foreach (GameClient thisClient in WorldMgr.GetClientsOfRegion(player.CurrentRegionID))
						{
							list.Add(thisClient.Player);
						}
						break;
					}
				case eBroadcastType.Server:
					{
						foreach (GameClient thisClient in WorldMgr.GetAllPlayingClients())
						{
							list.Add(thisClient.Player);
						}
						break;
					}
				case eBroadcastType.Visible:
					{
						foreach (GamePlayer p in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
						{
							list.Add(p);
						}
						break;
					}
				case eBroadcastType.Zone:
					{
						foreach (GameClient thisClient in WorldMgr.GetClientsOfRegion(player.CurrentRegionID))
						{
							if (thisClient.Player.CurrentZone == player.CurrentZone)
							{
								list.Add(thisClient.Player);
							}
						}
						break;
					}
			}

			return list;
		}
	}
}
