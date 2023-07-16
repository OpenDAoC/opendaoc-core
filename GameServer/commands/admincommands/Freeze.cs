using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&freeze",
		ePrivLevel.Admin,
		"Freeze The region timer you're in. (Test purpose only)",
		"/freeze {seconds}")]
	public class Freeze : AbstractCommandHandler, ICommandHandler
	{
		private int delay = 0;
		
		public void OnCommand(GameClient client, string[] args)
		{
			if (args.Length < 2)
			{
				DisplaySyntax(client);
				return;
			}
			
			if (client != null && client.Player != null)
			{
				try
				{
					delay = Convert.ToInt32(args[1]);
					new ECSGameTimer(client.Player, FreezeCallback).Start(1);
				}
				catch
				{
				}
			}

		}
		
		private int FreezeCallback(ECSGameTimer timer)
		{
			System.Threading.Thread.Sleep(delay * 1000);
			return 0;
		}

	}
}
