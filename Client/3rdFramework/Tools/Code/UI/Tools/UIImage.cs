using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Image组件扩展
/// </summary>
[RequireComponent(typeof(Image))]
public class UIImage : UITriggerHandler
{
    private Image _image;
    /// <summary>
    /// Image组件
    /// </summary>
    public Image Image
    {
        get
        {
            if (_image == null)
                _image = GetComponent<Image>();
            return _image;
        }
    }

    /// <summary>
    /// 设置精灵图片
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="isNativeSize"></param>
    public void SetSprite(Sprite sprite, bool isNativeSize = true)
    {
        _image.sprite = sprite;
        if (isNativeSize)
            _image.SetNativeSize();
    }


}

