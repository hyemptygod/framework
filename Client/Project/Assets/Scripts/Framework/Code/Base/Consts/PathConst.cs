using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 地址信息
    /// </summary>
    public static class PathConst
    {

#if UNITY_EDITOR
        /// <summary>
        /// 本地地址
        /// </summary>
        public static string Local
        {
            get
            {
                return Application.dataPath + "/";
            }
        }

        /// <summary>
        /// 本地地址
        /// </summary>
        public static string LocalAssets
        {
            get
            {
                return Local + "GameAssets/";
            }
        }

        /// <summary>
        /// 项目数据表地址
        /// </summary>
        public static string LocalDataPath = LocalAssets + "Data/";

        /// <summary>
        /// 项目配置表地址
        /// </summary>
        public static string LocalSettingPath = LocalAssets + "Settings/";

        /// <summary>
        /// 本地Json文件路径
        /// </summary>
        public static string LocalJsonPath = Local.Replace("Client/Project/Assets/", "Data/Json/");

        public const string kSimulateAssetBundles = "SimulateAssetBundles";
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true);
            }
        }
#endif

        /// <summary>
        /// Stream地址只读
        /// </summary>
        public static string Streaming
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor)
                    return "file:///" + Application.streamingAssetsPath + "/";
                return Application.streamingAssetsPath + "/";
            }
        }

        /// <summary>
        /// Persistent地址可读写
        /// </summary>
        public static string Persistent
        {
            get
            {
                return Application.persistentDataPath + "/";
            }
        }

        /// <summary>
        /// temporaryCache 只读
        /// </summary>
        public static string TemporaryCache
        {
            get
            {
                return Application.temporaryCachePath + "/";
            }
        }

        private static string _platform;
        /// <summary>
        /// 当前平台
        /// </summary>
        public static string Platform
        {
            get
            {
                if (string.IsNullOrEmpty(_platform))
                    _platform = GetPlatformName(Application.platform);
                return _platform;
            }
        }

        private static string _abStramingPath;
        /// <summary>
        /// Streaming 下的AB包
        /// </summary>
        public static string ABStreamingPath
        {
            get
            {
                if (string.IsNullOrEmpty(_abStramingPath))
                    _abStramingPath = Streaming + "AssetBundles/" + Platform + "/";
                return _abStramingPath;
            }
        }

        private static string _abPersistentPath;
        /// <summary>
        /// Persistent下的AB包
        /// </summary>
        public static string ABPersistentPath
        {
            get
            {
                if (string.IsNullOrEmpty(_abPersistentPath))
                    _abPersistentPath = Persistent + "AssetBundles/" + Platform + "/";
                return _abPersistentPath;
            }
        }

        private static string _hotFixDLL;
        public static string HotFixDLL
        {
            get
            {
                if (string.IsNullOrEmpty(_hotFixDLL))
                {
#if UNITY_EDITOR
                    if (SimulateAssetBundleInEditor)
                        _hotFixDLL = Streaming + "dll/HotFix.dll";
                    else
                        _hotFixDLL = ABPersistentPath + "dll/HotFix.dll";
#else
                    _hotFixDLL = ABPersistentPath + "dll/HotFix.dll";
#endif
                }

                return _hotFixDLL;
            }
        }

        private static string _hotFixPDB;
        public static string HotFixPDB
        {
            get
            {
                if (string.IsNullOrEmpty(_hotFixPDB))
                {
#if UNITY_EDITOR
                    if (SimulateAssetBundleInEditor)
                        _hotFixPDB = Streaming + "dll/HotFix.pdb";
                    else
                        _hotFixPDB = ABPersistentPath + "dll/HotFix.pdb";
#else
                    _hotFixPDB = ABPersistentPath + "dll/HotFix.pdb";
#endif
                }
                return _hotFixPDB;
            }
        }

        /// <summary>
        /// 数据表的AB
        /// </summary>
        public const string DATA_AB_NAME = "data";

        /// <summary>
        /// 配置文件的AB
        /// </summary>
        public const string SETTING_AB_NAME = "settings";

        /// <summary>
        /// UIPrefab的AB
        /// </summary>
        public const string UI_PREFAB_AB_NAME = "prefab/ui/";

        /// <summary>
        /// 游戏配置表名
        /// </summary>
        public const string GAME_SETTING = "GameSetting";

        /// <summary>
        /// 数据表配置名
        /// </summary>
        public const string TABLE_ITEMS = "TableItems";

        /// <summary>
        /// 获取平台名
        /// </summary>
        /// <param name="platform">当前运行平台</param>
        /// <returns></returns>
        public static string GetPlatformName(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WebGLPlayer:
                    return "Web";
                case RuntimePlatform.PS4:
                    return "PS4";
                case RuntimePlatform.Switch:
                    return "Switch";
                case RuntimePlatform.tvOS:
                    return "TvOS";
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return "WSAPlayer";
                case RuntimePlatform.XboxOne:
                    return "XboxOne";
            }

            return "Windows";
        }

#if UNITY_EDITOR
        /// <summary>
        /// 获取平台名
        /// </summary>
        /// <param name="buildTarget">打包平台类型</param>
        /// <returns></returns>
        public static string GetPlatformName(UnityEditor.BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return "Windows";
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return "OSX";
                case UnityEditor.BuildTarget.StandaloneLinux64:
                    return "Linux";
                case UnityEditor.BuildTarget.iOS:
                    return "IOS";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.WebGL:
                    return "Web";
                case UnityEditor.BuildTarget.PS4:
                    return "PS4";
                case UnityEditor.BuildTarget.Switch:
                    return "Switch";
                case UnityEditor.BuildTarget.tvOS:
                    return "TvOS";
                case UnityEditor.BuildTarget.WSAPlayer:
                    return "WSAPlayer";
                case UnityEditor.BuildTarget.XboxOne:
                    return "XboxOne";
            }

            return "Windows";
        }
#endif
    }
}
