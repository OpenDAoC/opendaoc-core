 
namespace DOL.GS.Commands
{
	[Command("&stand", EPrivLevel.Player, "Stands up when sitting", "/stand")]
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

	[Command("&sit", new string[] { "&rest" }, EPrivLevel.Player, "Sit", "/sit")]
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