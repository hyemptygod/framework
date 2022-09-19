using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIText))]
public class UITextEditor : Editor
{
    protected UIText _target;

    protected virtual void OnEnable()
    {
        _target = target as UIText;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_target.Value != _target.Component.text)
        {
            _target.Value = _target.Component.text;
        }
    }
}