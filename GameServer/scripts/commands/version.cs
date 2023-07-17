using System.Reflection;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[Command(
		"&version",
		EPrivLevel.Player,
		"Get the version of the GameServer",
		"/version")]
	public class VersionCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			AssemblyName an = Assembly.GetAssembly(typeof(GameServer)).GetName();
			client.Out.SendMessage("Dawn of Light " + an.Name + " Version: " + an.Version, eChatType.CT_System, eChatLoc.CL_SystemWindow);
		}
	}
}