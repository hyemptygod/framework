using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UObject = UnityEngine.Object;

namespace Framework.Resource
{
    /// <summary>
    /// Loaded assetBundle contains the references count which can be used to
    /// unload dependent assetBundles automatically.
    /// </summary>
    public class LoadedAssetBundle
    {
        public AssetBundle assetBundle;
        public int referencedCount;

        internal event Action Unload;

        internal void OnUnload()
        {
            assetBundle.Unload(false);
            Unload?.Invoke();
        }

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            referencedCount = 1;
        }
    }

    /// <summary>
    /// Class takes care of loading assetBundle and its dependencies
    /// automatically, loading variants automatically.
    /// </summary>
    public class AssetBundleManager : Singletone<AssetBundleManager>
    {
        private const string LOAD_ASSET_TAG = "AssetBundleManagerLoadAsset";
        private const int CAPACITY = 64;

        private AssetBundleManifest m_AssetBundleManifest = null;

        private readonly Dictionary<string, LoadedAssetBundle> _loadedAssetBundles;

        private readonly Dictionary<string, string> _downloadingErrors;

        private readonly List<string> _downloadingBundles;

        private readonly List<AssetBundleLoadOperation> _inProgressOperations;

        private readonly Dictionary<string, string[]> _dependencies;

        private SortedDictionary<string, List<string>> _bundlesWithVariant;

        private string[] _activeVariants;

        /// <summary>
        /// Manifest
        /// </summary>
        public AssetBundleManifest AssetBundleManifest
        {
            get { return m_AssetBundleManifest; }
            set { m_AssetBundleManifest = value; }
        }

        public bool InitializeSuccess { get; set; }

        /// <summary>
        /// Initializes asset bundle namager and starts download of manifest asset bundle.
        /// Returns the manifest asset bundle downolad operation object.
        /// </summary>
        public AssetBundleManager()
        {
            _loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>(CAPACITY);
            _downloadingErrors = new Dictionary<string, string>(CAPACITY / 4);
            _downloadingBundles = new List<string>(CAPACITY / 4);
            _inProgressOperations = new List<AssetBundleLoadOperation>(CAPACITY / 4);
            _dependencies = new Dictionary<string, string[]>(CAPACITY);

#if UNITY_EDITOR
            if (PathConst.SimulateAssetBundleInEditor)
            {
                InitializeSuccess = true;
                return;
            }
#endif
            LoadManifest(PathConst.Platform);
        }

        public void Update()
        {
            // Update all in progress operations
            for (int i = 0; i < _inProgressOperations.Count;)
            {
                var operation = _inProgressOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    _inProgressOperations.RemoveAt(i);
                    ProcessFinishedOperation(operation);
                }
            }
        }

        void ProcessFinishedOperation(AssetBundleLoadOperation operation)
        {
            if (!(operation is AssetBundleDownloadOperation download))
                return;

            if (string.IsNullOrEmpty(download.error))
                _loadedAssetBundles.Add(download.assetBundleName, download.assetBundle);
            else
            {
                string msg = string.Format("Failed downloading bundle {0} from {1}: {2}",
                        download.assetBundleName, download.GetSourceURL(), download.error);
                _downloadingErrors.Add(download.assetBundleName, msg);
            }

            _downloadingBundles.Remove(download.assetBundleName);
        }

        private void LoadManifest(string name)
        {
            UnityWebRequest download = UnityWebRequestAssetBundle.GetAssetBundle(PathConst.ABPersistentPath + name);
            _inProgressOperations.Add(new AssetBundleDownloadFromWebOperation(name, download));
            _downloadingBundles.Add(name);
            _inProgressOperations.Add(new AssetBundleLoadManifestOperation(name, "AssetBundleManifest", typeof(AssetBundleManifest)));
        }

        /// <summary>
        /// Retrieves an asset bundle that has previously been requested via LoadAssetBundle.
        /// Returns null if the asset bundle or one of its dependencies have not been downloaded yet.
        /// </summary>
        public LoadedAssetBundle GetLoadedAssetBundle(string abName, out string error)
        {
            if (_downloadingErrors.TryGetValue(abName, out error))
                return null;

            _loadedAssetBundles.TryGetValue(abName, out LoadedAssetBundle bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            if (!_dependencies.TryGetValue(abName, out string[] dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (_downloadingErrors.TryGetValue(dependency, out error))
                    return null;

                // Wait all the dependent assetBundles being loaded.
                _loadedAssetBundles.TryGetValue(dependency, out LoadedAssetBundle dependentBundle);

                if (dependentBundle == null)
                    return null;
            }

            return bundle;
        }


        // Starts the download of the asset bundle identified by the given name, and asset bundles
        // that this asset bundle depends on.
        private void LoadAssetBundle(string abName)
        {
            if (m_AssetBundleManifest == null)
            {
                Log.Error("Manifest has not loaded....");
                return;
            }

            // first load need to load dependencies.
            if (!LoadAssetBundleInternal(abName))
                LoadDependencies(abName);
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        private string RemapVariantName(string abName)
        {
            if (_activeVariants == null || _activeVariants.Length <= 0)
                return abName;

            if (_bundlesWithVariant == null)
            {
                _bundlesWithVariant = new SortedDictionary<string, List<string>>();

                string[] assets = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

                for (int i = 0; i < assets.Length; i++)
                {
                    string[] split = assets[i].Split('.');
                    if (!_bundlesWithVariant.ContainsKey(split[0]))
                    {
                        _bundlesWithVariant.Add(split[0], new List<string>());
                    }
                    _bundlesWithVariant[split[0]].Add(split[1]);
                }
            }

            string baseName = abName.Split('.')[0];

            if (!_bundlesWithVariant.TryGetValue(baseName, out List<string> variants))
            {
                return abName;
            }

            for (int i = 0; i < _activeVariants.Length; i++)
            {
                if (!variants.Contains(_activeVariants[i]))
                    continue;
                return baseName + "." + variants[i];
            }

            return baseName + "." + variants[0];
        }

        // Sets up download operation for the given asset bundle if it's not downloaded already.
        private bool LoadAssetBundleInternal(string abName)
        {
            // Already loaded.
            _loadedAssetBundles.TryGetValue(abName, out LoadedAssetBundle bundle);
            if (bundle != null)
            {
                bundle.referencedCount++;
                return true;
            }
            if (_downloadingBundles.Contains(abName))
                return true;
            string url = PathConst.ABPersistentPath + abName;
            UnityWebRequest download = UnityWebRequestAssetBundle.GetAssetBundle(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
            _inProgressOperations.Add(new AssetBundleDownloadFromWebOperation(abName, download));
            _downloadingBundles.Add(abName);
            return false;
        }

        // Where we get all the dependencies and load them all.
        private void LoadDependencies(string abName)
        {
            if (m_AssetBundleManifest == null)
            {
                Log.Error("Manifest has not loaded....");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            _dependencies.Add(abName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
                LoadAssetBundleInternal(dependencies[i]);
        }


        /// <summary>
        /// Load Asset (打开 SimulateAssetBundles 时会从编辑器中加载资源)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadAsset<T>(string abName, string assetName, Action<T> callback) where T : UObject
        {
#if UNITY_EDITOR
            if (PathConst.SimulateAssetBundleInEditor)
            {
                string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abName, assetName);
                if (assetPaths.Length == 0)
                {
                    Log.Error("AssetBundleManger : Load " + assetName + " from " + abName + " error");
                    return;
                }
                T result = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[0]) as T;
                if (result == null)
                    Log.Error("AssetBundleManger : Load " + assetName + " from " + abName + " error");
                else
                {
                    Log.Info("AssetBundleManger : Load " + assetName + " from " + abName + " success");
                    callback?.Invoke(result);
                }

                return;
            }
#endif
            Log.Info("AssetBundleManger:" + "Start Load " + abName + ":" + assetName);
            CoroutineManager.RunCoroutine(LoadAssetInternal(abName, assetName, callback), LOAD_ASSET_TAG);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator<float> LoadAssetInternal<T>(string abName, string assetName, Action<T> callback) where T : UObject
        {
            // 等待Manifest加载完成
            while (m_AssetBundleManifest == null)
                yield return CoroutineManager.WaitForOneFrame;

            abName = RemapVariantName(abName);
            LoadAssetBundle(abName);

            LoadedAssetBundle bundle = null;
            string error = null;

            while (bundle == null && string.IsNullOrEmpty(error))
            {
                bundle = GetLoadedAssetBundle(abName, out error);
                yield return CoroutineManager.WaitForOneFrame;
            }

            if (!string.IsNullOrEmpty(error))
            {
                Log.Error(error);
                yield break;
            }

            if (bundle == null)
                yield break;

            AssetBundleRequest request = bundle.assetBundle.LoadAssetAsync<T>(assetName);

            yield return CoroutineManager.WaitUntilDone(request);

            T result = request.asset as T;

            if (result == null)
                Log.Error("AssetBundleManger : Load " + assetName + " from " + abName + " error");
            else
            {
                Log.Info("AssetBundleManger : Load " + assetName + " from " + abName + " success");
                callback?.Invoke(result);
            }
        }

        /// <summary>
        /// Starts a load operation for a level from the given asset bundle.
        /// </summary>
        public AssetBundleLoadOperation LoadLevelAsync(string abName, string levelName, bool isAdditive)
        {
            Log.Info("Loading " + levelName + " from " + abName + " bundle");

            abName = RemapVariantName(abName);
            LoadAssetBundle(abName);
            AssetBundleLoadOperation operation = new AssetBundleLoadLevelOperation(abName, levelName, isAdditive);

            _inProgressOperations.Add(operation);

            return operation;
        }

        /// <summary>
        /// Unloads assetbundle and its dependencies.
        /// </summary>
        public void UnloadAssetBundle(string abName)
        {
            abName = RemapVariantName(abName);

            UnloadAssetBundleInternal(abName);
            UnloadDependencies(abName);
        }

        public void UnloadUnusedAssetBundle()
        {
            foreach (string item in _loadedAssetBundles.Keys)
            {
                UnloadAssetBundle(item);
            }
        }

        private void UnloadDependencies(string abName)
        {
            if (!_dependencies.TryGetValue(abName, out string[] dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            _dependencies.Remove(abName);
        }

        private void UnloadAssetBundleInternal(string abName)
        {
            LoadedAssetBundle bundle = GetLoadedAssetBundle(abName, out string error);
            if (bundle == null)
                return;

            if (--bundle.referencedCount == 0)
            {
                bundle.OnUnload();
                _loadedAssetBundles.Remove(abName);

                Log.Info(abName + " has been unloaded successfully");
            }
        }
    }
}
