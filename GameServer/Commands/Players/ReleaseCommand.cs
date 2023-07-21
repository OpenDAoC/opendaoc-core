 
 using DOL.Database;

 namespace DOL.GS.Commands
{
	[Command(
		"&release", new string[] { "&rel" },
		EPrivLevel.Player,
		"When you are dead you can '/release'. This will bring you back to your bindpoint!",
		"/release")]
	public class ReleaseCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.CurrentRegion.IsRvR && !client.Player.CurrentRegion.IsDungeon || ServerProperties.ServerProperties.EVENT_THIDRANKI || ServerProperties.ServerProperties.EVENT_TUTORIAL)
			{
				client.Player.Release(EReleaseType.RvR, false);
				return;
			}

            if (args.Length > 1 && args[1].ToLower() == "city")
            {
	            if (ServerProperties.ServerProperties.EVENT_THIDRANKI || ServerProperties.ServerProperties.EVENT_TUTORIAL)
		            return;
				client.Player.Release(EReleaseType.City, false);
					return;
			}

            if (args.Length > 1 && args[1].ToLower() == "house")
            {
	            if (ServerProperties.ServerProperties.EVENT_THIDRANKI || ServerProperties.ServerProperties.EVENT_TUTORIAL)
		            return;
                client.Player.Release(EReleaseType.House, false);
                return;
            }
			client.Player.Release(EReleaseType.Normal, false);
		}
	}
}