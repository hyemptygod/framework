using System;
using System.Collections.Generic;
using Framework.Resource;
using Framework.Core;
using UnityEngine;
using HotFix.Base;
using Framework;

namespace HotFix
{
    public class UIManager : HotFixSingletone<UIManager>
    {
        private Dictionary<string, UIPanel> _loadedPanel = new Dictionary<string, UIPanel>();
        private List<UIPanel> _showPanel = new List<UIPanel>();

        public UIManager()
        {
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <typeparam name="TPanel"></typeparam>
        public TPanel ShowPanel<TPanel>() where TPanel : UIPanel, new()
        {
            var t = typeof(TPanel);
            UIPanel panel = null;
            if (!_loadedPanel.TryGetValue(t.FullName, out panel) || panel == null)
            {
                panel = new TPanel();
                _loadedPanel.Add(t.FullName, panel);
                LoadPanel(panel);
            }
            else
            {
                ShowPanelHandle(panel);
            }

            return panel as TPanel;
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <typeparam name="TPanel"></typeparam>
        public void HidePanel<TPanel>(float disposeDelay = -1) where TPanel : UIPanel, new()
        {
            var t = typeof(TPanel);
            UIPanel panel = null;
            if (!_loadedPanel.TryGetValue(t.FullName, out panel) || panel == null)
                return;
            if (_showPanel.Contains(panel))
                _showPanel.Remove(panel);
            panel.SetActive(false);
            if (disposeDelay >= 0)
                panel.Dispose(disposeDelay);
        }

        private void ShowPanelHandle(UIPanel panel)
        {
            panel.SetActive(true);

            if (!_showPanel.Contains(panel))
                _showPanel.Add(panel);
        }

        private void LoadPanel(UIPanel panel)
        {
            var paneltype = panel.GetType();
            var attributes = paneltype.GetCustomAttributes(typeof(UIPanelAttribute), false);
            if (attributes == null || attributes.Length <= 0)
            {
                Log.Error("请使用UIPanelAttribute标记类：" + paneltype.FullName);
                return;
            }
            var panelatt = attributes[0] as UIPanelAttribute;
            var abname = PathConst.UI_PREFAB_AB_NAME + panelatt.ABName;
            var assetname = panelatt.AssetName;
            var level = (DisplayLevel)panelatt.Level;
            ResourceManager.Instance.LoadAsset<GameObject>(abname, assetname, obj =>
            {
                if (obj == null)
                {
                    Log.Error("不存在Assetbundle资源：" + PathConst.UI_PREFAB_AB_NAME + abname);
                    return;
                }
                var go = Get(obj, level);
                if (go == null)
                    return;
                panel.Init(go);
                ShowPanelHandle(panel);
            });
        }

        private GameObject Get(GameObject matrix, DisplayLevel level)
        {
            try
            {
                var parent = Framework.Core.UIManager.Instance.GetSortCanvas(level);
                var go = GameObject.Instantiate(matrix, parent);
                go.transform.localPosition = matrix.transform.localPosition;
                go.transform.localScale = matrix.transform.localScale;
                go.transform.localRotation = matrix.transform.localRotation;
                return go;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return null;
            }
        }
    }
}
