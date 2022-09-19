using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

public class UObjectPool<T> : BasePool where T : UObject
{
    private readonly Stack<T> _idles;
    private readonly List<T> _usings;

    private Transform _poolTrans;

    public int Idle { get { return _idles.Count; } }
    public int Using { get { return _usings.Count; } }

    public UObjectPool(Transform poolTrans, int capacity = 0) : base(capacity)
    {
        _idles = new Stack<T>(_capacity);
        _usings = new List<T>(_capacity);
        _poolTrans = poolTrans;
    }

    public T Get(T matrix, Transform parent = null)
    {
        parent = parent ?? GetTransform(matrix).parent;
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
            T result = GameObject.Instantiate(matrix, parent);
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

        var trans = GetTransform(target);
        trans.gameObject.SetActive(false);
        if (_poolTrans)
            trans.SetParent(_poolTrans);
    }

    public void Init(T target, T matrix, Transform parent)
    {
        var ttrans = GetTransform(target);
        var mtrans = GetTransform(matrix);
        target.name = matrix.name;
        ttrans.SetParent(parent);
        ttrans.localPosition = mtrans.localPosition;
        ttrans.localRotation = mtrans.localRotation;
        ttrans.localScale = mtrans.localScale;
    }

    public Transform GetTransform(T target)
    {
        if (typeof(T) == typeof(GameObject))
            return (target as GameObject).transform;
        else
            return (target as Component).transform;
    }
}

/// <summary>
/// 所有 GameObject 对象池的管理
/// </summary>
public class PoolManager : Manager<PoolManager>
{
    private const int CAPACITY = 16;

    private Dictionary<UObject, BasePool> _gameObjectPools;

    protected override void Init()
    {
        base.Init();

        _gameObjectPools = new Dictionary<UObject, BasePool>();
    }

    public T Get<T>(T matrix, Transform parent, int capacity = 0) where T : UObject
    {
        if (matrix == null)
            throw new Exception("母体不能为空!");

        if (_gameObjectPools.SafeGet(matrix) is UObjectPool<T> pool)
            return pool.Get(matrix);

        pool = new UObjectPool<T>(parent, capacity);
        _gameObjectPools.Add(matrix, pool);

        return pool.Get(matrix);
    }

    public void Recycle<T>(T target) where T : UObject
    {
        if (!(_gameObjectPools.SafeGet(target) is UObjectPool<T> pool))
            return;

        pool.Recycle(target);
    }
}

