using System.Collections.Generic;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.CLR.Utils;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class ILRTHelper
{
    private static AppDomain _appdomain;

    public static void Run(AppDomain domain)
    {
        _appdomain = domain;

        RegisterDelegates();

        RegisterInheritanceAdapter();

        RegisterValueTypeBinding();

        RegisterCLRMethod();
    }

    /// <summary>
    /// 注册委托(ILRuntime 中只能使用Action,Func,或者在ILRuntime内部定义的委托)
    /// </summary>
    private static void RegisterDelegates()
    {
        _appdomain.DelegateManager.RegisterMethodDelegate<Dictionary<string, string>>();
        _appdomain.DelegateManager.RegisterMethodDelegate<TextAsset>();
        _appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();
        _appdomain.DelegateManager.RegisterMethodDelegate<BaseEventData>();
        _appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform>();
        _appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Int32>();


        _appdomain.DelegateManager.RegisterFunctionDelegate<int, int, int>();
        _appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance>();
        _appdomain.DelegateManager.RegisterFunctionDelegate<System.Type, System.Boolean>();
        _appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject>();
        _appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Transform>();

        _appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<int>>((act) =>
        {
            return new System.Comparison<int>((x, y) =>
            {
                return ((System.Func<int, int, int>)act)(x, y);
            });
        });
        _appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<BaseEventData>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<BaseEventData>((arg0) =>
            {
                ((System.Action<BaseEventData>)act)(arg0);
            });
        });
        _appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        {
            return new DG.Tweening.TweenCallback(() =>
            {
                ((System.Action)act)();
            });
        });
        _appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Type>>((act) =>
        {
            return new System.Predicate<System.Type>((obj) =>
            {
                return ((System.Func<System.Type, System.Boolean>)act)(obj);
            });
        });
    }

    /// <summary>
    /// 跨域继承
    /// </summary>
    private static void RegisterInheritanceAdapter()
    {
        _appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        _appdomain.RegisterCrossBindingAdaptor(new IEnumeratorAdaptor());
        _appdomain.RegisterCrossBindingAdaptor(new BasePoolAdapter());
    }

    /// <summary>
    /// 注册值类型绑定
    /// </summary>
    private static void RegisterValueTypeBinding()
    {

    }

    private unsafe static void RegisterCLRMethod()
    {
        foreach (Log.LogType t in System.Enum.GetValues(typeof(Log.LogType)))
        {
            _appdomain.RegisterCLRMethodRedirection(typeof(Log).GetMethod(t.ToString(), new System.Type[] { typeof(object) }), CLRLog(t));
        }

        foreach (var i in typeof(GameObject).GetMethods())
        {
            if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
            {
                _appdomain.RegisterCLRMethodRedirection(i, GetComponent);
            }
            else if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                _appdomain.RegisterCLRMethodRedirection(i, AddComponent);
            }
        }
    }

    private unsafe static CLRRedirectionDelegate CLRLog(Log.LogType type)
    {
        return (__intp, __esp, __mStack, __method, isNewObj) =>
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);
            stacktrace = AdjustStackTrace(stacktrace);
            Log.Output(message + "\n" + stacktrace, type);
            return __ret;
        };
    }

    private unsafe static CLRRedirectionDelegate CLRDebugLogError()
    {
        return (__intp, __esp, __mStack, __method, isNewObj) =>
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);
            stacktrace = AdjustStackTrace(stacktrace);
            Debug.LogError(message + "\n" + stacktrace);
            return __ret;
        };
    }

    private static string AdjustStackTrace(string stacktrace)
    {
        stacktrace = stacktrace.Replace("\\", "/");

        var hotfixpath = Application.dataPath.Replace("Assets", "HotFix/").Replace("\\", "/");

        var newtrace = "";

        var rows = stacktrace.Split('\n');
        foreach (var row in rows)
        {
            if (row.Contains(hotfixpath))
                newtrace += (row.Replace(hotfixpath, "(at Hotfix/") + ")").Replace(":Line ", ":");
            else
                newtrace += row;
            newtrace += "\n";
        }

        return newtrace;
    }

    private unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        var ptr = __esp - 1;
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)
            {
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                var ilInstance = new ILTypeInstance(type as ILType, false);
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                ilInstance.CLRInstance = clrInstance;
                res = clrInstance.ILInstance;

                clrInstance.Awake();
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    private unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res = null;
            if (type is CLRType)
            {
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null)
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance;
                            break;
                        }
                    }
                }
            }
            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    public static List<System.Type> GetAdapterTypes()
    {
        return new List<System.Type>()
        {
            typeof(BasePool),
        };
    }

    //=============================================================================

    private static string HOTFIX_UTIL = "HotFix.Base.HotFixUtil";

    public static object GetDataDontainer(IType t)
    {
        return _appdomain.InvokeGenericMethod(HOTFIX_UTIL, "GetDontainer", new IType[] { t }, null);
    }

    public static object AddData(IType t, ILTypeInstance data, object container)
    {
        return _appdomain.InvokeGenericMethod(HOTFIX_UTIL, "AddData", new IType[] { t }, null, data, container);
    }

    public static void ToExcel(IType t, object container, string name)
    {
        _appdomain.InvokeGenericMethod(HOTFIX_UTIL, "ToExcel", new IType[] { t }, null, container, name, t.ReflectionType);
    }
}

