using System;
using System.Collections.Generic;

namespace HotFix.Base.Data
{

    /// <summary>
    /// 数据集合
    /// </summary>
    public abstract class DataSet
    {
        /// <summary>
        /// 数据字典
        /// </summary>
        protected SortedList<uint, BaseData> _datas;

        protected string _tableName;

        /// <summary>
        /// 
        /// </summary>
        public DataSet(string tableName)
        {
            _tableName = tableName;
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get
            {
                if (_datas == null)
                    return 0;
                return _datas.Count;
            }
        }

        /// <summary>
        /// 查找数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract T Get<T>(uint id) where T : BaseData;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract T GetByIndex<T>(int index) where T : BaseData;

        /// <summary>
        /// 查找数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public abstract bool TryGet<T>(uint id, out T result) where T : BaseData;

        /// <summary>
        /// 检查数据是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool Contains(uint id);

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public abstract T Find<T>(Func<T, bool> predicate) where T : BaseData;

        /// <summary>
        /// 条件查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public abstract List<T> FindAll<T>(Func<T, bool> predicate) where T : BaseData;

        /// <summary>
        /// 遍历数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public abstract void Foreach<T>(Action<T> callback) where T : BaseData;
    }


}
