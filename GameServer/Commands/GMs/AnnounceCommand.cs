using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DOL.GS;
using DOL.GS.ServerProperties;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[Command(
		"&announce",
		ePrivLevel.GM,
	    "GMCommands.Announce.Description",
	    "GMCommands.Announce.Usage")]
	public class AnnounceCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (args.Length < 3)
			{
				DisplaySyntax(client);
				return;
			}

			string message = string.Join(" ", args, 2, args.Length - 2);
			if (message == "")
				return;

			switch (args.GetValue(1).ToString().ToLower())
			{
				#region Log
				case "log":
					{
						foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if(clients != null)
							    clients.Out.SendMessage(LanguageMgr.GetTranslation(clients, "GMCommands.Announce.LogAnnounce", message), eChatType.CT_Important, eChatLoc.CL_SystemWindow);
						break;
					}
				#endregion Log
				#region Window
				case "window":
					{
						var messages = new List<string>();
						messages.Add(message);

						foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if(clients != null)
							    clients.Player.Out.SendCustomTextWindow(LanguageMgr.GetTranslation(clients, "GMCommands.Announce.WindowAnnounce", client.Player.Name), messages);
						break;
					}
				#endregion Window
				#region Send
				case "send":
					{
						foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if(clients != null)
							    clients.Out.SendMessage(LanguageMgr.GetTranslation(clients, "GMCommands.Announce.SendAnnounce", message), eChatType.CT_Send, eChatLoc.CL_ChatWindow);
						break;
					}
				#endregion Send
				#region Center
				case "center":
					{
                        foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if (clients != null)
							    clients.Out.SendMessage(message, eChatType.CT_ScreenCenter, eChatLoc.CL_SystemWindow);
						break;
					}
				#endregion Center
				#region Confirm
				case "confirm":
					{
						foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if(clients != null)
							    clients.Out.SendDialogBox(eDialogCode.SimpleWarning, 0, 0, 0, 0, eDialogType.Ok, true, LanguageMgr.GetTranslation(clients, "GMCommands.Announce.ConfirmAnnounce", client.Player.Name, message));
						break;
					}
				#endregion Confirm
				#region Default
				default:
					{
						DisplaySyntax(client);
						return;
					}
				#endregion Default
			}
		}
	}
}
