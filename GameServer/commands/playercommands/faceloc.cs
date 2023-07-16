/*
 * Author:	Ogre <ogre@videogasm.com>
 * Rev:		$Id: faceloc.cs,v 1.6 2005/05/10 13:36:38 noret Exp $
 *
 * Desc:	Implements /faceloc command
 *
 */
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&faceloc",
		ePrivLevel.Player,
		"Turns and faces your character into the direction of the x, y coordinates provided (using Mythic zone coordinates).",
		"/faceloc [x] [y]")]
	public class LocFaceCommandHandler : AbstractCommandHandler,ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "faceloc"))
				return;

			if (client.Player.IsTurningDisabled)
			{
				DisplayMessage(client, "You can't use this command now!");
				return;
			}

			if (args.Length < 3)
			{
				client.Out.SendMessage
					(
					"Please enter X and Y coordinates.",
					eChatType.CT_System,
					eChatLoc.CL_SystemWindow
					);
				return;
			}
			int x = 0;
			int y = 0;
			try
			{
				x = System.Convert.ToInt32(args[1]);
				y = System.Convert.ToInt32(args[2]);
			}
			catch
			{
				client.Out.SendMessage("Please enter a valid X and Y location.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			int Xoffset = client.Player.CurrentZone.XOffset;
			int Yoffset = client.Player.CurrentZone.YOffset;
            Point2D gloc = new Point2D( Xoffset + x, Yoffset + y );
			ushort direction = client.Player.GetHeading(gloc);
			client.Player.Heading = direction;
			client.Out.SendPlayerJump(true);
		}
	}
}