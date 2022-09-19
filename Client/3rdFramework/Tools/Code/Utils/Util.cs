using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 通用工具类
/// </summary>
public static class Util
{
    /// <summary>
    /// Cast
    /// </summary>
    /// <typeparam name="T">集合中的元素类型</typeparam>
    /// <typeparam name="V">指定字段的类型</typeparam>
    /// <param name="list">目标集合</param>
    /// <param name="get">取字段值的方法</param>
    /// <returns></returns>
    public static List<V> Cast<T, V>(this IEnumerable<T> list, Func<T, V> get)
    {
        if (list == null || get == null)
            return null;

        var result = new List<V>(list.Count());

        foreach (T item in list)
        {
            if (item == null)
                continue;
            result.Add(get(item));
        }

        return result;
    }

    /// <summary>
    /// 移除字典中的一类数据
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <param name="target"></param>
    /// <param name="predicate"></param>
    public static void RemoveAll<TK, TV>(this Dictionary<TK, TV> target, Func<KeyValuePair<TK, TV>, bool> predicate)
    {
        if (predicate == null) return;

        foreach (var item in target.Where(item => predicate(item)))
        {
            target.Remove(item.Key);
        }
    }

    /// <summary>
    /// 字典安全获得
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue SafeGet<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key)
    {
        if (dic.TryGetValue(key, out TValue result))
        {
            return result;
        }
        return default(TValue);
    }

    /// <summary>
    /// 字典安全添加
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static void SafeAdd<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue value)
    {
        if (dic.ContainsKey(key))
            dic[key] = value;
        else
            dic.Add(key, value);
    }
}
