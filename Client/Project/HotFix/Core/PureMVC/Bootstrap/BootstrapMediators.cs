using System;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using PureMVC.Patterns.Mediator;
using System.Reflection;

namespace HotFix.Core
{
    /// <summary>
    /// 注册命名空间中所有Mediator的工具类
    /// </summary>
    public class BootstrapMediators : SimpleCommand
    {
        private ConstructorInfo _constructor;

        /// <summary>
        /// 注册所有的Mediator
        /// </summary>
        /// <param name="notification"></param>
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            RegisterMeditors();
        }

        private void RegisterMeditors()
        {
            var types = App.ModuleTypes;
            var b = typeof(BaseMediator<>);
            foreach (var t in types)
            {
                if (t.BaseType == null || t.BaseType.GenericTypeArguments == null || t.BaseType.GenericTypeArguments.Length != 1)
                    continue;
                var generictype = t.BaseType.GenericTypeArguments[0];
                var basetype = b.MakeGenericType(generictype);
                if (t.BaseType != basetype)
                    continue;
                _constructor = t.GetConstructor(new Type[] { });
                if (_constructor == null) continue;
                Facade.RegisterMediator((Mediator)_constructor.Invoke(new object[] { }));
            }
        }
    }
}
