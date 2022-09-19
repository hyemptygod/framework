using System;
using System.Diagnostics;

public class Timer
{
    private static DateTime _startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    private string _title = "";
    private Stopwatch _stopwatch = new Stopwatch();

    public Timer(string title)
    {
        _title = title;
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
        Log.Info(string.Format("{0}:{1} ms", _title, _stopwatch.ElapsedMilliseconds));
    }

    /// <summary>
    /// 获取当前时间戳(秒)
    /// </summary>
    /// <returns></returns>
    public static long NowTimeStampSecond
    {
        get
        {
            return Convert.ToInt64((DateTime.Now - _startTime).TotalSeconds);
        }
    }

    /// <summary>
    /// 获取当前时间戳(豪秒)
    /// </summary>
    /// <returns></returns>
    public static long NowTimeStampMillisecond
    {
        get
        {
            return Convert.ToInt64((DateTime.Now - _startTime).TotalMilliseconds);
        }
    }

}
