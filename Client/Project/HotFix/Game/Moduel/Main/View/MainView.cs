using HotFix.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HotFix.Base.Data;

namespace HotFix.Game
{
    [UIPanel("main/mainview", "MainView", (int)DisplayLevel.UI)]
    public class MainView : BaseView
    {
        public class MainItem : UIComponent
        {
            [UIField("Text")]
            private UIText _contentTxt;

            private Image _backImg;

            protected override void Awake()
            {
                base.Awake();
                _backImg = gameObject.GetComponent<Image>();
            }

            public void SetData(Weapon weapon)
            {
                _contentTxt.SetText(weapon.name);
            }
        }

        [UIField("ScrollRect")]
        private LoopVerticalScrollRect _scrollRect;
        [UIField("ScrollRect1")]
        private LoopHorizontalScrollRect _scrollRect1;
        [UIField("Item")]
        private GameObject _itemMatrix;
        [UIField("Item1")]
        private GameObject _itemMatrix1;

        private UIComponentLoopItem<MainItem, Weapon> _scrollRectHelper;
        private UIComponentLoopItem<MainItem, Weapon> _scrollRectHelper1;

        protected override void Awake()
        {
            base.Awake();

            _scrollRectHelper = new UIComponentLoopItem<MainItem, Weapon>(_itemMatrix, _scrollRect, transform)
            {
                GetDataByIndex = index => DataManager.GetByIndex<Weapon>(index),
                SetDataAction = (item, data) => item.SetData(data)
            };
            _scrollRectHelper1 = new UIComponentLoopItem<MainItem, Weapon>(_itemMatrix1, _scrollRect1, transform)
            {
                GetDataByIndex = index => DataManager.GetByIndex<Weapon>(index),
                SetDataAction = (item, data) => item.SetData(data)
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _scrollRectHelper.Refresh(DataManager.GetTable<Weapon>().Count, LoopScrollRectRefreshType.Refill, false);
            _scrollRectHelper1.Refresh(DataManager.GetTable<Weapon>().Count, LoopScrollRectRefreshType.Refill, false);
        }

        protected override void OnBindListener()
        {
            //var back = gameObject.QuickGetComponent<UIImage>();
            //back[TriggerType.Click] = data =>
            //{
            //    Hide();
            //};
        }
    }
}
