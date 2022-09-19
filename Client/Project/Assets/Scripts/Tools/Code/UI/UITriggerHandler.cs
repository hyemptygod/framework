using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 事件类型
/// </summary>
public enum TriggerType
{
    /// <summary>
    /// PointerEnter
    /// </summary>
    PointerEnter = 1,
    /// <summary>
    /// PointerExit
    /// </summary>
    PointerExit = 1 << 1,
    /// <summary>
    /// PointerDown
    /// </summary>
    PointerDown = 1 << 2,
    /// <summary>
    /// PointerUp
    /// </summary>
    PointerUp = 1 << 3,
    /// <summary>
    /// Click
    /// </summary>
    Click = 1 << 4,
    /// <summary>
    /// Drag
    /// </summary>
    Drag = 1 << 5,
    /// <summary>
    /// Drop
    /// </summary>
    Drop = 1 << 6,
    /// <summary>
    /// Scroll
    /// </summary>
    Scroll = 1 << 7,
    /// <summary>
    /// UpdateSelected
    /// </summary>
    UpdateSelected = 1 << 8,
    /// <summary>
    /// Select
    /// </summary>
    Select = 1 << 9,
    /// <summary>
    /// Deselect
    /// </summary>
    Deselect = 1 << 10,
    /// <summary>
    /// Move
    /// </summary>
    Move = 1 << 11,
    /// <summary>
    /// PointerEnter
    /// </summary>
    InitializePotentialDrag = 1 << 12,
    /// <summary>
    /// BeginDrag
    /// </summary>
    BeginDrag = 1 << 13,
    /// <summary>
    /// EndDrag
    /// </summary>
    EndDrag = 1 << 14,
    /// <summary>
    /// Submit
    /// </summary>
    Submit = 1 << 15,
    /// <summary>
    /// Cancel
    /// </summary>
    Cancel = 1 << 16,
    /// <summary>
    /// DoubleClick
    /// </summary>
    DoubleClick = 1 << 17,
    /// <summary>
    /// LongClick
    /// </summary>
    LongClick = 1 << 18
}

