using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundText : MonoBehaviour
{

    private UIText _contentTxt;
    private Image _backImg;

    private RectTransform _contentTrans;
    private RectTransform _backTrans;

    [TextArea(3, 10)]
    public string value = "New Text";

    [Header("Font Size")]
    public int minSize = 15;
    public int maxSize = 30;

    [Header("Text Padding")]
    public RectOffset padding;

    [HideInInspector]
    public bool widthLimit;
    [HideInInspector]
    public bool heightLimit;
    [HideInInspector]
    public float minWidth = -1;
    [HideInInspector]
    public float maxWidth = -1;
    [HideInInspector]
    public float minHeight = -1;
    [HideInInspector]
    public float maxHeight = -1;

    private Vector2 _currentSize;
    private Vector2 _offsetSize;
    private Vector2 _anchoredPosition;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _contentTxt = GetComponentInChildren<UIText>();
        _contentTrans = _contentTxt.RectTransform;

        _backImg = GetComponent<Image>();
        _backTrans = _backImg.rectTransform;

        _contentTxt.ResetCallback = Draw;
    }

    public void SetText(string value)
    {
        this.value = value;
        _contentTxt.Value = this.value;
    }

    public void Draw(string value)
    {
        this.value = value;

        _offsetSize.x = padding.left + padding.right;
        _offsetSize.y = padding.top + padding.bottom;

        _anchoredPosition.x = (padding.left - padding.right) / 2f;
        _anchoredPosition.y = (padding.bottom - padding.top) / 2f;
        _contentTrans.anchoredPosition = _anchoredPosition;

        ResizeRect(maxSize);

        _contentTxt.RectTransform.sizeDelta = _currentSize;
        var size = _currentSize + _offsetSize;
        if (minWidth > 0)
            size.x = Mathf.Max(minWidth, size.x);
        if (minHeight > 0)
            size.y = Mathf.Max(minHeight, size.y);
        _backTrans.sizeDelta = size;
    }

    private void ResizeRect(int fontSize)
    {
        _contentTxt.Component.fontSize = fontSize;

        _currentSize.x = _contentTxt.Component.preferredWidth;
        if (maxWidth > 0 && _currentSize.x > maxWidth)
        {
            _currentSize.x = maxWidth;
            _contentTxt.RectTransform.sizeDelta = _currentSize;
            _currentSize.y = _contentTxt.Component.preferredHeight;
            if (maxHeight > 0 && _currentSize.y > maxHeight)
            {
                if (fontSize > minSize)
                    ResizeRect(fontSize - 1);
            }
        }
        else
        {
            _contentTxt.RectTransform.sizeDelta = _currentSize;
            _currentSize.y = _contentTxt.Component.preferredHeight;
        }
    }
}