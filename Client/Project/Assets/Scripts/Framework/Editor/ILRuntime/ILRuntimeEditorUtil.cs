using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Framework;
using Framework.Core;
using ILRuntime.Runtime.Enviorment;

namespace FrameworkEditor.ILRuntimeHelper
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public static class ILRuntimeEditorUtil
    {
        public static string hotFixDLLEditor = PathConst.Streaming + "dll/HotFix.dll";
        public static string generatedPath = "Assets/3rd-part/ILRT/Generated";
        public static string adapterPath = "Assets/3rd-part/ILRT/Adapter/";
        public static string bindCode = "Assets/3rd-part/ILRT/Default/CLRBindings.txt";

        public static ILRuntime.Runtime.Enviorment.AppDomain GetDomain()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            var domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            var fs = new System.IO.FileStream(hotFixDLLEditor, FileMode.Open, FileAccess.Read);

            var dll = new byte[fs.Length];
            fs.Read(dll, 0, (int)(fs.Length));

            var mss = new System.IO.MemoryStream(dll);
            domain.LoadAssembly(mss);

            fs.Close();

            ILRTHelper.Run(domain);

            return domain;
        }

        public static List<Type> GetHotfixType(string baseTypeFullName)
        {
            var domain = ILRuntimeHelper.ILRuntimeEditorUtil.GetDomain();

            var basetype = domain.LoadedTypes[baseTypeFullName].ReflectionType;
            if (basetype == null)
                return null;

            var result = new List<Type>();
            foreach (var item in domain.LoadedTypes.Values)
            {
                var t = item.GetType();
                if (t.IsSubclassOf(basetype))
                {
                    result.Add(t);
                }
            }
            return result;
        }

        public static void GenerateCLRBindingByAnalysis()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            var domain = GetDomain();

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, generatedPath);

            AssetDatabase.Refresh();
        }

        public static void RemoveCLRBinding()
        {
            if (Directory.Exists(generatedPath))
            {
                Directory.Delete(generatedPath, true);
            }

            Directory.CreateDirectory(generatedPath);

            File.WriteAllText(generatedPath + "/CLRBindings.cs", File.ReadAllText(bindCode));

            AssetDatabase.Refresh();
        }

        public static void GenerateCrossbindAdapter()
        {
            foreach (var t in ILRTHelper.GetAdapterTypes())
            {
                using (var sw = new StreamWriter(adapterPath + t.Name + "Adapter.cs"))
                {
                    sw.WriteLine(CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(t, "Framework.ILRuntimeHelper"));
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
