 
namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&pray",
		ePrivLevel.Player,
		"You can pray on your gravestones to get some experience back",
		"/pray")]
	public class PrayCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "pray"))
				return;

			client.Player.Pray();
		}
	}
}