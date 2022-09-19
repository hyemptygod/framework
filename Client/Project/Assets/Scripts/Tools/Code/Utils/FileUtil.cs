using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public static class FileUtil
{
    /// <summary>
    /// 路径合并
    /// </summary>
    /// <param name="path"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string PathCombine(this string path, params string[] args)
    {
        if (args == null || args.Length == 0)
            return path;

        for (int i = 0; i < args.Length; i++)
        {
            path = Path.Combine(path, args[i]);
        }

        return path;
    }

    /// <summary>
    /// 从文件里读取String内容
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static string ReadContentFromFile(string uri)
    {
        if (!File.Exists(uri))
            return string.Empty;
        return File.ReadAllText(uri);
    }

    /// <summary>
    /// 从文件里读取Byte内容
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static byte[] ReadBytesFromFile(string uri)
    {
        if (!File.Exists(uri)) return null;
        return File.ReadAllBytes(uri);
    }

    /// <summary>
    /// UnityWebRequest Get 读取string内容
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static CoroutineHandle ReadContentByRequest(string url, Action<string> callback)
    {
        return CoroutineManager.RunCoroutine(ReadByRequestInternal(url, false, callback, null), "downloadtext:" + url);
    }

    /// <summary>
    /// UnityWebRequest Get 读取Byte内容
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static CoroutineHandle ReadBytesByRequest(string url, Action<byte[]> callback)
    {
        return CoroutineManager.RunCoroutine(ReadByRequestInternal(url, true, null, callback), "downloadbytes:" + url);
    }

    /// <summary>
    /// UnityWebRequest 读取内容
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="readBytes"></param>
    /// <param name="callback1"></param>
    /// <param name="callback2"></param>
    /// <returns></returns>
    private static IEnumerator<float> ReadByRequestInternal(string url, bool readBytes, Action<string> callback1, Action<byte[]> callback2)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            yield return CoroutineManager.WaitUntilDone(request.SendWebRequest());

            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.Error(string.Format("Url:{0} read error : {1}", url, request.result));
                if (!readBytes && callback1 != null)
                    callback1("");
                else if (readBytes && callback2 != null)
                    callback2(null);
            }
            else
            {
                if (!readBytes && callback1 != null)
                    callback1(request.downloadHandler.text);
                else if (readBytes && callback2 != null)
                    callback2(request.downloadHandler.data);
            }
        }
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path"></param>
    public static void CreateDir(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="bytes"></param>
    public static void CreateFile(string path, string name, byte[] bytes)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var file = Path.Combine(path, name);
        if (File.Exists(file))
            File.Delete(file);

        File.WriteAllBytes(file, bytes);
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="content"></param>
    public static void CreateFile(string path, string name, string content)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var file = Path.Combine(path, name);
        if (File.Exists(file))
            File.Delete(file);

        File.WriteAllText(file, content);
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileInfo GetFileInfo(string filePath)
    {
        return new FileInfo(filePath);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileName"></param>
    public static void RemoveFile(string fileName)
    {
        if (!File.Exists(fileName)) return;

        File.Delete(fileName);
    }
}
