using UnityEngine;
using UnityEditor;
using Framework.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FrameworkEditor
{
    public class PlayerPrefsWindow : BaseWindow<PlayerPrefsWindow>
    {
        protected override string TitleName => "UserData Window";

        protected override WindowType WindowType => WindowType.Normal;

        private Dictionary<string, Type> _items = new Dictionary<string, Type>();
        private Dictionary<string, object> _default = new Dictionary<string, object>();

        private void OnEnable()
        {
            var userdata = new UserData();
            var t = typeof(UserData);
            var fields = t.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(f => f.IsLiteral && !f.IsInitOnly).ToArray();

            foreach (var field in fields)
            {
                var key = field.GetRawConstantValue().ToString();

                var name = key.Replace(" ", "");
                var first = name[0].ToString();
                name = "_" + first.ToLower() + name.Substring(1);
                var f = t.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
                _items.Add(key, f.FieldType);
                _default.Add(key, f.GetValue(userdata));
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            using (var scope = new EditorGUILayout.VerticalScope("box"))
            {
                foreach (var item in _items)
                {
                    using (var scope1 = new EditorGUILayout.HorizontalScope("box"))
                    {
                        EditorGUILayout.LabelField(item.Key);
                        GetValue(item.Key, item.Value, _default[item.Key]);
                    }
                }
            }
        }

        private void GetValue(string key, Type t, object data)
        {
            if (t == typeof(string))
            {
                var value = PlayerPrefs.GetString(key, data.ToString());
                value = EditorGUILayout.TextField(value);
                if (value != PlayerPrefs.GetString(key, data.ToString()))
                {
                    PlayerPrefs.SetString(key, value);
                }
            }
            else if (t == typeof(int))
            {
                var value = PlayerPrefs.GetInt(key, (int)data);
                value = EditorGUILayout.IntField(value);
                if (value != PlayerPrefs.GetInt(key, (int)data))
                {
                    PlayerPrefs.SetInt(key, value);
                }
            }
            else if (t == typeof(float))
            {
                var value = PlayerPrefs.GetFloat(key, (float)data);
                value = EditorGUILayout.FloatField(value);
                if (value != PlayerPrefs.GetFloat(key, (float)data))
                {
                    PlayerPrefs.SetFloat(key, value);
                }
            }
            else if (t.IsEnum)
            {
                var select = PlayerPrefs.HasKey(key) ? (Enum)Enum.Parse(t, PlayerPrefs.GetString(key)) : (Enum)data;

                select = EditorGUILayout.EnumPopup(select);

                if (select.ToString() != PlayerPrefs.GetString(key))
                {
                    PlayerPrefs.SetString(key, select.ToString());
                }
            }
            else
            {

            }
        }
    }
}
