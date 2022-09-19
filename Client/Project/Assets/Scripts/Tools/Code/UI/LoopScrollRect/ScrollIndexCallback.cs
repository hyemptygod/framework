using UnityEngine;

/*
 * 热更新里用不了泛型
public class ScrollIndexCallback<T>:MonoBehaviour
{
    public System.Action<T,int> scrollCellIndex;
    public T item;
    void ScrollCellIndex(int idx)
    {
        if(scrollCellIndex != null)
            scrollCellIndex(item,idx);
    }
}*/
public class ScrollIndexCallback: MonoBehaviour
{
    public System.Action<object, int> scrollCellIndex;
    public object item;
    void ScrollCellIndex(int idx)
    {
        if (scrollCellIndex != null)
            scrollCellIndex(item, idx);
    }
}


