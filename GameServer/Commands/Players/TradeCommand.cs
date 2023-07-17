
using DOL.Language;
using DOL.GS.ServerProperties;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts.discord;


namespace DOL.GS.Commands
{
	[Command(
		 "&trade",
		 EPrivLevel.Player,
		 "Broadcast a trade message to other players in the same region",
		 "/trade <message>")]
	public class TradeCommand : AbstractCommandHandler, ICommandHandler
	{
		private const string tradeTimeoutString = "lastTradeTick";

		public void OnCommand(GameClient client, string[] args)
		{
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
			
			var lastTradeTick = client.Player.TempProperties.getProperty<long>(tradeTimeoutString);
			var slowModeLength = Properties.TRADE_SLOWMODE_LENGTH * 1000;
			
			if ((GameLoop.GameLoopTime - lastTradeTick) < slowModeLength && client.Account.PrivLevel == 1) // 60 secs
			{
				// Message: You must wait {0} seconds before using this command again.
				ChatUtil.SendSystemMessage(client, "PLCommands.Trade.List.Wait", Properties.TRADE_SLOWMODE_LENGTH - (GameLoop.GameLoopTime - lastTradeTick) / 1000);
				return;
			}
			
			string message = string.Join(" ", args, 1, args.Length - 1);
			
			Broadcast(client.Player, message);
			
		}

		private void Broadcast(GamePlayer player, string message)
		{
			foreach (GameClient c in WorldMgr.GetClientsOfRealm(player.Realm))
			{
				if (c.Player.Realm == player.Realm || player.Client.Account.PrivLevel > 1)
				{
					c.Out.SendMessage($"[Trade] {player.Name}: {message}", eChatType.CT_Trade, eChatLoc.CL_ChatWindow);
				}
			}
			
			if (Properties.DISCORD_ACTIVE) WebhookMessage.LogChatMessage(player, eChatType.CT_Trade, message);
			
			if (player.Client.Account.PrivLevel == 1)
			{
				player.Client.Player.TempProperties.setProperty(tradeTimeoutString, GameLoop.GameLoopTime);
			}
		}
	}
}


