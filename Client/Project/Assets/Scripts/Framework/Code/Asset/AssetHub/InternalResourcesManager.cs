using System;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Linq;

namespace Framework.Resource
{
    /// <summary>
    /// 内部Resources文件夹下的资源管理
    /// </summary>
    public class InternalResourcesManager : Singletone<InternalResourcesManager>, IDisposable
    {
        private string _loadAssetTag = "ResourceManagerLoadAsset";
        private int _capacity = 32;

        private Dictionary<string, UObject> _resources;

        /// <summary>
        /// 
        /// </summary>
        public InternalResourcesManager()
        {
            _resources = new Dictionary<string, UObject>(_capacity);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadAsset<T>(string assetPath, string assetName, Action<T> callback) where T : UObject
        {
            if (!string.IsNullOrEmpty(assetPath) && !assetPath.EndsWith("/"))
                assetPath += "/";
            assetPath += assetName;

            Log.Info("ResourceManger:" + "Start Load " + assetPath);

            if (_resources.ContainsKey(assetPath))
            {
                T result = _resources[assetPath] as T;
                if (result == null)
                    Log.Error("ResourceManger:" + "Load " + assetPath + " failed! Asset is not type " + typeof(T));
                else
                {
                    if (callback != null)
                        callback(result);
                    Log.Info("ResourceManger:" + "Load " + assetPath + " success");
                }
                return;
            }

            CoroutineManager.RunCoroutine(LoadAssetInternal(assetPath, callback), _loadAssetTag);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetName"></param>
        public void UnLoadAsset(string assetPath, string assetName)
        {
            if (!assetPath.EndsWith("/"))
                assetPath += "/";
            assetPath += assetName;

            if (!_resources.ContainsKey(assetPath) || _resources[assetPath] == null) return;

            Resources.UnloadAsset(_resources[assetPath]);

            _resources.Remove(assetPath);
        }

        /// <summary>
        /// 卸载无用的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();

            _resources.RemoveAll(item => !item.Value);
        }

        private IEnumerator<float> LoadAssetInternal<T>(string assetPath, Action<T> callback) where T : UObject
        {
            ResourceRequest request = Resources.LoadAsync<T>(assetPath);

            yield return CoroutineManager.WaitUntilDone(request);

            T result = request.asset as T;
            if (result == null)
                Log.Error("ResourceManager:" + "Load " + assetPath + " error!");
            else
            {
                Log.Info("ResourceManger:" + "Load " + assetPath + " success!");
                if (callback != null)
                    callback(result);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Dispose()
        {
            CoroutineManager.KillCoroutines(_loadAssetTag);

            foreach (var obj in _resources.Values)
            {
                if (!obj) continue;
                Resources.UnloadAsset(obj);
            }

            _resources.Clear();
        }
    }
}
