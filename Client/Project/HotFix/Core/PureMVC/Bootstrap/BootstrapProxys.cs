using System;
using System.Collections.Generic;
using System.Linq;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Reflection;

namespace HotFix.Core
{
    /// <summary>
    /// 注册命名空间中所有Proxy的工具类
    /// </summary>
    public class BootstrapProxys : SimpleCommand
    {
        private ConstructorInfo _constructor;

        /// <summary>
        /// 注册所有Proxy
        /// </summary>
        /// <param name="notification"></param>
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            RegisterProxys();
        }

        private void RegisterProxys()
        {
            var types = App.ModuleTypes;
            if (types == null)
                return;
            foreach (var t in types.Where(t => t.IsSubclassOf(typeof(BaseProxy))))
            {
                _constructor = t.GetConstructor(new Type[] { });
                if (_constructor == null) continue;
                Facade.RegisterProxy((BaseProxy)_constructor.Invoke(new object[] { }));
            }
        }
    }
}
