using System;
using System.Reflection;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command("&serverinfo", //command to handle
		EPrivLevel.Player, //minimum privelege level
		"Shows information about the server", //command description
		"/serverinfo")] //usage
	public class ServerInfoCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			client.Out.SendMessage("Atlas", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
			var an = Assembly.GetAssembly(typeof(GameServer)).GetName();
			client.Out.SendMessage("Online: " + WorldMgr.GetAllPlayingClientsCount(), EChatType.CT_System, EChatLoc.CL_SystemWindow);
			if (client.Player == null) return;
			var uptime = DateTime.Now.Subtract(GameServer.Instance.StartupTime);
				
			var sec = uptime.TotalSeconds;
			var min = Convert.ToInt64(sec) / 60;
			var hours = min / 60;
			var days = hours / 24;
				
			DisplayMessage(client, $"Uptime: {days}d {hours % 24}h {min % 60}m {sec % 60:00}s");
		}
	}
}