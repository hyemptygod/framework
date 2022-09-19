using System;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace HotFix.Core
{
    /// <summary>
    /// 注册命名空间中所有Command的工具类
    /// </summary>
    public class BootstrapCommands : SimpleCommand
    {
        private const string SUFFIX = "Command";

        /// <summary>
        /// 注册所有的Command
        /// </summary>
        /// <param name="notification"></param>
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            RegisterCommands();
        }

        /// <summary>
        /// 反射注册命令
        /// </summary>
        private void RegisterCommands()
        {
            var types = App.ModuleTypes;
            if (types == null)
                return;

            // 获取所有的Commands名
            var type = System.Array.Find(types, t => t.Name == "Commands");
            if (type == null)
                return;

            foreach (var filed in type.GetFields())
            {
                var name = filed.GetRawConstantValue().ToString();
                if (string.IsNullOrEmpty(name)) continue;

                var func = GetCommandFunc(types, name);
                if (func == null) continue;

                Facade.RegisterCommand(name, func);
            }
        }

        /// <summary>
        /// 根据命令名获取命令的实例对象
        /// </summary>
        /// <param name="types"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Func<ICommand> GetCommandFunc(Type[] types, string name)
        {
            var class_name = name.EndsWith(SUFFIX) ? name : name + SUFFIX;

            var type = System.Array.Find(types, t => t.Name == class_name);
            if (type == null || !typeof(ICommand).IsAssignableFrom(type))
            {
                Log.Error(string.Format("{0} is not SimpleCommand or MacroCommand", class_name));
                return null;
            }

            var constructor = type.GetConstructor(new Type[] { });

            return () => { return constructor.Invoke(new object[] { }) as ICommand; };
        }
    }
}
