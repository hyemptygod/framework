using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UnityUtil
{
    public static T QuickGetComponent<T>(this GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
}
