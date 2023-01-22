using System;
using DOL.Database;
using DOL.Database.Attributes;

namespace DOL.Database;

[DataTable(TableName = "Area")]
public class DBArea : DataObject
{
    private string m_translationId;
    private string m_description;
    private int m_x;
    private int m_y;
    private int m_z;
    private int m_radius;
    private ushort m_region;
    private string m_classType = string.Empty;
    private bool m_canBroadcast;
    private byte m_sound;
    private bool m_checkLOS;
    private string m_points;

    public DBArea()
    {
    }

    [DataElement(AllowDbNull = true)]
    public string TranslationId
    {
        get => m_translationId;
        set
        {
            Dirty = true;
            m_translationId = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public string Description
    {
        get => m_description;
        set
        {
            Dirty = true;
            m_description = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public int X
    {
        get => m_x;
        set
        {
            Dirty = true;
            m_x = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public int Y
    {
        get => m_y;
        set
        {
            Dirty = true;
            m_y = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public int Z
    {
        get => m_z;
        set
        {
            Dirty = true;
            m_z = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public int Radius
    {
        get => m_radius;
        set
        {
            Dirty = true;
            m_radius = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public ushort Region
    {
        get => m_region;
        set
        {
            Dirty = true;
            m_region = value;
        }
    }

    [DataElement(AllowDbNull = true)]
    public string ClassType
    {
        get => m_classType;
        set
        {
            Dirty = true;
            m_classType = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public bool CanBroadcast
    {
        get => m_canBroadcast;
        set
        {
            Dirty = true;
            m_canBroadcast = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public byte Sound
    {
        get => m_sound;
        set
        {
            Dirty = true;
            m_sound = value;
        }
    }

    [DataElement(AllowDbNull = false)]
    public bool CheckLOS
    {
        get => m_checkLOS;
        set
        {
            Dirty = true;
            m_checkLOS = value;
        }
    }

    [DataElement(AllowDbNull = true)]
    public string Points
    {
        get => m_points;
        set
        {
            Dirty = true;
            m_points = value;
        }
    }
}