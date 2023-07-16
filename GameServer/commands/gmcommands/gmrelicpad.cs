using System;
using DOL.GS.PacketHandler;
using DOL.GS;
using DOL.Database;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&gmrelicpad",
		ePrivLevel.GM,
		"GMCommands.GMRelicPad.Description",
		"GMCommands.GMRelicPad.Usage")]
	public class GMRelicPadCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (args.Length != 4 || (args[1] != "magic" && args[1] != "strength"))
			{
				DisplaySyntax(client);
				return;
			}

			ushort emblem = ushort.Parse(args[3]);
			emblem += (ushort)((args[1] == "magic") ? 10 : 0);

			GameRelicPad pad = new GameRelicPad();
			pad.Name = args[2];
			pad.Realm = (eRealm)byte.Parse(args[3]);
			pad.Emblem = emblem;
			pad.CurrentRegionID = client.Player.CurrentRegionID;
			pad.X = client.Player.X;
			pad.Y = client.Player.Y;
			pad.Z = client.Player.Z;
			pad.Heading = client.Player.Heading;
			pad.AddToWorld();
			pad.SaveIntoDatabase();
		}
	}
}
