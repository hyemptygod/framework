using System;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class UIFieldAttribute : Attribute
{
    public UIFieldAttribute() : base()
    {
    }

    public UIFieldAttribute(string name)
        : base()
    {
        Parent = "";
        Name = name;
    }

    public UIFieldAttribute(string parent, string name)
        : base()
    {
        Parent = parent;
        if (!Parent.EndsWith("/")) Parent += "/";
        Name = name;
    }

    public string Name { get; set; }
    public string Parent { get; set; }

    public string Path
    {
        get
        {
            return Parent + Name;
        }
    }
}
