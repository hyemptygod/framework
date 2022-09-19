using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    public class UIComponentPool<T> : BasePool where T : UIComponent, new()
    {
        private readonly Stack<T> _idles;
        private readonly List<T> _usings;

        private Transform _poolTrans;

        public int Idle { get { return _idles.Count; } }
        public int Using { get { return _usings.Count; } }

        public UIComponentPool(Transform poolTrans, int capacity = 0) : base(capacity)
        {
            _idles = new Stack<T>(_capacity);
            _usings = new List<T>(_capacity);
            _poolTrans = poolTrans;
        }

        public T Get(GameObject matrix, Transform parent = null)
        {
            parent = parent ?? matrix.transform.parent;
            if (Idle > 0)
            {
                T result = _idles.Pop();
                if (result != null)
                {
                    Init(result, matrix, parent);
                    _usings.Add(result);
                    return result;
                }
            }

            if (_capacity == 0 || Using < _capacity)
            {
                T result = new T();
                result.Init(GameObject.Instantiate(matrix, parent));
                Init(result, matrix, parent);
                return result;
            }

            T target = _usings[0];
            _usings.RemoveAt(0);
            return target;
        }

        public void Recycle(T target)
        {
            if (target == null) return;

            _usings.Remove(target);
            _idles.Push(target);

            target.SetActive(false);
            if (_poolTrans)
                target.transform.SetParent(_poolTrans);
        }

        public void Init(T target, GameObject matrix, Transform parent)
        {
            target.transform.SetParent(parent);
            target.transform.localPosition = matrix.transform.localPosition;
            target.transform.localRotation = matrix.transform.localRotation;
            target.transform.localScale = matrix.transform.localScale;
        }
    }
}
