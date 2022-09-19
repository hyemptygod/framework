using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Text组件扩展
/// </summary>
[RequireComponent(typeof(Text))]
public class UIText : UITriggerHandler
{
    public static int MIN_FONT_SIZE = 15;

    private Text _component;
    /// <summary>
    /// Text组件
    /// </summary>
    public Text Component
    {
        get
        {
            if (_component == null)
                _component = GetComponent<Text>();
            return _component;
        }
    }

    private string _value;
    public string Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            SetText(_value);
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            return Component.rectTransform;
        }
    }

    public Action<string> ResetCallback { get; set; }

    /// <summary>
    /// 设置内容
    /// </summary>
    /// <param name="value"></param>
    /// <param name="horizontal"></param>
    public void SetText(string value)
    {
        Component.text = value;

        ResetCallback?.Invoke(value);
    }
}
