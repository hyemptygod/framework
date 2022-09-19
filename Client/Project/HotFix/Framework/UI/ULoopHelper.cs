using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace HotFix
{
    public enum LoopScrollRectRefreshType
    {
        Refresh,
        Refill,
        RefillOneByOne,
    }

    public abstract class LoopItemBase<TObject, TData>
    {
        protected TObject _matrix;
        protected LoopScrollRect _scrollRect;

        protected Transform _poolItemParent;

        public Func<int, TData> GetDataByIndex { get; set; }
        public Action<TObject, TData> SetDataAction { get; set; }

        public LoopItemBase(LoopScrollRect scrollRect, Transform poolItemParentParent)
        {
            _scrollRect = scrollRect;

            _poolItemParent = new GameObject("ItemPools").transform;
            _poolItemParent.SetParent(poolItemParentParent);
            _poolItemParent.gameObject.SetActive(false);

            _scrollRect.getObjectDelegate = GetObject;
            _scrollRect.returnObjectDelegate = Recyle;
        }

        public LoopItemBase(TObject matrix, LoopScrollRect scrollRect, Transform poolItemParentParent)
        {
            _matrix = matrix;
            _scrollRect = scrollRect;

            _poolItemParent = new GameObject("ItemPools").transform;
            _poolItemParent.SetParent(poolItemParentParent);
            _poolItemParent.gameObject.SetActive(false);

            _scrollRect.getObjectDelegate = GetObject;
            _scrollRect.returnObjectDelegate = Recyle;
        }

        private GameObject GetObject()
        {
            var obj = GetObjectFromPool();
            var trans = GetObjectTransform(obj);
            var callback = trans.gameObject.QuickGetComponent<ScrollIndexCallback>();
            callback.item = obj;
            callback.scrollCellIndex = ScrollCellIndex;
            return trans.gameObject;
        }

        protected void ScrollCellIndex(object o, int index)
        {
            if (index < 0 || SetDataAction == null || GetDataByIndex == null)
                return;

            var data = GetDataByIndex(index);
            if (data == null)
                return;
            SetDataAction((TObject)o, data);
        }

        public void Refresh(int count, LoopScrollRectRefreshType type, bool fromEnd, int offset = 0)
        {
            _scrollRect.totalCount = count;

            switch (type)
            {
                case LoopScrollRectRefreshType.Refill:
                    _scrollRect.RefillCells(fromEnd, offset);
                    break;
                case LoopScrollRectRefreshType.Refresh:
                    _scrollRect.RefreshCells();
                    break;
                case LoopScrollRectRefreshType.RefillOneByOne:
                    Log.Warn("请使用 RefreshOneByOne 方法");
                    _scrollRect.RefillCells(fromEnd, offset);
                    break;
            }
        }

        public void RefreshOneByOne(int count, bool fromEnd, Action callback, float interval = 0, int offset = 0)
        {
            _scrollRect.totalCount = count;

            _scrollRect.RefillCellsOneByOne(fromEnd, callback, interval, offset);
        }

        protected abstract TObject GetObjectFromPool();
        protected abstract Transform GetObjectTransform(TObject target);
        protected abstract void Recyle(Transform tf);
    }

    public class UIComponentLoopItem<TObject, TData> : LoopItemBase<TObject, TData> where TObject : UIComponent, new()
    {
        private GameObject _matrixGo;
        private UIComponentPool<TObject> _pool;

        public UIComponentLoopItem(GameObject matrix, LoopScrollRect scrollRect, Transform poolItemParentParent) : base(scrollRect, poolItemParentParent)
        {
            _matrixGo = matrix;
            _pool = new UIComponentPool<TObject>(_poolItemParent);
        }

        protected override TObject GetObjectFromPool()
        {
            return _pool.Get(_matrixGo, _scrollRect.content);
        }

        protected override Transform GetObjectTransform(TObject target)
        {
            return target.transform;
        }

        protected override void Recyle(Transform tf)
        {
            _pool.Recycle(tf.GetComponent<ScrollIndexCallback>().item as TObject);
        }
    }

    public class ULoopItem<TObject, TData> : LoopItemBase<TObject, TData> where TObject : UObject
    {
        private UObjectPool<TObject> _pool;

        public ULoopItem(TObject matrix, LoopScrollRect scrollRect, Transform poolItemParentParent) : base(matrix, scrollRect, poolItemParentParent)
        {
            _pool = new UObjectPool<TObject>(_poolItemParent);
        }

        protected override TObject GetObjectFromPool()
        {
            return _pool.Get(_matrix, _scrollRect.content);
        }

        protected override Transform GetObjectTransform(TObject target)
        {
            return _pool.GetTransform(target);
        }

        protected override void Recyle(Transform tf)
        {
            _pool.Recycle(tf.GetComponent<ScrollIndexCallback>().item as TObject);
        }
    }
}