/// <summary>
/// 事件管理工具类
/// </summary>
public class UITriggerHandler : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
{
    /// <summary>
    /// 事件对象
    /// </summary>
    [Serializable]
    public class TriggerEvent : UnityEvent<BaseEventData> { }
    /// <summary>
    /// Entry
    /// </summary>
    [Serializable]
    public class Entry
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public TriggerType eventID;
        /// <summary>
        /// 回调
        /// </summary>
        public TriggerEvent callback = new TriggerEvent();
    }

    private const float LONG_CLICK = 0.6f;

    /// <summary>
    /// 事件列表
    /// </summary>
    public static Dictionary<TriggerType, TriggerEvent> _commonCallbacks;
    /// <summary>
    /// 事件是否有通用回调
    /// </summary>
    private Dictionary<TriggerType, bool> _useCommon;


    [SerializeField]
    private List<Entry> _Delegates;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public UnityAction<BaseEventData> this[TriggerType t]
    {
        get
        {
            return null;
        }
        set
        {
            if (value == null) RemoveAllListeners(t);
            else AddListener(t, value);
        }
    }

    /// <summary>
    /// All the functions registered in this EventTrigger
    /// </summary>
    public List<Entry> Triggers
    {
        get
        {
            if (_Delegates == null)
                _Delegates = new List<Entry>();
            return _Delegates;
        }
        set { _Delegates = value; }
    }

    private float _pointerDownTime;
    private float _pointerUpTime;
    private float _duration { get { return _pointerUpTime - _pointerDownTime; } }

    /// <summary>
    /// 添加事件的通用回调
    /// </summary>
    /// <param name="t">事件类型</param>
    /// <param name="callback">回调</param>
    public static void AddCommonCallback(TriggerType t, UnityAction<BaseEventData> callback)
    {
        if (_commonCallbacks == null)
            _commonCallbacks = new Dictionary<TriggerType, TriggerEvent>(19);

        if (!_commonCallbacks.ContainsKey(t))
            _commonCallbacks.Add(t, new TriggerEvent());

        _commonCallbacks[t].AddListener(callback);
    }

    private static bool ExistCommonCallback(TriggerType id)
    {
        return _commonCallbacks != null && _commonCallbacks.ContainsKey(id) && _commonCallbacks[id] != null;
    }

    private static void ExecuteCommon(TriggerType id, BaseEventData data)
    {
        if (!ExistCommonCallback(id))
            return;

        _commonCallbacks[id].Invoke(data);
    }

    /// <summary>
    /// 设置是否使用通用的事件回调
    /// </summary>
    /// <param name="id"></param>
    /// <param name="use"></param>
    public void SetUseCommon(TriggerType id, bool use)
    {
        if (!use && (_useCommon == null || !_useCommon.ContainsKey(id)))
            return;

        if (!ExistCommonCallback(id))
            return;

        if (_useCommon == null)
            _useCommon = new Dictionary<TriggerType, bool>(19);

        _useCommon[id] = use;
    }

    private bool GetUseCommon(TriggerType id)
    {
        if (_useCommon == null || !_useCommon.ContainsKey(id))
            return false;

        if (!ExistCommonCallback(id))
            return false;

        return _useCommon[id];
    }

    private void Execute(TriggerType id, BaseEventData eventData)
    {
        for (int i = 0, imax = Triggers.Count; i < imax; ++i)
        {
            if (GetUseCommon(id))
                ExecuteCommon(id, eventData);
            var ent = Triggers[i];
            if (ent.eventID == id && ent.callback != null)
                ent.callback.Invoke(eventData);
        }
    }

    private bool Exists(TriggerType id)
    {
        return Triggers.Exists(item => item.eventID == id);
    }

    private void AddListener(TriggerType type, UnityAction<BaseEventData> call)
    {
        var entry = Triggers.Find(a => a.eventID == type);
        if (entry == null)
        {
            entry = new Entry() { eventID = type };
            Triggers.Add(entry);
        }
        entry.callback.AddListener(call);
    }

    private void RemoveAllListeners(TriggerType type)
    {
        var entry = Triggers.Find(a => a.eventID == type);
        if (entry == null) return;
        entry.callback.RemoveAllListeners();
    }

    /// <summary>
    /// OnInitializePotentialDrag
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        Execute(TriggerType.InitializePotentialDrag, eventData);
    }

    /// <summary>
    /// OnBeginDrag
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Execute(TriggerType.BeginDrag, eventData);
    }

    /// <summary>
    /// OnDrag
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnDrag(PointerEventData eventData)
    {
        Execute(TriggerType.Drag, eventData);
    }

    /// <summary>
    /// OnEndDrag
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        Execute(TriggerType.EndDrag, eventData);
    }

    /// <summary>
    /// OnDrop
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnDrop(PointerEventData eventData)
    {
        Execute(TriggerType.Drop, eventData);
    }

    /// <summary>
    /// OnPointerClick
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2 && Exists(TriggerType.DoubleClick))
        {
            Execute(TriggerType.DoubleClick, eventData);
            return;
        }

        if (_duration > LONG_CLICK && Exists(TriggerType.LongClick)) return;

        Execute(TriggerType.Click, eventData);
    }

    /// <summary>
    /// OnPointerDown
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        Execute(TriggerType.PointerDown, eventData);
        _pointerDownTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// OnPointerEnter
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Execute(TriggerType.PointerEnter, eventData);
    }

    /// <summary>
    /// OnPointerExit
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Execute(TriggerType.PointerExit, eventData);
    }

    /// <summary>
    /// OnPointerUp
    /// </summary>
    /// <param name="eventData">PointerEventData</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        Execute(TriggerType.PointerUp, eventData);
        _pointerUpTime = Time.realtimeSinceStartup;

        if (_duration > LONG_CLICK)
            Execute(TriggerType.LongClick, eventData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnScroll(PointerEventData eventData)
    {
        Execute(TriggerType.Scroll, eventData);
    }

    /// <summary>
    /// OnSelect
    /// </summary>
    /// <param name="eventData">BaseEventData</param>
    public void OnSelect(BaseEventData eventData)
    {
        Execute(TriggerType.Select, eventData);
    }

    /// <summary>
    /// OnUpdateSelected
    /// </summary>
    /// <param name="eventData">BaseEventData</param>
    public void OnUpdateSelected(BaseEventData eventData)
    {
        Execute(TriggerType.UpdateSelected, eventData);
    }

    /// <summary>
    /// OnDeselect
    /// </summary>
    /// <param name="eventData">BaseEventData</param>
    public void OnDeselect(BaseEventData eventData)
    {
        Execute(TriggerType.Deselect, eventData);
    }

    /// <summary>
    /// OnSubmit
    /// </summary>
    /// <param name="eventData">BaseEventData</param>
    public void OnSubmit(BaseEventData eventData)
    {
        Execute(TriggerType.Submit, eventData);
    }

    /// <summary>
    /// OnCancel
    /// </summary>
    /// <param name="eventData">BaseEventData</param>
    public void OnCancel(BaseEventData eventData)
    {
        Execute(TriggerType.Cancel, eventData);
    }

    /// <summary>
    /// OnMove
    /// </summary>
    /// <param name="eventData">AxisEventData</param>
    public void OnMove(AxisEventData eventData)
    {
        Execute(TriggerType.Move, eventData);
    }
}


