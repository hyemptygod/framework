using System;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class UIPanelAttribute : Attribute
{
    public string ABName { get; set; }
    public string AssetName { get; set; }
    public int Level { get; set; }

    public UIPanelAttribute(string abName, string assetName)
        : base()
    {
        ABName = abName;
        AssetName = assetName;
        Level = 0;
    }

    public UIPanelAttribute(string abName, string assetName, int level)
        : base()
    {
        ABName = abName;
        AssetName = assetName;
        Level = level;
    }
}
