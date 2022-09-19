using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Core;
using System.Reflection;
using UnityEngine;
using Framework.Resource;
using Framework;
using Framework.Data;

namespace HotFix.Base.Data
{
    /// <summary>
    /// 数据表管理工具
    /// </summary>
    public class TableDataHelper
    {
        private List<TableItem> _tableSetting;
        private Dictionary<string, DataSet> _datas;

        private int _creatingCount;

        public float Progress
        {
            get
            {
                if (_tableSetting == null)
                    return 0f;
                if (_tableSetting.Count <= 0)
                    return 1f;
                return (float)_creatingCount / _tableSetting.Count;
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet this[string type]
        {
            get
            {
                if (!_datas.TryGetValue(type, out DataSet dataset) || dataset == null)
                {
                    Log.Error(string.Format("不存在数据类型为{0}的数据表", type));
                    return null;
                }

                return dataset;
            }
        }

        /// <summary>
        /// 加载数据表配置文件
        /// </summary>
        public void LoadTableData()
        {
            ResourceManager.Instance.LoadAsset<TextAsset>(PathConst.SETTING_AB_NAME, PathConst.TABLE_ITEMS, asset =>
            {
                try
                {
                    _tableSetting = asset.text.FromJson<List<TableItem>>();
                    _datas = new Dictionary<string, DataSet>(_tableSetting.Count);
                    foreach (var item in _tableSetting)
                    {
                        CreateDataSet(item);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("数据表配置文件出错! \n" + e.ToString());
                }
            });
        }

        /// <summary>
        /// 创建 DataSet
        /// </summary>
        /// <param name="item"></param>
        private void CreateDataSet(TableItem item)
        {
            switch (item.mode)
            {
                case DataLoadMode.Json:
                    ResourceManager.Instance.LoadAsset<TextAsset>(PathConst.DATA_AB_NAME, item.name, asset =>
                    {
                        var datatype = Type.GetType(item.typeName);
                        var containertype = typeof(DataContainer<>).MakeGenericType(datatype);

                        var key = datatype.GetField(item.primaryKey);
                        if (key.FieldType != typeof(uint))
                        {
                            Log.Warn("所设置的primaryKey值类型不为uint,将以id作为primary");
                            key = datatype.GetField("id");
                        }
                        _datas.Add(item.typeName, new JsonDataSet(item.name, asset.text, key, containertype));

                        Log.Info(string.Format("table {0} prase success", item.name));

                        _creatingCount++;
                    });
                    break;
                default:
                    _creatingCount++;
                    break;
            }

        }
    }
}
