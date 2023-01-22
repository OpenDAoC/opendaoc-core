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
using DOL.Language;
using DOL.GS.PacketHandler;

namespace DOL.GS.Trainer;

/// <summary>
/// Enchanter Trainer
/// </summary>
[NPCGuildScript("Enchanter Trainer",
    eRealm.Hibernia)] // this attribute instructs DOL to use this script for all "Enchanter Trainer" NPC's in Albion (multiple guilds are possible for one script)
public class EnchanterTrainer : GameTrainer
{
    public override eCharacterClass TrainedClass => eCharacterClass.Enchanter;

    public const string WEAPON_ID1 = "enchanter_item";

    public EnchanterTrainer() : base()
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
        if (player.CharacterClass.ID == (int) TrainedClass)
        {
            player.Out.SendMessage(
                LanguageMgr.GetTranslation(player.Client.Account.Language, "EnchanterTrainer.Interact.Text2",
                    Name, player.GetName(0, false)), eChatType.CT_Say, eChatLoc.CL_ChatWindow);
        }
        else
        {
            // perhaps player can be promoted
            if (CanPromotePlayer(player))
            {
                player.Out.SendMessage(
                    LanguageMgr.GetTranslation(player.Client.Account.Language, "EnchanterTrainer.Interact.Text1",
                        Name), eChatType.CT_System, eChatLoc.CL_PopupWindow);
                if (!player.IsLevelRespecUsed) OfferRespecialize(player);
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
        var player = source as GamePlayer;
        var lowerCase = text.ToLower();

        if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language,
                "EnchanterTrainer.WhisperReceiveCase.Text1"))
            // promote player to other class
            if (CanPromotePlayer(player))
            {
                PromotePlayer(player, (int) eCharacterClass.Enchanter,
                    LanguageMgr.GetTranslation(player.Client.Account.Language,
                        "EnchanterTrainer.WhisperReceive.Text1", player.GetName(0, false)), null);
                player.ReceiveItem(this, WEAPON_ID1);
            }

        return true;
    }
}