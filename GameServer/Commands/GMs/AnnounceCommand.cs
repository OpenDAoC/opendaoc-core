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
		EPrivLevel.GM,
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
							    clients.Out.SendMessage(LanguageMgr.GetTranslation(clients, "GMCommands.Announce.LogAnnounce", message), EChatType.CT_Important, EChatLoc.CL_SystemWindow);
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
							    clients.Out.SendMessage(LanguageMgr.GetTranslation(clients, "GMCommands.Announce.SendAnnounce", message), EChatType.CT_Send, EChatLoc.CL_ChatWindow);
						break;
					}
				#endregion Send
				#region Center
				case "center":
					{
                        foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if (clients != null)
							    clients.Out.SendMessage(message, EChatType.CT_ScreenCenter, EChatLoc.CL_SystemWindow);
						break;
					}
				#endregion Center
				#region Confirm
				case "confirm":
					{
						foreach (GameClient clients in WorldMgr.GetAllPlayingClients())
                            if(clients != null)
							    clients.Out.SendDialogBox(EDialogCode.SimpleWarning, 0, 0, 0, 0, EDialogType.Ok, true, LanguageMgr.GetTranslation(clients, "GMCommands.Announce.ConfirmAnnounce", client.Player.Name, message));
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
