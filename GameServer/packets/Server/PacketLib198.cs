﻿/*
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

using System.Reflection;

namespace DOL.GS.PacketHandler
{
	[PacketLib(198, GameClient.eClientVersion.Version198)]
	public class PacketLib198 : PacketLib197
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructs a new PacketLib for Version 1.98 clients
		/// </summary>
		/// <param name="client">the gameclient this lib is associated with</param>
		public PacketLib198(GameClient client)
			: base(client)
		{
			//Dunnerholl 2009-07-28: TODO CtoS_0x11 player market search got some new search parameters, they are simply appended as a few more bytes (12) need to do analysis and update handler
		}
	}
}
