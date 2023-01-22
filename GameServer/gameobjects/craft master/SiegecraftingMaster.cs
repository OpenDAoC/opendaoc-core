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

using DOL.Language;

namespace DOL.GS;

[NPCGuildScript("Siegecrafting Master")]
public class SiegecraftingMaster : CraftNPC
{
    private static readonly eCraftingSkill[] m_trainedSkills =
    {
        eCraftingSkill.MetalWorking,
        eCraftingSkill.WoodWorking,
        eCraftingSkill.SiegeCrafting
    };

    public override eCraftingSkill[] TrainedSkills => m_trainedSkills;

    public override string GUILD_ORDER =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "SiegecraftingMaster.GuildOrder");

    public override string ACCEPTED_BY_ORDER_NAME =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "SiegecraftingMaster.AcceptedByOrderName");

    /// <summary>
    /// The eCraftingSkill
    /// </summary>
    public override eCraftingSkill TheCraftingSkill => eCraftingSkill.SiegeCrafting;

    /// <summary>
    /// The text for join order
    /// </summary>
    public override string InitialEntersentence =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "SiegecraftingMaster.InitialEntersentence");
}