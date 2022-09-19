using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace FrameworkEditor
{
    public class GUIStyleViewer : BaseWindow<GUIStyleViewer>
    {
        protected override string TitleName => "内置控件风格";
        protected override WindowType WindowType => WindowType.Normal;

        private Vector2 scrollVector2 = Vector2.zero;
        private string search = "";

        protected override void OnGUI()
        {
            using (var helpbox = new EditorGUILayout.HorizontalScope("HelpBox"))
            {
                GUILayout.Space(30);
                search = EditorGUILayout.TextField("", search, "SearchTextField", GUILayout.MaxWidth(position.x / 3));
                GUILayout.Label("", "SearchCancelButtonEmpty");
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollVector2))
            {
                foreach (GUIStyle style in GUI.skin.customStyles)
                {
                    if (style.name.ToLower().Contains(search.ToLower()))
                    {
                        DrawStyleItem(style);
                    }
                }
                scrollVector2 = scroll.scrollPosition;
            }
        }

        void DrawStyleItem(GUIStyle style)
        {
            using (var scope = new EditorGUILayout.HorizontalScope("box"))
            {
                GUILayout.Space(40);
                EditorGUILayout.SelectableLabel(style.name);
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel(style.name, style);
                GUILayout.Space(40);
                EditorGUILayout.SelectableLabel("", style, GUILayout.Height(40), GUILayout.Width(40));
                GUILayout.Space(50);
                if (GUILayout.Button("复制到剪贴板"))
                {
                    TextEditor textEditor = new TextEditor();
                    textEditor.text = style.name;
                    textEditor.OnFocus();
                    textEditor.Copy();
                }
            }
            GUILayout.Space(10);
        }
    }
}