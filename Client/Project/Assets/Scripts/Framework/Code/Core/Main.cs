using UnityEngine;
using Framework.Start;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private bool _checkAssetVersion;
        [SerializeField]
        private DownloadNewAssetView _downloadView;

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        void Awake()
        {
            // 设置Log
            Log.mode = Log.LogMode.Unity;
            Log.level = Log.LogType.Info | Log.LogType.Warn | Log.LogType.Error;

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        void Start()
        {
            if (!_checkAssetVersion || !_downloadView)
            {
                Initialize();
                return;
            }

            _downloadView.Callback = Initialize;

            CoroutineManager.RunCoroutine(_downloadView.CheckAsset(), "Download New Asset");
        }

        private void Initialize()
        {
            gameObject.AddComponent<GameManager>();
        }

#if UNITY_EDITOR

        [MenuItem("GameObject/Main", priority = 0)]
        public static void CreateMain()
        {
            var manager = new GameObject("Main").AddComponent<Main>();
            manager.transform.localScale = Vector3.one;
            manager.transform.localPosition = Vector3.zero;
            manager.transform.localRotation = Quaternion.identity;
        }
#endif
    }
}

