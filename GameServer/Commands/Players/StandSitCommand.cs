 
namespace DOL.GS.Commands
{
	[Command("&stand", ePrivLevel.Player, "Stands up when sitting", "/stand")]
	public class StandCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (!IsSpammingCommand(client.Player, "sitstand"))
			{
				client.Player.Sit(false);
			}
		}
	}

	[Command("&sit", new string[] { "&rest" }, ePrivLevel.Player, "Sit", "/sit")]
	public class SitCommand : AbstractCommandHandler, ICommandHandler
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