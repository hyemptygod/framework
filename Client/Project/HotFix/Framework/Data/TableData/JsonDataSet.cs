using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HotFix.Base.Data
{
    /// <summary>
    /// json数据集
    /// </summary>
    public class JsonDataSet : DataSet
    {
        private static BindingFlags _publicFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// 
        /// </summary>
        public JsonDataSet(string tableName, string json, FieldInfo keyField, Type t) : base(tableName)
        {
            var values = json.FromJson(t);

            var count = (int)t.GetMethod("Length", _publicFlags).Invoke(values, null);

            _datas = new SortedList<uint, BaseData>(count);

            var get_item = t.GetMethod("get_Item", _publicFlags);

            var args = new object[1];
            for (int i = 0; i < count; i++)
            {
                args[0] = i;
                var data = get_item.Invoke(values, args);
                if (data == null)
                    continue;
                var key = (uint)keyField.GetValue(data);
                _datas.Add(key, (BaseData)data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Contains(uint id)
        {
            return _datas.ContainsKey(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T Get<T>(uint id)
        {
            TryGet(id, out T result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public override T GetByIndex<T>(int index)
        {
            if (index >= Count)
            {
                Log.Error(string.Format("表{0}中的数据小于{1}条", _tableName, index));
                return null;
            }
            return (T)_datas.Values[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGet<T>(uint id, out T result)
        {
            if (_datas.TryGetValue(id, out BaseData data))
            {
                result = (T)data;
                if (result == null)
                {
                    Log.Error(string.Format("表{0}中不存在primarykey值为{1}的数据", _tableName, id));
                    return false;
                }
                return true;
            }
            result = null;
            Log.Error(string.Format("表{0}中不存在primarykey值为{1}的数据", _tableName, id));
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override T Find<T>(Func<T, bool> predicate)
        {
            T data = null;
            foreach (var temp in _datas)
            {
                data = temp as T;
                if (data == null)
                    continue;
                if (predicate(data))
                {
                    return data;
                }
            }
            Log.Error(string.Format("表{0}中不存在满足条件{1}的的数据", _tableName, predicate.ToString()));
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override List<T> FindAll<T>(Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var temp in _datas)
            {
                T data = temp as T;
                if (data == null)
                    continue;
                if (predicate(data))
                {
                    result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public override void Foreach<T>(Action<T> callback)
        {
            foreach (var data in _datas)
            {
                callback(data as T);
            }
        }
    }
}
