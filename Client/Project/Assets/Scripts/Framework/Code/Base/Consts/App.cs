using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
    /// <summary>
    /// Framework dll
    /// </summary>
    public static class App
    {
        private static Assembly _assembly;

        static App()
        {
            _assembly = typeof(App).Assembly;
        }

        /// <summary>
        /// 类型名获得类型
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            return _assembly.GetType(typeName);
        }
    }
}
