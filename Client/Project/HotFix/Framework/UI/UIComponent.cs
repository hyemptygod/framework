using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// UI组件
    /// </summary>
    public partial class UIComponent
    {
        /// <summary>
        /// Mono组件工具类
        /// </summary>
        public class UIComponentView : MonoBehaviour
        {
            /// <summary>
            /// 
            /// </summary>
            public UIComponent Object { get; set; }

            protected void Start()
            {
                Object.Start();
            }

            private void Update()
            {
                Object.Update();
            }

            private void OnDestroy()
            {
                Object.OnDestory();
            }
        }

        /// <summary>
        /// Transform组件
        /// </summary>
        public RectTransform rectTransform { get; private set; }
        /// <summary>
        /// GameObject组件
        /// </summary>
        public GameObject gameObject { get; private set; }
        /// <summary>
        /// Transform 组件
        /// </summary>
        public Transform transform { get; private set; }

        public bool HasCreateed { get; private set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShowing { get; private set; }

        /// <summary>
        /// Mono组件对象
        /// </summary>
        protected UIComponentView _component;

        public bool UpdateEnable { get; protected set; }

        private CoroutineHandle _disposeHandle;

        public UIComponent()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="go"></param>
        public UIComponent(GameObject go)
        {
            Init(go);
        }

        public void Init(GameObject go)
        {
            gameObject = go;
            rectTransform = go.GetComponent<RectTransform>();
            transform = go.transform;

            _component = go.AddComponent<UIComponentView>();
            _component.Object = this;

            Awake();
        }

        /// <summary>
        /// Awake(new 新的对象时)
        /// </summary>
        protected virtual void Awake()
        {
            ParseUIFields();

            HasCreateed = true;

            IsShowing = gameObject.activeInHierarchy;

            OnBindListener();
        }

        /// <summary>
        /// 每次打开的时候执行
        /// </summary>
        protected virtual void OnEnable()
        {
            CoroutineManager.KillCoroutines(_disposeHandle);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 第一次打开的时候执行，只执行一次
        /// </summary>
        protected virtual void Start()
        {

        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// 每次关闭的时候执行
        /// </summary>
        protected virtual void OnDisable()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnDestory()
        {
            gameObject = null;
            transform = null;
            rectTransform = null;
        }

        protected virtual void OnBindListener()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActive(bool active)
        {
            IsShowing = active;

            if (active)
                OnEnable();
            else
                OnDisable();
        }

        public void Dispose(float delay)
        {
            CoroutineManager.KillCoroutines(_disposeHandle);
            _disposeHandle = CoroutineManager.RunCoroutine(DisposeHandle(delay));
        }

        private IEnumerator<float> DisposeHandle(float delay)
        {
            yield return CoroutineManager.WaitForSeconds(delay);

            GameObject.DestroyImmediate(gameObject);
        }
    }
}
