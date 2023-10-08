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

using DOL.GS.PacketHandler;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Scout Trainer
	/// </summary>
	[NPCGuildScript("Scout Trainer", ERealm.Albion)]		// this attribute instructs DOL to use this script for all "Scout Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class ScoutTrainer : GameTrainer
	{
		public override EPlayerClass TrainedClass
		{
			get { return EPlayerClass.Scout; }
		}

		public const string WEAPON_ID1 = "scout_item";
		public ScoutTrainer() : base()
		{
		}

		/// <summary>
		/// Interact with trainer
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;
			
			// check if class matches.
			if (player.PlayerClass.ID == (int)TrainedClass)
			{
				OfferTraining(player);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(this.Name + " says, \"Do you desire to [join the Defenders of Albion] and defend our realm as a Scout?\"",EChatType.CT_Say,EChatLoc.CL_PopupWindow);
					if (!player.IsLevelRespecUsed)
					{
						OfferRespecialize(player);
					}
				}
				else
				{
					CheckChampionTraining(player);
				}
			}
			return true;
		}

		/// <summary>
		/// Talk to trainer
		/// </summary>
		/// <param name="source"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public override bool WhisperReceive(GameLiving source, string text)
		{
			if (!base.WhisperReceive(source, text)) return false;
			GamePlayer player = source as GamePlayer;
			
			switch (text) {
				case "join the Defenders of Albion":
					// promote player to other class
					if (CanPromotePlayer(player)) {
						PromotePlayer(player, (int)EPlayerClass.Scout, "Welcome young warrior! May your time in Albion's army be rewarding! Here is your Huntsman's Longbow. The bow is your guild weapon! Be sure to carry it with you always.", null);
						player.ReceiveItem(this,WEAPON_ID1);
					}
					break;
			}
			return true;
		}
	}
}
