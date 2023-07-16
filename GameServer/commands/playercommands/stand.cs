 
namespace DOL.GS.Commands
{
	[CmdAttribute("&stand", ePrivLevel.Player, "Stands up when sitting", "/stand")]
	public class StandCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (!IsSpammingCommand(client.Player, "sitstand"))
			{
				client.Player.Sit(false);
			}
		}
	}

	[CmdAttribute("&sit", new string[] { "&rest" }, ePrivLevel.Player, "Sit", "/sit")]
	public class SitCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (!IsSpammingCommand(client.Player, "sitstand"))
			{
				client.Player.Sit(true);
			}
		}
	}
}