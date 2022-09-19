using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(BackgroundText))]
public class BackgroundTextEditor : Editor
{
    private BackgroundText _target;

    private RectOffset _padding;



    private void OnEnable()
    {
        _target = target as BackgroundText;

        _target.Initialize();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(15);
        GUILayout.Label("Background Size", "HeaderLabel");
        _target.widthLimit = EditorGUILayout.Toggle("Range Width", _target.widthLimit);
        if (_target.widthLimit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                using (var scope1 = new EditorGUILayout.VerticalScope())
                {
                    _target.minWidth = EditorGUILayout.FloatField("min", _target.minWidth);
                    _target.maxWidth = EditorGUILayout.FloatField("max", _target.maxWidth);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            _target.minWidth = -1;
            _target.maxWidth = -1;
        }

        _target.heightLimit = EditorGUILayout.Toggle("Range Height", _target.heightLimit);
        if (_target.heightLimit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    _target.minHeight = EditorGUILayout.FloatField("min", _target.minHeight);
                    _target.maxHeight = EditorGUILayout.FloatField("max", _target.maxHeight);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            _target.minHeight = -1;
            _target.maxHeight = -1;
        }

        _target.SetText(_target.value);
    }
}
