﻿/*
 *
 * Atlas - Logger for max level times
 * 
 */

using System;
using DOL.Database.Attributes;

namespace DOL.Database;

/// <summary>
/// Database Storage of Tasks
/// </summary>
[DataTable(TableName = "TimeXLevel")]
public class DBTimeXLevel : DataObject
{
    protected int m_TimeXLevel_ID;
    protected string m_characterid = string.Empty;
    protected string m_characterName = string.Empty;
    protected int m_realm;
    protected string m_characterClass = string.Empty;
    protected int m_level;
    protected int m_solo;
    protected int m_hardcore;
    protected string m_timetolevel = string.Empty;
    protected long m_secondstolevel;
    protected long m_hourstolevel;
    protected int m_boosted;

    public DBTimeXLevel()
    {
    }

    [PrimaryKey(AutoIncrement = true)]
    public int TimeXLevel_ID
    {
        get => m_TimeXLevel_ID;
        set => m_TimeXLevel_ID = value;
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public string Character_ID
    {
        get => m_characterid;
        set
        {
            Dirty = true;
            m_characterid = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public string Character_Name
    {
        get => m_characterName;
        set
        {
            Dirty = true;
            m_characterName = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public int Character_Realm
    {
        get => m_realm;
        set
        {
            Dirty = true;
            m_realm = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public string Character_Class
    {
        get => m_characterClass;
        set
        {
            Dirty = true;
            m_characterClass = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public int Character_Level
    {
        get => m_level;
        set
        {
            Dirty = true;
            m_level = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public int Solo
    {
        get => m_solo;
        set
        {
            Dirty = true;
            m_solo = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public int Hardcore
    {
        get => m_hardcore;
        set
        {
            Dirty = true;
            m_hardcore = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public int Boosted
    {
        get => m_boosted;
        set
        {
            Dirty = true;
            m_boosted = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public string TimeToLevel
    {
        get => m_timetolevel;
        set
        {
            Dirty = true;
            m_timetolevel = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public long SecondsToLevel
    {
        get => m_secondstolevel;
        set
        {
            Dirty = true;
            m_secondstolevel = value;
        }
    }

    [DataElement(AllowDbNull = false, Unique = false)]
    public long HoursToLevel
    {
        get => m_hourstolevel;
        set
        {
            Dirty = true;
            m_hourstolevel = value;
        }
    }
}