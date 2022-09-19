using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace HotFix
{
    public partial class UIComponent
    {
        public enum UIFieldType
        {
            GameObject,
            Transform,
            RectTransform,
            SubOfUIComponnet,
            SubOfComponent,
            None,
        }

        public class UIFieldInfo
        {
            public Type type;
            public UIFieldType uiFieldType;
            public Transform child;

            public object GetValue()
            {
                switch (uiFieldType)
                {
                    case UIFieldType.GameObject:
                        return child.gameObject;
                    case UIFieldType.Transform:
                        return child;
                    case UIFieldType.RectTransform:
                        return child as RectTransform;
                    case UIFieldType.SubOfUIComponnet:
                        var ct = type.GetConstructor(new Type[] { typeof(GameObject) });
                        if (ct != null)
                        {
                            return ct.Invoke(new object[] { child.gameObject });
                        }
                        return null;
                    case UIFieldType.SubOfComponent:
                        return child.gameObject.GetComponent(type);
                    default:
                        Debug.Log(type.FullName + " 不能作为UIField");
                        return null;
                }
            }
        }

        /// <summary>
        /// UIField赋值
        /// </summary>
        private void ParseUIFields()
        {
            var fields = GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fields == null || fields.Length == 0)
                return;

            foreach (var item in AnalyzeComponentUIFields(fields))
            {
                var obj = item.Value.GetValue();
                if (obj == null) continue;
                item.Key.SetValue(this, obj);
            }
        }

        /// <summary>
        /// 解析UI相关字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<FieldInfo, UIFieldInfo> AnalyzeComponentUIFields(FieldInfo[] fields)
        {
            var result = new Dictionary<FieldInfo, UIFieldInfo>(fields.Length);

            foreach (var field in fields)
            {
                if (field == null) continue;

                var attributes = field.GetCustomAttributes(typeof(UIFieldAttribute), false);
                if (attributes == null || attributes.Length <= 0)
                    continue;
                var attribute = attributes[0] as UIFieldAttribute;
                if (attribute == null)
                    continue;
                var child = transform.Find(attribute.Path);
                if (child == null)
                {
                    Log.Error(transform.name + " 不存在 " + attribute.Path + "子物体");
                    continue;
                }

                var ui_field_type = GetUIFieldType(field.FieldType);

                var ui_field = new UIFieldInfo()
                {
                    type = field.FieldType,
                    uiFieldType = ui_field_type,
                    child = child
                };

                result.Add(field, ui_field);
            }

            return result;
        }

        /// <summary>
        /// 根据Type获得UIFieldType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private UIFieldType GetUIFieldType(Type type)
        {
            if (type == typeof(UnityEngine.GameObject))
                return UIFieldType.GameObject;
            else if (type == typeof(UnityEngine.Transform))
                return UIFieldType.Transform;
            else if (type == typeof(UnityEngine.RectTransform))
                return UIFieldType.RectTransform;
            else if (type.IsSubclassOf(typeof(UIComponent)))
                return UIFieldType.SubOfUIComponnet;
            else if (type.IsSubclassOf(typeof(UnityEngine.Component)))
                return UIFieldType.SubOfComponent;
            else
                return UIFieldType.None;
        }
    }
}
