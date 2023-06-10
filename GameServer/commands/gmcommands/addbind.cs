/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&addbind",
		// Message: '/addbind' - Adds a new bind point based on the client's current in-game position.
		"GMCommands.AddBind.CmdList.Description",
		// Message: <----- '/{0}' Command {1}----->
		"AllCommands.Header.General.Commands",
		// Required minimum privilege level to use the command
		ePrivLevel.GM,
		// Message: Adds a new bind point based on the client's current in-game position.
		"GMCommands.AddBind.Description",
		// Message: /addbind <radius>
		"GMCommands.AddBind.Syntax.Add",
		// Message: Creates a new bind point in-game.
		"GMCommands.AddBind.Usage.Add"
	)]
	public class AddBindCommandHandler : AbstractCommandHandler, ICommandHandler
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
					// Message: [Error] {0}
					ChatUtil.SendTypeMessage(eMsg.Error, client, "GMCommands.Error", e.Message);
					return;
				}
			}
			BindPoint bp = new BindPoint();
			bp.X = client.Player.X;
			bp.Y = client.Player.Y;
			bp.Z = client.Player.Z;
			bp.Region = client.Player.CurrentRegionID;
			bp.Radius = bindRadius;
			
			GameServer.Database.AddObject(bp);
			client.Player.CurrentRegion.AddArea(new Area.BindArea("bind point", bp));
			
			// Message: [SUCCESS] Bind point added: X={0}, Y={1}, Z={2}, Radius={3}, Region={4}
			ChatUtil.SendTypeMessage(eMsg.Success, client, "GMCommands.AddBind.BindPointAdded", bp.X, bp.Y, bp.Z, bp.Radius, bp.Region);
		}
	}
}