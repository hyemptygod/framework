using System;
using UnityEngine;

/// <summary>
/// 日志输出
/// </summary>
public static class Log
{
    /// <summary>
    /// 输出日志级别
    /// </summary>
    public enum LogType
    {
        Info = 1,
        Warn = 2,
        Error = 4,
    }

    /// <summary>
    /// 日志输出类型
    /// </summary>
    public enum LogMode
    {
        Unity,
        Console,
    }

    public static LogMode mode = LogMode.Unity;

    public static LogType level = LogType.Info;

    public static void Info(object msg)
    {
        if ((level & LogType.Info) == 0) return;

        LogInfoHandler(msg);
    }

    public static void Warn(object msg)
    {
        if ((level & LogType.Warn) == 0) return;

        LogWarningHandler(msg);
    }

    public static void Error(object msg)
    {
        if ((level & LogType.Error) == 0) return;

        LogErrorHandler(msg);
    }

    public static void Output(object msg, LogType t)
    {
        if (t == LogType.Info)
            Info(msg);
        else if (t == LogType.Warn)
            Warn(msg);
        else
            Error(msg);
    }

    private static void LogInfoHandler(object message)
    {
        switch (mode)
        {

            case LogMode.Unity:
                Debug.Log(message);
                break;
            case LogMode.Console:
                Console.WriteLine(message);
                break;
        }
    }

    private static void LogWarningHandler(object message)
    {
        switch (mode)
        {
            case LogMode.Unity:
                Debug.LogWarning(message);
                break;
            case LogMode.Console:
                Console.WriteLine(message);
                break;
        }
    }

    private static void LogErrorHandler(object message)
    {
        switch (mode)
        {
            case LogMode.Unity:
                Debug.LogError(message);
                break;
            case LogMode.Console:
                Console.WriteLine(message);
                break;
        }
    }

    //#if UNITY_EDITOR

    //        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    //        public static bool OnOpenAsset(int instanceID, int line)
    //        {
    //            var stacktrance = GetStackTrance();
    //            if (!string.IsNullOrEmpty(stacktrance))
    //            {
    //                var matches = Regex.Matches(stacktrance, @"\(at (.+)\)");
    //                var pathline = "";
    //                foreach (Match match in matches)
    //                {
    //                    pathline = match.Groups[1].Value;
    //                    if (!pathline.Contains("Assets/Framework/Scripts/Tools/Log/Log.cs"))
    //                    {
    //                        var index = pathline.LastIndexOf(":");
    //                        string path = pathline.Substring(0, index);
    //                        var linestr = pathline.Substring(index + 1).TrimEnd(')');
    //                        if (!int.TryParse(linestr, out line))
    //                            return false;
    //                        string fullPath = Application.dataPath.Replace("Assets", path);
    //                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, line);
    //                        return true;
    //                    }
    //                }
    //            }

    //            return false;
    //        }

    //        public static string GetStackTrance()
    //        {
    //            var windowtype = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
    //            var fieldinfo = windowtype.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
    //            var instance = fieldinfo.GetValue(null);

    //            if (instance != null)
    //            {
    //                if ((object)EditorWindow.focusedWindow == instance)
    //                {
    //                    var listviewtype = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
    //                    fieldinfo = windowtype.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    //                    var listview = fieldinfo.GetValue(instance);
    //                    fieldinfo = listviewtype.GetField("row", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
    //                    var row = (int)fieldinfo.GetValue(listview);
    //                    fieldinfo = windowtype.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
    //                    string text = fieldinfo.GetValue(instance).ToString();
    //                    return text;
    //                }
    //            }

    //            return null;
    //        }

    //#endif

}
