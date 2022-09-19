using System;
using System.Collections.Generic;

public enum CommonFieldType : uint
{
    UInt = 1,
    Int = 2,
    String = 3,
    Array = 4,
    Object = 5,
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class DescriptionAttribute : Attribute
{
    private string _description;
    private uint _type;

    public DescriptionAttribute(string description, uint type = 1)
    {
        _description = description;
        _type = type;
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public uint Type
    {
        get { return _type; }
        set { _type = value; }
    }
}
