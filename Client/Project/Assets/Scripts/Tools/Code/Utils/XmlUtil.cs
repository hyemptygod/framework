using System;
using System.IO;
using System.Xml.Serialization;

public static class XmlUtil
{
    /// <summary>
    /// xml数据转换为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T FromXmlFile<T>(string path, string fileName)
    {
        var result = default(T);
        var name = Path.Combine(path, fileName);
        if (!File.Exists(name))
        {
            return result;
        }

        var serializer = new XmlSerializer(typeof(T));

        using (StreamReader reader = new StreamReader(name))
        {
            return (T)serializer.Deserialize(reader);
        }
    }

    /// <summary>
    /// string数据转对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    public static T FromXmlString<T>(string content)
    {
        if (string.IsNullOrEmpty(content)) return default(T);

        var serializer = new XmlSerializer(typeof(T));

        using (StringReader reader = new StringReader(content))
        {
            return (T)serializer.Deserialize(reader);
        }
    }

    /// <summary>
    /// 对象转xml数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    public static void ToXml<T>(this T target, string path, string fileName)
    {
        var name = Path.Combine(path, fileName);
        FileUtil.RemoveFile(name);
        FileUtil.CreateDir(path);

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, target);
            string content = writer.ToString();
            using (StreamWriter stream_writer = new StreamWriter(name))
            {
                stream_writer.Write(content);
            }
        }
    }

    public static void ToXml(object obj, Type t, string path, string fileName)
    {
        var name = Path.Combine(path, fileName);
        FileUtil.RemoveFile(name);
        FileUtil.CreateDir(path);

        Log.Info(t.FullName);

        XmlSerializer serializer = new XmlSerializer(t);

        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, obj);
            string content = writer.ToString();
            using (StreamWriter stream_writer = new StreamWriter(name))
            {
                stream_writer.Write(content);
            }
        }
    }
}
