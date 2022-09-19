using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Core
{
    public class ILRuntimeMgr : Manager<ILRuntimeMgr>
    {
        public bool Inited { get; private set; }

        private MemoryStream _dll;
        private MemoryStream _pdb;
        private AppDomain _appdomain;

        /// <summary>
        /// 热更新中的Start
        /// </summary>
        private IMethod _hotfixStart;

        /// <summary>
        /// 热更新中的Update
        /// </summary>
        private IMethod _hotfixUpdate;


        protected override void Init()
        {
            base.Init();
            _appdomain = null;
            _dll = null;
            _pdb = null;
            Inited = false;
            CoroutineManager.RunCoroutine(LoadHotFixAssembly(), "LoadHotFixAssembly");
        }

        private IEnumerator<float> LoadHotFixAssembly()
        {
            var timer = new Timer("ILRuntime Initializel Complete");

            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            _appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            yield return CoroutineManager.WaitUntilDone(FileUtil.ReadBytesByRequest(PathConst.HotFixDLL, dll =>
            {
                _dll = new MemoryStream(dll);
            }));

            if (_dll == null)
                yield break;

#if !DISABLE_ILRUNTIME_DEBUG
            if (Application.isEditor)
            {
                yield return CoroutineManager.WaitUntilDone(FileUtil.ReadBytesByRequest(PathConst.HotFixPDB, pdb =>
                {
                    _pdb = new MemoryStream(pdb);
                }));
            }
#endif
            _appdomain.LoadAssembly(_dll, _pdb, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

            if (Application.isEditor)
                _appdomain.DebugService.StartDebugService(56000);

            InitializeILRuntime();

            timer.Stop();

            OnHotFixLoaded();

            Inited = true;
        }

        /// <summary>
        /// 初始化ILRuntime
        /// </summary>
        private void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            _appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

            ILRTHelper.Run(_appdomain);

            CLRBindings.Initialize(_appdomain);
        }

        /// <summary>
        /// 加载热更中的方法
        /// </summary>
        private void OnHotFixLoaded()
        {
            _hotfixStart = _appdomain.LoadedTypes["HotFix.App"].GetMethod("Start", 1);
            _hotfixUpdate = _appdomain.LoadedTypes["HotFix.App"].GetMethod("Update", 0);

            var types = new List<System.Type>();
            foreach (var it in _appdomain.LoadedTypes.Values)
            {
                types.Add(it.ReflectionType);
            }
            _appdomain.Invoke(_hotfixStart, null, new object[] { types.ToArray() });
        }

        void Update()
        {
            if (!Inited || _appdomain == null)
                return;

            _appdomain.Invoke(_hotfixUpdate, null);
        }

        internal void Dispose()
        {
            Component.Destroy(this);
        }

        private void OnDestroy()
        {
            _appdomain = null;
        }
    }
}


