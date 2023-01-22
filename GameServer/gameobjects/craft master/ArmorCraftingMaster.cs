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

/// <summary>
/// the master for armorcrafting
/// </summary>
[NPCGuildScript("Armorsmiths Master")]
public class ArmorCraftingMaster : CraftNPC
{
    private static readonly eCraftingSkill[] m_trainedSkills =
    {
        eCraftingSkill.ArmorCrafting,
        eCraftingSkill.ClothWorking,
        eCraftingSkill.Fletching,
        eCraftingSkill.LeatherCrafting,
        eCraftingSkill.SiegeCrafting,
        eCraftingSkill.Tailoring,
        eCraftingSkill.WeaponCrafting,
        eCraftingSkill.MetalWorking,
        eCraftingSkill.WoodWorking
    };

    public override eCraftingSkill[] TrainedSkills => m_trainedSkills;

    public override string GUILD_ORDER =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "ArmorCraftingMaster.GuildOrder");

    public override string ACCEPTED_BY_ORDER_NAME =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "ArmorCraftingMaster.AcceptedByOrderName");

    public override eCraftingSkill TheCraftingSkill => eCraftingSkill.ArmorCrafting;

    public override string InitialEntersentence =>
        LanguageMgr.GetTranslation(ServerProperties.Properties.SERV_LANGUAGE,
            "ArmorCraftingMaster.InitialEntersentence");
}