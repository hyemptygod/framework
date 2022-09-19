using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UObject = UnityEngine.Object;

namespace Framework.Resource
{
    /// <summary>
    /// 资源管理
    /// </summary>
    public class ResourceManager : Manager<ResourceManager>
    {
        /// <summary>
        /// 资源加载模式
        /// </summary>
        public enum LoadType
        {
            /// <summary>
            /// Resource模式加载
            /// </summary>
            Resource,
            /// <summary>
            /// AssetBundle模式加载
            /// </summary>
            AssetBundle
        }

        private AssetBundleManager _assetBundleManager;
        private InternalResourcesManager _resourceManager;

        /// <summary>
        /// 资源管理起初始化成功
        /// </summary>
        public bool InitializeSuccess
        {
            get
            {
                return _assetBundleManager.InitializeSuccess;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            base.Init();

            _assetBundleManager = AssetBundleManager.Instance;
            _resourceManager = InternalResourcesManager.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Update()
        {
            _assetBundleManager.Update();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源目录</param>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <param name="type">加载方式</param>
        public void LoadAsset<T>(string abName, string assetName, Action<T> callback, LoadType type = LoadType.AssetBundle) where T : UObject
        {
            switch (type)
            {
                case LoadType.Resource:
                    _resourceManager.LoadAsset<T>(abName, assetName, callback);
                    break;
                case LoadType.AssetBundle:
                    _assetBundleManager.LoadAsset<T>(abName, assetName, callback);
                    break;
            }
        }

        /// <summary>
        /// 从内存中卸载资源
        /// </summary>
        /// <param name="abName">资源目录</param>
        /// <param name="assetName">资源名</param>
        /// <param name="type"></param>
        public void UnloadAsset(string abName, string assetName, LoadType type = LoadType.AssetBundle)
        {
            switch (type)
            {
                case LoadType.Resource:
                    _resourceManager.UnLoadAsset(abName, assetName);
                    break;
                case LoadType.AssetBundle:
                    _assetBundleManager.UnloadAssetBundle(abName);
                    break;
            }
        }

        /// <summary>
        /// 从内存中移除无用的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            _resourceManager.UnloadUnusedAssets();
            _assetBundleManager.UnloadUnusedAssetBundle();
        }
    }
}
