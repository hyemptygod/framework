using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletone<T> where T : Singletone<T>, new()
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new T();
            return m_Instance;
        }
    }
}

public class MonoSingletone<T> : MonoBehaviour where T : MonoSingletone<T>
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();
                if (m_Instance == null)
                    m_Instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
            return m_Instance;
        }
    }
}

public class Manager<T> : MonoBehaviour where T : Manager<T>
{
    private static T m_Instance;
    public static T Instance { get { return m_Instance; } }

    private void Awake()
    {
        m_Instance = this as T;

        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init()
    {

    }
}

