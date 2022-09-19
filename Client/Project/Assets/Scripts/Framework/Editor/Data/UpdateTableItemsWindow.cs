using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Framework.Data;
using Framework;
using System.Linq;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.CLR.Method;

namespace FrameworkEditor
{
    public class UpdateTableItemsWindow : BaseWindow<UpdateTableItemsWindow>
    {
        protected override string TitleName => "数据表";
        protected override WindowType WindowType => WindowType.Normal;


        private AppDomain _appdomain;
        private IMethod _containerAddMethod;

        private List<IType> _types = new List<IType>();
        private List<TableItem> _items = new List<TableItem>();

        private void OnEnable()
        {
            _appdomain = ILRuntimeHelper.ILRuntimeEditorUtil.GetDomain();
            var basetype = _appdomain.LoadedTypes["HotFix.Base.Data.BaseData"];
            foreach (var item in _appdomain.LoadedTypes.Values.ToArray())
            {
                if (item.BaseType == basetype)
                {
                    _types.Add(item);
                }
            }

            _items = FileUtil.ReadContentFromFile(PathConst.LocalSettingPath + PathConst.TABLE_ITEMS + ".json").FromJson<List<TableItem>>();

            if (_items == null)
                _items = new List<TableItem>();
            _items.RemoveAll(item => !_types.Exists(t => t.FullName == item.typeName));

            foreach (var t in _types)
            {
                AddItem(t);
            }
        }

        private void AddItem(IType t)
        {
            var instance = _appdomain.Instantiate(t.FullName);
            try
            {
                var attribute = (TableDataAttribute)t.ReflectionType.GetCustomAttributes(typeof(TableDataAttribute), false)[0];
                var item = _items.Find(a => a.typeName == t.FullName);
                if (item == null)
                {
                    _items.Add(new TableItem()
                    {
                        name = attribute.name,
                        primaryKey = attribute.primaryKey,
                        typeName = t.FullName,
                        mode = (DataLoadMode)System.Enum.Parse(typeof(DataLoadMode), attribute.mode)
                    });
                }
            }
            catch (System.Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            using (var scope = new EditorGUILayout.VerticalScope("box"))
            {
                var width = (position.size.x - 32) / 5;

                CreateHead(width);

                foreach (var t in _types)
                {
                    CreateItem(width, t);
                }

                if (GUILayout.Button("Update", GUILayout.Width(position.size.x - 32)))
                {
                    FileUtil.CreateFile(PathConst.LocalSettingPath, PathConst.TABLE_ITEMS + ".json", _items.ToJson());
                    AssetDatabase.Refresh();
                }
            }
        }

        private void CreateItem(float width, IType t)
        {
            var item = _items.Find(a => a.typeName == t.FullName);
            if (item == null)
            {
                AddItem(t);
            }

            using (var scope = new EditorGUILayout.HorizontalScope("TE NodeBox"))
            {
                GUILayout.Button(item.name, GUILayout.Width(width));
                GUILayout.Button(item.primaryKey, GUILayout.Width(width));
                GUILayout.Button(item.mode.ToString(), GUILayout.Width(width));

                // Excel to Json
                // 判断是否存在Excel
                DrawCreateExcelButton(item, t, width);

                DrawToJsonButton(item, t, width);
            }
        }

        private void DrawCreateExcelButton(TableItem item, IType t, float width)
        {
            var fullname = ExcelUtil.GetExcelName(item.name);
            if (!File.Exists(fullname))
            {
                if (GUILayout.Button("创建EXCEL", GUILayout.Width(width)))
                {
                    var container = ILRTHelper.GetDataDontainer(t);
                    var data = _appdomain.Instantiate(t.FullName);
                    t.ReflectionType.GetField("id").SetValue(data, 0);
                    container = ILRTHelper.AddData(t, data, container);
                    ILRTHelper.ToExcel(t, container, item.name);
                }
            }
            else
            {
                using (var scope = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("查看EXCEL", GUILayout.Width(width / 2 - 2f)))
                    {
                        Application.OpenURL(fullname);
                    }
                    if (GUILayout.Button("删除EXCEL", GUILayout.Width(width / 2 - 2f)))
                    {
                        var backpath = ExcelUtil.LocalExcelPath + "Back/";
                        // 创建备份文件
                        if (!Directory.Exists(backpath))
                            Directory.CreateDirectory(backpath);
                        File.Copy(fullname, backpath + item.name + Timer.NowTimeStampMillisecond + ".xlsx");
                        File.Delete(fullname);
                    }
                }
            }
        }

        private void DrawToJsonButton(TableItem item, IType t, float width)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("转JSON", GUILayout.Width(width / 2f - 2f)))
                {
                    var diclist = ExcelUtil.FromExcel(item.name);
                    var container = ILRTHelper.GetDataDontainer(t);

                    foreach (var dic in diclist)
                    {
                        var ins = _appdomain.Instantiate(t.FullName);
                        foreach (var kv in dic)
                        {
                            var field = t.ReflectionType.GetField(kv.Key);
                            if (field == null)
                            {
                                Log.Warn(string.Format("Excel文件 {0} 中的字段 {1} 在类 {2} 中不存在!!!", item.name, kv.Key, t.ReflectionType.Name));
                                continue;
                            }
                            var attrubite = ExcelUtil.GetDesciptionAttribute(field);
                            var ct = CommonFieldType.UInt;
                            if (attrubite != null)
                            {
                                ct = (CommonFieldType)attrubite.Type;
                            }
                            field.SetValue(ins, ExcelUtil.ToObject(ct, kv.Value, field.FieldType));
                        }
                        container = ILRTHelper.AddData(t, ins, container);
                    }
                    var json = container.ToJson();

                    FileUtil.CreateFile(PathConst.LocalDataPath, item.name + ".json", json);
                    FileUtil.CreateFile(PathConst.LocalJsonPath, item.name + ".json", json);

                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("选择", GUILayout.Width(width / 2f - 2f)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameAssets/Data/" + item.name + ".json");
                }
            }
        }

        private void CreateHead(float width)
        {
            using (var scope = new EditorGUILayout.HorizontalScope("TE NodeBox"))
            {
                GUILayout.Button("表名", GUILayout.Width(width));
                GUILayout.Button("唯一标识", GUILayout.Width(width));
                GUILayout.Button("加载方式", GUILayout.Width(width));
                GUILayout.Button("操作", GUILayout.Width(width));
                GUILayout.Button("检测", GUILayout.Width(width));
            }
        }

    }
}
