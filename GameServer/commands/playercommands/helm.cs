namespace DOL.GS.Commands
{
	[CmdAttribute("&helm", //command to handle
		ePrivLevel.Player, //minimum privelege level
	 "Toggles the helm on and off when wearing an helm.", //command description
	  "/helm")] //usage
	public class HelmCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "helm"))
				return;

			client.Player.IsHelmInvisible = !client.Player.IsHelmInvisible;
		}
	}
}
