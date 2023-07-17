using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[Command(
		"&addbind",
		ePrivLevel.GM,
		"GMCommands.AddBind.Description",
		"GMCommands.AddBind.Usage")]
	public class AddBindCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			ushort bindRadius = 750;
			if (args.Length >= 2)
			{
				try
				{
					bindRadius = UInt16.Parse(args[1]);
				}
				catch (Exception e)
				{
					DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Error", e.Message));
					return;
				}
			}
			DbBindPoints bp = new DbBindPoints();
			bp.X = client.Player.X;
			bp.Y = client.Player.Y;
			bp.Z = client.Player.Z;
			bp.Region = client.Player.CurrentRegionID;
			bp.Radius = bindRadius;
			GameServer.Database.AddObject(bp);
			client.Player.CurrentRegion.AddArea(new Area.BindArea("bind point", bp));
			DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.AddBind.BindPointAdded", bp.X, bp.Y, bp.Z, bp.Radius, bp.Region));
		}
	}
}