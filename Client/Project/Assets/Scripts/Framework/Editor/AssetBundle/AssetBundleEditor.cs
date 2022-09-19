using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.IMGUI;
using UObject = UnityEngine.Object;
using System.Linq;
using Framework;
using Framework.Resource;

namespace FrameworkEditor.AssetBundle
{
    public static class AssetBundleHelperEditor
    {
        private readonly static string assetPath = "Assets/GameAssets/";
        private readonly static string[] _pngs = new string[] { "png" };

        private static Dictionary<string, int> _bundlePathDic = new Dictionary<string, int>()
        {
            {assetPath + "Image/",1 },
            {assetPath + "Atlas/",0 },
            {assetPath + "Settings/",0 },
            {assetPath + "Data/",0 },
        };

        private static List<string> _bundleAssetDic = new List<string>()
        {
            assetPath + "Prefab/",
        };

        public static void AssetToBundle(UObject obj)
        {
            if (obj == null)
            {
                AllAssetToBundle();
                return;
            }

            var path = AssetDatabase.GetAssetPath(obj);
            var mode = EditorUtil.CheckPathType(path);
            if (!path.StartsWith(assetPath) || mode == PathType.None)
            {
                AllAssetToBundle();
                return;
            }

            var newpath = path.Replace("\\", "/");

            var root = CheckAssetsToOneBundle(newpath);
            if (!string.IsNullOrEmpty(root))
            {
                AssetsToOneBundle(root, newpath.Replace(root.TrimEnd('/'), ""));
                return;
            }

            if (CheckAssetsToMutilBundle(newpath))
            {
                AssetsToMutilBundles(path);
                return;
            }
        }

        private static string CheckAssetsToOneBundle(string path)
        {
            foreach (var item in _bundlePathDic)
            {
                if (path.StartsWith(item.Key.TrimEnd('/')))
                {
                    return item.Key;
                }
            }
            return null;
        }

        private static bool CheckAssetsToMutilBundle(string path)
        {
            foreach (var item in _bundleAssetDic)
            {
                if (path.StartsWith(item.TrimEnd('/')))
                {
                    return true;
                }
            }
            return false;
        }

        private static void AllAssetToBundle()
        {
            foreach (var item in _bundlePathDic.Keys)
            {
                AssetsToOneBundle(item);
            }

            foreach (var item in _bundleAssetDic)
            {
                AssetsToMutilBundles(item);
            }
        }

        /// <summary>
        /// 同一目录的为同一个assetbundle
        /// </summary>
        /// <param name="root"></param>
        /// <param name="level"></param>
        private static void AssetsToOneBundle(string root, string child = "")
        {
            var dirs = new List<DirectoryInfo>();
            var level = _bundlePathDic[root];
            if (!string.IsNullOrEmpty(child))
            {
                for (int i = 0; i < level; i++)
                {
                    var index = child.IndexOf("/");
                    if (index == -1)
                    {
                        root += child;
                        child = "";
                        break;
                    }
                    root += child.Substring(0, index + 1);
                    child = child.Substring(index + 1);
                }

                ToBundle(root.Replace(assetPath, ""), root + child);
            }
            else
            {
                dirs.Add(new DirectoryInfo(root));
                var current = 0;
                while (current < level)
                {
                    var newdirs = new List<DirectoryInfo>();
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        newdirs.AddRange(dirs[i].GetDirectories());
                    }
                    dirs = newdirs;
                    current++;
                }
                foreach (var dir in dirs)
                {
                    var path = dir.FullName.Replace("\\", "/");
                    path = path.Substring(path.IndexOf("Assets/"));
                    var abname = path.Replace(assetPath, "");
                    ToBundle(abname, path);
                }
            }
        }

        private static void ToBundle(string abName, string path)
        {
            var mode = EditorUtil.CheckPathType(path);
            if (mode == PathType.File)
                SetAssetBundleInfo(AssetImporter.GetAtPath(path), abName);
            else
                foreach (var item in EditorUtil.GetFiles(path))
                {
                    SetAssetBundleInfo(AssetImporter.GetAtPath(item), abName);
                }
        }

        /// <summary>
        /// 目录下的文件都分别为单独的Bundle
        /// </summary>
        /// <param name="roots"></param>
        private static void AssetsToMutilBundles(params string[] roots)
        {
            var resources = new Dictionary<string, uint>();
            var files = new List<string>();
            foreach (var path in roots)
            {
                var mode = EditorUtil.CheckPathType(path);
                if (mode == PathType.File)
                    files.Add(path);
                else
                    files.AddRange(EditorUtil.GetFiles(path));
            }

            /// resources 值
            /// 0 表示直接文件需要打包
            /// 1 表示引用1次的文件不需要打包
            /// 大于1 表示引用超过1次的文件需要单独打包
            GetBuilds(files, ref resources);

            foreach (var item in resources)
            {
                if (item.Value == 1)
                    continue;
                var assetpath = item.Key;
                var cutindex = assetpath.IndexOf(".");
                if (cutindex != -1)
                    assetpath = assetpath.Substring(0, cutindex);
                assetpath = assetpath.Replace(assetPath, "");
                SetAssetBundleInfo(AssetImporter.GetAtPath(item.Key), assetpath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static void GetBuilds(List<string> files, ref Dictionary<string, uint> resources)
        {
            foreach (var assetpath in files)
            {
                resources.SafeAdd(assetpath, 0u);

                string[] dependencies = AssetDatabase.GetDependencies(assetpath);
                foreach (var item in dependencies)
                {
                    if (item.Contains("Assets/GameAssets/Image")) // 图片都是单独打包的
                        continue;

                    if (item.EndsWith(".cs")) // 不包含脚本
                        continue;

                    var refrence = resources.SafeGet(item);
                    if (refrence != 0)
                        resources[item] = refrence + 1;
                }

            }
        }

        private static void SetAssetBundleInfo(AssetImporter importer, string abName)
        {
            abName = abName.Trim('/');
            string assetname = abName.Substring(abName.LastIndexOf("/") + 1);
            string variant = "";
            if (assetname.Contains("-"))
            {
                string[] result = assetname.Split('-');
                assetname = result[0];
                variant = result[1];
            }
            importer.SetAssetBundleNameAndVariant(abName, variant);

            Log.Info(string.Format("{0} 设置 abname={1} <color=green>成功</color>", importer.assetPath, abName.ToLower()));
        }
    }
}
