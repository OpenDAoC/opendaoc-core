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

using System;
using System.Text;

namespace DOL.GS;
//Dinberg: Added this for instances as we dont want to have to parse XML every time we create an instance,
//but we need to put zones into the instance.

/// <summary>
/// Holds the information of a child of the zone config file, that can be used later for instance creation.
/// </summary>
public class ZoneData
{
    public ushort ZoneID
    {
        get => m_ZoneID;
        set => m_ZoneID = value;
    }

    public ushort RegionID
    {
        get => m_RegionID;
        set => m_RegionID = value;
    }

    public byte OffX
    {
        get => m_OffX;
        set => m_OffX = value;
    }

    public byte OffY
    {
        get => m_OffY;
        set => m_OffY = value;
    }

    public byte Height
    {
        get => m_Height;
        set => m_Height = value;
    }

    public byte Width
    {
        get => m_Width;
        set => m_Width = value;
    }

    public string Description
    {
        get => m_description;
        set => m_description = value;
    }

    public byte DivingFlag
    {
        get => m_divingFlag;
        set => m_divingFlag = value;
    }

    public int WaterLevel
    {
        get => m_waterLevel;
        set => m_waterLevel = value;
    }

    public bool IsLava
    {
        get => m_IsLava;
        set => m_IsLava = value;
    }

    private byte m_OffX, m_OffY, m_Height, m_Width;
    private ushort m_ZoneID, m_RegionID;
    private string m_description;
    private int m_waterLevel;
    private byte m_divingFlag;
    private bool m_IsLava;
}