using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePool
{
    protected int _capacity;

    public BasePool(int capacity = 0)
    {
        _capacity = capacity;

        if (_capacity < 0)
        {
            throw new System.Exception("对象池容量不能小于0");
        }
    }
}

public class Pool<T> : BasePool
{
    private readonly Stack<T> _idles;
    private readonly List<T> _usings;

    public int Idle { get { return _idles.Count; } }
    public int Using { get { return _usings.Count; } }

    private Func<T> _createAction;
    private Action<T> _initAction;
    private Action<T> _recyleAction;

    public Pool(Func<T> createAction, Action<T> initAction, Action<T> recyleAction, int capacity = 0) : base(capacity)
    {
        _createAction = createAction;
        _initAction = initAction;
        _recyleAction = recyleAction;

        if (_createAction == null)
            throw new System.Exception("创建目标对象的方法不能为空");

        _idles = new Stack<T>(_capacity);
        _usings = new List<T>(_capacity);
    }

    public T Get(T matrix)
    {
        if (Idle > 0)
        {
            T result = _idles.Pop();
            if (result != null)
            {
                if (_initAction != null)
                    _initAction(result);
                _usings.Add(result);
                return result;
            }
        }

        if (_capacity == 0 || Using < _capacity)
        {
            T result = _createAction();
            if (_initAction != null)
                _initAction(result);
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

        if (_recyleAction != null)
            _recyleAction(target);
    }
}

