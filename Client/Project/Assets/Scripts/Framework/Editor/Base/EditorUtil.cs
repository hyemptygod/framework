using System;
using System.Collections.Generic;
using System.IO;

namespace FrameworkEditor
{
    public enum PathType
    {
        None,
        File,
        Directory
    }

    public static class EditorUtil
    {
        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();

            Stack<string> stack = new Stack<string>();
            stack.Push(path);

            while (stack.Count > 0)
            {
                string current = stack.Pop();

                foreach (string item in Directory.GetDirectories(current))
                    stack.Push(item);

                foreach (var item in Directory.GetFiles(current))
                {
                    if (item.EndsWith(".meta")) continue;

                    string name = item.Substring(item.IndexOf("Assets")).Replace("\\", "/");

                    if (files.Contains(name)) continue;

                    files.Add(name);
                }
            }
            return files;
        }

        public static PathType CheckPathType(string path)
        {
            var mode = PathType.None;
            if (Directory.Exists(path))
                mode = PathType.Directory;
            else if (File.Exists(path))
                mode = PathType.File;
            return mode;
        }
    }
}
