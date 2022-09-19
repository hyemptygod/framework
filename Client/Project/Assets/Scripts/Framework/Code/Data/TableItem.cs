using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Data
{
    /// <summary>
    /// 数据加载模式
    /// </summary>
    [Serializable]
    public enum DataLoadMode
    {
        /// <summary>
        /// Json模式
        /// </summary>
        Json,
    }

    /// <summary>
    /// 数据表配置类
    /// </summary>
    [Serializable]
    public class TableItem
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string name;
        /// <summary>
        /// 表的唯一标识符
        /// </summary>
        public string primaryKey;
        /// <summary>
        /// 表中数据的类型名
        /// </summary>
        public string typeName;

        /// <summary>
        /// 数据加载模式
        /// </summary>
        public DataLoadMode mode;
    }
}
