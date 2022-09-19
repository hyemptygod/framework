

public static class SizeUtil
{
    private static string[] _sizeUnit = { "B", "K", "M", "G", "T" };

    /// <summary>
    /// 数据长度转换为数据大小
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetSize(long length)
    {
        var size = (double)length;

        for (int i = 0; i < _sizeUnit.Length - 1; i++)
        {
            if (size < 1024f)
                return size.ToString("0.00") + _sizeUnit[i];

            size = size / 1024f;
        }

        return size.ToString("0.00") + _sizeUnit[_sizeUnit.Length - 1];
    }
}
