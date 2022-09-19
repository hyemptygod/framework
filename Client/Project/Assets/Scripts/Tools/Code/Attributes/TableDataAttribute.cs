using System;

/// <summary>
/// 数据表特性标签
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class TableDataAttribute : Attribute
{
    /// <summary>
    /// 数据类型对应的表名
    /// </summary>
    public string name;

    /// <summary>
    /// 数据表唯一标识符
    /// </summary>
    public string primaryKey;

    /// <summary>
    /// 数据表加载方式
    /// </summary>
    public string mode;

    public TableDataAttribute() : base()
    {

    }

    /// <summary>
    /// 数据表特性
    /// </summary>
    /// <param name="name">对应的数据表名</param>
    /// <param name="primaryKey">数据的唯一标识符号(必须为类型的某一字段)</param>
    /// <param name="mode">数据表加载模式</param>
    public TableDataAttribute(string name, string primaryKey = "id", string mode = "Json") : base()
    {
        this.name = name;
        this.primaryKey = primaryKey;
        this.mode = mode;
    }
}

