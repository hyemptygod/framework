using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// 加密工具类
/// </summary>
public static class EncryptionUtil
{
    private static byte[] _desKey;
    private static byte[] _desIv;

    static EncryptionUtil()
    {
        _desKey = Encoding.ASCII.GetBytes("huoyunxs");
        _desIv = Encoding.ASCII.GetBytes("huoyunxs");
    }

    /// <summary>
    /// DES加密
    /// </summary>
    /// <param name="data">加密数据</param>
    /// <returns></returns>
    public static string DESEncrypt(byte[] data)
    {
        return DESEncrypt(Convert.ToBase64String(data));
    }

    /// <summary>
    /// DES加密
    /// </summary>
    /// <param name="data">加密数据</param>
    /// <returns></returns>
    public static string DESEncrypt(string data)
    {
        var provider = new DESCryptoServiceProvider();
        int i = provider.KeySize;
        var ms = new MemoryStream();
        var cst = new CryptoStream(ms, provider.CreateEncryptor(_desKey, _desIv), CryptoStreamMode.Write);
        var sw = new StreamWriter(cst);
        sw.Write(data);
        sw.Flush();
        cst.FlushFinalBlock();
        sw.Flush();
        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
    }

    /// <summary>
    /// DES解密
    /// </summary>
    /// <param name="data">解密数据</param>
    /// <returns></returns>
    public static string DESDecrypt(string data)
    {
        return DESDecrypt(Convert.FromBase64String(data));
    }

    /// <summary>
    /// DES解密
    /// </summary>
    /// <param name="data">解密数据</param>
    /// <returns></returns>
    public static string DESDecrypt(byte[] data)
    {
        var provider = new DESCryptoServiceProvider();
        var ms = new MemoryStream(data);
        var cst = new CryptoStream(ms, provider.CreateDecryptor(_desKey, _desIv), CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(cst);
        return sr.ReadToEnd();
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string MD5Encrypt(string source)
    {
        return MD5Encrypt(Encoding.UTF8.GetBytes(source));
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string MD5Encrypt(byte[] source)
    {
        using (var md5hash = MD5.Create())
        {
            var data = md5hash.ComputeHash(source);
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            string hash = sBuilder.ToString();
            return hash.ToUpper();
        }
    }
}
