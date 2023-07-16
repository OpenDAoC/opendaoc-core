 
namespace DOL.GS.Commands
{
	/// <summary>
	/// Command handler for the /bind command
	/// </summary>
	[CmdAttribute(
		"&bind",
		ePrivLevel.Player,
		"Binds your soul to a bind location, you will start from there after you die and /release",
		"/bind")]
	public class BindCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		/// <summary>
		/// Method to handle the command and any arguments
		/// </summary>
		/// <param name="client"></param>
		/// <param name="args"></param>
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "bind"))
				return;

			client.Player.Bind(false);
		}
	}
}