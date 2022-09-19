using System;
using System.Collections.Generic;
using LitJson;

public static class JsonUtil
{
    /// <summary>
    /// json数据转对象(类型为主工程代码的中的类)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    public static T FromJson<T>(this string content)
    {
        if (string.IsNullOrEmpty(content))
            return default(T);

        return JsonMapper.ToObject<T>(content);
    }

    /// <summary>
    /// json数据转对象(类型为热更代码的中的类)
    /// </summary>
    /// <param name="content"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static object FromJson(this string content, Type t)
    {
        return JsonMapper.ToObject(content, t);
    }

    /// <summary>
    /// json数据转中间对象
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static JsonData FromJson(string content)
    {
        return JsonMapper.ToObject(content);
    }

    /// <summary>
    /// 转换为JSON字符串
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToJson(this object data)
    {
        return JsonMapper.ToJson(data);
    }
}
