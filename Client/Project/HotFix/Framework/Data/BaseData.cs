using System;
using System.Reflection;
using System.Collections.Generic;

namespace HotFix.Base.Data
{
    /// <summary>
    /// 基础数据类
    /// </summary>
    [Serializable]
    [TableData("table_testdata")]
    public class BaseData
    {
        /// <summary>
        /// 数据唯一标识符
        /// </summary>
        [Description("序列号")]
        public uint id;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = "[";
            foreach (var item in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                result += string.Format("({0},{1})", item.Name, item.GetValue(this).ToString());
            }
            result += "]";
            return result;
        }
    }

    /// <summary>
    /// 数据容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataContainer<T> where T : BaseData
    {
        /// <summary>
        /// 数据List
        /// </summary>
        public List<T> datas = new List<T>();

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return datas[index];
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Length()
        {
            return datas.Count;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        public void Add(T data)
        {
            datas.Add(data);
        }
    }
}
