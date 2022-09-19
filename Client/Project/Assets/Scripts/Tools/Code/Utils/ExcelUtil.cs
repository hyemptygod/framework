using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using OfficeOpenXml;
using Excel;
using UnityEngine;

public static class ExcelUtil
{
    /// <summary>
    /// 本地Excel文件路径
    /// </summary>
    public static string LocalExcelPath = Application.dataPath.Replace("Client/Project/Assets", "Data/Excel/");

    public static string GetExcelName(string name)
    {
        return LocalExcelPath + name + ".xlsx";
    }

    public static void ToExcel<T>(this List<T> datas, string name, FieldInfo[] fields = null)
    {
        using (var package = new ExcelPackage(new FileInfo(GetExcelName(name))))
        {
            if (package.Workbook.Worksheets[name] != null)
                package.Workbook.Worksheets.Delete(name);

            var sheet = package.Workbook.Worksheets.Add(name);

            var map = new Dictionary<FieldInfo, CommonFieldType>();

            if (fields == null)
                fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);

            var col = 1;
            foreach (var f in fields)
            {
                // 第一行是字段名
                sheet.Cells[1, col].Value = f.Name;

                var attribute = GetDesciptionAttribute(f);
                if (attribute == null)
                    map.Add(f, CommonFieldType.UInt);
                else
                    map.Add(f, (CommonFieldType)attribute.Type);

                // 第二行是字段描述
                sheet.Cells[2, col].Value = attribute == null ? f.Name : attribute.Description + "(" + map[f].ToString() + ")";

                col++;
            }

            // 第三行开始为内容
            for (int i = 0; i < datas.Count; i++)
            {
                col = 1;
                foreach (var f in fields)
                {
                    sheet.Cells[i + 3, col++].Value = ToExcelValue(map[f], f.GetValue(datas[i]));
                }
            }

            package.Save();
        }
    }

    public static List<Dictionary<string, string>> FromExcel(string name)
    {
        var excelname = GetExcelName(name);
        if (!File.Exists(excelname))
        {
            Log.Error(string.Format("请先创建名为 {0} 的Excel文件", name));
            return null;
        }

        using (var fs = File.Open(excelname, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateOpenXmlReader(fs))
            {
                var table = reader.AsDataSet().Tables[name];
                if (table == null)
                {
                    Log.Error(string.Format("{0} 表中没有名为 {0}的Sheet", name));
                    return null;
                }

                var keys = new List<string>();
                var result = new List<Dictionary<string, string>>();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var obj = table.Rows[0][i];
                    if (obj == null)
                        break;
                    keys.Add(obj.ToString());
                }

                var cols = keys.Count;
                var rows = table.Rows.Count;
                for (int row = 2; row < rows; row++)
                {
                    var dic = new Dictionary<string, string>();
                    for (int col = 0; col < cols; col++)
                    {
                        dic.Add(keys[col], table.Rows[row][col].ToString());
                    }
                    result.Add(dic);
                }

                return result;
            }
        }
    }

    public static DescriptionAttribute GetDesciptionAttribute(FieldInfo f)
    {
        var attributes = f.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes == null || attributes.Length <= 0)
            return null;
        return ((DescriptionAttribute)attributes[0]);
    }

    public static object ToExcelValue(CommonFieldType t, object o)
    {
        if (t == CommonFieldType.Object || t == CommonFieldType.Array)
            return o.ToJson();

        return o;
    }

    public static object ToObject(CommonFieldType ct, string o, Type t)
    {
        if (ct == CommonFieldType.Array || ct == CommonFieldType.Object)
            return o.FromJson(t);
        else if (ct == CommonFieldType.Int)
            return int.Parse(o);
        else if (ct == CommonFieldType.UInt)
            return uint.Parse(o);
        else
            return o;

    }
}