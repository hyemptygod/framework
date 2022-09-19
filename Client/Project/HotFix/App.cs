using System;
using System.Linq;
using HotFix.Base;

namespace HotFix
{
    /// <summary>
    /// 热更代码入口
    /// </summary>
    public class App : HotFixSingletone<App>
    {
        private static Type[] _types;

        private static Type[] _moduleTypes;
        public static Type[] ModuleTypes
        {
            get
            {
                if (_types == null)
                    return null;
                if (_moduleTypes == null)
                    _moduleTypes = _types.Where(a => a.Namespace == "HotFix.Game").ToArray();
                return _moduleTypes;
            }
        }

        public App()
        {

        }

        private void StartHandle()
        {
            Game.GameFacade.Instance.Start();
            Game.GameFacade.ShowPanel<Game.LoadDataView>();

            //加载数据表
            DataManager.updateCallback += p =>
            {
                Game.GameFacade.Instance.SendNotification(Game.EventList.LOADDATA_VIEW_REFRESH, p);
            };
            DataManager.completeCallback += () =>
            {
                Game.GameFacade.Instance.SendNotification(Game.EventList.LOADDATA_VIEW_COMPLETE);
            };
            DataManager.Instance.Start();

        }

        private void UpdateHandle()
        {
            //Log.Info("Update");
        }

        private void StopHandle()
        {
            Log.Info("Stop");
        }

        public static void Start(Type[] types)
        {
            _types = types;
            Instance.StartHandle();
        }

        public static void Update()
        {
            Instance.UpdateHandle();
        }

        public static void Stop()
        {
            Instance.StopHandle();
        }
    }
}
