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
using System.Collections.Generic;
using System.Text;
using DOL.GS;
using DOL.Database;
using System.Collections;
using DOL.GS.Spells;
using log4net;
using System.Reflection;
using DOL.GS.PacketHandler;

namespace DOL.GS;

/// <summary>
/// Midgard SI teleporter.
/// </summary>
/// <author>Aredhel</author>
public class MidgardSITeleporter : GameTeleporter
{
    /// <summary>
    /// Add equipment to the teleporter.
    /// </summary>
    /// <returns></returns>
    public override bool AddToWorld()
    {
        var template = new GameNpcInventoryTemplate();
        template.AddNPCEquipment(eInventorySlot.TorsoArmor, 983, 26);
        template.AddNPCEquipment(eInventorySlot.HandsArmor, 986, 26);
        template.AddNPCEquipment(eInventorySlot.LegsArmor, 984, 26);
        template.AddNPCEquipment(eInventorySlot.FeetArmor, 987, 26);
        template.AddNPCEquipment(eInventorySlot.Cloak, 57, 26);
        Inventory = template.CloseTemplate();

        SwitchWeapon(eActiveWeaponSlot.TwoHanded);
        VisibleActiveWeaponSlots = 34;
        return base.AddToWorld();
    }

    private string[] m_destination =
    {
        "Aegirhamn",
        "Bjarken",
        "Hagall",
        "Knarr"
    };

    /// <summary>
    /// Display the teleport indicator around this teleporters feet
    /// </summary>
    public override bool ShowTeleporterIndicator => true;

    /// <summary>
    /// Player right-clicked the teleporter.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public override bool Interact(GamePlayer player)
    {
        if (!base.Interact(player))
            return false;

        var playerAreaList = new List<string>();
        foreach (AbstractArea area in player.CurrentAreas)
            playerAreaList.Add(area.Description);

        SayTo(player, "Greetings. Where can I send you?");
        foreach (var destination in m_destination)
            if (!playerAreaList.Contains(destination))
                player.Out.SendMessage(string.Format("[{0}]", destination),
                    eChatType.CT_Say, eChatLoc.CL_PopupWindow);

        return true;
    }

    /// <summary>
    /// Player has picked a destination.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    protected override void OnDestinationPicked(GamePlayer player, Teleport destination)
    {
        // Not porting to where we already are.

        var playerAreaList = new List<string>();
        foreach (AbstractArea area in player.CurrentAreas)
            playerAreaList.Add(area.Description);

        if (playerAreaList.Contains(destination.TeleportID))
            return;

        switch (destination.TeleportID.ToLower())
        {
            case "aegirhamn":
                break;
            case "bjarken":
                break;
            case "hagall":
                break;
            case "knarr":
                break;
            default:
                return;
        }

        SayTo(player, "Have a safe journey!");
        base.OnDestinationPicked(player, destination);
    }

    /// <summary>
    /// Teleport the player to the designated coordinates.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    protected override void OnTeleport(GamePlayer player, Teleport destination)
    {
        OnTeleportSpell(player, destination);
    }
}