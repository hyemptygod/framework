using System;
using UnityEngine;
using UnityEngine.Events;
using Framework.Resource;
using HotFix.Core;
using System.Collections.Generic;
using HotFix.Base;
using HotFix.Base.Data;

namespace HotFix
{
    /// <summary>
    /// 数据管理器
    /// </summary>
    public class DataManager : HotFixSingletone<DataManager>
    {
        private GameSetting _gameSetting;
        /// <summary>
        /// 游戏配置
        /// </summary>
        public static GameSetting GameSetting
        {
            get
            {
                return Instance._gameSetting;
            }
        }

        private TableDataHelper _tableData = new TableDataHelper();
        /// <summary>
        /// 数据表管理器
        /// </summary>
        public static TableDataHelper TableData
        {
            get
            {
                return Instance._tableData;
            }
        }

        public static ProgressInfo progressInfo = new ProgressInfo();
        public static Action<ProgressInfo> updateCallback;
        public static Action completeCallback;

        public void Start()
        {
            CoroutineManager.RunCoroutine(StartHandle(), "Load Data");
        }

        private IEnumerator<float> StartHandle()
        {
            var timer = new Timer("Loading GameSetting");
            DispatchUpdate(DispatchProgressEvent.Init, "Loading GameSetting", 5);
            LoadGameSettingHandle();
            while (_gameSetting == null)
            {
                DispatchUpdate(DispatchProgressEvent.Update);
                yield return CoroutineManager.WaitForOneFrame;
            }
            DispatchUpdate(DispatchProgressEvent.Complete);
            timer.Stop();

            timer = new Timer("Loading Tables");
            DispatchUpdate(DispatchProgressEvent.Init, "Loading Tables", 5);
            _tableData.LoadTableData();
            while (_tableData.Progress < 1f)
            {
                DispatchUpdate(DispatchProgressEvent.Update);
                yield return CoroutineManager.WaitForOneFrame;
            }
            DispatchUpdate(DispatchProgressEvent.Complete);
            timer.Stop();

            if (completeCallback != null)
                completeCallback();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private void LoadGameSettingHandle()
        {
            ResourceManager.Instance.LoadAsset<TextAsset>(Framework.PathConst.SETTING_AB_NAME, Framework.PathConst.GAME_SETTING, asset =>
            {
                if (asset == null)
                {
                    Log.Error("GameSetting.xml 数据错误");
                    return;
                }

                try
                {
                    _gameSetting = asset.text.FromJson(typeof(GameSetting)) as GameSetting;
                }
                catch (Exception e)
                {
                    Log.Error("GameSetting 加载错误!\n" + e.ToString());
                    _gameSetting = null;
                }
            });
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <typeparam name="T">数据表的item类型</typeparam>
        /// <returns></returns>
        public static DataSet GetTable<T>()
        {
            return TableData[typeof(T).FullName];
        }

        public static T Get<T>(uint id) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return null;
            return set.Get<T>(id);
        }

        public static T GetByIndex<T>(int index) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return null;
            return set.GetByIndex<T>(index);
        }

        public static bool TryGet<T>(uint id, out T result) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
            {
                result = null;
                return false;
            }
            return set.TryGet<T>(id, out result);
        }

        public static bool Contains<T>(uint id) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return false;
            return set.Contains(id);
        }

        public static T Find<T>(Func<T, bool> predicate) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return null;
            return set.Find<T>(predicate);
        }

        public static List<T> FindAll<T>(Func<T, bool> predicate) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return null;
            return set.FindAll<T>(predicate);
        }

        public static void Foreach<T>(Action<T> callback) where T : BaseData
        {
            var set = TableData[typeof(T).FullName];
            if (set == null)
                return;
            set.Foreach<T>(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="title"></param>
        /// <param name="frames"></param>
        /// <param name="step"></param>
        public static void DispatchUpdate(DispatchProgressEvent e, string title = "", float frames = 10, float step = 1)
        {
            switch (e)
            {
                case DispatchProgressEvent.Init:
                    progressInfo.title = title;
                    progressInfo.frames = frames;
                    progressInfo.step = step;
                    progressInfo.current = 0;
                    break;
                case DispatchProgressEvent.Update:
                    progressInfo.Update();
                    break;
                case DispatchProgressEvent.Complete:
                    progressInfo.Complete();
                    break;
            }
            if (updateCallback != null)
                updateCallback(progressInfo);
        }
    }
}
