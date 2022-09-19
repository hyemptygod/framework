using System;
using System.Linq;
using PureMVC.Patterns.Facade;
using PureMVC.Patterns.Mediator;
using PureMVC.Patterns.Proxy;

namespace HotFix.Core
{
    /// <summary>
    /// Facade 基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseFacade<T> : Facade where T : BaseFacade<T>, new()
    {
        private static T _instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseFacade() : base(typeof(T).ToString())
        {
            Log.Info(typeof(T).ToString() + " Init");
        }

        /// <summary>
        /// 初始化Controller
        /// </summary>
        protected override void InitializeController()
        {
            base.InitializeController();

            RegisterCommand("STARTUP", () => new StartupCommand());
        }

        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Start()
        {
            var timer = new Timer("Register Module");

            SendNotification("STARTUP");

            RemoveCommand("STARTUP");

            timer.Stop();
        }

        /// <summary>
        /// 获取Proxy实例
        /// </summary>
        /// <typeparam name="TProxy"></typeparam>
        /// <returns></returns>
        public static TProxy GetProxy<TProxy>() where TProxy : BaseProxy
        {
            return Instance.RetrieveProxy(typeof(TProxy).ToString()) as TProxy;
        }

        /// <summary>
        /// 获取Mediator实例
        /// </summary>
        /// <typeparam name="TMediator"></typeparam>
        /// <returns></returns>
        public static TMediator GetMediator<TMediator, TView>() where TMediator : BaseMediator<TView> where TView : BaseView, new()
        {
            return Instance.RetrieveMediator(typeof(TMediator).ToString()) as TMediator;
        }
    }
}
