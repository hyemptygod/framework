using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum DisplayLevel
{
    Default,
    Scene,
    UI,
    Background,
    Window,
    Tips,
}

public static class UIUtil
{
    /// <summary>
    /// 处理嵌套的ContentSizeFitter组件的刷新
    /// </summary>
    /// <param name="target"></param>
    public static void RebuildContentSizeFitter(this RectTransform target)
    {
        var stack = new Stack<RectTransform>();

        // 查找嵌套的ContentSizeFitter
        FindNestedCompontents<ContentSizeFitter>(target, stack);

        // 没有嵌套
        if (stack.Count < 2) return;

        while (stack.Count > 0)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(stack.Pop());
        }
    }

    /// <summary>
    /// 深度查找嵌套的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    public static void FindNestedCompontents<T>(RectTransform target, Stack<RectTransform> stack) where T : Component
    {
        stack.Push(target);

        if (target.childCount <= 0) return;

        Transform current;
        for (int i = 0; i < target.childCount; i++)
        {
            current = target.GetChild(i);
            if (current.GetComponent<T>())
                FindNestedCompontents<T>(current.GetComponent<RectTransform>(), stack);
        }
    }
}