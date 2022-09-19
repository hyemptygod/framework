using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

namespace Framework.Core
{
    public class UIManager : Manager<UIManager>
    {
        private Canvas _mainCanvas;
        private CanvasScaler _mainCanvasScale;
        private Dictionary<DisplayLevel, Canvas> _sortCanvas = new Dictionary<DisplayLevel, Canvas>();

        private Canvas _sceneCanvas;

        protected override void Init()
        {
            base.Init();

            _mainCanvas = GetComponent<Canvas>();
            _mainCanvasScale = GetComponent<CanvasScaler>();

            var trans = _mainCanvas.transform;
            foreach (DisplayLevel v in System.Enum.GetValues(typeof(DisplayLevel)))
            {
                try
                {
                    _sortCanvas.Add(v, trans.Find(v.ToString()).GetComponent<Canvas>());
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("不存在{0}的canvas层", v.ToString()) + "\n" + e.ToString());
                    continue;
                }
            }

            _sceneCanvas = GameObject.FindWithTag("SceneCanvas").GetComponent<Canvas>();

            ScreenAdaptation();
        }

        /// <summary>
        /// 屏幕自适应
        /// </summary>
        private void ScreenAdaptation()
        {
            var defaultaspect = _mainCanvasScale.referenceResolution.x / _mainCanvasScale.referenceResolution.y;
            var curaspect = Screen.width / Screen.height;
            if (curaspect < defaultaspect)
                _mainCanvasScale.matchWidthOrHeight = 0;
            else
                _mainCanvasScale.matchWidthOrHeight = 1;

            //刘海屏幕处理
        }

        public RectTransform GetSortCanvas(DisplayLevel level)
        {
            var canvas = _sortCanvas.SafeGet(level);
            if (canvas == null)
                return null;

            return canvas.transform as RectTransform;
        }
    }
}
